using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;
using DemirbaşTakipSistemi.Models.ViewModels;
using DemirbaşTakipSistemi.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize(Roles = "Admin,BilgiIslem")]
    public class AktarmaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AktarmaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Ana aktarma sayfası
        public IActionResult Index()
        {
            return View();
        }

        // Personel arama
        [HttpGet]
        public async Task<IActionResult> PersonelAra(string aramaKriteri)
        {
            if (string.IsNullOrEmpty(aramaKriteri))
            {
                return Json(new List<object>());
            }

            var personeller = await _context.Users
                .Where(u => (u.Ad.Contains(aramaKriteri) || u.Soyad.Contains(aramaKriteri) || 
                           (u.Ad + " " + u.Soyad).Contains(aramaKriteri)) || 
                           (u.SicilNo != null && u.SicilNo.Contains(aramaKriteri)))
                .Select(u => new
                {
                    id = u.Id,
                    adSoyad = u.Ad + " " + u.Soyad,
                    sicilNo = u.SicilNo,
                    departman = u.Departman
                })
                .ToListAsync();

            return Json(personeller);
        }

        // Personelin zimmetli ürünlerini getir
        [HttpGet]
        public async Task<IActionResult> PersonelDemirbaslar(string personelId)
        {
            if (string.IsNullOrEmpty(personelId))
            {
                return Json(new List<object>());
            }

            var zimmetler = await _context.Zimmetler
                .Include(z => z.Demirbas)
                .Where(z => z.PersonelId == personelId && z.IsAktif)
                .Select(z => new
                {
                    zimmetId = z.Id,
                    demirbasId = z.DemirbasId,
                    demirbasKodu = z.Demirbas.DemirbasKodu,
                    demirbasAdi = z.Demirbas.Ad,
                    marka = z.Demirbas.Marka,
                    model = z.Demirbas.Model,
                    zimmetTarihi = z.ZimmetTarihi.ToString("dd.MM.yyyy")
                })
                .ToListAsync();

            return Json(zimmetler);
        }

        // Aktarma işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AktarmaYap(string kaynakPersonelId, string hedefPersonelId, List<int> seciliDemirbaslar)
        {
            if (string.IsNullOrEmpty(kaynakPersonelId) || string.IsNullOrEmpty(hedefPersonelId) || seciliDemirbaslar == null || !seciliDemirbaslar.Any())
            {
                TempData["Error"] = "Gerekli bilgiler eksik!";
                return RedirectToAction(nameof(Index));
            }

            if (kaynakPersonelId == hedefPersonelId)
            {
                TempData["Error"] = "Kaynak ve hedef personel aynı olamaz!";
                return RedirectToAction(nameof(Index));
            }

            var currentUserId = _userManager.GetUserId(User);
            var islemTarihi = DateTime.Now;

            try
            {
                foreach (var demirbasId in seciliDemirbaslar)
                {
                    // Mevcut zimmeti bul
                    var mevcutZimmet = await _context.Zimmetler
                        .Include(z => z.Demirbas)
                        .FirstOrDefaultAsync(z => z.DemirbasId == demirbasId && z.PersonelId == kaynakPersonelId && z.IsAktif);

                    if (mevcutZimmet != null)
                    {
                        // Mevcut zimmeti kapat
                        mevcutZimmet.IsAktif = false;
                        mevcutZimmet.IadeTarihi = islemTarihi;
                        mevcutZimmet.TeslimAlanId = currentUserId;
                        mevcutZimmet.Aciklama = string.IsNullOrEmpty(mevcutZimmet.Aciklama) 
                            ? "Aktarma işlemi ile kapatıldı." 
                            : mevcutZimmet.Aciklama + " | Aktarma işlemi ile kapatıldı.";
                        _context.Update(mevcutZimmet);

                        // Yeni zimmet oluştur
                        var yeniZimmet = new Zimmet
                        {
                            PersonelId = hedefPersonelId,
                            DemirbasId = demirbasId,
                            ZimmetTarihi = islemTarihi,
                            IsAktif = true,
                            TeslimEdenId = currentUserId,
                            Aciklama = "Aktarma işlemi ile oluşturuldu."
                        };
                        _context.Zimmetler.Add(yeniZimmet);

                        // Demirbaş durumunu güncelle (Zimmetli olarak kalsın)
                        if (mevcutZimmet.Demirbas != null)
                        {
                            mevcutZimmet.Demirbas.Durum = DemirbasDurum.Zimmetli;
                            _context.Update(mevcutZimmet.Demirbas);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Aktarma işlemi başarıyla tamamlandı.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Aktarma işlemi sırasında bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}