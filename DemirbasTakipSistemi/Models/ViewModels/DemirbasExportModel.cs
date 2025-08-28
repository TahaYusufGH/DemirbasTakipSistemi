using System;
using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class DemirbasExportModel
    {
        [Display(Name = "Demirbaş Kodu")]
        public string DemirbasKodu { get; set; }

        [Display(Name = "Ad")]
        public string Ad { get; set; }

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Display(Name = "Kategori")]
        public string Kategori { get; set; }

        [Display(Name = "Marka")]
        public string Marka { get; set; }

        [Display(Name = "Model")]
        public string? Model { get; set; }

        [Display(Name = "Alım Tarihi")]
        public string AlimTarihi { get; set; }

        [Display(Name = "Durum")]
        public string Durum { get; set; }

        [Display(Name = "Seri No")]
        public string? SeriNo { get; set; }

        [Display(Name = "Oda")]
        public string? OdaAdi { get; set; }

        [Display(Name = "Oda Kodu")]
        public string? OdaKodu { get; set; }

        [Display(Name = "Zimmetli Personel")]
        public string? ZimmetliPersonel { get; set; }
    }
} 