using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DemirbaşTakipSistemi.Models;
using DemirbaşTakipSistemi.Models.Enums;

namespace DemirbaşTakipSistemi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Demirbas> Demirbaslar { get; set; } = null!;
        public DbSet<Oda> Odalar { get; set; } = null!;
        public DbSet<Zimmet> Zimmetler { get; set; } = null!;
        public DbSet<ArizaKaydi> ArizaKayitlari { get; set; } = null!;
        public DbSet<Bakim> Bakimlar { get; set; } = null!;
        public DbSet<Ariza> Arizalar { get; set; } = null!;
        public DbSet<SarfMalzeme> SarfMalzemeler { get; set; } = null!;
        public DbSet<DepoIslem> DepoIslemler { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUser configurations
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.SicilNo)
                .IsUnique(false);

            builder.Entity<ApplicationUser>()
                .Property(u => u.Ad)
                .IsRequired()
                .HasMaxLength(50);

            builder.Entity<ApplicationUser>()
                .Property(u => u.Soyad)
                .IsRequired()
                .HasMaxLength(50);

            // Demirbas configurations
            builder.Entity<Demirbas>()
                .HasOne(d => d.Oda)
                .WithMany(o => o.Demirbaslar)
                .HasForeignKey(d => d.OdaId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Demirbas>()
                .HasIndex(d => d.DemirbasKodu)
                .IsUnique();

            builder.Entity<Demirbas>()
                .HasIndex(d => d.SeriNo)
                .IsUnique(false);

            builder.Entity<Demirbas>()
                .Property(d => d.Durum)
                .HasConversion<string>();

            // Zimmet configurations
            builder.Entity<Zimmet>()
                .HasOne(z => z.Personel)
                .WithMany(u => u.Zimmetler)
                .HasForeignKey(z => z.PersonelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Zimmet>()
                .HasOne(z => z.Demirbas)
                .WithMany(d => d.Zimmetler)
                .HasForeignKey(z => z.DemirbasId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Zimmet>()
                .HasOne(z => z.TeslimEden)
                .WithMany()
                .HasForeignKey(z => z.TeslimEdenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Zimmet>()
                .HasOne(z => z.TeslimAlan)
                .WithMany()
                .HasForeignKey(z => z.TeslimAlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Zimmet>()
                .HasIndex(z => new { z.DemirbasId, z.IsAktif })
                .HasFilter("[IsAktif] = 1");

            // Ariza configurations
            builder.Entity<Ariza>()
                .HasOne(a => a.Demirbas)
                .WithMany(d => d.Arizalar)
                .HasForeignKey(a => a.DemirbasId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ariza>()
                .HasOne(a => a.Bildiren)
                .WithMany(u => u.Arizalar)
                .HasForeignKey(a => a.BildirenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ariza>()
                .HasOne(a => a.CozenPersonel)
                .WithMany()
                .HasForeignKey(a => a.CozenPersonelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ariza>()
                .HasIndex(a => a.Durum);

            builder.Entity<Ariza>()
                .HasIndex(a => a.ArizaTarihi);

            // Bakim configurations
            builder.Entity<Bakim>()
                .HasOne(b => b.Demirbas)
                .WithMany(d => d.Bakimlar)
                .HasForeignKey(b => b.DemirbasId)
                .OnDelete(DeleteBehavior.Restrict);

            // DepoIslem configurations
            builder.Entity<DepoIslem>()
                .HasOne(d => d.SarfMalzeme)
                .WithMany(s => s.DepoIslemler)
                .HasForeignKey(d => d.SarfMalzemeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DepoIslem>()
                .HasOne(d => d.IslemYapan)
                .WithMany(u => u.DepoIslemler)
                .HasForeignKey(d => d.IslemYapanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DepoIslem>()
                .HasOne(d => d.TalepEden)
                .WithMany()
                .HasForeignKey(d => d.TalepEdenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DepoIslem>()
                .HasOne(d => d.Onaylayan)
                .WithMany()
                .HasForeignKey(d => d.OnaylayanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DepoIslem>()
                .Property(d => d.IslemTipi)
                .HasConversion<string>();

            builder.Entity<DepoIslem>()
                .HasIndex(d => d.IslemTarihi);

            // Oda configurations
            builder.Entity<Oda>()
                .HasOne(o => o.SorumluPersonel)
                .WithMany()
                .HasForeignKey(o => o.SorumluPersonelId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Oda>()
                .HasIndex(o => o.OdaKodu)
                .IsUnique();

            // SarfMalzeme configurations
            builder.Entity<SarfMalzeme>()
                .HasIndex(s => s.MalzemeKodu)
                .IsUnique();
        }
    }
} 