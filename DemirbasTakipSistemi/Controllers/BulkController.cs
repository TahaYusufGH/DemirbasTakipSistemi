using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;
using DemirbaşTakipSistemi.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BulkController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BulkController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ImportDemirbas()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportDemirbas(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Lütfen bir dosya seçiniz.";
                return RedirectToAction(nameof(ImportDemirbas));
            }

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Lütfen bir CSV dosyası seçiniz.";
                return RedirectToAction(nameof(ImportDemirbas));
            }

            var importedItems = new List<DemirbasImportModel>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null
            };

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, config))
                {
                    importedItems = csv.GetRecords<DemirbasImportModel>().ToList();
                }

                int successCount = 0;
                int errorCount = 0;
                var errors = new List<string>();

                foreach (var item in importedItems)
                {
                    try
                    {
                        // Check if demirbaş with the same code already exists
                        if (await _context.Demirbaslar.AnyAsync(d => d.DemirbasKodu == item.DemirbasKodu))
                        {
                            errors.Add($"Demirbaş kodu '{item.DemirbasKodu}' zaten mevcut.");
                            errorCount++;
                            continue;
                        }

                        var demirbas = new Demirbas
                        {
                            DemirbasKodu = item.DemirbasKodu,
                            Ad = item.Ad,
                            Aciklama = item.Aciklama,
                            Kategori = item.Kategori,
                            Marka = item.Marka,
                            Model = item.Model,
                            AlimTarihi = item.AlimTarihi,
                            SeriNo = item.SeriNo,
                            Durum = Models.Enums.DemirbasDurum.Aktif
                        };

                        // If OdaKodu is provided, find the corresponding Oda
                        if (!string.IsNullOrEmpty(item.OdaKodu))
                        {
                            var oda = await _context.Odalar.FirstOrDefaultAsync(o => o.OdaKodu == item.OdaKodu);
                            if (oda != null)
                            {
                                demirbas.OdaId = oda.Id;
                            }
                            else
                            {
                                errors.Add($"Oda kodu '{item.OdaKodu}' bulunamadı. Demirbaş '{item.DemirbasKodu}' için oda ataması yapılmadı.");
                            }
                        }

                        _context.Add(demirbas);
                        await _context.SaveChangesAsync();
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Demirbaş '{item.DemirbasKodu}' eklenirken hata: {ex.Message}");
                        errorCount++;
                    }
                }

                TempData["SuccessMessage"] = $"{successCount} demirbaş başarıyla içe aktarıldı.";
                if (errorCount > 0)
                {
                    TempData["ErrorMessage"] = $"{errorCount} demirbaş içe aktarılamadı. Hatalar: {string.Join(" | ", errors)}";
                }

                return RedirectToAction("Index", "Demirbas");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Dosya işlenirken hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(ImportDemirbas));
            }
        }

        public IActionResult ExportDemirbas()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportDemirbas(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                TempData["ErrorMessage"] = "Lütfen bir dosya formatı seçiniz.";
                return RedirectToAction(nameof(ExportDemirbas));
            }

            var demirbaslar = await _context.Demirbaslar
                .Include(d => d.Oda)
                .Include(d => d.Zimmetler)
                    .ThenInclude(z => z.Personel)
                .ToListAsync();

            var exportData = demirbaslar.Select(d => new DemirbasExportModel
            {
                DemirbasKodu = d.DemirbasKodu,
                Ad = d.Ad,
                Aciklama = d.Aciklama,
                Kategori = d.Kategori,
                Marka = d.Marka,
                Model = d.Model,
                AlimTarihi = d.AlimTarihi.ToString("dd.MM.yyyy"),
                Durum = d.Durum.ToString(),
                SeriNo = d.SeriNo,
                OdaAdi = d.Oda?.Ad,
                OdaKodu = d.Oda?.OdaKodu,
                ZimmetliPersonel = d.Zimmetler.FirstOrDefault(z => z.IsAktif)?.Personel != null ? 
                    $"{d.Zimmetler.FirstOrDefault(z => z.IsAktif)?.Personel.Ad} {d.Zimmetler.FirstOrDefault(z => z.IsAktif)?.Personel.Soyad}" : null
            }).ToList();

            string fileName = $"Demirbaslar_{DateTime.Now:yyyyMMddHHmmss}";

            if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                return await ExportToCsv(exportData, fileName);
            }
            else
            {
                TempData["ErrorMessage"] = "Desteklenmeyen dosya formatı.";
                return RedirectToAction(nameof(ExportDemirbas));
            }
        }

        public IActionResult ImportSarfMalzeme()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportSarfMalzeme(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Lütfen bir dosya seçiniz.";
                return RedirectToAction(nameof(ImportSarfMalzeme));
            }

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Lütfen bir CSV dosyası seçiniz.";
                return RedirectToAction(nameof(ImportSarfMalzeme));
            }

            var importedItems = new List<SarfMalzemeImportModel>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null
            };

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, config))
                {
                    importedItems = csv.GetRecords<SarfMalzemeImportModel>().ToList();
                }

                int successCount = 0;
                int errorCount = 0;
                var errors = new List<string>();

                foreach (var item in importedItems)
                {
                    try
                    {
                        // Check if sarf malzeme with the same code already exists
                        if (await _context.SarfMalzemeler.AnyAsync(s => s.MalzemeKodu == item.MalzemeKodu))
                        {
                            errors.Add($"Malzeme kodu '{item.MalzemeKodu}' zaten mevcut.");
                            errorCount++;
                            continue;
                        }

                        var sarfMalzeme = new SarfMalzeme
                        {
                            MalzemeKodu = item.MalzemeKodu,
                            Ad = item.Ad,
                            Aciklama = item.Aciklama,
                            Kategori = item.Kategori,
                            Marka = item.Marka,
                            Model = item.Model,
                            Birim = item.Birim,
                            MinimumStok = item.MinimumStok,
                            MevcutStok = item.MevcutStok,
                            SonGuncelleme = DateTime.Now
                        };

                        _context.Add(sarfMalzeme);
                        await _context.SaveChangesAsync();
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Sarf malzeme '{item.MalzemeKodu}' eklenirken hata: {ex.Message}");
                        errorCount++;
                    }
                }

                TempData["SuccessMessage"] = $"{successCount} sarf malzeme başarıyla içe aktarıldı.";
                if (errorCount > 0)
                {
                    TempData["ErrorMessage"] = $"{errorCount} sarf malzeme içe aktarılamadı. Hatalar: {string.Join(" | ", errors)}";
                }

                return RedirectToAction("Index", "SarfMalzeme");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Dosya işlenirken hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(ImportSarfMalzeme));
            }
        }

        public IActionResult ExportSarfMalzeme()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportSarfMalzeme(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                TempData["ErrorMessage"] = "Lütfen bir dosya formatı seçiniz.";
                return RedirectToAction(nameof(ExportSarfMalzeme));
            }

            var sarfMalzemeler = await _context.SarfMalzemeler.ToListAsync();

            var exportData = sarfMalzemeler.Select(s => new SarfMalzemeExportModel
            {
                MalzemeKodu = s.MalzemeKodu,
                Ad = s.Ad,
                Aciklama = s.Aciklama,
                Kategori = s.Kategori,
                Marka = s.Marka,
                Model = s.Model,
                Birim = s.Birim,
                MinimumStok = s.MinimumStok,
                MevcutStok = s.MevcutStok,
                SonGuncelleme = s.SonGuncelleme.ToString("dd.MM.yyyy HH:mm")
            }).ToList();

            string fileName = $"SarfMalzemeler_{DateTime.Now:yyyyMMddHHmmss}";

            if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                return await ExportToCsv(exportData, fileName);
            }
            else
            {
                TempData["ErrorMessage"] = "Desteklenmeyen dosya formatı.";
                return RedirectToAction(nameof(ExportSarfMalzeme));
            }
        }

        private async Task<FileContentResult> ExportToCsv<T>(List<T> data, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(data);
                await writer.FlushAsync();
                return File(memoryStream.ToArray(), "text/csv", $"{fileName}.csv");
            }
        }
    }
} 