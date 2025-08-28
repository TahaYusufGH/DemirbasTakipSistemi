using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;
using DemirbaşTakipSistemi.Models.ViewModels;
using System.Linq;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DemirbasRaporu()
        {
            var model = new DemirbasRaporViewModel
            {
                // Demirbaş Durumlarına Göre Dağılım
                DemirbasDurumlari = await _context.Demirbaslar
                    .GroupBy(d => d.Durum)
                    .Select(g => new DemirbasDurumGrubu
                    {
                        Durum = g.Key.ToString(),
                        Sayi = g.Count()
                    })
                    .ToListAsync(),

                // Kategorilere Göre Dağılım
                DemirbasKategorileri = await _context.Demirbaslar
                    .GroupBy(d => d.Kategori)
                    .Select(g => new DemirbasKategoriGrubu
                    {
                        Kategori = g.Key,
                        Sayi = g.Count()
                    })
                    .ToListAsync(),

                // Odalara Göre Dağılım
                OdaDagilimi = await _context.Odalar
                    .Select(o => new OdaDemirbasGrubu
                    {
                        OdaAdi = o.Ad,
                        DemirbasSayisi = o.Demirbaslar.Count
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> ArizaRaporu()
        {
            var model = new ArizaRaporViewModel
            {
                // Arıza Durumlarına Göre Dağılım
                ArizaDurumlari = await _context.Arizalar
                    .GroupBy(a => a.Durum)
                    .Select(g => new ArizaDurumGrubu
                    {
                        Durum = g.Key,
                        Sayi = g.Count()
                    })
                    .ToListAsync(),

                // Demirbaş Bazında Arıza Sayıları
                DemirbasArizalari = await _context.Demirbaslar
                    .Where(d => d.Arizalar.Any())
                    .Select(d => new DemirbasArizaGrubu
                    {
                        DemirbasAdi = d.Ad,
                        ArizaSayisi = d.Arizalar.Count
                    })
                    .OrderByDescending(d => d.ArizaSayisi)
                    .Take(10)
                    .ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> DepoRaporu()
        {
            var model = new DepoRaporViewModel
            {
                // Sarf Malzeme Stok Durumu
                SarfMalzemeStokDurumu = await _context.SarfMalzemeler
                    .Select(s => new SarfMalzemeStokGrubu
                    {
                        MalzemeAdi = s.Ad,
                        Kategori = s.Kategori,
                        MevcutStok = s.MevcutStok,
                        MinimumStok = s.MinimumStok,
                        Birim = s.Birim,
                        StokDurumu = s.MevcutStok <= s.MinimumStok ? "Kritik" : "Normal"
                    })
                    .ToListAsync(),

                // İşlem Tiplerine Göre Dağılım
                IslemTipDagilimi = await _context.DepoIslemler
                    .GroupBy(d => d.IslemTipi)
                    .Select(g => new DepoIslemTipGrubu
                    {
                        IslemTipi = g.Key.ToString(),
                        IslemSayisi = g.Count()
                    })
                    .ToListAsync(),

                // Personele Göre Talep Dağılımı
                PersonelTalepleri = await _context.DepoIslemler
                    .Where(d => d.TalepEden != null)
                    .GroupBy(d => d.TalepEden.Id)
                    .Select(g => new PersonelTalepGrubu
                    {
                        PersonelAdi = g.First().TalepEden.Ad + " " + g.First().TalepEden.Soyad,
                        TalepSayisi = g.Count()
                    })
                    .OrderByDescending(p => p.TalepSayisi)
                    .Take(10)
                    .ToListAsync()
            };

            return View(model);
        }
    }
} 