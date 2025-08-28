using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using DemirbaşTakipSistemi.Models;

namespace DemirbaşTakipSistemi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string? Departman { get; set; }
        public string? SicilNo { get; set; }

        // Computed property for full name
        public string AdSoyad => $"{Ad} {Soyad}";

        // Navigation Properties
        public virtual ICollection<Zimmet> Zimmetler { get; set; }
        public virtual ICollection<Ariza> Arizalar { get; set; }
        public virtual ICollection<DepoIslem> DepoIslemler { get; set; }

        public ApplicationUser()
        {
            Zimmetler = new HashSet<Zimmet>();
            Arizalar = new HashSet<Ariza>();
            DepoIslemler = new HashSet<DepoIslem>();
        }
    }
} 