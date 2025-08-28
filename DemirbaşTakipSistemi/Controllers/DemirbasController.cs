using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using DemirbaşTakipSistemi.Models.Enums;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize]
    public class DemirbasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DemirbasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Demirbas
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var demirbaslar = await _context.Demirbaslar
                .Include(d => d.Oda)
                .Include(d => d.Zimmetler)
                    .ThenInclude(z => z.Personel)
                .ToListAsync();

            if (!isAdmin)
            {
                // Personel only sees assets assigned to them
                demirbaslar = demirbaslar.Where(d => 
                    d.Zimmetler.Any(z => z.PersonelId == user.Id && z.IsAktif)).ToList();
            }

            return View(demirbaslar);
        }

        // GET: Demirbas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var demirbas = await _context.Demirbaslar
                .Include(d => d.Oda)
                .Include(d => d.Zimmetler)
                    .ThenInclude(z => z.Personel)
                .Include(d => d.Arizalar)
                    .ThenInclude(a => a.Bildiren)
                .Include(d => d.Bakimlar)
                    .ThenInclude(b => b.BakimYapan)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (demirbas == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin && !demirbas.Zimmetler.Any(z => z.PersonelId == user.Id && z.IsAktif))
            {
                return Forbid();
            }

            return View(demirbas);
        }

        // GET: Demirbas/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["OdaId"] = new SelectList(await _context.Odalar.ToListAsync(), "Id", "Ad");
            return View();
        }

        // POST: Demirbas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("DemirbasKodu,Ad,Aciklama,Kategori,Marka,Model,AlimTarihi,Durum,SeriNo,OdaId")] Demirbas demirbas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(demirbas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OdaId"] = new SelectList(await _context.Odalar.ToListAsync(), "Id", "Ad", demirbas.OdaId);
            return View(demirbas);
        }

        // GET: Demirbas/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var demirbas = await _context.Demirbaslar.FindAsync(id);
            if (demirbas == null)
            {
                return NotFound();
            }
            ViewData["OdaId"] = new SelectList(await _context.Odalar.ToListAsync(), "Id", "Ad", demirbas.OdaId);
            return View(demirbas);
        }

        // POST: Demirbas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DemirbasKodu,Ad,Aciklama,Kategori,AlimTarihi,Durum,SeriNo,OdaId")] Demirbas demirbas)
        {
            if (id != demirbas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(demirbas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DemirbasExists(demirbas.Id))
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
            ViewData["OdaId"] = new SelectList(await _context.Odalar.ToListAsync(), "Id", "Ad", demirbas.OdaId);
            return View(demirbas);
        }

        // GET: Demirbas/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var demirbas = await _context.Demirbaslar
                .Include(d => d.Oda)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (demirbas == null)
            {
                return NotFound();
            }

            return View(demirbas);
        }

        // POST: Demirbas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var demirbas = await _context.Demirbaslar.FindAsync(id);
            if (demirbas != null)
            {
                _context.Demirbaslar.Remove(demirbas);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DemirbasExists(int id)
        {
            return _context.Demirbaslar.Any(e => e.Id == id);
        }

        // GET: Demirbas/Assign/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Assign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var demirbas = await _context.Demirbaslar
                .Include(d => d.Oda)
                .Include(d => d.Zimmetler)
                    .ThenInclude(z => z.Personel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (demirbas == null)
            {
                return NotFound();
            }

            // Get users for dropdown
            var users = await _userManager.GetUsersInRoleAsync("Personel");
            var userList = users.Select(u => new { 
                Id = u.Id, 
                AdSoyad = u.Ad + " " + u.Soyad + " (" + u.SicilNo + ")"
            }).ToList();
            ViewData["PersonelId"] = new SelectList(userList, "Id", "AdSoyad");

            return View(demirbas);
        }

        // POST: Demirbas/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Assign(int demirbasId, string personelId, string aciklama)
        {
            if (string.IsNullOrEmpty(personelId))
            {
                ModelState.AddModelError("", "Personel seçilmelidir.");
                return RedirectToAction(nameof(Assign), new { id = demirbasId });
            }

            var demirbas = await _context.Demirbaslar
                .Include(d => d.Zimmetler)
                .FirstOrDefaultAsync(d => d.Id == demirbasId);

            if (demirbas == null)
            {
                return NotFound();
            }

            // Deactivate current assignments
            foreach (var zimmet in demirbas.Zimmetler.Where(z => z.IsAktif))
            {
                zimmet.IsAktif = false;
                zimmet.IadeTarihi = DateTime.Now;
            }

            // Create new assignment
            var yeniZimmet = new Zimmet
            {
                DemirbasId = demirbasId,
                PersonelId = personelId,
                ZimmetTarihi = DateTime.Now,
                Aciklama = aciklama,
                IsAktif = true
            };

            _context.Zimmetler.Add(yeniZimmet);
            
            // Update demirbas status
            demirbas.Durum = DemirbasDurum.Zimmetli;
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Demirbas/Return/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zimmet = await _context.Zimmetler
                .Include(z => z.Demirbas)
                .Include(z => z.Personel)
                .FirstOrDefaultAsync(z => z.Id == id && z.IsAktif);

            if (zimmet == null)
            {
                return NotFound();
            }

            return View(zimmet);
        }

        // POST: Demirbas/ReturnConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReturnConfirmed(int id)
        {
            var zimmet = await _context.Zimmetler
                .Include(z => z.Demirbas)
                .FirstOrDefaultAsync(z => z.Id == id && z.IsAktif);

            if (zimmet == null)
            {
                return NotFound();
            }

            // Deactivate assignment
            zimmet.IsAktif = false;
            zimmet.IadeTarihi = DateTime.Now;

            // Update demirbas status
            var demirbas = zimmet.Demirbas;
            demirbas.Durum = DemirbasDurum.Aktif;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
} 