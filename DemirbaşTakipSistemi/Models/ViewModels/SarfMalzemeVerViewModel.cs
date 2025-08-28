using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class SarfMalzemeVerViewModel
    {
        public List<SarfMalzeme> SarfMalzemeler { get; set; }
        public List<ApplicationUser> Personeller { get; set; }
        public List<Oda> Odalar { get; set; }

        [Required(ErrorMessage = "Malzeme seçimi zorunludur.")]
        [Display(Name = "Malzeme")]
        public int SecilenMalzemeId { get; set; }

        [Required(ErrorMessage = "Personel seçimi zorunludur.")]
        [Display(Name = "Personel")]
        public string SecilenPersonelId { get; set; }

        [Display(Name = "Oda")]
        public int SecilenOdaId { get; set; }

        [Required(ErrorMessage = "Miktar zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalıdır.")]
        public int Miktar { get; set; }

        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }

        public SarfMalzemeVerViewModel()
        {
            SarfMalzemeler = new List<SarfMalzeme>();
            Personeller = new List<ApplicationUser>();
            Odalar = new List<Oda>();
            Miktar = 1;
        }
    }
}