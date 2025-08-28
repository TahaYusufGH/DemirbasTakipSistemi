using System;

namespace DemirbaşTakipSistemi.Models
{
    public enum ArizaDurum { Bekliyor, Cozuldu, Reddedildi }
    public class ArizaKaydi
    {
        public int Id { get; set; }
        public int DemirbasId { get; set; }
        public Demirbas Demirbas { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Aciklama { get; set; }
        public DateTime Tarih { get; set; }
        public ArizaDurum Durum { get; set; }
    }
} 