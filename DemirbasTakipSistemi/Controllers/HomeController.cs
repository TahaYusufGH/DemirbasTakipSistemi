using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;
using DemirbaşTakipSistemi.Models.ViewModels;
using DemirbaşTakipSistemi.Models.Enums;
using System.Threading.Tasks;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isBilgiIslem = await _userManager.IsInRoleAsync(user, "BilgiIslem");

            if (isAdmin)
            {
                return await AdminDashboard();
            }
            else if (isBilgiIslem)
            {
                return await BilgiIslemDashboard();
            }
            else
            {
                return await PersonelDashboard();
            }
        }

        private async Task<IActionResult> AdminDashboard()
        {
            var model = new AdminDashboardViewModel
            {
                // Demirbaş İstatistikleri
                ToplamDemirbasSayisi = await _context.Demirbaslar.CountAsync(),
                ZimmetliDemirbasSayisi = await _context.Demirbaslar.CountAsync(d => d.Durum == Models.Enums.DemirbasDurum.Zimmetli),
                ArizaliDemirbasSayisi = await _context.Demirbaslar.CountAsync(d => d.Durum == Models.Enums.DemirbasDurum.Arizali),
                BakimdakiDemirbasSayisi = await _context.Demirbaslar.CountAsync(d => d.Durum == Models.Enums.DemirbasDurum.Bakimda),
                HurdaDemirbasSayisi = await _context.Demirbaslar.CountAsync(d => d.Durum == Models.Enums.DemirbasDurum.Hurda),

                // Arıza İstatistikleri
                BekleyenArizaSayisi = await _context.Arizalar.CountAsync(a => a.Durum == "Beklemede"),
                IncelenenArizaSayisi = await _context.Arizalar.CountAsync(a => a.Durum == "İncelemede"),
                CozulenArizaSayisi = await _context.Arizalar.CountAsync(a => a.Durum == "Çözüldü"),

                // Depo İşlem İstatistikleri
                BekleyenDepoIslemSayisi = await _context.DepoIslemler.CountAsync(d => d.OnayDurumu == "Beklemede"),
                OnaylananDepoIslemSayisi = await _context.DepoIslemler.CountAsync(d => d.OnayDurumu == "Onaylandı"),
                ReddedilenDepoIslemSayisi = await _context.DepoIslemler.CountAsync(d => d.OnayDurumu == "Reddedildi"),

                // Stok Uyarıları
                MinimumStokAltindakiMalzemeler = await _context.SarfMalzemeler
                    .Where(s => s.MevcutStok <= s.MinimumStok)
                    .OrderBy(s => s.Kategori)
                    .ThenBy(s => s.Ad)
                    .ToListAsync(),

                // Son Arızalar
                SonArizalar = await _context.Arizalar
                    .Include(a => a.Demirbas)
                    .Include(a => a.Bildiren)
                    .OrderByDescending(a => a.ArizaTarihi)
                    .Take(5)
                    .ToListAsync(),

                // Son Depo İşlemleri
                SonDepoIslemleri = await _context.DepoIslemler
                    .Include(d => d.SarfMalzeme)
                    .Include(d => d.IslemYapan)
                    .Include(d => d.TalepEden)
                    .OrderByDescending(d => d.IslemTarihi)
                    .Take(5)
                    .ToListAsync()
            };

            return View("AdminDashboard", model);
        }

        private async Task<IActionResult> BilgiIslemDashboard()
        {
            var model = new BilgiIslemDashboardViewModel
            {
                // Arıza İstatistikleri
                BekleyenArizaSayisi = await _context.Arizalar.CountAsync(a => a.Durum == "Beklemede"),
                IncelenenArizaSayisi = await _context.Arizalar.CountAsync(a => a.Durum == "İncelemede"),
                CozulenArizaSayisi = await _context.Arizalar.CountAsync(a => a.Durum == "Çözüldü"),
                
                // Demirbaş İstatistikleri
                ToplamDemirbasSayisi = await _context.Demirbaslar.CountAsync(),
                ZimmetliDemirbasSayisi = await _context.Demirbaslar.CountAsync(d => d.Durum == Models.Enums.DemirbasDurum.Zimmetli),
                ArizaliDemirbasSayisi = await _context.Demirbaslar.CountAsync(d => d.Durum == Models.Enums.DemirbasDurum.Arizali),
                
                // Stok Uyarıları
                MinimumStokAltindakiMalzemeler = await _context.SarfMalzemeler
                    .Where(s => s.MevcutStok <= s.MinimumStok)
                    .OrderBy(s => s.Kategori)
                    .ThenBy(s => s.Ad)
                    .ToListAsync(),

                // Bekleyen Arızalar
                BekleyenArizalar = await _context.Arizalar
                    .Include(a => a.Demirbas)
                    .Include(a => a.Bildiren)
                    .Where(a => a.Durum == "Beklemede" || a.Durum == "İncelemede")
                    .OrderByDescending(a => a.ArizaTarihi)
                    .ToListAsync(),

                // Son Çözülen Arızalar
                SonCozulenArizalar = await _context.Arizalar
                    .Include(a => a.Demirbas)
                    .Include(a => a.Bildiren)
                    .Include(a => a.CozenPersonel)
                    .Where(a => a.Durum == "Çözüldü")
                    .OrderByDescending(a => a.CozumTarihi)
                    .Take(5)
                    .ToListAsync(),
                    
                // Bakım Gerektiren Demirbaşlar
                BakimGerekenDemirbaslar = await _context.Bakimlar
                    .Include(b => b.Demirbas)
                    .Where(b => b.SonrakiBakimTarihi.HasValue && b.SonrakiBakimTarihi.Value <= DateTime.Now.AddDays(30))
                    .OrderBy(b => b.SonrakiBakimTarihi)
                    .Take(10)
                    .ToListAsync()
            };

            return View("BilgiIslemDashboard", model);
        }

        private async Task<IActionResult> PersonelDashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new PersonelDashboardViewModel
            {
                // Zimmetli Demirbaşlar
                ZimmetliDemirbaslar = await _context.Zimmetler
                    .Include(z => z.Demirbas)
                    .Where(z => z.PersonelId == user.Id && z.IsAktif)
                    .Select(z => z.Demirbas)
                    .ToListAsync(),

                // Arıza Bildirimleri
                Arizalar = await _context.Arizalar
                    .Include(a => a.Demirbas)
                    .Where(a => a.BildirenId == user.Id)
                    .OrderByDescending(a => a.ArizaTarihi)
                    .Take(5)
                    .ToListAsync(),

                // Depo İşlemleri
                DepoIslemleri = await _context.DepoIslemler
                    .Include(d => d.SarfMalzeme)
                    .Where(d => d.TalepEdenId == user.Id)
                    .OrderByDescending(d => d.IslemTarihi)
                    .Take(5)
                    .ToListAsync(),
                    
                // Departmandaki Demirbaşlar
                DepartmanDemirbaslari = user.Departman != null
                    ? await _context.Zimmetler
                        .Include(z => z.Demirbas)
                        .Include(z => z.Personel)
                        .Where(z => z.Personel.Departman == user.Departman && z.IsAktif)
                        .Select(z => z.Demirbas)
                        .Distinct()
                        .Take(10)
                        .ToListAsync()
                    : new List<Demirbas>()
            };

            return View("PersonelDashboard", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
} 