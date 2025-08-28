using System;
using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class DemirbasImportModel
    {
        [Required]
        [Display(Name = "Demirbaş Kodu")]
        public string DemirbasKodu { get; set; }

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
        [Display(Name = "Alım Tarihi")]
        [DataType(DataType.Date)]
        public DateTime AlimTarihi { get; set; }

        [Display(Name = "Seri No")]
        public string? SeriNo { get; set; }

        [Display(Name = "Oda Kodu")]
        public string? OdaKodu { get; set; }
    }
} 