using System.Collections.Generic;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Demirbaş İstatistikleri
        public int ToplamDemirbasSayisi { get; set; }
        public int ZimmetliDemirbasSayisi { get; set; }
        public int ArizaliDemirbasSayisi { get; set; }
        public int BakimdakiDemirbasSayisi { get; set; }
        public int HurdaDemirbasSayisi { get; set; }

        // Arıza İstatistikleri
        public int BekleyenArizaSayisi { get; set; }
        public int IncelenenArizaSayisi { get; set; }
        public int CozulenArizaSayisi { get; set; }

        // Depo İşlem İstatistikleri
        public int BekleyenDepoIslemSayisi { get; set; }
        public int OnaylananDepoIslemSayisi { get; set; }
        public int ReddedilenDepoIslemSayisi { get; set; }

        // Stok Uyarıları
        public List<SarfMalzeme> MinimumStokAltindakiMalzemeler { get; set; }

        // Son Arızalar
        public List<Ariza> SonArizalar { get; set; }

        // Son Depo İşlemleri
        public List<DepoIslem> SonDepoIslemleri { get; set; }
    }
} 