using DemirbaşTakipSistemi.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models
{
    public class Demirbas
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Demirbaş kodu zorunludur.")]
        [Display(Name = "Demirbaş Kodu")]
        public string DemirbasKodu { get; set; }

        [Required(ErrorMessage = "Demirbaş adı zorunludur.")]
        [Display(Name = "Demirbaş Adı")]
        public string Ad { get; set; }

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Kategori zorunludur.")]
        [Display(Name = "Kategori")]
        public string Kategori { get; set; }

        [Required(ErrorMessage = "Marka zorunludur.")]
        [Display(Name = "Marka")]
        public string Marka { get; set; }

        [Display(Name = "Model")]
        public string? Model { get; set; }

        [Required(ErrorMessage = "Alım tarihi zorunludur.")]
        [Display(Name = "Alım Tarihi")]
        [DataType(DataType.Date)]
        public DateTime AlimTarihi { get; set; }

        [Required]
        [Display(Name = "Durum")]
        public DemirbasDurum Durum { get; set; }

        [Display(Name = "Seri No")]
        public string? SeriNo { get; set; }

        // Navigation Properties
        public int? OdaId { get; set; }
        public virtual Oda? Oda { get; set; }

        public virtual ICollection<Zimmet> Zimmetler { get; set; }
        public virtual ICollection<Ariza> Arizalar { get; set; }
        public virtual ICollection<Bakim> Bakimlar { get; set; }
        public virtual ICollection<ArizaKaydi> ArizaKayitlari { get; set; }

        public Demirbas()
        {
            Zimmetler = new HashSet<Zimmet>();
            Arizalar = new HashSet<Ariza>();
            Bakimlar = new HashSet<Bakim>();
            ArizaKayitlari = new HashSet<ArizaKaydi>();
        }
    }
} 