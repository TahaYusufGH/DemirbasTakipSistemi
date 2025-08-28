using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;
using DemirbaşTakipSistemi.Models.ViewModels;
using DemirbaşTakipSistemi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize(Roles = "Admin,BilgiIslem")]
    public class DepoSarfMalzemeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepoSarfMalzemeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DepoSarfMalzeme
        public async Task<IActionResult> Index()
        {
            var sarfMalzemeler = await _context.SarfMalzemeler
                .OrderBy(s => s.Kategori)
                .ThenBy(s => s.Ad)
                .ToListAsync();

            return View(sarfMalzemeler);
        }

        // GET: DepoSarfMalzeme/Ayarlar
        public async Task<IActionResult> Ayarlar()
        {
            var kategoriler = await _context.SarfMalzemeler
                .Select(s => s.Kategori)
                .Distinct()
                .OrderBy(k => k)
                .ToListAsync();

            var viewModel = new SarfMalzemeAyarlarViewModel
            {
                Kategoriler = kategoriler,
                YeniKategori = "",
                YeniMalzeme = new SarfMalzeme()
            };

            return View(viewModel);
        }

        // POST: DepoSarfMalzeme/YeniKategoriEkle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YeniKategoriEkle(string yeniKategori)
        {
            if (!string.IsNullOrEmpty(yeniKategori))
            {
                // Kategori zaten var mı kontrol et
                var kategoriVarMi = await _context.SarfMalzemeler
                    .AnyAsync(s => s.Kategori.ToLower() == yeniKategori.ToLower());

                if (!kategoriVarMi)
                {
                    // Yeni kategori için örnek bir malzeme ekle
                    var yeniMalzeme = new SarfMalzeme
                    {
                        MalzemeKodu = "TEMP-" + Guid.NewGuid().ToString().Substring(0, 8),
                        Ad = "Kategori Tanımlayıcı",
                        Kategori = yeniKategori,
                        Birim = "Adet",
                        Marka = "Genel",
                        MinimumStok = 0,
                        MevcutStok = 0,
                        Aciklama = "Bu malzeme yeni kategori oluşturmak için otomatik olarak eklendi. Düzenleyebilir veya silebilirsiniz."
                    };

                    _context.Add(yeniMalzeme);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"'{yeniKategori}' kategorisi başarıyla eklendi.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"'{yeniKategori}' kategorisi zaten mevcut.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Kategori adı boş olamaz.";
            }

            return RedirectToAction(nameof(Ayarlar));
        }

        // POST: DepoSarfMalzeme/YeniMalzemeEkle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YeniMalzemeEkle(SarfMalzeme yeniMalzeme)
        {
            if (ModelState.IsValid)
            {
                yeniMalzeme.SonGuncelleme = DateTime.Now;
                _context.Add(yeniMalzeme);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"'{yeniMalzeme.Ad}' malzemesi başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            var kategoriler = await _context.SarfMalzemeler
                .Select(s => s.Kategori)
                .Distinct()
                .OrderBy(k => k)
                .ToListAsync();

            var viewModel = new SarfMalzemeAyarlarViewModel
            {
                Kategoriler = kategoriler,
                YeniKategori = "",
                YeniMalzeme = yeniMalzeme
            };

            return View("Ayarlar", viewModel);
        }

        // GET: DepoSarfMalzeme/MalzemeVer
        public async Task<IActionResult> MalzemeVer()
        {
            var sarfMalzemeler = await _context.SarfMalzemeler
                .Where(s => s.MevcutStok > 0)
                .OrderBy(s => s.Kategori)
                .ThenBy(s => s.Ad)
                .ToListAsync();

            var personeller = await _userManager.Users
                .OrderBy(u => u.Ad)
                .ThenBy(u => u.Soyad)
                .ToListAsync();

            var odalar = await _context.Odalar
                .OrderBy(o => o.OdaKodu)
                .ToListAsync();

            var viewModel = new SarfMalzemeVerViewModel
            {
                SarfMalzemeler = sarfMalzemeler,
                Personeller = personeller,
                Odalar = odalar,
                SecilenMalzemeId = 0,
                SecilenPersonelId = "",
                SecilenOdaId = 0,
                Miktar = 1,
                Aciklama = ""
            };

            return View(viewModel);
        }

        // POST: DepoSarfMalzeme/MalzemeVer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MalzemeVer(SarfMalzemeVerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sarfMalzeme = await _context.SarfMalzemeler.FindAsync(model.SecilenMalzemeId);
                if (sarfMalzeme == null)
                {
                    TempData["ErrorMessage"] = "Seçilen malzeme bulunamadı.";
                    return RedirectToAction(nameof(MalzemeVer));
                }

                if (sarfMalzeme.MevcutStok < model.Miktar)
                {
                    TempData["ErrorMessage"] = $"Yetersiz stok. Mevcut stok: {sarfMalzeme.MevcutStok} {sarfMalzeme.Birim}";
                    return RedirectToAction(nameof(MalzemeVer));
                }

                var personel = await _userManager.FindByIdAsync(model.SecilenPersonelId);
                if (personel == null)
                {
                    TempData["ErrorMessage"] = "Seçilen personel bulunamadı.";
                    return RedirectToAction(nameof(MalzemeVer));
                }

                var currentUser = await _userManager.GetUserAsync(User);

                // Depo İşlemi Oluştur
                var depoIslem = new DepoIslem
                {
                    SarfMalzemeId = sarfMalzeme.Id,
                    IslemTipi = DepoIslemTip.Cikis,
                    IslemTarihi = DateTime.Now,
                    Miktar = model.Miktar,
                    Aciklama = model.Aciklama,
                    IslemYapanId = currentUser.Id,
                    TalepEdenId = model.SecilenPersonelId,
                    OnayDurumu = "Onaylandı",
                    OnaylayanId = currentUser.Id,
                    OnayTarihi = DateTime.Now
                };

                if (model.SecilenOdaId > 0)
                {
                    var oda = await _context.Odalar.FindAsync(model.SecilenOdaId);
                    if (oda != null)
                    {
                        depoIslem.Aciklama += $" (Oda: {oda.OdaKodu} - {oda.Ad})";
                    }
                }

                _context.Add(depoIslem);

                // Stok Güncelle
                sarfMalzeme.MevcutStok -= model.Miktar;
                sarfMalzeme.SonGuncelleme = DateTime.Now;
                _context.Update(sarfMalzeme);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{model.Miktar} {sarfMalzeme.Birim} {sarfMalzeme.Ad}, {personel.Ad} {personel.Soyad} personeline verildi.";
                return RedirectToAction(nameof(MalzemeVer));
            }

            // ModelState geçerli değilse, view model'i yeniden doldur
            var sarfMalzemeler = await _context.SarfMalzemeler
                .Where(s => s.MevcutStok > 0)
                .OrderBy(s => s.Kategori)
                .ThenBy(s => s.Ad)
                .ToListAsync();

            var personeller = await _userManager.Users
                .OrderBy(u => u.Ad)
                .ThenBy(u => u.Soyad)
                .ToListAsync();

            var odalar = await _context.Odalar
                .OrderBy(o => o.OdaKodu)
                .ToListAsync();

            model.SarfMalzemeler = sarfMalzemeler;
            model.Personeller = personeller;
            model.Odalar = odalar;

            return View(model);
        }

        // GET: DepoSarfMalzeme/MalzemeArsivi
        public async Task<IActionResult> MalzemeArsivi(DateTime? baslangicTarihi, DateTime? bitisTarihi, string personelId, int? sarfMalzemeId)
        {
            var query = _context.DepoIslemler
                .Include(d => d.SarfMalzeme)
                .Include(d => d.IslemYapan)
                .Include(d => d.TalepEden)
                .Include(d => d.Onaylayan)
                .Where(d => d.IslemTipi == DepoIslemTip.Cikis)
                .OrderByDescending(d => d.IslemTarihi)
                .AsQueryable();

            if (baslangicTarihi.HasValue)
            {
                query = query.Where(d => d.IslemTarihi >= baslangicTarihi.Value);
            }

            if (bitisTarihi.HasValue)
            {
                query = query.Where(d => d.IslemTarihi <= bitisTarihi.Value.AddDays(1).AddSeconds(-1));
            }

            if (!string.IsNullOrEmpty(personelId))
            {
                query = query.Where(d => d.TalepEdenId == personelId);
            }

            if (sarfMalzemeId.HasValue)
            {
                query = query.Where(d => d.SarfMalzemeId == sarfMalzemeId.Value);
            }

            var islemler = await query.ToListAsync();

            var personeller = await _userManager.Users
                .OrderBy(u => u.Ad)
                .ThenBy(u => u.Soyad)
                .ToListAsync();

            var sarfMalzemeler = await _context.SarfMalzemeler
                .OrderBy(s => s.Kategori)
                .ThenBy(s => s.Ad)
                .ToListAsync();

            var viewModel = new SarfMalzemeArsivViewModel
            {
                DepoIslemler = islemler,
                Personeller = personeller,
                SarfMalzemeler = sarfMalzemeler,
                BaslangicTarihi = baslangicTarihi,
                BitisTarihi = bitisTarihi,
                SecilenPersonelId = personelId,
                SecilenSarfMalzemeId = sarfMalzemeId
            };

            return View(viewModel);
        }
    }
}