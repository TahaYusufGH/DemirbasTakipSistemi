using System.Collections.Generic;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class DepoRaporViewModel
    {
        public List<SarfMalzemeStokGrubu> SarfMalzemeStokDurumu { get; set; }
        public List<DepoIslemTipGrubu> IslemTipDagilimi { get; set; }
        public List<PersonelTalepGrubu> PersonelTalepleri { get; set; }

        public DepoRaporViewModel()
        {
            SarfMalzemeStokDurumu = new List<SarfMalzemeStokGrubu>();
            IslemTipDagilimi = new List<DepoIslemTipGrubu>();
            PersonelTalepleri = new List<PersonelTalepGrubu>();
        }
    }

    public class SarfMalzemeStokGrubu
    {
        public string MalzemeAdi { get; set; }
        public string Kategori { get; set; }
        public int MevcutStok { get; set; }
        public int MinimumStok { get; set; }
        public string Birim { get; set; }
        public string StokDurumu { get; set; }
    }

    public class DepoIslemTipGrubu
    {
        public string IslemTipi { get; set; }
        public int IslemSayisi { get; set; }
    }

    public class PersonelTalepGrubu
    {
        public string PersonelAdi { get; set; }
        public int TalepSayisi { get; set; }
    }
} 