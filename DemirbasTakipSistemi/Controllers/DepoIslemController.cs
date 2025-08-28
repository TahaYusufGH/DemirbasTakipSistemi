using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;
using DemirbaşTakipSistemi.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize(Roles = "Admin,BilgiIslem")]
    public class DepoIslemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepoIslemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DepoIslem
        public async Task<IActionResult> Index()
        {
            var depoIslemler = await _context.DepoIslemler
                .Include(d => d.SarfMalzeme)
                .Include(d => d.IslemYapan)
                .Include(d => d.TalepEden)
                .Include(d => d.Onaylayan)
                .OrderByDescending(d => d.IslemTarihi)
                .ToListAsync();

            return View(depoIslemler);
        }

        // GET: DepoIslem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depoIslem = await _context.DepoIslemler
                .Include(d => d.SarfMalzeme)
                .Include(d => d.IslemYapan)
                .Include(d => d.TalepEden)
                .Include(d => d.Onaylayan)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (depoIslem == null)
            {
                return NotFound();
            }

            return View(depoIslem);
        }

        // GET: DepoIslem/Create
        public async Task<IActionResult> Create()
        {
            var sarfMalzemeList = await _context.SarfMalzemeler
                .Select(s => new {
                    Id = s.Id,
                    Display = $"{s.Ad} - {s.Kategori} (Stok: {s.MevcutStok} {s.Birim})"
                }).ToListAsync();
            ViewBag.SarfMalzemeId = new SelectList(sarfMalzemeList, "Id", "Display");
            
            var personelUsers = await _userManager.GetUsersInRoleAsync("Personel");
            var personelList = personelUsers.Select(u => new {
                Id = u.Id,
                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
            }).ToList();
            ViewBag.TalepEdenId = new SelectList(personelList, "Id", "Display");
            
            // İşlem tipleri için enum değerlerini SelectList olarak hazırla
            ViewBag.IslemTipleri = new SelectList(Enum.GetValues(typeof(DepoIslemTip))
                .Cast<DepoIslemTip>()
                .Select(t => new { Id = (int)t, Name = t.ToString() }), "Id", "Name");
                
            return View();
        }

        // POST: DepoIslem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SarfMalzemeId,IslemTipi,Miktar,Aciklama,TalepEdenId,BelgeNo,Tedarikci")] DepoIslem depoIslem)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["Error"] = "Kullanıcı bilgisi alınamadı.";
                    return RedirectToAction(nameof(Index));
                }
                
                depoIslem.IslemYapanId = user.Id;
                depoIslem.IslemTarihi = DateTime.Now;
                depoIslem.OnayDurumu = "Onaylandı"; // Doğrudan onaylanmış olarak kaydet
                depoIslem.OnaylayanId = user.Id;
                depoIslem.OnayTarihi = DateTime.Now;

                _context.Add(depoIslem);

                var sarfMalzeme = await _context.SarfMalzemeler.FindAsync(depoIslem.SarfMalzemeId);
                if (sarfMalzeme != null)
                {
                    if (depoIslem.IslemTipi == DepoIslemTip.Giris)
                    {
                        sarfMalzeme.MevcutStok += depoIslem.Miktar;
                    }
                    else if (depoIslem.IslemTipi == DepoIslemTip.Cikis)
                    {
                        if (sarfMalzeme.MevcutStok < depoIslem.Miktar)
                        {
                            ModelState.AddModelError("Miktar", "Çıkış miktarı mevcut stoktan fazla olamaz.");
                            
                            var sarfMalzemeListError = await _context.SarfMalzemeler
                                .Select(s => new {
                                    Id = s.Id,
                                    Display = $"{s.Ad} - {s.Kategori} (Stok: {s.MevcutStok} {s.Birim})"
                                }).ToListAsync();
                            ViewData["SarfMalzemeId"] = new SelectList(sarfMalzemeListError, "Id", "Display", depoIslem.SarfMalzemeId);
                            
                            var personelUsersError = await _userManager.GetUsersInRoleAsync("Personel");
                            var personelListError = personelUsersError.Select(u => new {
                                Id = u.Id,
                                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
                            }).ToList();
                            ViewData["TalepEdenId"] = new SelectList(personelListError, "Id", "Display", depoIslem.TalepEdenId);
                            
                            ViewData["IslemTipleri"] = new SelectList(Enum.GetValues(typeof(DepoIslemTip))
                                .Cast<DepoIslemTip>()
                                .Select(t => new { Id = (int)t, Name = t.ToString() }), "Id", "Name", depoIslem.IslemTipi);
                            return View(depoIslem);
                        }
                        sarfMalzeme.MevcutStok -= depoIslem.Miktar;
                    }

                    sarfMalzeme.SonGuncelleme = DateTime.Now;
                    _context.Update(sarfMalzeme);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Depo işlemi başarıyla kaydedildi.";
                return RedirectToAction(nameof(Index));
            }

            // ModelState geçersiz - hata mesajları için ViewBag'leri yeniden doldur
            TempData["Error"] = "Form verileri geçersiz. Lütfen kontrol edin.";
            
            var sarfMalzemeListEnd = await _context.SarfMalzemeler
                .Select(s => new {
                    Id = s.Id,
                    Display = $"{s.Ad} - {s.Kategori} (Stok: {s.MevcutStok} {s.Birim})"
                }).ToListAsync();
            ViewBag.SarfMalzemeId = new SelectList(sarfMalzemeListEnd, "Id", "Display", depoIslem.SarfMalzemeId);
            
            var personelUsersEnd = await _userManager.GetUsersInRoleAsync("Personel");
            var personelListEnd = personelUsersEnd.Select(u => new {
                Id = u.Id,
                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
            }).ToList();
            ViewBag.TalepEdenId = new SelectList(personelListEnd, "Id", "Display", depoIslem.TalepEdenId);
            
            ViewBag.IslemTipleri = new SelectList(Enum.GetValues(typeof(DepoIslemTip))
                .Cast<DepoIslemTip>()
                .Select(t => new { Id = (int)t, Name = t.ToString() }), "Id", "Name", depoIslem.IslemTipi);
            return View(depoIslem);
        }

        // GET: DepoIslem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depoIslem = await _context.DepoIslemler.FindAsync(id);
            if (depoIslem == null)
            {
                return NotFound();
            }

            var sarfMalzemeListEdit = await _context.SarfMalzemeler
                .Select(s => new {
                    Id = s.Id,
                    Display = $"{s.Ad} - {s.Kategori} (Stok: {s.MevcutStok} {s.Birim})"
                }).ToListAsync();
            ViewData["SarfMalzemeId"] = new SelectList(sarfMalzemeListEdit, "Id", "Display", depoIslem.SarfMalzemeId);
            
            var personelUsersEdit = await _userManager.GetUsersInRoleAsync("Personel");
            var personelListEdit = personelUsersEdit.Select(u => new {
                Id = u.Id,
                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
            }).ToList();
            ViewData["TalepEdenId"] = new SelectList(personelListEdit, "Id", "Display", depoIslem.TalepEdenId);
            
            ViewData["IslemTipleri"] = new SelectList(Enum.GetValues(typeof(DepoIslemTip))
                .Cast<DepoIslemTip>()
                .Select(t => new { Id = (int)t, Name = t.ToString() }), "Id", "Name", depoIslem.IslemTipi);
            return View(depoIslem);
        }

        // POST: DepoIslem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SarfMalzemeId,IslemTipi,Miktar,Aciklama,TalepEdenId,BelgeNo,Tedarikci,OnayDurumu,RedNedeni")] DepoIslem depoIslem)
        {
            if (id != depoIslem.Id)
            {
                return NotFound();
            }

            var existingDepoIslem = await _context.DepoIslemler
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (existingDepoIslem == null)
            {
                return NotFound();
            }

            // Preserve the original values
            depoIslem.IslemYapanId = existingDepoIslem.IslemYapanId;
            depoIslem.IslemTarihi = existingDepoIslem.IslemTarihi;

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (depoIslem.OnayDurumu != existingDepoIslem.OnayDurumu)
                    {
                        if (depoIslem.OnayDurumu == "Onaylandı")
                        {
                            depoIslem.OnaylayanId = user.Id;
                            depoIslem.OnayTarihi = DateTime.Now;

                            var sarfMalzeme = await _context.SarfMalzemeler.FindAsync(depoIslem.SarfMalzemeId);
                            if (sarfMalzeme != null)
                            {
                                if (depoIslem.IslemTipi == DepoIslemTip.Giris)
                                {
                                    sarfMalzeme.MevcutStok += depoIslem.Miktar;
                                }
                                else if (depoIslem.IslemTipi == DepoIslemTip.Cikis)
                                {
                                    if (sarfMalzeme.MevcutStok < depoIslem.Miktar)
                                    {
                                        ModelState.AddModelError("Miktar", "Çıkış miktarı mevcut stoktan fazla olamaz.");
                                        
                                        var sarfMalzemeListEdit2 = await _context.SarfMalzemeler
                                            .Select(s => new {
                                                Id = s.Id,
                                                Display = $"{s.Ad} - {s.Kategori} (Stok: {s.MevcutStok} {s.Birim})"
                                            }).ToListAsync();
                                        ViewData["SarfMalzemeId"] = new SelectList(sarfMalzemeListEdit2, "Id", "Display", depoIslem.SarfMalzemeId);
                                        
                                        var personelUsersEdit2 = await _userManager.GetUsersInRoleAsync("Personel");
                                        var personelListEdit2 = personelUsersEdit2.Select(u => new {
                                            Id = u.Id,
                                            Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
                                        }).ToList();
                                        ViewData["TalepEdenId"] = new SelectList(personelListEdit2, "Id", "Display", depoIslem.TalepEdenId);
                                        
                                        ViewData["IslemTipleri"] = new SelectList(Enum.GetValues(typeof(DepoIslemTip))
                                            .Cast<DepoIslemTip>()
                                            .Select(t => new { Id = (int)t, Name = t.ToString() }), "Id", "Name", depoIslem.IslemTipi);
                                        return View(depoIslem);
                                    }
                                    sarfMalzeme.MevcutStok -= depoIslem.Miktar;
                                }

                                sarfMalzeme.SonGuncelleme = DateTime.Now;
                                _context.Update(sarfMalzeme);
                            }
                        }
                        else if (depoIslem.OnayDurumu == "Reddedildi")
                        {
                            depoIslem.OnaylayanId = user.Id;
                            depoIslem.OnayTarihi = DateTime.Now;
                        }
                    }

                    _context.Update(depoIslem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepoIslemExists(depoIslem.Id))
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

            var sarfMalzemeListFinal = await _context.SarfMalzemeler
                .Select(s => new {
                    Id = s.Id,
                    Display = $"{s.Ad} - {s.Kategori} (Stok: {s.MevcutStok} {s.Birim})"
                }).ToListAsync();
            ViewData["SarfMalzemeId"] = new SelectList(sarfMalzemeListFinal, "Id", "Display", depoIslem.SarfMalzemeId);
            
            var personelUsersFinal = await _userManager.GetUsersInRoleAsync("Personel");
            var personelListFinal = personelUsersFinal.Select(u => new {
                Id = u.Id,
                Display = $"{u.Ad} {u.Soyad} ({u.SicilNo})"
            }).ToList();
            ViewData["TalepEdenId"] = new SelectList(personelListFinal, "Id", "Display", depoIslem.TalepEdenId);
            
            ViewData["IslemTipleri"] = new SelectList(Enum.GetValues(typeof(DepoIslemTip))
                .Cast<DepoIslemTip>()
                .Select(t => new { Id = (int)t, Name = t.ToString() }), "Id", "Name", depoIslem.IslemTipi);
            return View(depoIslem);
        }

        // GET: DepoIslem/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depoIslem = await _context.DepoIslemler
                .Include(d => d.SarfMalzeme)
                .Include(d => d.IslemYapan)
                .Include(d => d.TalepEden)
                .Include(d => d.Onaylayan)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (depoIslem == null)
            {
                return NotFound();
            }

            return View(depoIslem);
        }

        // POST: DepoIslem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var depoIslem = await _context.DepoIslemler.FindAsync(id);
            _context.DepoIslemler.Remove(depoIslem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepoIslemExists(int id)
        {
            return _context.DepoIslemler.Any(e => e.Id == id);
        }
    }
} 