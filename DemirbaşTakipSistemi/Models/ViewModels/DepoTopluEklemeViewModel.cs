using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DemirbaşTakipSistemi.Models.Enums;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class DepoTopluEklemeViewModel
    {
        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "Demirbaş Kategorisi")]
        public string Kategori { get; set; }

        [Display(Name = "Demirbaş Alt Kategorisi")]
        public string AltKategori { get; set; }

        [Required(ErrorMessage = "Durum seçimi zorunludur.")]
        [Display(Name = "Durumu")]
        public DemirbasDurum Durum { get; set; }

        [Required(ErrorMessage = "Birim seçimi zorunludur.")]
        [Display(Name = "Birimi")]
        public string Birim { get; set; }

        [Display(Name = "Oda")]
        public int? OdaId { get; set; }

        [Display(Name = "Marka")]
        public string Marka { get; set; }

        [Required(ErrorMessage = "Alım tarihi zorunludur.")]
        [Display(Name = "Alım Tarihi")]
        [DataType(DataType.Date)]
        public DateTime AlimTarihi { get; set; }

        [Required(ErrorMessage = "Seri numaraları zorunludur.")]
        [Display(Name = "Seri Numaraları")]
        public string SeriNumaralari { get; set; }

        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }

        // Referans için listeler
        public List<string> Kategoriler { get; set; }
        public List<Oda> Odalar { get; set; }

        public DepoTopluEklemeViewModel()
        {
            Kategoriler = new List<string>();
            Odalar = new List<Oda>();
            AlimTarihi = DateTime.Now;
            Durum = DemirbasDurum.Depoda;
        }
    }
}