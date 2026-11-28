
using CadenaSuministro.Data;
using CadenaSuministro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CadenaSuministro.Controllers;

public class OrdenesCompraController : Controller
{
    private readonly AppDbContext _context;

    public OrdenesCompraController(AppDbContext context)
    {
        _context = context;
    }

    // GET: OrdenesCompra
    public async Task<IActionResult> Index()
    {
        var ordenes = await _context.OrdenesCompra
            .Include(o => o.Cotizacion)
                .ThenInclude(c => c!.Proveedor)
            .Include(o => o.Cotizacion)
                .ThenInclude(c => c!.Producto)
            .Include(o => o.Empleado)
            .OrderByDescending(o => o.Id)
            .ToListAsync();

        return View(ordenes);
    }

    // GET: OrdenesCompra/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var orden = await _context.OrdenesCompra
            .Include(o => o.Cotizacion)
                .ThenInclude(c => c!.Proveedor)
            .Include(o => o.Cotizacion)
                .ThenInclude(c => c!.Producto)
            .Include(o => o.Empleado)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (orden == null)
            return NotFound();

        return View(orden);
    }

    // GET: OrdenesCompra/Create
    public async Task<IActionResult> Create()
    {
        await CargarListasDesplegables();
        return View();
    }

    // POST: OrdenesCompra/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("CotizacionId,EmpleadoId")] OrdenCompra orden)
    {
        bool cotizacionExiste = await _context.Cotizaciones
            .AnyAsync(c => c.Id == orden.CotizacionId);

        bool empleadoExiste = await _context.Empleados
            .AnyAsync(e => e.Id == orden.EmpleadoId);

        if (!cotizacionExiste)
            ModelState.AddModelError("CotizacionId",
                "Selecciona una cotización válida.");

        if (!empleadoExiste)
            ModelState.AddModelError("EmpleadoId",
                "Selecciona un empleado válido.");

        if (ModelState.IsValid)
        {
            var nuevaOrden = new OrdenCompra
            {
                CotizacionId = orden.CotizacionId,
                EmpleadoId = orden.EmpleadoId,
                Estado = EstadoOrden.Pendiente,
                FechaAprobacion = null
            };

            _context.OrdenesCompra.Add(nuevaOrden);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Orden de compra creada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        await CargarListasDesplegables(
            orden.CotizacionId,
            orden.EmpleadoId);

        return View(orden);
    }

    // POST: OrdenesCompra/Aprobar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Aprobar(int id)
    {
        return await CambiarEstado(id, EstadoOrden.Aprobada);
    }

    // POST: OrdenesCompra/Rechazar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rechazar(int id)
    {
        return await CambiarEstado(id, EstadoOrden.Rechazada);
    }

    private async Task<IActionResult> CambiarEstado(
        int id,
        EstadoOrden nuevoEstado)
    {
        var orden = await _context.OrdenesCompra
            .FirstOrDefaultAsync(o => o.Id == id);

        if (orden == null)
            return NotFound();

        if (orden.Estado != EstadoOrden.Pendiente)
        {
            TempData["Error"] =
                "Esta orden ya fue procesada y no puede cambiarse nuevamente.";

            return RedirectToAction(nameof(Index));
        }

        orden.Estado = nuevoEstado;
        orden.FechaAprobacion = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Mensaje"] = nuevoEstado == EstadoOrden.Aprobada
            ? "Orden aprobada correctamente."
            : "Orden rechazada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasDesplegables(
        int? cotizacionId = null,
        int? empleadoId = null)
    {
        var cotizaciones = await _context.Cotizaciones
            .Include(c => c.Proveedor)
            .Include(c => c.Producto)
            .Select(c => new
            {
                c.Id,
                Nombre = "Cotización #" + c.Id +
                         " - " + c.Proveedor!.Nombre +
                         " - " + c.Producto!.Descripcion
            })
            .ToListAsync();

        ViewData["CotizacionId"] = new SelectList(
            cotizaciones,
            "Id",
            "Nombre",
            cotizacionId);

        ViewData["EmpleadoId"] = new SelectList(
            await _context.Empleados.ToListAsync(),
            "Id",
            "Nombre",
            empleadoId);
    }
}
