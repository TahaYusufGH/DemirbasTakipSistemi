using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using DemirbaşTakipSistemi.Models.Enums;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize]
    public class ArizaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ArizaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Ariza
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isBilgiIslem = await _userManager.IsInRoleAsync(user, "BilgiIslem");

            var arizalar = await _context.Arizalar
                .Include(a => a.Demirbas)
                .Include(a => a.Bildiren)
                .Include(a => a.CozenPersonel)
                .OrderByDescending(a => a.ArizaTarihi)
                .ToListAsync();

            if (!isAdmin && !isBilgiIslem)
            {
                // Personel only sees their own fault reports
                arizalar = arizalar.Where(a => a.BildirenId == user.Id).ToList();
            }

            return View(arizalar);
        }
        
        // GET: Ariza/AcikArizalar
        public async Task<IActionResult> AcikArizalar()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isBilgiIslem = await _userManager.IsInRoleAsync(user, "BilgiIslem");

            var arizalar = await _context.Arizalar
                .Include(a => a.Demirbas)
                    .ThenInclude(d => d.Oda)
                .Include(a => a.Demirbas)
                    .ThenInclude(d => d.Zimmetler)
                        .ThenInclude(z => z.Personel)
                .Include(a => a.Bildiren)
                .Include(a => a.CozenPersonel)
                .Where(a => a.Durum == "Beklemede" || a.Durum == "İncelemede")
                .OrderByDescending(a => a.ArizaTarihi)
                .ToListAsync();

            if (!isAdmin && !isBilgiIslem)
            {
                // Personel only sees their own fault reports
                arizalar = arizalar.Where(a => a.BildirenId == user.Id).ToList();
            }

            return View(arizalar);
        }
        
        // GET: Ariza/GecmisArizalar
        public async Task<IActionResult> GecmisArizalar()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isBilgiIslem = await _userManager.IsInRoleAsync(user, "BilgiIslem");

            var arizalar = await _context.Arizalar
                .Include(a => a.Demirbas)
                    .ThenInclude(d => d.Oda)
                .Include(a => a.Demirbas)
                    .ThenInclude(d => d.Zimmetler)
                        .ThenInclude(z => z.Personel)
                .Include(a => a.Bildiren)
                .Include(a => a.CozenPersonel)
                .Where(a => a.Durum == "Çözüldü" || a.Durum == "İptal")
                .OrderByDescending(a => a.ArizaTarihi)
                .ToListAsync();

            if (!isAdmin && !isBilgiIslem)
            {
                // Personel only sees their own fault reports
                arizalar = arizalar.Where(a => a.BildirenId == user.Id).ToList();
            }

            return View(arizalar);
        }

        // GET: Ariza/MyArizalar
        public async Task<IActionResult> MyArizalar()
        {
            var user = await _userManager.GetUserAsync(User);

            var arizalar = await _context.Arizalar
                .Include(a => a.Demirbas)
                .Include(a => a.Bildiren)
                .Include(a => a.CozenPersonel)
                .Where(a => a.BildirenId == user.Id)
                .OrderByDescending(a => a.ArizaTarihi)
                .ToListAsync();

            return View(arizalar);
        }

        // GET: Ariza/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ariza = await _context.Arizalar
                .Include(a => a.Demirbas)
                .Include(a => a.Bildiren)
                .Include(a => a.CozenPersonel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ariza == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isBilgiIslem = await _userManager.IsInRoleAsync(user, "BilgiIslem");

            if (!isAdmin && !isBilgiIslem && ariza.BildirenId != user.Id)
            {
                return Forbid();
            }

            return View(ariza);
        }

        // GET: Ariza/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isBilgiIslem = await _userManager.IsInRoleAsync(user, "BilgiIslem");

            var demirbaslar = await _context.Demirbaslar
                .Include(d => d.Zimmetler)
                .ToListAsync();

            if (!isAdmin && !isBilgiIslem)
            {
                // Personel can only report faults for assets assigned to them
                demirbaslar = demirbaslar.Where(d => 
                    d.Zimmetler.Any(z => z.PersonelId == user.Id && z.IsAktif)).ToList();
            }

            var demirbasSelectList = demirbaslar.Select(d => new {
                Id = d.Id,
                Display = $"{d.DemirbasKodu} - {d.Ad} ({d.Marka})"
            }).ToList();
            ViewData["DemirbasId"] = new SelectList(demirbasSelectList, "Id", "Display");
            return View();
        }

        // POST: Ariza/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DemirbasId,Aciklama")] Ariza ariza)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isBilgiIslem = await _userManager.IsInRoleAsync(user, "BilgiIslem");

            if (!isAdmin && !isBilgiIslem)
            {
                // Verify that the asset is assigned to the user
                var demirbas = await _context.Demirbaslar
                    .Include(d => d.Zimmetler)
                    .FirstOrDefaultAsync(d => d.Id == ariza.DemirbasId);

                if (demirbas == null || !demirbas.Zimmetler.Any(z => z.PersonelId == user.Id && z.IsAktif))
                {
                    return Forbid();
                }
            }

            if (ModelState.IsValid)
            {
                ariza.BildirenId = user.Id;
                ariza.ArizaTarihi = DateTime.Now;
                ariza.Durum = "Beklemede";

                _context.Add(ariza);

                var demirbas = await _context.Demirbaslar.FindAsync(ariza.DemirbasId);
                if (demirbas != null)
                {
                    demirbas.Durum = Models.Enums.DemirbasDurum.Arizali;
                    _context.Update(demirbas);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var demirbaslar = await _context.Demirbaslar
                .Include(d => d.Zimmetler)
                .ToListAsync();

            if (!isAdmin && !isBilgiIslem)
            {
                demirbaslar = demirbaslar.Where(d => 
                    d.Zimmetler.Any(z => z.PersonelId == user.Id && z.IsAktif)).ToList();
            }

            var demirbasSelectListError = demirbaslar.Select(d => new {
                Id = d.Id,
                Display = $"{d.DemirbasKodu} - {d.Ad} ({d.Marka})"
            }).ToList();
            ViewData["DemirbasId"] = new SelectList(demirbasSelectListError, "Id", "Display", ariza.DemirbasId);
            return View(ariza);
        }

        // GET: Ariza/Edit/5
        [Authorize(Roles = "Admin,BilgiIslem")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ariza = await _context.Arizalar.FindAsync(id);
            if (ariza == null)
            {
                return NotFound();
            }

            ViewData["DemirbasId"] = new SelectList(_context.Demirbaslar, "Id", "DemirbasKodu", ariza.DemirbasId);
            
            // Get users who can solve issues (Admin and BilgiIslem roles)
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var bilgiIslemUsers = await _userManager.GetUsersInRoleAsync("BilgiIslem");
            var techUsers = adminUsers.Concat(bilgiIslemUsers).Distinct().ToList();
            var techUsersList = techUsers.Select(u => new {
                Id = u.Id,
                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
            }).ToList();
            
            ViewData["CozenPersonelId"] = new SelectList(techUsersList, "Id", "Display", ariza.CozenPersonelId);
            return View(ariza);
        }

        // POST: Ariza/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,BilgiIslem")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DemirbasId,BildirenId,ArizaTarihi,Aciklama,Cozum,Durum,CozumTarihi,CozenPersonelId")] Ariza ariza)
        {
            if (id != ariza.Id)
            {
                return NotFound();
            }

            var existingAriza = await _context.Arizalar
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existingAriza == null)
            {
                return NotFound();
            }

            // Preserve the original values
            ariza.BildirenId = existingAriza.BildirenId;
            ariza.ArizaTarihi = existingAriza.ArizaTarihi;

            if (ModelState.IsValid)
            {
                try
                {
                    // If status is changed to "Çözüldü", set the solution date
                    if (ariza.Durum == "Çözüldü" && existingAriza.Durum != "Çözüldü")
                    {
                        ariza.CozumTarihi = DateTime.Now;
                        
                        // If no solver is specified, set the current user as solver
                        if (string.IsNullOrEmpty(ariza.CozenPersonelId))
                        {
                            var user = await _userManager.GetUserAsync(User);
                            ariza.CozenPersonelId = user.Id;
                        }
                        
                        // Update the asset status
                        var demirbas = await _context.Demirbaslar.FindAsync(ariza.DemirbasId);
                        if (demirbas != null)
                        {
                            demirbas.Durum = Models.Enums.DemirbasDurum.Aktif;
                            _context.Update(demirbas);
                        }
                    }
                    else if (ariza.Durum != "Çözüldü")
                    {
                        ariza.CozumTarihi = null;
                    }

                    _context.Update(ariza);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArizaExists(ariza.Id))
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

            ViewData["DemirbasId"] = new SelectList(_context.Demirbaslar, "Id", "DemirbasKodu", ariza.DemirbasId);
            
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var bilgiIslemUsers = await _userManager.GetUsersInRoleAsync("BilgiIslem");
            var techUsers = adminUsers.Concat(bilgiIslemUsers).Distinct().ToList();
            var techUsersListError = techUsers.Select(u => new {
                Id = u.Id,
                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
            }).ToList();
            
            ViewData["CozenPersonelId"] = new SelectList(techUsersListError, "Id", "Display", ariza.CozenPersonelId);
            return View(ariza);
        }

        // GET: Ariza/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ariza = await _context.Arizalar
                .Include(a => a.Demirbas)
                .Include(a => a.Bildiren)
                .Include(a => a.CozenPersonel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ariza == null)
            {
                return NotFound();
            }

            return View(ariza);
        }

        // POST: Ariza/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ariza = await _context.Arizalar.FindAsync(id);
            _context.Arizalar.Remove(ariza);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArizaExists(int id)
        {
            return _context.Arizalar.Any(e => e.Id == id);
        }
    }
} 