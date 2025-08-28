using System;
using System.ComponentModel.DataAnnotations;
using DemirbaşTakipSistemi.Models.Enums;

namespace DemirbaşTakipSistemi.Models
{
    public class DepoIslem
    {
        public int Id { get; set; }

        [Required]
        public int SarfMalzemeId { get; set; }
        public virtual SarfMalzeme SarfMalzeme { get; set; }

        [Required]
        [Display(Name = "İşlem Tipi")]
        public DepoIslemTip IslemTipi { get; set; }

        [Required]
        [Display(Name = "İşlem Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime IslemTarihi { get; set; }

        [Required]
        [Display(Name = "Miktar")]
        public int Miktar { get; set; }

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Required]
        [Display(Name = "İşlemi Yapan")]
        public string IslemYapanId { get; set; }
        public virtual ApplicationUser IslemYapan { get; set; }

        [Display(Name = "Belge No")]
        public string? BelgeNo { get; set; }

        [Display(Name = "Tedarikçi")]
        public string? Tedarikci { get; set; }

        [Display(Name = "Talep Eden")]
        public string? TalepEdenId { get; set; }
        public virtual ApplicationUser? TalepEden { get; set; }

        [Required]
        [Display(Name = "Onay Durumu")]
        public string OnayDurumu { get; set; } = "Beklemede"; // Beklemede, Onaylandı, Reddedildi

        [Display(Name = "Onaylayan")]
        public string? OnaylayanId { get; set; }
        public virtual ApplicationUser? Onaylayan { get; set; }

        [Display(Name = "Onay Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime? OnayTarihi { get; set; }

        [Display(Name = "Red Nedeni")]
        public string? RedNedeni { get; set; }
    }
} 