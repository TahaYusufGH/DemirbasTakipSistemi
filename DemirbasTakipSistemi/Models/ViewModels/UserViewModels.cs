using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        
        [Display(Name = "E-posta")]
        public string Email { get; set; }
        
        [Display(Name = "Ad")]
        public string Ad { get; set; }
        
        [Display(Name = "Soyad")]
        public string Soyad { get; set; }
        
        [Display(Name = "Roller")]
        public string Roller { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public string Ad { get; set; }
        
        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public string Soyad { get; set; }
        
        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
        
        [Required(ErrorMessage = "Rol seçimi zorunludur.")]
        [Display(Name = "Rol")]
        public string Role { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }
        
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public string Ad { get; set; }
        
        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public string Soyad { get; set; }
        
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre (Boş bırakılırsa değiştirilmez)")]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
        
        [Required(ErrorMessage = "Rol seçimi zorunludur.")]
        [Display(Name = "Rol")]
        public string Role { get; set; }
    }
} 