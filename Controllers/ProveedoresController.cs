using CadenaSuministro.Data;
using CadenaSuministro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CadenaSuministro.Controllers;

public class ProveedoresController : Controller
{
    private readonly AppDbContext _context;

    public ProveedoresController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Proveedores?buscar=texto
    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _context.Proveedores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            query = query.Where(p => p.Nombre.Contains(buscar) || p.Nit.Contains(buscar));
        }

        ViewData["Buscar"] = buscar;
        return View(await query.OrderBy(p => p.Nombre).ToListAsync());
    }

    // GET: Proveedores/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var proveedor = await _context.Proveedores.FirstOrDefaultAsync(p => p.Id == id);
        if (proveedor == null) return NotFound();

        return View(proveedor);
    }

    // GET: Proveedores/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Proveedores/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Nombre,Nit,Contacto,Direccion,Telefono,Email")] Proveedor proveedor)
    {
        if (await _context.Proveedores.AnyAsync(p => p.Nit == proveedor.Nit))
        {
            ModelState.AddModelError(nameof(Proveedor.Nit), "Ya existe un proveedor con este NIT.");
        }

        if (ModelState.IsValid)
        {
            _context.Add(proveedor);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Proveedor creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        return View(proveedor);
    }

    // GET: Proveedores/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null) return NotFound();

        return View(proveedor);
    }

    // POST: Proveedores/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,Nombre,Nit,Contacto,Direccion,Telefono,Email")] Proveedor proveedor)
    {
        if (id != proveedor.Id) return NotFound();

        if (await _context.Proveedores.AnyAsync(p => p.Nit == proveedor.Nit && p.Id != proveedor.Id))
        {
            ModelState.AddModelError(nameof(Proveedor.Nit), "Ya existe otro proveedor con este NIT.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(proveedor);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Proveedor actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Proveedores.AnyAsync(p => p.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View(proveedor);
    }

    // GET: Proveedores/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var proveedor = await _context.Proveedores.FirstOrDefaultAsync(p => p.Id == id);
        if (proveedor == null) return NotFound();

        return View(proveedor);
    }

    // POST: Proveedores/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null) return RedirectToAction(nameof(Index));

        // No se puede eliminar un proveedor con cotizaciones asociadas
        if (await _context.Cotizaciones.AnyAsync(c => c.ProveedorId == id))
        {
            TempData["Error"] = "No se puede eliminar: el proveedor tiene cotizaciones asociadas.";
            return RedirectToAction(nameof(Index));
        }

        _context.Proveedores.Remove(proveedor);
        await _context.SaveChangesAsync();
        TempData["Ok"] = "Proveedor eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}