using System;
using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models
{
    public class Ariza
    {
        public int Id { get; set; }

        [Required]
        public int DemirbasId { get; set; }
        public virtual Demirbas Demirbas { get; set; } = null!;

        [Required]
        public string BildirenId { get; set; } = null!;
        public virtual ApplicationUser Bildiren { get; set; } = null!;

        [Required]
        [Display(Name = "Arıza Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime ArizaTarihi { get; set; }

        [Required(ErrorMessage = "Arıza açıklaması zorunludur.")]
        [Display(Name = "Arıza Açıklaması")]
        [StringLength(1000)]
        public string Aciklama { get; set; } = null!;

        [Display(Name = "Çözüm")]
        [StringLength(1000)]
        public string? Cozum { get; set; }

        [Required]
        [Display(Name = "Durum")]
        [StringLength(20)]
        public string Durum { get; set; } = "Beklemede"; // Beklemede, İncelemede, Çözüldü, İptal

        [Display(Name = "Çözüm Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime? CozumTarihi { get; set; }

        [Display(Name = "Çözen Personel")]
        public string? CozenPersonelId { get; set; }
        public virtual ApplicationUser? CozenPersonel { get; set; }

        // Aliases for backward compatibility
        public ApplicationUser? CozumYapan => CozenPersonel;
        public string? CozumYapanId => CozenPersonelId;
    }
} 