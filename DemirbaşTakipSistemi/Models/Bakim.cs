using System;
using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models
{
    public class Bakim
    {
        public int Id { get; set; }

        [Required]
        public int DemirbasId { get; set; }
        public virtual Demirbas Demirbas { get; set; }

        [Required]
        [Display(Name = "Bakım Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime BakimTarihi { get; set; }

        [Required(ErrorMessage = "Bakım türü zorunludur.")]
        [Display(Name = "Bakım Türü")]
        public string BakimTuru { get; set; } // Periyodik, Arıza Sonrası, Önleyici

        [Required(ErrorMessage = "Bakım açıklaması zorunludur.")]
        [Display(Name = "Bakım Açıklaması")]
        public string Aciklama { get; set; }

        [Display(Name = "Yapılan İşlemler")]
        public string? YapilanIslemler { get; set; }

        [Required]
        [Display(Name = "Durum")]
        public string Durum { get; set; } = "Planlandı"; // Planlandı, Başladı, Tamamlandı, İptal

        [Display(Name = "Tamamlanma Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime? TamamlanmaTarihi { get; set; }

        [Required]
        [Display(Name = "Bakım Yapan")]
        public string BakimYapanId { get; set; }
        public virtual ApplicationUser BakimYapan { get; set; }

        [Display(Name = "Sonraki Bakım Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? SonrakiBakimTarihi { get; set; }
    }
} 