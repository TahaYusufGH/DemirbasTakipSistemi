using System.Collections.Generic;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class PersonelDashboardViewModel
    {
        // Zimmetli Demirbaşlar
        public List<Demirbas> ZimmetliDemirbaslar { get; set; }

        // Arıza Bildirimleri
        public List<Ariza> Arizalar { get; set; }

        // Depo İşlemleri
        public List<DepoIslem> DepoIslemleri { get; set; }
        
        // Departmandaki Demirbaşlar
        public List<Demirbas> DepartmanDemirbaslari { get; set; }
    }
} 