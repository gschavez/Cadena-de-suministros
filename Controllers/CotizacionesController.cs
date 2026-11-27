using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CadenaSuministro.Data;
using CadenaSuministro.Models;

namespace CadenaSuministro.Controllers
{
    public class CotizacionesController : Controller
    {
        private readonly AppDbContext _context;

        public CotizacionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Cotizaciones
        public async Task<IActionResult> Index()
        {
            // El Include trae los datos de Proveedor y Producto para que se vean los nombres
            var appDbContext = _context.Cotizaciones
                .Include(c => c.Producto)
                .Include(c => c.Proveedor);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Cotizaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Producto)
                .Include(c => c.Proveedor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cotizacion == null) return NotFound();

            return View(cotizacion);
        }

        // GET: Cotizaciones/Create
        public IActionResult Create()
        {
            CargarListasDesplegables();
            return View();
        }

        // POST: Cotizaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProveedorId,ProductoId,Cantidad,MontoEstimado,Fecha,Estado")] Cotizacion cotizacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cotizacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            CargarListasDesplegables(cotizacion.ProveedorId, cotizacion.ProductoId);
            return View(cotizacion);
        }

        // GET: Cotizaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cotizacion = await _context.Cotizaciones.FindAsync(id);
            if (cotizacion == null) return NotFound();

            CargarListasDesplegables(cotizacion.ProveedorId, cotizacion.ProductoId);
            return View(cotizacion);
        }

        // POST: Cotizaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProveedorId,ProductoId,Cantidad,MontoEstimado,Fecha,Estado")] Cotizacion cotizacion)
        {
            if (id != cotizacion.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cotizacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CotizacionExists(cotizacion.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            CargarListasDesplegables(cotizacion.ProveedorId, cotizacion.ProductoId);
            return View(cotizacion);
        }

        // GET: Cotizaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Producto)
                .Include(c => c.Proveedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cotizacion == null) return NotFound();

            return View(cotizacion);
        }

        // POST: Cotizaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);
            if (cotizacion != null)
            {
                _context.Cotizaciones.Remove(cotizacion);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CotizacionExists(int id)
        {
            return _context.Cotizaciones.Any(e => e.Id == id);
        }

        // --- MÉTODO MÁGICO PARA LOS MENÚS DESPLEGABLES ---
        private void CargarListasDesplegables(int? proveedorId = null, int? productoId = null)
        {
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre", proveedorId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", productoId);
        }
    }
}