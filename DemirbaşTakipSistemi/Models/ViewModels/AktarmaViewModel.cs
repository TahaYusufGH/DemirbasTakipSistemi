using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemirbaşTakipSistemi.Models.ViewModels
{
    public class AktarmaViewModel
    {
        [Required(ErrorMessage = "Kaynak personel seçilmelidir.")]
        [Display(Name = "Kaynak Personel")]
        public string KaynakPersonelId { get; set; }
        
        [Display(Name = "Kaynak Personel Adı")]
        public string KaynakPersonelAdi { get; set; }
        
        [Required(ErrorMessage = "Hedef personel seçilmelidir.")]
        [Display(Name = "Hedef Personel")]
        public string HedefPersonelId { get; set; }
        
        [Display(Name = "Hedef Personel Adı")]
        public string HedefPersonelAdi { get; set; }
        
        [Required(ErrorMessage = "En az bir demirbaş seçilmelidir.")]
        [Display(Name = "Seçili Demirbaşlar")]
        public List<int> SeciliDemirbaslar { get; set; }
        
        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }
    }
    
    public class PersonelViewModel
    {
        public string Id { get; set; }
        public string AdSoyad { get; set; }
        public string SicilNo { get; set; }
        public string Departman { get; set; }
    }
    
    public class DemirbasViewModel
    {
        public int ZimmetId { get; set; }
        public int DemirbasId { get; set; }
        public string DemirbasKodu { get; set; }
        public string DemirbasAdi { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public string ZimmetTarihi { get; set; }
    }
}