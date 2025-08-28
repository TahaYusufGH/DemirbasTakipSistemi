using System.Collections.Generic;
using DemirbaşTakipSistemi.Models.Enums;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class DepoViewModel
    {
        public List<Demirbas> Demirbaslar { get; set; }
        public DepoSearchModel SearchModel { get; set; }

        public DepoViewModel()
        {
            Demirbaslar = new List<Demirbas>();
            SearchModel = new DepoSearchModel();
        }
    }

    public class DepoSearchModel
    {
        public string SeriNo { get; set; }
        public string Kategori { get; set; }
        public string AltKategori { get; set; }
        public DemirbasDurum? Durum { get; set; }
        public string Birim { get; set; }
        public string OdaNo { get; set; }
        public string OdaTuru { get; set; }
        public string SicilNo { get; set; }
        public string PersonelAdi { get; set; }
    }
} 