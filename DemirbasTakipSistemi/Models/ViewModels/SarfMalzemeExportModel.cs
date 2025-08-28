using System;
using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class SarfMalzemeExportModel
    {
        [Display(Name = "Malzeme Kodu")]
        public string MalzemeKodu { get; set; }

        [Display(Name = "Ad")]
        public string Ad { get; set; }

        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }

        [Display(Name = "Kategori")]
        public string Kategori { get; set; }

        [Display(Name = "Marka")]
        public string Marka { get; set; }

        [Display(Name = "Model")]
        public string? Model { get; set; }

        [Display(Name = "Birim")]
        public string Birim { get; set; }

        [Display(Name = "Minimum Stok")]
        public int MinimumStok { get; set; }

        [Display(Name = "Mevcut Stok")]
        public int MevcutStok { get; set; }

        [Display(Name = "Son Güncelleme")]
        public string SonGuncelleme { get; set; }
    }
} 