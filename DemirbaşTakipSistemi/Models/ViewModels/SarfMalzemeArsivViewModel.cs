using System;
using System.Collections.Generic;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class SarfMalzemeArsivViewModel
    {
        public List<DepoIslem> DepoIslemler { get; set; }
        public List<ApplicationUser> Personeller { get; set; }
        public List<SarfMalzeme> SarfMalzemeler { get; set; }
        
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string SecilenPersonelId { get; set; }
        public int? SecilenSarfMalzemeId { get; set; }

        public SarfMalzemeArsivViewModel()
        {
            DepoIslemler = new List<DepoIslem>();
            Personeller = new List<ApplicationUser>();
            SarfMalzemeler = new List<SarfMalzeme>();
        }
    }
}