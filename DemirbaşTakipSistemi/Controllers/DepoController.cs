using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;
using DemirbaşTakipSistemi.Models.Enums;
using DemirbaşTakipSistemi.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize(Roles = "Admin,BilgiIslem")]
    public class DepoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Depo
        public async Task<IActionResult> Index()
        {
            var demirbaslar = await _context.Demirbaslar
                .Include(d => d.Oda)
                .Include(d => d.Zimmetler)
                    .ThenInclude(z => z.Personel)
                .Where(d => d.Durum == Models.Enums.DemirbasDurum.Depoda)
                .ToListAsync();

            var viewModel = new DepoViewModel
            {
                Demirbaslar = demirbaslar,
                SearchModel = new DepoSearchModel()
            };

            // Populate dropdown lists for search form
            ViewData["Kategoriler"] = new SelectList(_context.Demirbaslar.Select(d => d.Kategori).Distinct(), "Kategori");
            ViewData["Durumlar"] = new SelectList(
                Enum.GetValues(typeof(Models.Enums.DemirbasDurum))
                    .Cast<Models.Enums.DemirbasDurum>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");
            ViewData["Odalar"] = new SelectList(_context.Odalar, "Id", "Ad");

            return View(viewModel);
        }

        // GET: Depo/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            ViewData["OdaId"] = new SelectList(_context.Odalar, "Id", "Ad", demirbas.OdaId);
            ViewData["Durumlar"] = new SelectList(
                Enum.GetValues(typeof(Models.Enums.DemirbasDurum))
                    .Cast<Models.Enums.DemirbasDurum>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name", demirbas.Durum);

            return View(demirbas);
        }

        // POST: Depo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DemirbasKodu,Ad,Aciklama,Kategori,Marka,Model,AlimTarihi,Durum,SeriNo,OdaId")] Demirbas demirbas)
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
            
            ViewData["OdaId"] = new SelectList(_context.Odalar, "Id", "Ad", demirbas.OdaId);
            ViewData["Durumlar"] = new SelectList(
                Enum.GetValues(typeof(Models.Enums.DemirbasDurum))
                    .Cast<Models.Enums.DemirbasDurum>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name", demirbas.Durum);
                
            return View(demirbas);
        }

        // GET: Depo/KayitSorgulama
        public IActionResult KayitSorgulama()
        {
            // Populate dropdown lists for search form
            ViewData["Kategoriler"] = new SelectList(_context.Demirbaslar.Select(d => d.Kategori).Distinct(), "Kategori");
            ViewData["Durumlar"] = new SelectList(
                Enum.GetValues(typeof(Models.Enums.DemirbasDurum))
                    .Cast<Models.Enums.DemirbasDurum>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");
            ViewData["Odalar"] = new SelectList(_context.Odalar, "Id", "Ad");

            var viewModel = new DepoViewModel
            {
                Demirbaslar = new List<Demirbas>(),
                SearchModel = new DepoSearchModel()
            };

            return View(viewModel);
        }
        
        // GET: Depo/Bakim
        public async Task<IActionResult> Bakim()
        {
            // Populate dropdown lists for search form
            ViewData["Kategoriler"] = new SelectList(_context.Demirbaslar.Select(d => d.Kategori).Distinct(), "Kategori");
            ViewData["Durumlar"] = new SelectList(
                Enum.GetValues(typeof(Models.Enums.DemirbasDurum))
                    .Cast<Models.Enums.DemirbasDurum>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");
            ViewData["Odalar"] = new SelectList(_context.Odalar, "Id", "Ad");

            // Get all items that need maintenance or are in maintenance
            var demirbaslar = await _context.Demirbaslar
                .Include(d => d.Oda)
                .Include(d => d.Zimmetler)
                    .ThenInclude(z => z.Personel)
                .Include(d => d.Bakimlar)
                .Include(d => d.Arizalar)
                .Where(d => d.Durum == Models.Enums.DemirbasDurum.Bakimda || 
                           d.Arizalar.Any(a => a.Durum == "Beklemede" || a.Durum == "İncelemede"))
                .ToListAsync();

            var viewModel = new DepoViewModel
            {
                Demirbaslar = demirbaslar,
                SearchModel = new DepoSearchModel()
            };

            return View(viewModel);
        }
        
        // GET: Depo/TopluDuzenleme
        public async Task<IActionResult> TopluDuzenleme(string seriNo, string kategori, DemirbasDurum? durum, int? odaId)
        {
            // Kategorileri getir
            var kategoriler = await _context.Demirbaslar
                .Select(d => d.Kategori)
                .Distinct()
                .OrderBy(k => k)
                .ToListAsync();

            // Odaları getir
            var odalar = await _context.Odalar
                .OrderBy(o => o.OdaKodu)
                .ToListAsync();

            // Kullanıcıları getir
            var kullanicilar = await _userManager.Users
                .OrderBy(u => u.Ad)
                .ThenBy(u => u.Soyad)
                .ToListAsync();

            // Demirbaşları filtrele
            var query = _context.Demirbaslar
                .Include(d => d.Oda)
                .AsQueryable();

            if (!string.IsNullOrEmpty(seriNo))
            {
                query = query.Where(d => d.SeriNo != null && d.SeriNo.Contains(seriNo));
            }

            if (!string.IsNullOrEmpty(kategori))
            {
                query = query.Where(d => d.Kategori == kategori);
            }

            if (durum.HasValue)
            {
                query = query.Where(d => d.Durum == durum.Value);
            }

            if (odaId.HasValue)
            {
                query = query.Where(d => d.OdaId == odaId.Value);
            }

            var demirbaslar = await query.ToListAsync();

            var viewModel = new DepoTopluDuzenlemeViewModel
            {
                Demirbaslar = demirbaslar,
                Kategoriler = kategoriler,
                Odalar = odalar,
                Kullanicilar = kullanicilar,
                SeriNo = seriNo,
                Kategori = kategori,
                DurumFiltresi = durum,
                OdaIdFiltresi = odaId,
                GuncellemeTarihi = DateTime.Now
            };

            return View(viewModel);
        }

        // POST: Depo/TopluDuzenleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TopluDuzenleme(DepoTopluDuzenlemeViewModel model)
        {
            if (model.SecilenDemirbasIds == null || !model.SecilenDemirbasIds.Any())
            {
                TempData["ErrorMessage"] = "Lütfen en az bir demirbaş seçiniz.";
                return RedirectToAction(nameof(TopluDuzenleme));
            }

            int guncellenenSayisi = 0;
            var hatalar = new List<string>();

            foreach (var demirbasId in model.SecilenDemirbasIds)
            {
                var demirbas = await _context.Demirbaslar.FindAsync(demirbasId);
                if (demirbas == null)
                {
                    hatalar.Add($"ID: {demirbasId} - Demirbaş bulunamadı.");
                    continue;
                }

                // Sadece dolu alanları güncelle
                if (!string.IsNullOrEmpty(model.EnvanterTuru))
                {
                    demirbas.Kategori = model.EnvanterTuru;
                }

                if (!string.IsNullOrEmpty(model.Birim))
                {
                    // Birim doğrudan Demirbas modelinde yok, açıklama alanına ekleyelim
                    demirbas.Aciklama = (demirbas.Aciklama ?? "") + $" [Birim: {model.Birim}]";
                }

                if (model.OdaId.HasValue)
                {
                    demirbas.OdaId = model.OdaId;
                }

                if (model.Durum.HasValue)
                {
                    demirbas.Durum = model.Durum.Value;
                }

                if (!string.IsNullOrEmpty(model.Aciklama))
                {
                    demirbas.Aciklama = model.Aciklama;
                }

                // Kontrol eden bilgisini kaydet
                if (!string.IsNullOrEmpty(model.KontrolEdenId))
                {
                                                        var kontrolEden = await _userManager.FindByIdAsync(model.KontrolEdenId);
                                    if (kontrolEden != null)
                                    {
                                        string adSoyad = kontrolEden.Ad + " " + kontrolEden.Soyad;
                                        demirbas.Aciklama = (demirbas.Aciklama ?? "") + $" [Kontrol: {adSoyad}, {model.GuncellemeTarihi.ToShortDateString()}]";
                                    }
                }

                _context.Update(demirbas);
                guncellenenSayisi++;
            }

            await _context.SaveChangesAsync();

            if (guncellenenSayisi > 0)
            {
                TempData["SuccessMessage"] = $"{guncellenenSayisi} adet demirbaş başarıyla güncellendi.";
            }

            if (hatalar.Any())
            {
                TempData["ErrorMessage"] = string.Join("<br>", hatalar);
            }

            // Aynı filtreleri koruyarak sayfaya geri dön
            return RedirectToAction(nameof(TopluDuzenleme), new 
            { 
                seriNo = model.SeriNo, 
                kategori = model.Kategori, 
                durum = model.DurumFiltresi, 
                odaId = model.OdaIdFiltresi 
            });
        }

        // GET: Depo/TopluEkleme
        public async Task<IActionResult> TopluEkleme()
        {
            // Kategorileri getir
            var kategoriler = await _context.Demirbaslar
                .Select(d => d.Kategori)
                .Distinct()
                .OrderBy(k => k)
                .ToListAsync();

            // Odaları getir
            var odalar = await _context.Odalar
                .OrderBy(o => o.OdaKodu)
                .ToListAsync();

            var viewModel = new DepoTopluEklemeViewModel
            {
                Kategoriler = kategoriler,
                Odalar = odalar,
                AlimTarihi = DateTime.Now,
                Durum = DemirbasDurum.Depoda
            };

            return View(viewModel);
        }

        // POST: Depo/TopluEkleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TopluEkleme(DepoTopluEklemeViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Seri numaralarını ayır
                var seriNumaralari = model.SeriNumaralari
                    .Split(new[] { ',', '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

                if (seriNumaralari.Count == 0)
                {
                    ModelState.AddModelError("SeriNumaralari", "En az bir seri numarası girilmelidir.");
                    
                    // Kategorileri ve odaları yeniden yükle
                    model.Kategoriler = await _context.Demirbaslar
                        .Select(d => d.Kategori)
                        .Distinct()
                        .OrderBy(k => k)
                        .ToListAsync();

                    model.Odalar = await _context.Odalar
                        .OrderBy(o => o.OdaKodu)
                        .ToListAsync();
                        
                    return View(model);
                }

                int basariliEkleme = 0;
                var hatalar = new List<string>();

                foreach (var seriNo in seriNumaralari)
                {
                    // Aynı seri numaralı demirbaş var mı kontrol et
                    var mevcutDemirbas = await _context.Demirbaslar
                        .FirstOrDefaultAsync(d => d.SeriNo == seriNo);

                    if (mevcutDemirbas != null)
                    {
                        hatalar.Add($"Seri No: {seriNo} - Bu seri numaralı demirbaş zaten mevcut.");
                        continue;
                    }

                    // Yeni demirbaş oluştur
                    var yeniDemirbas = new Demirbas
                    {
                        DemirbasKodu = "DMB-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 4),
                        Ad = model.Kategori + (string.IsNullOrEmpty(model.AltKategori) ? "" : " " + model.AltKategori),
                        Kategori = model.Kategori,
                        Model = model.AltKategori,
                        Marka = model.Marka ?? "Belirtilmemiş",
                        SeriNo = seriNo,
                        AlimTarihi = model.AlimTarihi,
                        Durum = model.Durum,
                        OdaId = model.OdaId,
                        Aciklama = model.Aciklama
                    };

                    _context.Add(yeniDemirbas);
                    basariliEkleme++;
                }

                await _context.SaveChangesAsync();

                if (basariliEkleme > 0)
                {
                    TempData["SuccessMessage"] = $"{basariliEkleme} adet demirbaş başarıyla eklendi.";
                }

                if (hatalar.Any())
                {
                    TempData["ErrorMessage"] = string.Join("<br>", hatalar);
                }

                return RedirectToAction(nameof(Index));
            }

            // ModelState geçerli değilse, view model'i yeniden doldur
            model.Kategoriler = await _context.Demirbaslar
                .Select(d => d.Kategori)
                .Distinct()
                .OrderBy(k => k)
                .ToListAsync();

            model.Odalar = await _context.Odalar
                .OrderBy(o => o.OdaKodu)
                .ToListAsync();

            return View(model);
        }

        // GET: Depo/EvveliyatArsivi
        public async Task<IActionResult> EvveliyatArsivi(string seriNo, string demirbasKodu, string kategori, DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            // Prepare query
            var query = _context.Demirbaslar
                .Include(d => d.Oda)
                .Include(d => d.Zimmetler)
                    .ThenInclude(z => z.Personel)
                .Include(d => d.Bakimlar)
                .Include(d => d.Arizalar)
                .AsQueryable();
            
            // Apply filters
            if (!string.IsNullOrEmpty(seriNo))
            {
                query = query.Where(d => d.SeriNo != null && d.SeriNo.Contains(seriNo));
            }
            
            if (!string.IsNullOrEmpty(demirbasKodu))
            {
                query = query.Where(d => d.DemirbasKodu.Contains(demirbasKodu));
            }
            
            if (!string.IsNullOrEmpty(kategori))
            {
                query = query.Where(d => d.Kategori.Contains(kategori));
            }
            
            // Get results
            var demirbaslar = await query.ToListAsync();
            
            // Filter history records by date if specified
            if (baslangicTarihi.HasValue || bitisTarihi.HasValue)
            {
                foreach (var demirbas in demirbaslar)
                {
                    // Filter Zimmet records
                    if (baslangicTarihi.HasValue)
                    {
                        demirbas.Zimmetler = demirbas.Zimmetler
                            .Where(z => z.ZimmetTarihi >= baslangicTarihi.Value)
                            .ToList();
                    }
                    
                    if (bitisTarihi.HasValue)
                    {
                        demirbas.Zimmetler = demirbas.Zimmetler
                            .Where(z => z.ZimmetTarihi <= bitisTarihi.Value)
                            .ToList();
                    }
                    
                    // Filter Ariza records
                    if (baslangicTarihi.HasValue)
                    {
                        demirbas.Arizalar = demirbas.Arizalar
                            .Where(a => a.ArizaTarihi >= baslangicTarihi.Value)
                            .ToList();
                    }
                    
                    if (bitisTarihi.HasValue)
                    {
                        demirbas.Arizalar = demirbas.Arizalar
                            .Where(a => a.ArizaTarihi <= bitisTarihi.Value)
                            .ToList();
                    }
                    
                    // Filter Bakim records
                    if (baslangicTarihi.HasValue)
                    {
                        demirbas.Bakimlar = demirbas.Bakimlar
                            .Where(b => b.BakimTarihi >= baslangicTarihi.Value)
                            .ToList();
                    }
                    
                    if (bitisTarihi.HasValue)
                    {
                        demirbas.Bakimlar = demirbas.Bakimlar
                            .Where(b => b.BakimTarihi <= bitisTarihi.Value)
                            .ToList();
                    }
                }
            }
            
            // Populate dropdown lists for search form
            ViewData["Kategoriler"] = new SelectList(_context.Demirbaslar.Select(d => d.Kategori).Distinct(), "Kategori");
            
            var viewModel = new DepoViewModel
            {
                Demirbaslar = demirbaslar,
                SearchModel = new DepoSearchModel 
                { 
                    SeriNo = seriNo,
                    Kategori = kategori
                }
            };
            
            // Add date filters to ViewBag
            ViewBag.BaslangicTarihi = baslangicTarihi?.ToString("yyyy-MM-dd");
            ViewBag.BitisTarihi = bitisTarihi?.ToString("yyyy-MM-dd");
            ViewBag.DemirbasKodu = demirbasKodu;
            
            return View(viewModel);
        }

        // GET: Depo/Search
        public async Task<IActionResult> Search(string seriNo, string kategori, string altKategori, string durum, string birim, string odaNo, string odaTuru, string sicilNo, string personelAdi)
        {
            var query = _context.Demirbaslar
                .Include(d => d.Oda)
                .Include(d => d.Zimmetler)
                    .ThenInclude(z => z.Personel)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(seriNo))
            {
                query = query.Where(d => d.SeriNo != null && d.SeriNo.Contains(seriNo));
            }

            if (!string.IsNullOrEmpty(kategori))
            {
                query = query.Where(d => d.Kategori.Contains(kategori));
            }

            if (!string.IsNullOrEmpty(altKategori))
            {
                query = query.Where(d => d.Model != null && d.Model.Contains(altKategori));
            }

            if (!string.IsNullOrEmpty(durum))
            {
                if (Enum.TryParse<Models.Enums.DemirbasDurum>(durum, out var durumEnum))
                {
                    query = query.Where(d => d.Durum == durumEnum);
                }
            }

            if (!string.IsNullOrEmpty(odaNo) && int.TryParse(odaNo, out int odaId))
            {
                query = query.Where(d => d.OdaId == odaId);
            }

            if (!string.IsNullOrEmpty(odaTuru))
            {
                query = query.Where(d => d.Oda != null && d.Oda.Ad.Contains(odaTuru));
            }

            if (!string.IsNullOrEmpty(sicilNo))
            {
                query = query.Where(d => d.Zimmetler.Any(z => z.Personel != null && z.Personel.SicilNo != null && z.Personel.SicilNo.Contains(sicilNo)));
            }

            if (!string.IsNullOrEmpty(personelAdi))
            {
                query = query.Where(d => d.Zimmetler.Any(z => 
                    z.Personel != null && (z.Personel.Ad + " " + z.Personel.Soyad).Contains(personelAdi)));
            }

            var results = await query.ToListAsync();
            
            // Populate dropdown lists for search form
            ViewData["Kategoriler"] = new SelectList(_context.Demirbaslar.Select(d => d.Kategori).Distinct(), "Kategori");
            ViewData["Durumlar"] = new SelectList(
                Enum.GetValues(typeof(Models.Enums.DemirbasDurum))
                    .Cast<Models.Enums.DemirbasDurum>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");
            ViewData["Odalar"] = new SelectList(_context.Odalar, "Id", "Ad");
            
            var viewModel = new DepoViewModel
            {
                Demirbaslar = results,
                SearchModel = new DepoSearchModel 
                { 
                    SeriNo = seriNo,
                    Kategori = kategori,
                    AltKategori = altKategori,
                    Birim = birim,
                    OdaNo = odaNo,
                    OdaTuru = odaTuru,
                    SicilNo = sicilNo,
                    PersonelAdi = personelAdi
                }
            };
            
            return View("KayitSorgulama", viewModel);
        }

        private bool DemirbasExists(int id)
        {
            return _context.Demirbaslar.Any(e => e.Id == id);
        }
    }
} 