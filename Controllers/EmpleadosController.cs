
using CadenaSuministro.Data;
using CadenaSuministro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CadenaSuministro.Controllers;

public class EmpleadosController : Controller
{
    private readonly AppDbContext _context;

    public EmpleadosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Empleados
    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _context.Empleados.AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            query = query.Where(e =>
                e.Nombre.Contains(buscar) ||
                e.Cargo.Contains(buscar) ||
                e.Departamento.Contains(buscar));
        }

        ViewData["Buscar"] = buscar;

        return View(await query.OrderBy(e => e.Nombre).ToListAsync());
    }

    // GET: Empleados/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var empleado = await _context.Empleados
            .FirstOrDefaultAsync(e => e.Id == id);

        if (empleado == null) return NotFound();

        return View(empleado);
    }

    // GET: Empleados/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Empleados/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Nombre,Cargo,Departamento")] Empleado empleado)
    {
        if (ModelState.IsValid)
        {
            _context.Add(empleado);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Empleado registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        return View(empleado);
    }

    // GET: Empleados/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var empleado = await _context.Empleados.FindAsync(id);

        if (empleado == null) return NotFound();

        return View(empleado);
    }

    // POST: Empleados/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,Nombre,Cargo,Departamento")] Empleado empleado)
    {
        if (id != empleado.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(empleado);
                await _context.SaveChangesAsync();

                TempData["Ok"] = "Empleado actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Empleados.AnyAsync(e => e.Id == id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(empleado);
    }

    // GET: Empleados/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var empleado = await _context.Empleados
            .FirstOrDefaultAsync(e => e.Id == id);

        if (empleado == null) return NotFound();

        return View(empleado);
    }

    // POST: Empleados/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var empleado = await _context.Empleados.FindAsync(id);

        if (empleado == null)
            return RedirectToAction(nameof(Index));

        bool tieneOrdenes = await _context.OrdenesCompra
            .AnyAsync(o => o.EmpleadoId == id);

        if (tieneOrdenes)
        {
            TempData["Error"] =
                "No se puede eliminar: el empleado tiene órdenes de compra asociadas.";

            return RedirectToAction(nameof(Index));
        }

        _context.Empleados.Remove(empleado);
        await _context.SaveChangesAsync();

        TempData["Ok"] = "Empleado eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
