using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DemirbaşTakipSistemi.Models;

namespace DemirbaşTakipSistemi.Models
{
    public class Oda
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Oda kodu zorunludur.")]
        [Display(Name = "Oda Kodu")]
        public string OdaKodu { get; set; }

        [Required(ErrorMessage = "Oda adı zorunludur.")]
        [Display(Name = "Oda Adı")]
        public string Ad { get; set; }

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Kat bilgisi zorunludur.")]
        [Display(Name = "Kat")]
        public int Kat { get; set; }

        [Required(ErrorMessage = "Bina bilgisi zorunludur.")]
        public string Bina { get; set; }

        [Display(Name = "Sorumlu Personel")]
        public string? SorumluPersonelId { get; set; }
        public virtual ApplicationUser? SorumluPersonel { get; set; }

        // Navigation Properties
        public virtual ICollection<Demirbas> Demirbaslar { get; set; }

        public Oda()
        {
            Demirbaslar = new HashSet<Demirbas>();
        }
    }
} 