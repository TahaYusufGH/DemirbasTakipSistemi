# Demirbaş Takip Sistemi

## 📋 Proje Hakkında

Demirbaş Takip Sistemi, kamu kurumları için geliştirilmiş kapsamlı bir demirbaş ve envanter yönetim sistemidir. Bu sistem, ASP.NET Core 8.0 teknolojisi kullanılarak geliştirilmiş olup, kurumların demirbaş malzemelerini, sarf malzemelerini, zimmet işlemlerini ve arıza takiplerini etkin bir şekilde yönetmelerine olanak sağlar.

## ✨ Özellikler

### 🏢 Kullanıcı Yönetimi
- **Rol Tabanlı Yetkilendirme**: Admin, Personel ve Bilgi İşlem rolleri
- **Güvenli Kimlik Doğrulama**: ASP.NET Core Identity ile entegre
- **Kullanıcı Profil Yönetimi**: Sicil numarası, departman bilgileri

### 📦 Demirbaş Yönetimi
- **Demirbaş Kayıt ve Düzenleme**: Kategori, marka, model, seri numarası takibi
- **Durum Takibi**: Aktif, Zimmetli, Arızalı, Bakımda, Hurda, Depoda durumları
- **Oda Bazlı Lokasyon Takibi**: Demirbaşların hangi odada olduğunun izlenmesi
- **Toplu İşlemler**: Çoklu demirbaş ekleme ve düzenleme

### 👤 Zimmet İşlemleri
- **Personel Zimmet Takibi**: Hangi personelin hangi demirbaşı kullandığının kaydı
- **Teslim İşlemleri**: Teslim eden ve teslim alan bilgileri
- **Zimmet Geçmişi**: Demirbaşın geçmiş zimmet kayıtları
- **Aktif Zimmet Kontrolü**: Aynı demirbaşın birden fazla kişiye zimmetlenmesinin önlenmesi

### 🔧 Arıza Yönetimi
- **Arıza Bildirimi**: Personellerin arıza bildirebilmesi
- **Arıza Takibi**: Açık, kapalı arıza durumları
- **Çözüm Süreci**: Arıza çözüm detayları ve sorumlu personel ataması
- **Arıza Raporları**: Geçmiş arıza kayıtları ve istatistikler

### 📊 Depo ve Sarf Malzeme Yönetimi
- **Sarf Malzeme Takibi**: Toner, kağıt, kırtasiye malzemeleri
- **Stok Yönetimi**: Giriş, çıkış, mevcut stok durumu
- **Talep Sistemi**: Personellerin sarf malzeme taleplerini oluşturması
- **Onay Süreci**: Taleplerin yöneticiler tarafından onaylanması

### 🏠 Oda Yönetimi
- **Oda Kayıtları**: Bina, kat, oda kodu bilgileri
- **Sorumlu Personel**: Her oda için sorumlu personel ataması
- **Demirbaş Lokasyonu**: Demirbaşların hangi odada olduğunun takibi

### 📈 Raporlama
- **Demirbaş Raporları**: Kategori, durum, lokasyon bazlı raporlar
- **Zimmet Raporları**: Personel bazlı zimmet durumları
- **Arıza Raporları**: Arıza istatistikleri ve trend analizleri
- **Depo Raporları**: Stok durumu ve hareketleri

### 📤 İçe/Dışa Aktarım
- **Excel İmport/Export**: Demirbaş ve sarf malzeme verilerinin Excel formatında aktarımı
- **CSV Desteği**: CsvHelper kütüphanesi ile veri aktarımı
- **Toplu Veri Yükleme**: Büyük miktarda verinin sistem içine aktarılması

## 🛠️ Teknolojiler

- **Backend**: ASP.NET Core 8.0
- **Frontend**: Razor Pages, Bootstrap, jQuery
- **Veritabanı**: SQL Server
- **ORM**: Entity Framework Core 8.0
- **Kimlik Doğrulama**: ASP.NET Core Identity
- **Veri İşleme**: CsvHelper
- **UI Framework**: Bootstrap 5

## 📋 Gereksinimler

- .NET 8.0 SDK
- SQL Server (LocalDB, Express, Standard veya Enterprise)
- Visual Studio 2022 (önerilen) veya Visual Studio Code
- IIS Express (geliştirme için)

## 🚀 Kurulum

### 1. Projeyi İndirme
```bash
git clone [repository-url]
cd DemirbaşTakipSistemi
```

### 2. Veritabanı Yapılandırması
`appsettings.json` dosyasında connection string'i düzenleyin:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 3. Paket Yükleme
```bash
dotnet restore
```

### 4. Veritabanı Migration
```bash
dotnet ef database update
```

### 5. Projeyi Çalıştırma
```bash
dotnet run
```

Uygulama `https://localhost:5001` adresinde çalışacaktır.

## 👥 Varsayılan Kullanıcılar

Sistem ilk çalıştırıldığında aşağıdaki varsayılan kullanıcılar oluşturulur:

### Admin Kullanıcısı
- **Email**: admin@adalet.gov.tr
- **Şifre**: Admin123!
- **Rol**: Admin

### Personel Kullanıcıları
- **Email**: personel1@adalet.gov.tr, **Şifre**: Personel1!
- **Email**: personel2@adalet.gov.tr, **Şifre**: Personel2!
- **Email**: personel3@adalet.gov.tr, **Şifre**: Personel3!

### Bilgi İşlem Kullanıcıları
- **Email**: bilgiislem1@adalet.gov.tr, **Şifre**: BilgiIslem1!
- **Email**: bilgiislem2@adalet.gov.tr, **Şifre**: BilgiIslem2!

## 🔐 Roller ve Yetkiler

### Admin
- Tüm sistem işlemleri
- Kullanıcı yönetimi
- Sistem ayarları
- Tüm raporlara erişim

### Personel
- Kendi zimmetlerini görüntüleme
- Arıza bildirimi
- Sarf malzeme talebi
- Kendi işlemlerini görüntüleme

### Bilgi İşlem
- Demirbaş yönetimi
- Arıza çözümleme
- Depo işlemleri
- Teknik raporlar

## 📁 Proje Yapısı

```
DemirbaşTakipSistemi/
├── Controllers/           # MVC Controller'ları
├── Data/                 # Entity Framework DbContext
├── Models/               # Veri modelleri
│   ├── Enums/           # Enum tanımları
│   └── ViewModels/      # View modelleri
├── Views/               # Razor View'ları
├── wwwroot/            # Statik dosyalar (CSS, JS, images)
├── Areas/              # Identity Area
└── Migrations/         # EF Core Migration dosyları
```

## 🔧 Önemli Dosyalar

- **Program.cs**: Uygulama yapılandırması ve dependency injection
- **ApplicationDbContext.cs**: Entity Framework veritabanı context'i
- **appsettings.json**: Uygulama ayarları ve connection string
- **Models/**: Tüm veri modelleri (Demirbas, Zimmet, Ariza, vb.)

## 📊 Veritabanı Şeması

Sistem aşağıdaki ana tablolara sahiptir:
- **AspNetUsers**: Kullanıcı bilgileri (Identity tablosu)
- **Demirbaslar**: Demirbaş kayıtları
- **Zimmetler**: Zimmet işlemleri
- **Arizalar**: Arıza kayıtları
- **SarfMalzemeler**: Sarf malzeme bilgileri
- **DepoIslemler**: Depo giriş/çıkış işlemleri
- **Odalar**: Oda/lokasyon bilgileri

## 🎯 Kullanım Senaryoları

### Demirbaş Ekleme
1. Admin/Bilgi İşlem kullanıcısı sisteme giriş yapar
2. Demirbaş menüsünden "Yeni Ekle" seçeneğini tıklar
3. Demirbaş bilgilerini doldurur (kod, ad, kategori, marka, vb.)
4. Kaydet butonuna tıklayarak demirbaşı sisteme ekler

### Zimmet İşlemi
1. Demirbaş listesinden zimmetlenecek demirbaş seçilir
2. "Zimmetle" butonuna tıklanır
3. Teslim alan personel seçilir
4. Zimmet işlemi tamamlanır

### Arıza Bildirimi
1. Personel sisteme giriş yapar
2. Arıza menüsünden "Yeni Arıza Bildir" seçeneğini tıklar
3. Arızalı demirbaş ve arıza detaylarını girer
4. Arıza kaydı oluşturulur

## 🔄 Güncelleme ve Bakım

### Migration Ekleme
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Veritabanı Sıfırlama
```bash
dotnet ef database drop
dotnet ef database update
```

## 🤝 Katkıda Bulunma

1. Projeyi fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

## 📝 Lisans

Bu proje [MIT Lisansı](LICENSE) altında lisanslanmıştır.


<img width="1894" height="907" alt="Ekran görüntüsü 2025-08-26 113626" src="https://github.com/user-attachments/assets/b92b41ba-fa36-4630-bb9f-7ab3ce5106fa" />
