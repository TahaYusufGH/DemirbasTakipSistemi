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

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize]
    public class ZimmetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ZimmetController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Admin ve Bilgi İşlem tüm zimmetleri görebilir
        [Authorize(Roles = "Admin,BilgiIslem")]
        public async Task<IActionResult> Index()
        {
            var zimmetler = await _context.Zimmetler
                .Include(z => z.Demirbas)
                .Include(z => z.Personel)
                .Include(z => z.TeslimEden)
                .Include(z => z.TeslimAlan)
                .OrderByDescending(z => z.ZimmetTarihi)
                .ToListAsync();
            return View(zimmetler);
        }

        // Admin yeni zimmet atayabilir
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            // Aktif durumda olan ve zimmetli olmayan demirbaşları getir
            var uygunDemirbaslar = await _context.Demirbaslar
                .Where(d => d.Durum == Models.Enums.DemirbasDurum.Aktif)
                .ToListAsync();

            ViewBag.Demirbaslar = new SelectList(uygunDemirbaslar, "Id", "DemirbasKodu");
            var personelUsers = await _userManager.GetUsersInRoleAsync("Personel");
            var personelList = personelUsers.Select(u => new {
                Id = u.Id,
                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
            }).ToList();
            ViewBag.Personeller = new SelectList(personelList, "Id", "Display");
            
            // Teslim eden olarak admin kullanıcıları
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var adminList = adminUsers.Select(u => new {
                Id = u.Id,
                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
            }).ToList();
            ViewBag.TeslimEdenler = new SelectList(adminList, "Id", "Display");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Zimmet zimmet)
        {
            if (ModelState.IsValid)
            {
                zimmet.ZimmetTarihi = DateTime.Now;
                zimmet.IsAktif = true;
                
                // Demirbaşın durumunu zimmetli olarak güncelle
                var demirbas = await _context.Demirbaslar.FindAsync(zimmet.DemirbasId);
                if (demirbas != null)
                {
                    demirbas.Durum = Models.Enums.DemirbasDurum.Zimmetli;
                    _context.Update(demirbas);
                }
                
                _context.Zimmetler.Add(zimmet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // Hata durumunda ViewBag'leri tekrar doldur
            var uygunDemirbaslar = await _context.Demirbaslar
                .Where(d => d.Durum == Models.Enums.DemirbasDurum.Aktif)
                .ToListAsync();

            ViewBag.Demirbaslar = new SelectList(uygunDemirbaslar, "Id", "DemirbasKodu", zimmet.DemirbasId);
            var personelUsersError = await _userManager.GetUsersInRoleAsync("Personel");
            var personelListError = personelUsersError.Select(u => new {
                Id = u.Id,
                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
            }).ToList();
            ViewBag.Personeller = new SelectList(personelListError, "Id", "Display", zimmet.PersonelId);
            var adminUsersError = await _userManager.GetUsersInRoleAsync("Admin");
            var adminListError = adminUsersError.Select(u => new {
                Id = u.Id,
                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
            }).ToList();
            ViewBag.TeslimEdenler = new SelectList(adminListError, "Id", "Display", zimmet.TeslimEdenId);
            
            return View(zimmet);
        }

        // Personel kendi zimmetlerini görür
        public async Task<IActionResult> MyZimmetler()
        {
            var userId = _userManager.GetUserId(User);
            var zimmetler = await _context.Zimmetler
                .Include(z => z.Demirbas)
                .Include(z => z.TeslimEden)
                .Include(z => z.TeslimAlan)
                .Where(z => z.PersonelId == userId)
                .OrderByDescending(z => z.ZimmetTarihi)
                .ToListAsync();
            return View(zimmetler);
        }

        // Admin zimmet iade işlemi
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Iade(int id)
        {
            var zimmet = await _context.Zimmetler
                .Include(z => z.Demirbas)
                .Include(z => z.Personel)
                .FirstOrDefaultAsync(z => z.Id == id);
            if (zimmet == null) return NotFound();
            
            // Teslim alan olarak admin kullanıcıları
            var adminUsersIade = await _userManager.GetUsersInRoleAsync("Admin");
            var adminListIade = adminUsersIade.Select(u => new {
                Id = u.Id,
                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
            }).ToList();
            ViewBag.TeslimAlanlar = new SelectList(adminListIade, "Id", "Display");
            
            return View(zimmet);
        }

        [HttpPost, ActionName("Iade")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IadeConfirmed(int id, string TeslimAlanId)
        {
            var zimmet = await _context.Zimmetler.FindAsync(id);
            if (zimmet != null)
            {
                zimmet.IadeTarihi = DateTime.Now;
                zimmet.IsAktif = false;
                zimmet.TeslimAlanId = TeslimAlanId;
                _context.Update(zimmet);
                
                // Demirbaşın durumunu aktif olarak güncelle
                var demirbas = await _context.Demirbaslar.FindAsync(zimmet.DemirbasId);
                if (demirbas != null)
                {
                    demirbas.Durum = Models.Enums.DemirbasDurum.Aktif;
                    _context.Update(demirbas);
                }
                
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        
        // Zimmet detaylarını görüntüleme
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zimmet = await _context.Zimmetler
                .Include(z => z.Demirbas)
                .Include(z => z.Personel)
                .Include(z => z.TeslimEden)
                .Include(z => z.TeslimAlan)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (zimmet == null)
            {
                return NotFound();
            }

            // Personel sadece kendi zimmetlerinin detaylarını görebilir
            if (User.IsInRole("Personel"))
            {
                var userId = _userManager.GetUserId(User);
                if (zimmet.PersonelId != userId)
                {
                    return Forbid();
                }
            }

            return View(zimmet);
        }
    }
} 