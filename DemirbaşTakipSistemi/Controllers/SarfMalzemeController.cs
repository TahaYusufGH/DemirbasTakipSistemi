using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SarfMalzemeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SarfMalzemeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SarfMalzeme
        public async Task<IActionResult> Index()
        {
            var sarfMalzemeler = await _context.SarfMalzemeler
                .Include(s => s.DepoIslemler)
                .OrderBy(s => s.Kategori)
                .ThenBy(s => s.Ad)
                .ToListAsync();

            return View(sarfMalzemeler);
        }

        // GET: SarfMalzeme/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sarfMalzeme = await _context.SarfMalzemeler
                .Include(s => s.DepoIslemler)
                    .ThenInclude(d => d.IslemYapan)
                .Include(s => s.DepoIslemler)
                    .ThenInclude(d => d.TalepEden)
                .Include(s => s.DepoIslemler)
                    .ThenInclude(d => d.Onaylayan)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sarfMalzeme == null)
            {
                return NotFound();
            }

            return View(sarfMalzeme);
        }

        // GET: SarfMalzeme/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SarfMalzeme/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MalzemeKodu,Ad,Aciklama,Birim,Kategori,Marka,Model,MinimumStok,MevcutStok")] SarfMalzeme sarfMalzeme)
        {
            if (ModelState.IsValid)
            {
                sarfMalzeme.SonGuncelleme = DateTime.Now;
                _context.Add(sarfMalzeme);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sarfMalzeme);
        }

        // GET: SarfMalzeme/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sarfMalzeme = await _context.SarfMalzemeler.FindAsync(id);
            if (sarfMalzeme == null)
            {
                return NotFound();
            }
            return View(sarfMalzeme);
        }

        // POST: SarfMalzeme/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MalzemeKodu,Ad,Aciklama,Birim,Kategori,Marka,Model,MinimumStok,MevcutStok")] SarfMalzeme sarfMalzeme)
        {
            if (id != sarfMalzeme.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    sarfMalzeme.SonGuncelleme = DateTime.Now;
                    _context.Update(sarfMalzeme);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SarfMalzemeExists(sarfMalzeme.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sarfMalzeme);
        }

        // GET: SarfMalzeme/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sarfMalzeme = await _context.SarfMalzemeler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sarfMalzeme == null)
            {
                return NotFound();
            }

            return View(sarfMalzeme);
        }

        // POST: SarfMalzeme/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sarfMalzeme = await _context.SarfMalzemeler.FindAsync(id);
            if (sarfMalzeme != null)
            {
                _context.SarfMalzemeler.Remove(sarfMalzeme);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SarfMalzemeExists(int id)
        {
            return _context.SarfMalzemeler.Any(e => e.Id == id);
        }
    }
} 