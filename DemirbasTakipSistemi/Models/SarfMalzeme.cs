using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models
{
    public class SarfMalzeme
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Malzeme kodu zorunludur.")]
        [Display(Name = "Malzeme Kodu")]
        [StringLength(50)]
        public string MalzemeKodu { get; set; } = null!;

        [Required(ErrorMessage = "Malzeme adı zorunludur.")]
        [Display(Name = "Malzeme Adı")]
        [StringLength(100)]
        public string Ad { get; set; } = null!;

        [Display(Name = "Açıklama")]
        [StringLength(500)]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Birim zorunludur.")]
        [StringLength(20)]
        public string Birim { get; set; } = null!; // Adet, Kutu, Paket vs.

        [Required(ErrorMessage = "Kategori zorunludur.")]
        [StringLength(50)]
        public string Kategori { get; set; } = null!;

        [Required(ErrorMessage = "Marka zorunludur.")]
        [Display(Name = "Marka")]
        [StringLength(50)]
        public string Marka { get; set; } = null!;

        [Display(Name = "Model")]
        [StringLength(50)]
        public string? Model { get; set; }

        [Required]
        [Display(Name = "Minimum Stok")]
        public int MinimumStok { get; set; }

        [Required]
        [Display(Name = "Mevcut Stok")]
        public int MevcutStok { get; set; }

        [Display(Name = "Son Güncelleme")]
        public DateTime SonGuncelleme { get; set; }

        // Navigation Properties
        public virtual ICollection<DepoIslem> DepoIslemler { get; set; }

        public SarfMalzeme()
        {
            DepoIslemler = new HashSet<DepoIslem>();
            SonGuncelleme = DateTime.Now;
        }
    }
} 