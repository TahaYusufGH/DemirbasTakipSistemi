using System.Collections.Generic;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class ArizaRaporViewModel
    {
        public List<ArizaDurumGrubu> ArizaDurumlari { get; set; }
        public List<DemirbasArizaGrubu> DemirbasArizalari { get; set; }

        public ArizaRaporViewModel()
        {
            ArizaDurumlari = new List<ArizaDurumGrubu>();
            DemirbasArizalari = new List<DemirbasArizaGrubu>();
        }
    }

    public class ArizaDurumGrubu
    {
        public string Durum { get; set; }
        public int Sayi { get; set; }
    }

    public class DemirbasArizaGrubu
    {
        public string DemirbasAdi { get; set; }
        public int ArizaSayisi { get; set; }
    }
} 