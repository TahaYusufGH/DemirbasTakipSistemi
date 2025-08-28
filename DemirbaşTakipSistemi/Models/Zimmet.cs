using System;
using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models
{
    public class Zimmet
    {
        public int Id { get; set; }

        [Required]
        public string PersonelId { get; set; } = null!;
        public virtual ApplicationUser Personel { get; set; } = null!;

        [Required]
        public int DemirbasId { get; set; }
        public virtual Demirbas Demirbas { get; set; } = null!;

        [Required]
        [Display(Name = "Zimmet Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime ZimmetTarihi { get; set; }

        [Display(Name = "İade Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime? IadeTarihi { get; set; }

        [Display(Name = "Açıklama")]
        [StringLength(500)]
        public string? Aciklama { get; set; }

        [Required]
        [Display(Name = "Aktif")]
        public bool IsAktif { get; set; } = true;

        [Display(Name = "Teslim Eden")]
        public string? TeslimEdenId { get; set; }
        public virtual ApplicationUser? TeslimEden { get; set; }

        [Display(Name = "Teslim Alan")]
        public string? TeslimAlanId { get; set; }
        public virtual ApplicationUser? TeslimAlan { get; set; }
    }
} 