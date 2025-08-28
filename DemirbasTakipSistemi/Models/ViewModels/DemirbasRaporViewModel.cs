using System.Collections.Generic;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class DemirbasRaporViewModel
    {
        public List<DemirbasDurumGrubu> DemirbasDurumlari { get; set; }
        public List<DemirbasKategoriGrubu> DemirbasKategorileri { get; set; }
        public List<OdaDemirbasGrubu> OdaDagilimi { get; set; }

        public DemirbasRaporViewModel()
        {
            DemirbasDurumlari = new List<DemirbasDurumGrubu>();
            DemirbasKategorileri = new List<DemirbasKategoriGrubu>();
            OdaDagilimi = new List<OdaDemirbasGrubu>();
        }
    }

    public class DemirbasDurumGrubu
    {
        public string Durum { get; set; }
        public int Sayi { get; set; }
    }

    public class DemirbasKategoriGrubu
    {
        public string Kategori { get; set; }
        public int Sayi { get; set; }
    }

    public class OdaDemirbasGrubu
    {
        public string OdaAdi { get; set; }
        public int DemirbasSayisi { get; set; }
    }
} 