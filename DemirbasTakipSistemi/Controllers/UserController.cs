using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DemirbaşTakipSistemi.Models;
using DemirbaşTakipSistemi.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace DemirbaşTakipSistemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Ad = user.Ad,
                    Soyad = user.Soyad,
                    Roller = string.Join(", ", roles)
                });
            }

            return View(userViewModels);
        }

        public IActionResult Create()
        {
            ViewData["Roles"] = _roleManager.Roles.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    TempData["SuccessMessage"] = "Kullanıcı başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["Roles"] = _roleManager.Roles.ToList();
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Ad = user.Ad,
                Soyad = user.Soyad,
                Role = userRoles.FirstOrDefault()
            };

            ViewData["Roles"] = _roleManager.Roles.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = model.Email;
                user.UserName = model.Email;
                user.Ad = model.Ad;
                user.Soyad = model.Soyad;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Mevcut rolleri temizle
                    var userRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, userRoles);

                    // Yeni rol ekle
                    await _userManager.AddToRoleAsync(user, model.Role);

                    // Şifre değiştirme (opsiyonel)
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, model.Password);
                    }

                    TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["Roles"] = _roleManager.Roles.ToList();
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Ad = user.Ad,
                Soyad = user.Soyad,
                Roller = string.Join(", ", roles)
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Admin kullanıcısını silmeyi engelle
                if (user.Email == "admin@demirbas.local")
                {
                    TempData["ErrorMessage"] = "Admin kullanıcısı silinemez.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Varsayılan şifre oluştur
            string defaultPassword = "Demirbas123!";
            
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, defaultPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Kullanıcı şifresi başarıyla sıfırlandı: {defaultPassword}";
            }
            else
            {
                TempData["ErrorMessage"] = "Şifre sıfırlanırken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 