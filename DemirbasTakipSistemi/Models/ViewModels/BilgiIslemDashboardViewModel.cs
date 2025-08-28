using System.Collections.Generic;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class BilgiIslemDashboardViewModel
    {
        // Arıza İstatistikleri
        public int BekleyenArizaSayisi { get; set; }
        public int IncelenenArizaSayisi { get; set; }
        public int CozulenArizaSayisi { get; set; }
        
        // Demirbaş İstatistikleri
        public int ToplamDemirbasSayisi { get; set; }
        public int ZimmetliDemirbasSayisi { get; set; }
        public int ArizaliDemirbasSayisi { get; set; }
        
        // Stok Uyarıları
        public List<SarfMalzeme> MinimumStokAltindakiMalzemeler { get; set; }
        
        // Bekleyen Arızalar
        public List<Ariza> BekleyenArizalar { get; set; }
        
        // Son Çözülen Arızalar
        public List<Ariza> SonCozulenArizalar { get; set; }
        
        // Bakım Gerektiren Demirbaşlar
        public List<Bakim> BakimGerekenDemirbaslar { get; set; }
    }
} 