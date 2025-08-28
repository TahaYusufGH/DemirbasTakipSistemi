using System.Collections.Generic;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class SarfMalzemeAyarlarViewModel
    {
        public List<string> Kategoriler { get; set; }
        public string YeniKategori { get; set; }
        public SarfMalzeme YeniMalzeme { get; set; }

        public SarfMalzemeAyarlarViewModel()
        {
            Kategoriler = new List<string>();
            YeniMalzeme = new SarfMalzeme();
        }
    }
}