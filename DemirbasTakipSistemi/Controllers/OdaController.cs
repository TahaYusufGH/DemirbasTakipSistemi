using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OdaController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OdaController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var odalar = await _context.Odalar.Include(o => o.SorumluPersonel).ToListAsync();
            return View(odalar);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Oda oda)
        {
            if (ModelState.IsValid)
            {
                _context.Add(oda);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(oda);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var oda = await _context.Odalar.FindAsync(id);
            if (oda == null) return NotFound();
            return View(oda);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Oda oda)
        {
            if (ModelState.IsValid)
            {
                _context.Update(oda);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(oda);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var oda = await _context.Odalar.FindAsync(id);
            if (oda == null) return NotFound();
            return View(oda);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oda = await _context.Odalar.FindAsync(id);
            if (oda != null)
            {
                _context.Odalar.Remove(oda);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 