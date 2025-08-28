using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DemirbaşTakipSistemi.Models.Enums;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class DepoTopluDuzenlemeViewModel
    {
        [Display(Name = "Seçilen Demirbaşlar")]
        public List<int> SecilenDemirbasIds { get; set; }

        [Display(Name = "Envanter Türü")]
        public string EnvanterTuru { get; set; }

        [Display(Name = "Birim")]
        public string Birim { get; set; }

        [Display(Name = "Oda")]
        public int? OdaId { get; set; }

        [Display(Name = "Durum")]
        public DemirbasDurum? Durum { get; set; }

        [Display(Name = "Kontrol Eden")]
        public string KontrolEdenId { get; set; }

        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }

        [Display(Name = "Güncelleme Tarihi")]
        [DataType(DataType.Date)]
        public DateTime GuncellemeTarihi { get; set; }

        // Arama kriterleri
        [Display(Name = "Seri Numarası")]
        public string SeriNo { get; set; }

        [Display(Name = "Kategori")]
        public string Kategori { get; set; }

        [Display(Name = "Durum Filtresi")]
        public DemirbasDurum? DurumFiltresi { get; set; }

        [Display(Name = "Oda Filtresi")]
        public int? OdaIdFiltresi { get; set; }

        // Referans için listeler
        public List<Demirbas> Demirbaslar { get; set; }
        public List<string> Kategoriler { get; set; }
        public List<Oda> Odalar { get; set; }
        public List<ApplicationUser> Kullanicilar { get; set; }

        public DepoTopluDuzenlemeViewModel()
        {
            SecilenDemirbasIds = new List<int>();
            Demirbaslar = new List<Demirbas>();
            Kategoriler = new List<string>();
            Odalar = new List<Oda>();
            Kullanicilar = new List<ApplicationUser>();
            GuncellemeTarihi = DateTime.Now;
        }
    }
}