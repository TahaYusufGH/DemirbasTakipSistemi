using System;
using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class SarfMalzemeImportModel
    {
        [Required]
        [Display(Name = "Malzeme Kodu")]
        public string MalzemeKodu { get; set; }

        [Required]
        [Display(Name = "Ad")]
        public string Ad { get; set; }

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Required]
        [Display(Name = "Kategori")]
        public string Kategori { get; set; }

        [Required]
        [Display(Name = "Marka")]
        public string Marka { get; set; }

        [Display(Name = "Model")]
        public string? Model { get; set; }

        [Required]
        [Display(Name = "Birim")]
        public string Birim { get; set; }

        [Required]
        [Display(Name = "Minimum Stok")]
        public int MinimumStok { get; set; }

        [Required]
        [Display(Name = "Mevcut Stok")]
        public int MevcutStok { get; set; }
    }
} 