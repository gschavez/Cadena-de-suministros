using CadenaSuministro.Data;
using CadenaSuministro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CadenaSuministro.Controllers;

// Módulo Inventario (productos, materia prima e insumos) - Integrante 4
public class ProductosController : Controller
{
    private readonly AppDbContext _context;

    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Productos?buscar=texto&tipo=MateriaPrima&soloBajo=true
    public async Task<IActionResult> Index(string? buscar, TipoItem? tipo, bool soloBajo = false)
    {
        var query = _context.Productos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
            query = query.Where(p => p.Codigo.Contains(buscar) || p.Descripcion.Contains(buscar));

        if (tipo.HasValue)
            query = query.Where(p => p.Tipo == tipo.Value);

        if (soloBajo)
            query = query.Where(p => p.Cantidad <= p.StockMinimo);

        ViewData["Buscar"] = buscar;
        ViewData["Tipo"] = tipo;
        ViewData["SoloBajo"] = soloBajo;
        ViewData["Total"] = await _context.Productos.CountAsync();
        ViewData["TotalBajo"] = await _context.Productos.CountAsync(p => p.Cantidad <= p.StockMinimo);

        return View(await query.OrderBy(p => p.Codigo).ToListAsync());
    }

    // GET: Productos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var producto = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    // GET: Productos/Create
    public IActionResult Create() => View(new Producto());

    // POST: Productos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Codigo,Descripcion,Tipo,UnidadMedida,Cantidad,StockMinimo,PrecioUnitario")] Producto producto)
    {
        producto.Codigo = (producto.Codigo ?? string.Empty).Trim().ToUpperInvariant();

        if (await _context.Productos.AnyAsync(p => p.Codigo == producto.Codigo))
            ModelState.AddModelError(nameof(Producto.Codigo), "Ya existe un producto con este código.");

        if (ModelState.IsValid)
        {
            _context.Add(producto);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Ítem de inventario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(producto);
    }

    // GET: Productos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    // POST: Productos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,Codigo,Descripcion,Tipo,UnidadMedida,Cantidad,StockMinimo,PrecioUnitario")] Producto producto)
    {
        if (id != producto.Id) return NotFound();

        producto.Codigo = (producto.Codigo ?? string.Empty).Trim().ToUpperInvariant();

        if (await _context.Productos.AnyAsync(p => p.Codigo == producto.Codigo && p.Id != producto.Id))
            ModelState.AddModelError(nameof(Producto.Codigo), "Ya existe otro producto con este código.");

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(producto);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Ítem de inventario actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Productos.AnyAsync(p => p.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(producto);
    }

    // GET: Productos/Ajustar/5  (entrada o salida de stock)
    public async Task<IActionResult> Ajustar(int? id)
    {
        if (id == null) return NotFound();
        var producto = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    // POST: Productos/Ajustar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ajustar(int id, string movimiento, int cantidad)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound();

        if (cantidad < 1)
            ModelState.AddModelError("", "La cantidad debe ser mayor o igual a 1.");
        else if (movimiento != "Entrada" && movimiento != "Salida")
            ModelState.AddModelError("", "Movimiento no válido.");
        else if (movimiento == "Salida" && cantidad > producto.Cantidad)
            ModelState.AddModelError("", $"No hay stock suficiente. Disponible: {producto.Cantidad} {producto.UnidadMedida}.");

        if (!ModelState.IsValid) return View(producto);

        producto.Cantidad += movimiento == "Entrada" ? cantidad : -cantidad;
        await _context.SaveChangesAsync();

        TempData["Ok"] = $"{movimiento} registrada. Stock actual de {producto.Codigo}: {producto.Cantidad} {producto.UnidadMedida}.";
        if (producto.Cantidad <= producto.StockMinimo)
            TempData["Error"] = $"Alerta: {producto.Codigo} quedó en o por debajo del stock mínimo ({producto.StockMinimo}).";

        return RedirectToAction(nameof(Index));
    }

    // GET: Productos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var producto = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    // POST: Productos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return RedirectToAction(nameof(Index));

        if (await _context.Cotizaciones.AnyAsync(c => c.ProductoId == id))
        {
            TempData["Error"] = "No se puede eliminar: el ítem tiene cotizaciones asociadas.";
            return RedirectToAction(nameof(Index));
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();
        TempData["Ok"] = "Ítem eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}