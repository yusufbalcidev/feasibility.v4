# 📍 Feasibility — EKA Enerji Şarj İstasyonu Fizibilite Sistemi

> Elektrikli araç şarj istasyonu yatırımları için fizibilite ve lokasyon yönetim platformu.
> Temiz katmanlı (N-Tier) mimari, SOLID prensipler, sayfa bazlı yetkilendirme, soft delete,
> middleware tabanlı aktivite loglama ve modern bir MVC yapısı üzerine kurgulanmıştır.

---

## 📑 İçindekiler

1. [Proje Hakkında](#1-proje-hakkında)
2. [Hızlı Başlangıç](#2-hızlı-başlangıç)
3. [Mimari ve Katmanlar](#3-mimari-ve-katmanlar)
4. [Teknoloji Yığını](#4-teknoloji-yığını)
5. [Klasör Yapısı](#5-klasör-yapısı)
6. [Entity Katmanı (`feasibility.entity`)](#6-entity-katmanı-feasibilityentity)
7. [DataAccess Katmanı (`feasibility.DataAccess`)](#7-dataaccess-katmanı-feasibilitydataaccess)
8. [Business Katmanı (`feasibility.Business`)](#8-business-katmanı-feasibilitybusiness)
9. [App (Presentation) Katmanı (`feasibility.App`)](#9-app-presentation-katmanı-feasibilityapp)
10. [Audit ve Soft Delete Mantığı](#10-audit-ve-soft-delete-mantığı)
11. [Rol ve Yetki (Page-Based Authorization) Mantığı](#11-rol-ve-yetki-page-based-authorization-mantığı)
12. [JWT Kimlik Doğrulama Akışı](#12-jwt-kimlik-doğrulama-akışı)
13. [Şifre Sıfırlama (Forgot Password) Akışı](#13-şifre-sıfırlama-forgot-password-akışı)
14. [ActivityLog ve Middleware Mantığı](#14-activitylog-ve-middleware-mantığı)
15. [AutoMapper Kullanımı](#15-automapper-kullanımı)
16. [FluentValidation Kullanımı](#16-fluentvalidation-kullanımı)
17. [Leaflet Harita Entegrasyonu](#17-leaflet-harita-entegrasyonu)
18. [AlertifyJS — Bildirim ve Onay](#18-alertifyjs--bildirim-ve-onay)
19. [wwwroot — Statik Varlık Yapısı](#19-wwwroot--statik-varlık-yapısı)
20. [Standart View Yapısı (Index / Create / Edit / Details)](#20-standart-view-yapısı-index--create--edit--details)
21. [Veritabanı Şeması](#21-veritabanı-şeması)
22. [Migration ve Seed Data](#22-migration-ve-seed-data)
23. [Yapılandırma (`appsettings.json`)](#23-yapılandırma-appsettingsjson)
24. [Süper Admin Hesabı](#24-süper-admin-hesabı)
25. [Geliştirici Rehberi — Yeni Sayfa Nasıl Eklenir?](#25-geliştirici-rehberi--yeni-sayfa-nasıl-eklenir)
26. [Sık Karşılaşılan Sorunlar ve Çözümler (Troubleshooting)](#26-sık-karşılaşılan-sorunlar-ve-çözümler-troubleshooting)
27. [SOLID Prensipleri ve Tasarım Kararları](#27-solid-prensipleri-ve-tasarım-kararları)
28. [Güvenlik Notları](#28-güvenlik-notları)

---

## 1. Proje Hakkında

**Feasibility**, EKA Enerji ve Teknoloji bünyesinde elektrikli araç (EV) şarj istasyonu
yatırımlarının fizibilite analizini ve sahadaki lokasyon-bakım yönetimini tek çatı altında
toplamak için geliştirilmiş bir ASP.NET Core 8 MVC web uygulamasıdır.

### Temel Yetenekler

| Modül | Açıklama |
|-------|----------|
| **Lokasyon Yönetimi** | Şarj istasyonu lokasyonlarının (il, ilçe, koordinat, açıklama, tip) eklenmesi, düzenlenmesi, harita üzerinde görüntülenmesi |
| **Lokasyon Bakım Tipleri** | Lokasyonların sınıflandırılması için tip katalogu (AVM, Otoyol, Site vb.) |
| **Kullanıcı Yönetimi** | Kullanıcı CRUD, role atama, şifre sıfırlama |
| **Rol Yönetimi** | Sayfa bazlı (Görüntüle / Ekle / Düzenle / Sil) izinler ile rol oluşturma ve güncelleme |
| **Aktivite Logları** | Tüm veri değiştiren HTTP isteklerinin middleware ile loglanması, request/response body görüntüleme |
| **Soft Delete** | Hiçbir kayıt fiziksel silinmez; `IsDeleted` ile pasifleşir, listede gözükmeye devam eder |
| **Audit Trail** | Her kaydın oluşturan/güncelleyen/silen kullanıcı bilgisi ve tarih damgaları |
| **Harita Entegrasyonu** | Leaflet + OpenStreetMap; il/ilçe seçimine göre Nominatim geocoding |
| **AlertifyJS** | Tüm bildirimler ve onay diyalogları AlertifyJS üzerinden |

---

## 2. Hızlı Başlangıç

### 2.1 Önkoşullar

- **.NET 8 SDK** (https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (LocalDB, Express veya tam sürüm)
- **EF Core CLI** araçları
  ```powershell
  dotnet tool install --global dotnet-ef
  ```

### 2.2 Bağımlılıkları Yükle

```powershell
cd feasibility.22.05-master
dotnet restore feasibility.sln
```

### 2.3 Bağlantı Stringini Kontrol Et

`feasibility.app/appsettings.json` içindeki `ConnectionStrings:DefaultConnection`:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=FeasibilityDb;Trusted_Connection=True;TrustServerCertificate=True"
```

Kendi SQL Server'ına göre düzenle.

### 2.4 Veritabanını Oluştur (İsteğe Bağlı)

Uygulama `Program.cs` içinde **`ctx.Database.Migrate()`** çağırıyor; ilk açılışta otomatik olarak
tabloları oluşturur ve seed data'yı (SuperAdmin rolü, süper admin kullanıcısı, sayfalar) ekler.

Manuel olarak yapmak istersen:
```powershell
dotnet ef database update --project feasbility.DataAcces\feasibility.DataAccess.csproj `
                          --startup-project feasibility.app\feasibility.App.csproj
```

### 2.5 Çalıştır

```powershell
dotnet run --project feasibility.app\feasibility.App.csproj
```

Tarayıcıdan açılış sayfası:

```
https://localhost:7xxx/Login/Index
```

### 2.6 Giriş Yap

| Alan | Değer |
|------|-------|
| Kullanıcı Adı | `superadmin` |
| Şifre | `Admin123!` |

> İlk girişten sonra rolüne göre dashboard'a yönlendirilir.

---

## 3. Mimari ve Katmanlar

Proje **N-Tier (4 katmanlı)** mimari prensiplerine uygun yapılandırılmıştır.
Her katman yalnızca **bir sonraki katmana** bağımlıdır; ters yöne bağımlılık yoktur.

```
┌──────────────────────────────────────────────────────────────┐
│                  feasibility.App (Presentation)              │
│  Controllers · Views · wwwroot · Program.cs                  │
└─────────────────────────────┬────────────────────────────────┘
                              │ depends on
                              ▼
┌──────────────────────────────────────────────────────────────┐
│             feasibility.Business (Business Logic)            │
│  Services (Generic, Auth, Jwt, Email, Log, RolePermission)   │
│  Middlewares  ·  Validators  ·  Mappings  ·  Extensions      │
└──────────────┬─────────────────────────────┬────────────────┘
               │ depends on                  │ depends on
               ▼                             ▼
┌──────────────────────────────┐  ┌────────────────────────────┐
│   feasibility.DataAccess     │  │     feasibility.Entity     │
│  AppDbContext · Repository   │  │  Entities · DTOs           │
│  Configurations · Seeds      │  │  Identity Validators       │
│  Extensions · Migrations     │  │                            │
└──────────────┬───────────────┘  └────────────────────────────┘
               │ depends on
               ▼
┌──────────────────────────────────────────────────────────────┐
│                   feasibility.Entity                         │
└──────────────────────────────────────────────────────────────┘
```

### Katman Bağımlılık Tablosu

| Katman | Bağımlı Olduğu Katmanlar | NuGet Bağımlılıkları |
|--------|--------------------------|----------------------|
| `feasibility.Entity` | — | `Microsoft.AspNetCore.Identity.EntityFrameworkCore` |
| `feasibility.DataAccess` | `Entity` | `EntityFrameworkCore`, `EntityFrameworkCore.SqlServer`, `Identity.EntityFrameworkCore` |
| `feasibility.Business` | `DataAccess`, `Entity` | `AutoMapper`, `FluentValidation`, `Authentication.JwtBearer`, `SharpGrip.FluentValidation.AutoValidation.Mvc` |
| `feasibility.App` | `Business`, `Entity` | `AspNetCore.Identity.EntityFrameworkCore`, `EntityFrameworkCore.Design` |

### N-Tier Prensiplerine Sadık Kalma Kuralları

1. **Controller** doğrudan `AppDbContext` veya `DbSet` ile konuşmaz; her zaman `IGenericService<T>` veya domain servisleri (`IAuthService`, `IActivityLogService`, vb.) üzerinden gider.
2. **Entity** sınıfları yalnızca veri modeli içerir; iş kuralı veya validation kodu **yoktur**.
3. **DTO** sınıfları katmanlar arasında veri taşır; UI ve persistence arasındaki dönüşümler `AutoMapper` ile yapılır.
4. **Validation** kuralları `Business` katmanındaki `FluentValidation` validator'larında tanımlanır.
5. Yeni teknik isim İngilizce; Türkçe yalnızca **kullanıcıya gösterilen metinlerde** (label, hata mesajı, view metni).

---

## 4. Teknoloji Yığını

### Backend

| Teknoloji | Sürüm | Kullanım Yeri |
|-----------|-------|---------------|
| .NET | 8.0 | Tüm projeler |
| ASP.NET Core MVC | 8.0 | App katmanı |
| Entity Framework Core | 8.0.27 | DataAccess |
| ASP.NET Core Identity | 8.0.27 | Kimlik doğrulama, kullanıcı/rol yönetimi |
| JWT Bearer | 8.0.27 | Token üretimi ve doğrulama |
| AutoMapper (Lucky Penny) | 16.1.1 | Entity ↔ DTO mapping |
| FluentValidation | 11.10.0 | DTO validation |
| SharpGrip.FluentValidation.AutoValidation.Mvc | 1.5.0 | MVC ile otomatik validation entegrasyonu |
| SQL Server | — | Veri depolama |

### Frontend

| Teknoloji | Sürüm | Kullanım Yeri |
|-----------|-------|---------------|
| Razor Views | — | Server-side rendering |
| Vanilla JS (ES5+) | — | Tüm script dosyaları |
| Bootstrap | 5.x (lib içinde) | İstenirse erişilebilir; ana stiller özel `site.css` |
| jQuery | 3.x | Validation unobtrusive + AlertifyJS bazlı kullanımlar |
| AlertifyJS | 1.13.1 (lokal) | Bildirim & onay diyalogu |
| Leaflet | 1.9.4 (CDN) | Harita |
| OpenStreetMap | — | Tile sağlayıcısı |
| Nominatim | — | Geocoding (il/ilçe → lat/lng) |
| Material Symbols (Google Fonts) | — | İkonlar |
| Hanken Grotesk (Google Fonts) | — | Tipografi |

---

## 5. Klasör Yapısı

```
feasibility.22.05-master/
│
├── feasibility.sln
├── README.md                  ← bu dosya
│
├── feasibility.entity/        ← Entity & DTO katmanı
│   ├── Entities/
│   │   ├── Common/
│   │   │   └── BaseEntity.cs
│   │   ├── Identity/
│   │   │   ├── AppUser.cs
│   │   │   └── AppRole.cs
│   │   ├── Location/
│   │   │   ├── Location.cs
│   │   │   └── LocationTypeMaintenance.cs
│   │   ├── Authorization/
│   │   │   ├── Page.cs
│   │   │   └── RolePermission.cs
│   │   └── Logging/
│   │       └── ActivityLog.cs
│   ├── Dtos/
│   │   ├── Auth/
│   │   ├── Common/AuditInfoDto.cs
│   │   ├── Location/
│   │   ├── LocationTypeMaintenance/
│   │   ├── User/
│   │   ├── Role/
│   │   └── ActivityLog/
│   └── Validations/IdentityValidation/CustomIdentityErrorDescriber.cs
│
├── feasbility.DataAcces/      ← DataAccess katmanı (klasör adı yazım hatasıyla mevcut; csproj adı doğru)
│   ├── Abstract/
│   │   └── IGenericRepository.cs
│   ├── Services/
│   │   └── GenericRepository.cs
│   ├── Context/
│   │   └── AppDbContext.cs
│   ├── Configurations/        ← IEntityTypeConfiguration<T> sınıfları
│   │   ├── AppUserConfiguration.cs
│   │   ├── AppRoleConfiguration.cs
│   │   ├── LocationConfiguration.cs
│   │   ├── LocationTypeMaintenanceConfiguration.cs
│   │   ├── ActivityLogConfiguration.cs
│   │   ├── PageConfiguration.cs
│   │   └── RolePermissionConfiguration.cs
│   ├── Seeds/                 ← Migration sırasında basılacak ilk veriler
│   │   ├── PageSeed.cs
│   │   ├── RoleSeed.cs
│   │   ├── UserSeed.cs
│   │   └── RolePermissionSeed.cs
│   ├── Extensions/
│   │   └── ServiceRegistration.cs
│   └── Migrations/            ← EF Core migration dosyaları
│
├── feasbility.business/       ← Business katmanı
│   ├── Abstract/              ← Servis arayüzleri
│   │   ├── IGenericService.cs
│   │   ├── IJwtService.cs
│   │   ├── IAuthService.cs
│   │   ├── IEmailService.cs
│   │   ├── IActivityLogService.cs
│   │   └── IRolePermissionService.cs
│   ├── Concrete/              ← Servis implementasyonları
│   │   ├── GenericManager.cs
│   │   ├── JwtService.cs
│   │   ├── AuthService.cs
│   │   ├── EmailService.cs
│   │   ├── ActivityLogManager.cs
│   │   └── RolePermissionManager.cs
│   ├── Mappings/              ← AutoMapper profilleri
│   │   ├── AuditProfile.cs
│   │   ├── LocationProfile.cs
│   │   ├── LocationTypeMaintenanceProfile.cs
│   │   ├── UserProfile.cs
│   │   ├── RoleProfile.cs
│   │   └── ActivityLogProfile.cs
│   ├── Validators/            ← FluentValidation validator'ları
│   │   ├── LocationValidators.cs
│   │   ├── LocationTypeMaintenanceValidators.cs
│   │   ├── UserValidators.cs
│   │   ├── RoleValidators.cs
│   │   └── AuthValidators.cs
│   ├── Middlewares/
│   │   ├── ActivityLogMiddleware.cs
│   │   └── PermissionRequirement.cs   ← [PagePermission] attribute
│   └── Extensions/
│       └── BusinessServiceRegistration.cs
│
└── feasibility.app/           ← Presentation (MVC) katmanı
    ├── Program.cs
    ├── appsettings.json
    ├── Controllers/
    │   ├── LoginController.cs
    │   ├── HomeController.cs
    │   ├── DashboardController.cs
    │   ├── LocationController.cs
    │   ├── LocationTypeMaintenanceController.cs
    │   ├── UserController.cs
    │   ├── RoleController.cs
    │   └── ActivityLogController.cs
    ├── Views/
    │   ├── _ViewImports.cshtml
    │   ├── _ViewStart.cshtml
    │   ├── Shared/
    │   │   ├── _BaseLayout.cshtml
    │   │   ├── _AuditPartial.cshtml
    │   │   ├── _ValidationScriptsPartial.cshtml
    │   │   └── NotFound.cshtml
    │   ├── Home/
    │   │   ├── AccessDenied.cshtml
    │   │   └── NotFound.cshtml
    │   ├── Login/
    │   │   ├── Index.cshtml
    │   │   ├── ForgotPassword.cshtml
    │   │   ├── VerifyCode.cshtml
    │   │   └── ResetPassword.cshtml
    │   ├── Dashboard/Index.cshtml
    │   ├── Location/
    │   │   ├── Index.cshtml      ← Liste + Harita tab
    │   │   ├── Create.cshtml     ← Leaflet harita ile
    │   │   ├── Edit.cshtml       ← Leaflet harita ile
    │   │   └── Details.cshtml
    │   ├── LocationTypeMaintenance/{Index,Create,Edit,Details}.cshtml
    │   ├── User/{Index,Create,Edit,Details}.cshtml
    │   ├── Role/
    │   │   ├── Index.cshtml
    │   │   ├── Create.cshtml
    │   │   ├── Edit.cshtml
    │   │   ├── Details.cshtml
    │   │   └── _PermissionsPartial.cshtml   ← Sayfa-yetki ızgarası
    │   └── ActivityLog/
    │       ├── Index.cshtml
    │       └── Details.cshtml
    └── wwwroot/
        ├── images/eka-logo-Photoroom.png
        ├── data/il-ilce.json
        ├── lib/
        │   ├── alertifyjs/{css,js}
        │   ├── jquery/dist/
        │   ├── jquery-validation/dist/
        │   ├── jquery-validation-unobtrusive/
        │   └── bootstrap/dist/
        ├── css/
        │   ├── layout/site.css       ← ortak stiller
        │   ├── login/login.css
        │   ├── dashboard/dashboard.css
        │   ├── location/{index,create,edit,details}/style.css
        │   ├── locationtypemaintenance/{index,create,edit,details}/style.css
        │   ├── user/{index,create,edit,details}/style.css
        │   ├── role/{index,create,edit,details}/style.css
        │   └── activitylog/{index,details}/style.css
        └── js/
            ├── layout/site.js        ← sidebar toggle + alertify confirm
            ├── login/login.js
            ├── location/
            │   ├── create/script.js  ← Leaflet + Nominatim
            │   └── index/script.js   ← Liste/Harita tab + Leaflet
            └── role/edit/script.js   ← Tümünü işaretle toggle
```

> **Not:** `feasbility.DataAcces` klasör adındaki yazım hatası tarihsel; `csproj` ve `namespace` adları doğru (`feasibility.DataAccess`).

---

## 6. Entity Katmanı (`feasibility.entity`)

Entity katmanı **yalnızca veri modeli** içerir. İş kuralı, validation veya UI kodu burada **bulunmaz**.

### 6.1 `BaseEntity` — Tüm domain entity'lerinin ortak temeli

`feasibility.entity/Entities/Common/BaseEntity.cs`

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? UpdatedByName { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public string? DeletedByName { get; set; }
}
```

**Önemli:** `AppUser` ve `AppRole` Identity sınıflarından türediği için `BaseEntity`'den **türemez**;
ama aynı audit alanlarını **birebir** kendi gövdelerinde tekrarlar. Bu, Identity'nin generic
yapısıyla çakışmamak için bilinçli bir tercih.

### 6.2 Domain Entity'leri

#### `Location`
```csharp
public class Location : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string? Address { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public Guid? LocationTypeMaintenanceId { get; set; }
    public LocationTypeMaintenance? LocationTypeMaintenance { get; set; }
}
```

#### `LocationTypeMaintenance`
```csharp
public class LocationTypeMaintenance : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Location> Locations { get; set; } = new List<Location>();
}
```

#### `AppUser` (Identity)
```csharp
public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // … BaseEntity alanlarıyla aynı audit alanları …
}
```

#### `AppRole` (Identity)
```csharp
public class AppRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
    // … audit alanları …
}
```

#### `Page` (Authorization)
```csharp
public class Page : BaseEntity
{
    public string Key { get; set; }          // ör: "Location", "User"
    public string Name { get; set; }         // ör: "Lokasyonlar"
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; }
}
```

#### `RolePermission` (Authorization)
```csharp
public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public AppRole? Role { get; set; }

    public Guid PageId { get; set; }
    public Page? Page { get; set; }

    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}
```

#### `ActivityLog` (Logging)
```csharp
public class ActivityLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string HttpMethod { get; set; }
    public string Path { get; set; }
    public string? QueryString { get; set; }
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public int StatusCode { get; set; }
    public long ElapsedMs { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string ActionType { get; set; }
    public string? Description { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RequestContentType { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseContentType { get; set; }
    public string? ResponseBody { get; set; }
}
```

`ActivityActionType` (string sabitleri sınıfı): `Read`, `Create`, `Update`, `Delete`,
`Login`, `Logout`, `LoginFailed`, `PasswordReset`, `Error`.

### 6.3 DTO Sınıfları

Her domain için 4 katmanlı DTO yaklaşımı:

| DTO | Görev | Örnek |
|-----|-------|-------|
| `*CreateDto` | Yeni kayıt ekleme formu (POST) | `LocationCreateDto` |
| `*UpdateDto` | Mevcut kaydı güncelleme (POST) | `LocationUpdateDto` |
| `*ListDto` | Liste tablosunda gösterilecek alanlar | `LocationListDto` |
| `*DetailDto` | Detay sayfasında gösterilecek alanlar + `AuditInfoDto` | `LocationDetailDto` |

**`AuditInfoDto`** ortak: detay sayfalarında `<partial name="_AuditPartial">` ile kullanılır.

### 6.4 Custom Identity Error Describer

`CustomIdentityErrorDescriber.cs` Identity hata mesajlarını Türkçeleştirir (DuplicateUserName,
PasswordTooShort vb.).

---

## 7. DataAccess Katmanı (`feasibility.DataAccess`)

### 7.1 `AppDbContext`

`feasbility.DataAcces/Context/AppDbContext.cs`

```csharp
public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public DbSet<Location> Locations { get; set; }
    public DbSet<LocationTypeMaintenance> LocationTypeMaintenances { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Identity tablo isimlerini İngilizce/temizlenmiş hale getir
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        // … vb.

        ApplySoftDeleteFilters(builder);   // Global query filter
        SeedStaticData(builder);            // İlk veriler
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        ApplyAuditAndSoftDelete();
        return base.SaveChangesAsync(ct);
    }
}
```

#### Global Query Filter (Soft Delete)

`Location`, `LocationTypeMaintenance`, `ActivityLog`, `Page`, `RolePermission` için
`HasQueryFilter(e => !e.IsDeleted)` uygulanır. Bu sayede `_context.Locations.ToList()`
**otomatik olarak** silinmiş kayıtları getirmez. Silinmişleri görmek için
`IgnoreQueryFilters()` çağrısı gerekir (Index sayfalarında pasif kayıtları görmek için kullanılıyor).

#### `ApplyAuditAndSoftDelete` (SaveChanges Override)

Her `SaveChanges` çağrısında devreye girer:

1. `IHttpContextAccessor` üzerinden mevcut kullanıcının `NameIdentifier` claim'i alınır.
2. **Added** durumdaki tüm entity'lerin `CreatedAt`, `CreatedBy`, `CreatedByName` alanları doldurulur.
3. **Modified** durumda `UpdatedAt`, `UpdatedBy`, `UpdatedByName` doldurulur. Eğer `IsDeleted = true` yapılmışsa ek olarak `DeletedAt`, `DeletedBy`, `DeletedByName` doldurulur.
4. **Deleted** durumdaki entity'ler **fiziksel silinmek yerine** `state = Modified` olarak güncellenir ve `IsDeleted = true` yapılır → soft delete zorunlu hale gelir.

> Bu sayede repository içinde `_dbSet.Remove(entity)` çağırsanız bile **fiziksel silme olmaz**, otomatik soft delete'e dönüşür.

### 7.2 `IGenericRepository<T>` ve `GenericRepository<T>`

Generic CRUD arayüzü:

```csharp
public interface IGenericRepository<T> where T : class
{
    IQueryable<T> Query(bool ignoreFilters = false);
    Task<List<T>> GetAllAsync(bool ignoreFilters = false, CancellationToken ct = default);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, bool ignoreFilters = false, CancellationToken ct = default);
    Task<T?> GetByIdAsync(Guid id, bool ignoreFilters = false, CancellationToken ct = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool ignoreFilters = false, CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool ignoreFilters = false, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

**Önemli parametre:** `bool ignoreFilters = false`
- `false` → Global query filter aktif (silinmiş kayıt gelmez).
- `true` → `IgnoreQueryFilters()` uygulanır (silinmiş kayıtlar da gelir). Listeleme sayfalarında pasif kayıtların görüntülenmesi için kullanılır.

`SoftDeleteAsync(Guid id)` davranışı:
1. `IgnoreQueryFilters` ile entity'yi bul.
2. `BaseEntity` ise `IsDeleted = true` yap ve Update et.
3. Aksi halde reflection ile `IsDeleted` property'sini ara; varsa true yap.
4. Hiçbiri yoksa fiziksel `Remove()` (bu durumda `SaveChanges` override yine Modified'a çevirir).

### 7.3 Configuration Sınıfları

Her entity için ayrı bir `IEntityTypeConfiguration<T>` sınıfı:
- Tablo adı (`ToTable("Locations")`)
- Property uzunlukları (`HasMaxLength(150)`)
- Decimal precision (`HasColumnType("decimal(9,6)")`)
- Index'ler (`HasIndex(l => l.Name)`)
- İlişkiler (`HasOne(...).WithMany(...).HasForeignKey(...)`)

**Örnek — LocationConfiguration:**
```csharp
public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");
        builder.Property(l => l.Name).IsRequired().HasMaxLength(150);
        builder.Property(l => l.Latitude).HasColumnType("decimal(9,6)");
        builder.Property(l => l.Longitude).HasColumnType("decimal(9,6)");
        builder.HasOne(l => l.LocationTypeMaintenance)
               .WithMany(t => t.Locations)
               .HasForeignKey(l => l.LocationTypeMaintenanceId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
```

### 7.4 Seed Data

Uygulama ilk açıldığında veritabanı boşsa otomatik basılan veriler:

| Seed | Dosya | İçerik |
|------|-------|--------|
| `PageSeed` | `Seeds/PageSeed.cs` | 6 sayfa: Dashboard, Location, LocationTypeMaintenance, User, Role, ActivityLog |
| `RoleSeed` | `Seeds/RoleSeed.cs` | 1 rol: `SuperAdmin` |
| `UserSeed` | `Seeds/UserSeed.cs` | 1 kullanıcı: `superadmin` (Admin123!) |
| `RolePermissionSeed` | `Seeds/RolePermissionSeed.cs` | SuperAdmin rolüne 6 sayfada tüm yetkiler |

Tüm GUID'ler sabit (deterministik) — migration tekrar üretildiğinde değişmezler.

`SeedDates.Anchor = 2026-01-01` → tarih damgaları sabit, migration diff'i temiz kalır.

### 7.5 `ServiceRegistration` (Extension)

```csharp
services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

services.AddIdentity<AppUser, AppRole>(/* password rules */)
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
```

---

## 8. Business Katmanı (`feasibility.Business`)

### 8.1 `IGenericService<T>` ve `GenericManager<T>`

Generic CRUD'in business katmanındaki karşılığı. **Tek noktadan beslenir** — `Location`, `User`, `LocationTypeMaintenance`, `AppUser` vb. hepsi için aynı servisi DI eder:

```csharp
public class LocationController(IGenericService<Location> _locations) : Controller { … }
public class UserController(IGenericService<AppUser> _users) : Controller { … }
```

DI kaydı tek satır:
```csharp
services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));
```

`GenericManager<T>` repository çağrısı sonrası `SaveChangesAsync` çağırır — controller'ın
ayrı bir `SaveChanges` çağrısı yapmasına gerek yok.

### 8.2 `IJwtService` / `JwtService`

Tek sorumluluk: **JWT token üretmek.**

```csharp
public interface IJwtService
{
    (string token, DateTime expiresAt) GenerateToken(AppUser user, IEnumerable<string> roles);
    IEnumerable<Claim> BuildClaims(AppUser user, IEnumerable<string> roles);
}
```

Claim listesi:
- `ClaimTypes.NameIdentifier` = UserId
- `ClaimTypes.Name` = UserName
- `ClaimTypes.Email` = Email
- `FirstName`, `LastName`
- Tüm rol isimleri → `ClaimTypes.Role`

`JwtSettings`:
- `SecretKey` → HmacSha256 imza anahtarı
- `Issuer`, `Audience` → token'da issue edici/hedef
- `ExpirationInHours` → token ömrü (varsayılan 4 saat)

Token cookie adı: **`feasibility_jwt`** (HttpOnly, Secure, SameSite=Strict).

> **Önemli:** JWT controller'da değil, business katmanında oluşur. Login controller sadece
> `_authService.LoginAsync(dto)` çağırır; servis kendi içinde `_jwtService.GenerateToken` ile
> token oluşturur ve geri döner.

### 8.3 `IAuthService` / `AuthService`

Tüm auth akışlarının (login, forgot, verify, reset) tek noktası:

```csharp
public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<AuthResultDto> SendResetCodeAsync(ForgotPasswordDto dto, CancellationToken ct = default);
    Task<AuthResultDto> VerifyCodeAsync(VerifyCodeDto dto, CancellationToken ct = default);
    Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default);
    Task<AuthResultDto> ResendResetCodeAsync(string email, CancellationToken ct = default);
    bool TryGetPendingResetToken(string email, out string token);
}
```

İçeride iki adet thread-safe **`ConcurrentDictionary`**:
- `_resetCodes` → e-posta → (kod, son geçerlilik tarihi, identity reset token)
- `_verifiedTokens` → e-posta → identity reset token (kullanıcı kodu doğruladıktan sonra)

Kod ömrü: **1 dakika**. Doğrulanan token ömrü: **10 dakika**.

#### LoginAsync davranışı

1. UserManager.FindByNameAsync
2. Lockout kontrolü → kilitliyse kalan süreyi mesajla döner
3. UserManager.CheckPasswordAsync
4. Hatalıysa AccessFailedAsync → 5 hata sonra 1 saat kilit
5. Doğruysa AccessFailedCount sıfırla, rolleri al, JWT üret, `AuthResultDto.Token` doldur.

### 8.4 `IEmailService` / `EmailService`

SMTP üzerinden Gmail ile e-posta gönderir. `appsettings.json:MailSettings`'de
`PrivateMail` ve `PrivatePassword` (Gmail uygulama şifresi) bulunur.

`SendResetCodeAsync` 6 haneli kodu HTML şablonuyla gönderir.

### 8.5 `IActivityLogService` / `ActivityLogManager`

`GenericManager<ActivityLog>` + özel metotlar:

```csharp
Task<List<ActivityLogListDto>> GetListAsync(int take = 500, CancellationToken ct = default);
Task<ActivityLogDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default);
Task LogAsync(ActivityLog log, CancellationToken ct = default);
```

`GetDetailAsync` AutoMapper ile `ActivityLog → ActivityLogDetailDto` map'ler (request/response
body dahil).

### 8.6 `IRolePermissionService` / `RolePermissionManager`

Rol bazlı sayfa yetkilerini yöneten servis:

```csharp
Task<List<RolePagePermissionDto>> GetPermissionsForRoleAsync(Guid roleId, CancellationToken ct = default);
Task<List<RolePagePermissionDto>> GetPermissionTemplateAsync(CancellationToken ct = default);
Task SaveRolePermissionsAsync(Guid roleId, IEnumerable<RolePagePermissionDto> permissions, CancellationToken ct = default);
Task<bool> UserHasPagePermissionAsync(Guid userId, string pageKey, string action, CancellationToken ct = default);
Task<HashSet<string>> GetVisiblePagesForUserAsync(Guid userId, CancellationToken ct = default);
```

- `GetPermissionTemplateAsync` → tüm sayfaları döner, hepsi default `false`.
- `GetPermissionsForRoleAsync(roleId)` → template'i alır, bu rol için DB'deki değerleri merge eder.
- `SaveRolePermissionsAsync(roleId, permissions)` → mevcutsa update, yoksa insert eder.
- `UserHasPagePermissionAsync(userId, pageKey, action)` → kullanıcı rolüne göre `view/create/edit/delete` izni olup olmadığını döner. `[PagePermission]` attribute içinde kullanılır.
- `GetVisiblePagesForUserAsync(userId)` → kullanıcının görebileceği sayfaların `Key` setini döner. Sidebar menüsü bunu kullanır.

### 8.7 `ActivityLogMiddleware`

Bkz. [Bölüm 14](#14-activitylog-ve-middleware-mantığı).

### 8.8 `PagePermissionAttribute`

Bkz. [Bölüm 11](#11-rol-ve-yetki-page-based-authorization-mantığı).

### 8.9 `BusinessServiceRegistration`

Tek bir `AddBusiness(configuration)` çağrısıyla:

```csharp
services.AddHttpContextAccessor();

// Generic ve özel servisler
services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));
services.AddScoped<IJwtService, JwtService>();
services.AddScoped<IEmailService, EmailService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IActivityLogService, ActivityLogManager>();
services.AddScoped<IRolePermissionService, RolePermissionManager>();

// AutoMapper + Lucky Penny lisans key
services.AddAutoMapper(cfg => {
    cfg.AddMaps(Assembly.GetExecutingAssembly());
    cfg.LicenseKey = "…";
});

// FluentValidation + otomatik MVC entegrasyonu
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
services.AddFluentValidationAutoValidation();

// JWT bearer
services.AddJwtAuthentication(configuration);
```

---

## 9. App (Presentation) Katmanı (`feasibility.App`)

### 9.1 `Program.cs` — Uygulama Bootstrap

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Global authorization: tüm controller'lar default authenticated
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddBusiness(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    ctx.Database.Migrate();                    // ⟵ Otomatik migration + seed
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/AccessDenied");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/NotFound");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ActivityLogMiddleware>();    // ⟵ Tüm istekleri logla

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
```

Önemli sıralama:
1. **Static files** — CSS/JS hiçbir yetki gerektirmez.
2. **Authentication** → **Authorization** → JWT cookie okunur, claim'ler kurulur.
3. **ActivityLogMiddleware** → authorization sonrası ki kullanıcı bilgisi log'a girsin.

### 9.2 Controller'lar

| Controller | URL prefix | Sorumluluk |
|------------|------------|------------|
| `LoginController` | `/Login` | Login, Logout, ForgotPassword, VerifyCode, ResetPassword, ResendCode |
| `HomeController` | `/Home` | AccessDenied, NotFound, root redirect → Dashboard |
| `DashboardController` | `/Dashboard` | İstatistikler + Son 5 lokasyon |
| `LocationController` | `/Location` | CRUD + Details + Liste/Harita |
| `LocationTypeMaintenanceController` | `/LocationTypeMaintenance` | CRUD + Details |
| `UserController` | `/User` | CRUD + Details, Identity üzerinden |
| `RoleController` | `/Role` | CRUD + Details + sayfa-yetki yönetimi |
| `ActivityLogController` | `/ActivityLog` | Index + Details (request/response body görüntüleme) |

### 9.3 Default Route

```
{controller=Login}/{action=Index}/{id?}
```

Authenticated değilse JWT bearer challenge → `/Login/Index`'e redirect.
Root URL (`/`) → `HomeController.Index` → `RedirectToAction("Index", "Dashboard")`.

---

## 10. Audit ve Soft Delete Mantığı

### 10.1 Audit Alanları

Her **persisted** entity (BaseEntity, AppUser, AppRole) şu alanları taşır:

| Alan | Doldurulduğu An |
|------|------------------|
| `CreatedAt`, `CreatedBy`, `CreatedByName` | EntityState.Added |
| `UpdatedAt`, `UpdatedBy`, `UpdatedByName` | EntityState.Modified |
| `IsDeleted` (bool) | Soft delete flag |
| `DeletedAt`, `DeletedBy`, `DeletedByName` | IsDeleted = true olarak set edildiği an |

`AppDbContext.ApplyAuditAndSoftDelete()` her `SaveChanges`'ten önce otomatik çalışır.
`HttpContextAccessor` üzerinden `ClaimTypes.NameIdentifier` ve `Identity.Name`'i okur.

### 10.2 Soft Delete Davranışı

**Kural 1:** Hiçbir entity fiziksel silinmez.

`SaveChanges`'te `EntityState.Deleted` durumdaki entity'ler **otomatik olarak**
`EntityState.Modified + IsDeleted=true`'a dönüştürülür.

```csharp
case EntityState.Deleted:
    entry.State = EntityState.Modified;
    entry.Entity.IsDeleted = true;
    entry.Entity.DeletedAt = now;
    entry.Entity.DeletedBy = userId;
    entry.Entity.DeletedByName = userName;
    break;
```

**Kural 2:** Default sorgular silinmişleri **getirmez**.

```csharp
builder.Entity<Location>().HasQueryFilter(e => !e.IsDeleted);
```

**Kural 3:** Listeleme ekranlarında **pasifler de gösterilir** (kullanıcı talebi).

Controller'larda `Query(ignoreFilters: true)` ile global filter bypass edilir:

```csharp
var list = await _locationService.Query(ignoreFilters: true)
                                  .Select(...)
                                  .ToListAsync();
```

Pasif kayıtlar UI'da kırmızı `Pasif` rozetiyle gösterilir; üzerinde sadece "Görüntüle"
eylemi vardır (Düzenle/Sil yoktur).

### 10.3 Detay Sayfalarında Audit Bilgisi

Her detay/düzenleme sayfasının altında `_AuditPartial`:

```cshtml
<partial name="_AuditPartial" model="Model.Audit" />
```

Gösterilen bilgiler: Oluşturulma, Ekleyen, Son Güncelleme, Güncelleyen, Durum (Aktif/Pasif),
Silinme tarihi ve Sileni (pasif kayıtlarda).

---

## 11. Rol ve Yetki (Page-Based Authorization) Mantığı

Bu projede yetkilendirme **rol bazlı değil sayfa-aksiyon bazlıdır**. Bir rolün "Lokasyonlar"
sayfasında "Görüntüle" yetkisi olabilir ama "Sil" yetkisi olmayabilir.

### 11.1 Yetki Matrisi

| Sayfa Key | Sayfa Adı | View | Create | Edit | Delete |
|-----------|-----------|------|--------|------|--------|
| `Dashboard` | Dashboard | ✅ | — | — | — |
| `Location` | Lokasyonlar | ✅ | ✅ | ✅ | ✅ |
| `LocationTypeMaintenance` | Lokasyon Bakım Tipleri | ✅ | ✅ | ✅ | ✅ |
| `User` | Kullanıcılar | ✅ | ✅ | ✅ | ✅ |
| `Role` | Roller | ✅ | ✅ | ✅ | ✅ |
| `ActivityLog` | Aktivite Logları | ✅ | — | — | — |

### 11.2 `[PagePermission]` Attribute

`feasbility.business/Middlewares/PermissionRequirement.cs`

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PagePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    public string PageKey { get; }
    public string Action { get; } // "view" | "create" | "edit" | "delete"

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true) {
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }
        if (user.IsInRole("SuperAdmin")) return;          // ⟵ SuperAdmin bypass

        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) {
            context.Result = new ForbidResult();
            return;
        }

        var service = context.HttpContext.RequestServices.GetRequiredService<IRolePermissionService>();
        var allowed = await service.UserHasPagePermissionAsync(userId, PageKey, Action);
        if (!allowed)
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
    }
}
```

### 11.3 Kullanım

```csharp
[PagePermission(PageSeed.LocationKey)]              // class default: "view"
public class LocationController : Controller
{
    [HttpGet]
    [PagePermission(PageSeed.LocationKey, "create")]
    public IActionResult Create() { … }

    [HttpPost]
    [PagePermission(PageSeed.LocationKey, "create")]
    public IActionResult Create(LocationCreateDto dto) { … }

    [HttpPost]
    [PagePermission(PageSeed.LocationKey, "delete")]
    public IActionResult Delete(Guid id) { … }
}
```

### 11.4 SuperAdmin Bypass

`SuperAdmin` rolündeki kullanıcılar `[PagePermission]` kontrolünü **her zaman geçer**.
Bu, ilk kurulumda yönetim erişiminin garantilenmesi içindir.

### 11.5 Sidebar Menüsü Yetkiye Göre Filtreler

`_BaseLayout.cshtml` `IRolePermissionService.GetVisiblePagesForUserAsync(userId)` çağırır,
yalnızca `CanView = true` olan sayfaların menü öğeleri render edilir. SuperAdmin için
6 sayfa hard-coded olarak görünür.

### 11.6 Rol Yönetim Ekranı

`/Role/Create` ve `/Role/Edit` sayfalarında `_PermissionsPartial.cshtml` 6 satırlık (sayfa
başına) bir matrix gösterir. Her satırda 4 checkbox: Görüntüle / Ekle / Düzenle / Sil.

**Önemli teknik detay:** Bool model binding için **hidden+checkbox tekniği KULLANILMAZ.**
Sebep: ASP.NET Core bool binder iki değer gönderildiğinde (`false,true`) ilk değeri alır ve
checkbox işaretli olsa bile `false` döner. Bu yüzden yalnızca checkbox kullanılır; işaretsizse
form'a hiçbir şey gönderilmez ve binder default `false` döner.

`SaveRolePermissionsAsync` mevcut RolePermission'ları update eder, yoksa yeni satır ekler:

```csharp
foreach (var dto in permissions)
{
    var current = existing.FirstOrDefault(e => e.PageId == dto.PageId);
    if (current is null)
        _context.RolePermissions.Add(new RolePermission { … });
    else
    {
        current.CanView = dto.CanView;
        current.CanCreate = dto.CanCreate;
        current.CanEdit = dto.CanEdit;
        current.CanDelete = dto.CanDelete;
    }
}
await _context.SaveChangesAsync(ct);
```

---

## 12. JWT Kimlik Doğrulama Akışı

### 12.1 Genel Akış Diyagramı

```
   ┌──────────────┐    1. POST /Login/Index
   │   Browser    ├───────────────────────┐
   └──────▲───────┘                       │
          │                               ▼
          │                ┌───────────────────────────┐
   8. cookie ile yönlendir│ LoginController.Index POST│
          │               │  └─ _authService.LoginAsync │
          │               └────────────┬──────────────┘
          │                            │ 2. CheckPasswordAsync
          │                            ▼
          │               ┌───────────────────────────┐
          │               │   UserManager (Identity)  │
          │               └────────────┬──────────────┘
          │                            │ 3. roles
          │                            ▼
          │               ┌───────────────────────────┐
          │               │  IJwtService.GenerateToken│
          │               │  └─ HmacSha256 imzalı JWT │
          │               └────────────┬──────────────┘
          │                            │ 4. token
          │                            ▼
          │               ┌───────────────────────────┐
          │               │ Response.Cookies.Append   │
          │               │ "feasibility_jwt" cookie  │
          │               └────────────┬──────────────┘
          │                            │
          └────────────────────────────┘
                  6. Sonraki istekler  ┌───────────────────────────┐
                  cookie ile gelir ───▶│  JwtBearerEvents          │
                                       │  OnMessageReceived: cookie│
                                       │  → context.Token          │
                                       └────────────┬──────────────┘
                                                    │ 7. validate
                                                    ▼
                                       ┌───────────────────────────┐
                                       │ ClaimsPrincipal kurulur,  │
                                       │ HttpContext.User dolu     │
                                       └───────────────────────────┘
```

### 12.2 Yapılandırma

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["feasibility_jwt"];
                if (!string.IsNullOrEmpty(token)) context.Token = token;
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/Login/Index");
                return Task.CompletedTask;
            }
        };
    });
```

### 12.3 Cookie Ayarları

```csharp
new CookieOptions
{
    HttpOnly = true,             // JS erişemez (XSS koruması)
    Secure = true,               // Sadece HTTPS
    SameSite = SameSiteMode.Strict,  // CSRF koruması
    Expires = token.ExpiresAt
}
```

### 12.4 Logout

`POST /Login/Logout` → `Response.Cookies.Delete("feasibility_jwt")` → `/Login/Index`'e
redirect.

---

## 13. Şifre Sıfırlama (Forgot Password) Akışı

```
1. /Login/ForgotPassword  ─── kullanıcı e-postayı girer ──▶ _authService.SendResetCodeAsync
                                                              ├─ UserManager.GeneratePasswordResetTokenAsync
                                                              ├─ 6 haneli kod üret
                                                              ├─ _resetCodes[email] = (code, expiry+1dk, token)
                                                              └─ _emailService.SendResetCodeAsync

2. /Login/VerifyCode      ─── kullanıcı kodu girer ───────▶ _authService.VerifyCodeAsync
                                                              ├─ Kod doğru? Süresi geçmedi mi?
                                                              ├─ Doğruysa _resetCodes'tan kaldır
                                                              └─ _verifiedTokens[email] = token (10 dk)

3. /Login/ResetPassword   ─── yeni şifre girer ───────────▶ _authService.ResetPasswordAsync
                                                              ├─ UserManager.ResetPasswordAsync(user, token, newPassword)
                                                              ├─ Başarılıysa _verifiedTokens'tan kaldır
                                                              └─ ResetAccessFailedCount
```

Kodun süresi 1 dakika → kullanıcıya görsel sayaç (`auth-timer`).

"Tekrar Kod Gönder" butonu süre dolana kadar `disabled`.

---

## 14. ActivityLog ve Middleware Mantığı

### 14.1 Middleware Çalışma Mantığı

`ActivityLogMiddleware` her HTTP isteğinde devreye girer:

```
   Request gelir
       │
       ▼
   Skip kontrolü ──────▶ /css, /js, /lib, /images, /favicon, /data ise SKIP
       │
       ▼
   Method GET/HEAD/OPTIONS mı? ─── EVET ───▶ Sadece next() çağır, LOGLA
       │
      HAYIR
       │
       ▼
   Stopwatch başlat
       │
       ▼
   Sensitive path mi? (/login, /account)
       │              ├─ Evet → requestBody = null
       │              └─ Hayır → requestBody'yi oku (max 50KB, 8000 char trim)
       │                       Form-urlencoded'ı maskeleyerek (password/token/secret → ***)
       ▼
   Response.Body = MemoryStream (buffer'a yönlendir)
       │
       ▼
   await _next(context)  ⟵ asıl pipeline çalışır
       │
       ▼
   Stopwatch durdur
       │
       ▼
   Response body capture (sadece JSON/text content-type'lar için)
   Buffer'ı orijinal stream'e kopyala (kullanıcıya gönder)
       │
       ▼
   ActivityLog kaydı oluştur:
   - User claims (Id, Name, Email)
   - HttpMethod, Path, QueryString, Controller, Action
   - StatusCode, ElapsedMs, IP, UserAgent
   - ActionType (Login/Logout/PasswordReset/Create/Update/Delete/Read/Error)
   - RequestBody, ResponseBody, ContentType'lar
   - ErrorMessage (exception yakalanırsa)
       │
       ▼
   IServiceScopeFactory üzerinden yeni scope aç → IActivityLogService.LogAsync
   (Loglama hatası asla request'i bozmaz, sadece ILogger ile yazılır)
```

### 14.2 Önemli Detaylar

| Konu | Davranış |
|------|----------|
| **GET istekleri** | **Hiç loglanmaz** (sadece veri değiştiren işlemler tutulur) |
| **/Login ve /Account** | Request/response body **kaydedilmez** (şifre koruması) |
| **Form-urlencoded body** | `password`, `token`, `secret` içeren key'ler `***` ile maskelenir |
| **Body boyutu** | `Content-Length > 50KB` ise hiç okunmaz; okunan body max 8000 karaktere trim'lenir |
| **Response body** | Sadece `application/json`, `text/plain`, `text/json`, `application/problem+json` saklanır (HTML cevaplar uzun olabileceği için saklanmaz) |
| **Action type** | POST + action="Create/Edit/Delete/Logout" → otomatik tanınır; Login controller'da Login/Logout/PasswordReset tipine çevrilir |

### 14.3 ActivityLog Detay Sayfası

`/ActivityLog/Details/{id}` → koyu temalı `<pre class="log-body">` blokları ile request ve
response body'leri tam ekran gösterir. Hata mesajı varsa kırmızı banner.

---

## 15. AutoMapper Kullanımı

### 15.1 DI Kaydı

```csharp
services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(Assembly.GetExecutingAssembly());      // Business assembly'sindeki tüm Profile'ları yükle
    cfg.LicenseKey = "...";                            // Lucky Penny Software lisansı
});
```

### 15.2 Profile Sınıfları

| Profile | Map'ler |
|---------|---------|
| `AuditProfile` | `BaseEntity → AuditInfoDto`, `AppUser → AuditInfoDto`, `AppRole → AuditInfoDto` |
| `LocationProfile` | Location CRUD/Detail/List dönüşümleri |
| `LocationTypeMaintenanceProfile` | LTM CRUD/Detail/List dönüşümleri |
| `UserProfile` | User CRUD/Detail/List (role bilgisi Ignore — controller'da ayrıca doldurulur) |
| `RoleProfile` | Role CRUD/Detail/List + `RolePermission → RolePagePermissionDto` |
| `ActivityLogProfile` | ActivityLog → ActivityLogListDto, ActivityLog → ActivityLogDetailDto |

### 15.3 Örnek Kullanım

```csharp
public class ActivityLogManager : GenericManager<ActivityLog>, IActivityLogService
{
    private readonly IMapper _mapper;

    public async Task<ActivityLogDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct: ct);
        return entity is null ? null : _mapper.Map<ActivityLogDetailDto>(entity);
    }
}
```

> **Not:** Controller'ların büyük çoğunluğu LINQ projection (`.Select(l => new XListDto { ... })`)
> kullanır. AutoMapper özellikle DetailDto gibi büyük objelerde devreye girer. Bu hibrit yaklaşım
> hem performansı (LINQ-to-SQL) hem okunabilirliği korur.

---

## 16. FluentValidation Kullanımı

### 16.1 DI Kaydı

```csharp
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
services.AddFluentValidationAutoValidation();  // SharpGrip paketi
```

`AutoValidation` paketinin marifeti: Controller action'larında `if (!ModelState.IsValid)` yine
çalışır ama doğrulama artık `DataAnnotations` yerine FluentValidation rule'ları üzerinden döner.

### 16.2 Validator'lar

| Validator | Hedef DTO |
|-----------|-----------|
| `LocationCreateDtoValidator` | LocationCreateDto |
| `LocationUpdateDtoValidator` | LocationUpdateDto |
| `LocationTypeMaintenanceCreateDtoValidator` | LocationTypeMaintenanceCreateDto |
| `LocationTypeMaintenanceUpdateDtoValidator` | LocationTypeMaintenanceUpdateDto |
| `UserCreateDtoValidator` | UserCreateDto |
| `UserUpdateDtoValidator` | UserUpdateDto |
| `RoleCreateDtoValidator` | RoleCreateDto |
| `RoleUpdateDtoValidator` | RoleUpdateDto |
| `LoginDtoValidator` | LoginDto |
| `ForgotPasswordDtoValidator` | ForgotPasswordDto |
| `VerifyCodeDtoValidator` | VerifyCodeDto |
| `ResetPasswordDtoValidator` | ResetPasswordDto |

### 16.3 Örnek

```csharp
public class LocationCreateDtoValidator : AbstractValidator<LocationCreateDto>
{
    public LocationCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Lokasyon adı zorunludur.").MaximumLength(150);
        RuleFor(x => x.City).NotEmpty().WithMessage("İl seçimi zorunludur.").MaximumLength(100);
        RuleFor(x => x.Latitude).InclusiveBetween(-90m, 90m).WithMessage("Enlem -90 ile 90 arasında olmalıdır.");
        RuleFor(x => x.Longitude).InclusiveBetween(-180m, 180m).WithMessage("Boylam -180 ile 180 arasında olmalıdır.");
    }
}
```

### 16.4 Conditional Validation

`UserUpdateDtoValidator`'da `NewPassword` opsiyoneldir ama dolduğunda en az 6 karakter olmalı:

```csharp
When(x => !string.IsNullOrWhiteSpace(x.NewPassword), () =>
{
    RuleFor(x => x.NewPassword!).MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
});
```

---

## 17. Leaflet Harita Entegrasyonu

### 17.1 Nerelerde Kullanılır?

| Sayfa | Davranış |
|-------|----------|
| **Location/Create** | Tek marker; tıklayarak / sürükleyerek koordinat seç |
| **Location/Edit** | Tek marker; mevcut koordinat ile başlar |
| **Location/Index** | Liste/Harita tab pane; tüm aktif lokasyonların marker'ları + popup |

### 17.2 Bağımlılık

CDN üzerinden:
```html
<link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" integrity="..." crossorigin="" />
<script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js" integrity="..." crossorigin=""></script>
```

Tile sağlayıcı: **OpenStreetMap**.

### 17.3 İl/İlçe → Otomatik Konum (Geocoding)

`/Location/Create` ve `/Location/Edit`'te kullanıcı `City` dropdown'undan il seçtiğinde
`/data/il-ilce.json` ile ilçeler doldurulur. İl veya ilçe değiştiğinde Nominatim API'ye
istek atılır:

```
GET https://nominatim.openstreetmap.org/search?format=json&limit=1&countrycodes=tr&q=Kadıköy,+İstanbul,+Türkiye
```

Cevaptaki `lat`/`lon` ile harita merkezi ve marker konumu güncellenir.

### 17.4 Index Sayfasında Tab

`<button class="tab-btn" data-tab="map">` tıklanınca:
1. `tab-list` gizlenir, `tab-map` görünür.
2. `initMap()` ilk seferde çağrılır (lazy init — gizliyken Leaflet boyut hesaplaması yapamaz).
3. JSON serialize edilmiş lokasyon listesi (`<script type="application/json">`) okunur.
4. Her lokasyon için marker + popup (Detayı görüntüle linki ile).
5. `fitBounds([...])` ile tüm marker'lar görünür.

---

## 18. AlertifyJS — Bildirim ve Onay

### 18.1 Lokal Kopya

`wwwroot/lib/alertifyjs/{css,js}/alertify.min.{css,js}` — internet bağlantısı olmadan da
çalışır.

### 18.2 Otomatik Bildirimler

`_BaseLayout.cshtml` her sayfada şu blok bulunur:

```html
<script>
    document.addEventListener('DOMContentLoaded', function () {
        var success = '@(TempData["Success"] as string ?? "")';
        var error = '@(TempData["Error"] as string ?? "")';
        if (success) alertify.success(success);
        if (error) alertify.error(error);
    });
</script>
```

Controller'lar `TempData["Success"]` veya `TempData["Error"]` set ederek mesaj yollar.

### 18.3 Silme Onayı

Tüm `form[data-confirm]` attribute'una sahip formlar `site.js` tarafından yakalanır:

```javascript
form.addEventListener('submit', function (e) {
    e.preventDefault();
    var message = form.getAttribute('data-confirm');
    alertify.confirm('Onay', message,
        function () { form.removeAttribute('data-confirm'); form.submit(); },
        function () { });
});
```

Markup örneği:
```html
<form asp-action="Delete" asp-route-id="@item.Id" method="post"
      data-confirm="@item.Name lokasyonunu silmek istediğinize emin misiniz?">
    @Html.AntiForgeryToken()
    <button type="submit" class="row-action-btn danger">Sil</button>
</form>
```

---

## 19. wwwroot — Statik Varlık Yapısı

CSS ve JS dosyaları **controller/action** klasör hiyerarşisi ile organize edilmiştir:

```
wwwroot/css/<controller>/<action>/style.css
wwwroot/js/<controller>/<action>/script.js
```

Örnek:
- `wwwroot/css/location/create/style.css`
- `wwwroot/js/location/create/script.js`
- `wwwroot/css/locationtypemaintenance/index/style.css`

Layout dışındaki ortak stiller `wwwroot/css/layout/site.css`'tedir. Sayfa-spesifik stiller view'ın `@section Styles` bloğunda `<link>` ile çekilir. Sayfa-spesifik JS'ler `@section Scripts` ile yüklenir.

---

## 20. Standart View Yapısı (Index / Create / Edit / Details)

Tüm CRUD sayfaları aynı standardı izler:

### 20.1 Index (Liste)

```
<page-header>
  <h1>Başlık</h1> <p>Açıklama</p>
  <header-actions>
    <a class="btn btn-primary"><span class="material-symbols-outlined">add</span> Yeni X</a>
  </header-actions>
</page-header>

<card>
  <card-header>
    <h2>X Listesi</h2>
    <span class="badge badge-info">@Model.Count kayıt</span>
  </card-header>
  <table>
    <thead>
      <tr>
        <th>...</th>
        <th>Durum</th>
        <th>Ekleyen</th>
        <th>Oluşturma Tarihi</th>
        <th align="right">İşlemler</th>
      </tr>
    </thead>
    <tbody>
      ... her satırda Görüntüle / Düzenle / Sil eylem grubu
      ... silinmiş kayıt için yalnızca Görüntüle gösterilir
    </tbody>
  </table>
</card>
```

### 20.2 Create / Edit

```
<page-header>
  <h1>Yeni X / X Düzenle</h1>
  <header-actions><a class="btn btn-secondary">← Geri</a></header-actions>
</page-header>

<card>
  <card-body>
    <form class="form">
      <div asp-validation-summary="ModelOnly" class="text-danger"></div>
      <div class="form-group">
        <label>Alan *</label>
        <input class="form-control" />
        <span class="field-validation-error"></span>
      </div>
      ...
      <div class="form-actions">  ⟵ sol yaslı
        <button type="submit" class="btn btn-primary">Kaydet/Güncelle</button>
        <a class="btn btn-secondary">İptal</a>
      </div>
    </form>
  </card-body>
</card>

@if (audit != null) {
  <partial name="_AuditPartial" model="audit" />
}
```

`.form-actions { justify-content: flex-start }` → tüm form butonları **sol yaslıdır**.

### 20.3 Details

```
<page-header>
  <h1>Kayıt Adı</h1>
  <header-actions>
    <a class="btn btn-secondary">← Geri</a>
    @if (!Model.Audit.IsDeleted) {
      <a class="btn btn-primary">Düzenle</a>
    }
  </header-actions>
</page-header>

<card>
  <card-body class="detail-grid">
    <div class="detail-item">
      <span class="detail-label">ALAN ADI</span>
      <span class="detail-value">Değer</span>
    </div>
    ...
  </card-body>
</card>

<partial name="_AuditPartial" model="Model.Audit" />
```

---

## 21. Veritabanı Şeması

```
┌──────────────────────────────┐
│         Users (AspNetUsers)  │
├──────────────────────────────┤
│ Id (Guid, PK)                │
│ FirstName, LastName          │
│ UserName, Email              │
│ NormalizedUserName/Email     │
│ PasswordHash                 │
│ EmailConfirmed,              │
│ LockoutEnd, AccessFailedCount│
│ CreatedAt/By/ByName          │
│ UpdatedAt/By/ByName          │
│ IsDeleted, DeletedAt/By/Name │
└──────────────┬───────────────┘
               │
               │  UserRoles (M:N)
               │
┌──────────────▼───────────────┐
│           Roles              │
├──────────────────────────────┤
│ Id (Guid, PK)                │
│ Name, NormalizedName         │
│ Description                  │
│ Audit + Soft delete alanları │
└──────────────┬───────────────┘
               │
               │  RolePermissions
               │
┌──────────────▼───────────────┐         ┌────────────────────┐
│       RolePermissions        │ N    1  │       Pages        │
├──────────────────────────────┤◀────────┤────────────────────┤
│ Id (Guid, PK)                │         │ Id (Guid, PK)      │
│ RoleId (FK)                  │         │ Key (unique)       │
│ PageId (FK)                  │         │ Name, Description  │
│ CanView, CanCreate           │         │ Icon, DisplayOrder │
│ CanEdit, CanDelete           │         │ Audit + Soft delete│
│ Audit + Soft delete          │         └────────────────────┘
│ UNIQUE(RoleId, PageId)       │
└──────────────────────────────┘

┌──────────────────────────────┐         ┌────────────────────────┐
│  LocationTypeMaintenances    │ 1    N  │      Locations         │
├──────────────────────────────┤◀────────┤────────────────────────┤
│ Id (Guid, PK)                │         │ Id (Guid, PK)          │
│ Name (HasIndex)              │         │ Name, Description      │
│ Description                  │         │ City, District, Address│
│ Audit + Soft delete          │         │ Latitude(decimal(9,6)) │
└──────────────────────────────┘         │ Longitude              │
                                          │ LocationTypeMaintenanceId (FK, nullable)
                                          │ Audit + Soft delete    │
                                          └────────────────────────┘

┌────────────────────────────────────────┐
│           ActivityLogs                 │
├────────────────────────────────────────┤
│ Id (Guid, PK)                          │
│ UserId, UserName, UserEmail            │
│ HttpMethod, Path, QueryString          │
│ Controller, Action                     │
│ StatusCode, ElapsedMs (bigint)         │
│ IpAddress, UserAgent                   │
│ ActionType, Description, ErrorMessage  │
│ RequestContentType, RequestBody (nmax) │
│ ResponseContentType, ResponseBody (nmax)│
│ Audit + Soft delete (asla silinmez)    │
│ INDEX(CreatedAt), INDEX(UserId)        │
└────────────────────────────────────────┘
```

### İlişki Davranışları

| İlişki | OnDelete |
|--------|----------|
| Location → LocationTypeMaintenance | `SetNull` (tip silinirse lokasyon kalır) |
| RolePermission → Role | `Cascade` (rol silinirse izinler de silinir — ama soft delete) |
| RolePermission → Page | `Cascade` |

---

## 22. Migration ve Seed Data

### 22.1 Mevcut Migration'lar

| Migration | İçerik |
|-----------|--------|
| `InitialClean` | Tüm tabloların ilk oluşturulması + seed data (Roles, Users, Pages, RolePermissions, UserRoles) |
| `ActivityLogBodyFields` | ActivityLogs tablosuna RequestBody, ResponseBody, ContentType alanları |

### 22.2 Yeni Migration Eklemek

```powershell
dotnet ef migrations add MigrationAdi `
  --project feasbility.DataAcces\feasibility.DataAccess.csproj `
  --startup-project feasibility.app\feasibility.App.csproj `
  --context AppDbContext
```

### 22.3 Migration'ı Veritabanına Uygulamak

```powershell
dotnet ef database update `
  --project feasbility.DataAcces\feasibility.DataAccess.csproj `
  --startup-project feasibility.app\feasibility.App.csproj
```

Veya uygulamayı çalıştırmak yeter — `Program.cs` otomatik uygular.

### 22.4 Migration Geri Almak

```powershell
dotnet ef migrations remove --project feasbility.DataAcces\feasibility.DataAccess.csproj `
                            --startup-project feasibility.app\feasibility.App.csproj
```

### 22.5 Seed Data Deterministik

Tüm seed GUID'leri ve tarih damgaları sabit:
- `SuperAdminRoleId = 22222222-2222-2222-2222-000000000001`
- `SuperAdminUserId = 33333333-3333-3333-3333-000000000001`
- `PageSeed.DashboardPageId = 11111111-1111-1111-1111-000000000001`
- `SeedDates.Anchor = 2026-01-01 00:00:00 UTC`
- `UserSeed.DefaultPasswordHash` = sabit Identity hash (Admin123! için)

Bu sayede migration tekrar üretildiğinde değişmez, diff temiz kalır.

---

## 23. Yapılandırma (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=FeasibilityDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "IdentitySettings": {
    "Password": {
      "RequiredLength": 6,
      "RequireNonAlphanumeric": false,
      "RequireLowercase": false,
      "RequireUppercase": false,
      "RequireDigit": false
    },
    "User": {
      "RequireUniqueEmail": true
    }
  },
  "JwtSettings": {
    "SecretKey": "ekaenerji-feasibility-super-secret-key-please-change-2026!",
    "Issuer": "feasibility.app",
    "Audience": "feasibility.users",
    "ExpirationInHours": 4
  },
  "MailSettings": {
    "PrivateMail": "yusuf439000@gmail.com",
    "PrivatePassword": "drio sjlb ewkh ntga"
  }
}
```

> ⚠️ Production'da `JwtSettings:SecretKey`, `MailSettings:PrivatePassword` ve ConnectionString
> **mutlaka değiştirilmelidir.** Bu değerleri `appsettings.Development.json`, environment
> variables veya Azure Key Vault gibi bir sırlı depodan okuyun.

---

## 24. Süper Admin Hesabı

| Alan | Değer |
|------|-------|
| **UserName** | `superadmin` |
| **E-posta** | `superadmin@ekaenerji.com.tr` |
| **Şifre** | `Admin123!` |
| **Ad Soyad** | Süper Admin |
| **Rol** | `SuperAdmin` (silinemez, adı değiştirilemez) |
| **UserId** | `33333333-3333-3333-3333-000000000001` |
| **RoleId** | `22222222-2222-2222-2222-000000000001` |

SuperAdmin rolündeki kullanıcılar `[PagePermission]` kontrolünü **her zaman geçer** —
ilk kurulumda sistem yönetiminin garantilenmesi için.

Süper Admin kullanıcısı arayüzden **silinmeye çalışılırsa** `"Süper admin kullanıcısı silinemez"`
hatası döner. SuperAdmin rolü de aynı şekilde korunur.

---

## 25. Geliştirici Rehberi — Yeni Sayfa Nasıl Eklenir?

Diyelim ki "Şarj Cihazları" (`ChargingDevice`) adlı yeni bir sayfa ekleyeceksin. Adım adım:

### 25.1 Entity

`feasibility.entity/Entities/ChargingDevice/ChargingDevice.cs`
```csharp
public class ChargingDevice : BaseEntity
{
    public string SerialNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int PowerKw { get; set; }
    public Guid? LocationId { get; set; }
    public Location? Location { get; set; }
}
```

### 25.2 DTO'lar

`feasibility.entity/Dtos/ChargingDevice/` altında:
- `ChargingDeviceCreateDto`
- `ChargingDeviceUpdateDto`
- `ChargingDeviceListDto`
- `ChargingDeviceDetailDto` (içinde `AuditInfoDto Audit { get; set; }`)

### 25.3 Configuration

`feasbility.DataAcces/Configurations/ChargingDeviceConfiguration.cs`
```csharp
public class ChargingDeviceConfiguration : IEntityTypeConfiguration<ChargingDevice>
{
    public void Configure(EntityTypeBuilder<ChargingDevice> builder)
    {
        builder.ToTable("ChargingDevices");
        builder.Property(d => d.SerialNumber).IsRequired().HasMaxLength(60);
        builder.Property(d => d.Model).IsRequired().HasMaxLength(120);
        builder.HasOne(d => d.Location).WithMany().HasForeignKey(d => d.LocationId).OnDelete(DeleteBehavior.SetNull);
    }
}
```

### 25.4 AppDbContext'e DbSet ekle

```csharp
public DbSet<ChargingDevice> ChargingDevices => Set<ChargingDevice>();
// Ve global query filter:
builder.Entity<ChargingDevice>().HasQueryFilter(e => !e.IsDeleted);
```

### 25.5 PageSeed'e ekle

```csharp
public static readonly Guid ChargingDevicePageId = new("11111111-1111-1111-1111-000000000007");
public const string ChargingDeviceKey = "ChargingDevice";

// All() metoduna ekle:
new() { Id = ChargingDevicePageId, Key = ChargingDeviceKey, Name = "Şarj Cihazları", Icon = "ev_station", DisplayOrder = 7, CreatedAt = SeedDates.Anchor }
```

### 25.6 RolePermissionSeed güncelle (SuperAdmin yeni sayfayı görsün)

Loop otomatik yeni page'i bulup SuperAdmin'e tüm yetkileri verir. Tek yapacağın: yeni
`Guid` üretmesi için index'i artıracak (zaten loop yapıyor).

### 25.7 Migration

```powershell
dotnet ef migrations add AddChargingDevice --project feasbility.DataAcces\... --startup-project feasibility.app\...
```

### 25.8 AutoMapper Profile

`feasbility.business/Mappings/ChargingDeviceProfile.cs`
```csharp
public class ChargingDeviceProfile : Profile
{
    public ChargingDeviceProfile()
    {
        CreateMap<ChargingDeviceCreateDto, ChargingDevice>();
        CreateMap<ChargingDeviceUpdateDto, ChargingDevice>();
        CreateMap<ChargingDevice, ChargingDeviceListDto>()
            .ForMember(d => d.LocationName, c => c.MapFrom(s => s.Location != null ? s.Location.Name : null));
        CreateMap<ChargingDevice, ChargingDeviceDetailDto>()
            .ForMember(d => d.Audit, c => c.MapFrom(s => s));
    }
}
```

### 25.9 FluentValidation

```csharp
public class ChargingDeviceCreateDtoValidator : AbstractValidator<ChargingDeviceCreateDto>
{
    public ChargingDeviceCreateDtoValidator()
    {
        RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(120);
        RuleFor(x => x.PowerKw).GreaterThan(0);
    }
}
```

### 25.10 Controller

```csharp
[PagePermission(PageSeed.ChargingDeviceKey)]
public class ChargingDeviceController : Controller
{
    private readonly IGenericService<ChargingDevice> _service;

    public ChargingDeviceController(IGenericService<ChargingDevice> service) => _service = service;

    public async Task<IActionResult> Index(CancellationToken ct) { … }

    [HttpGet]
    [PagePermission(PageSeed.ChargingDeviceKey, "create")]
    public IActionResult Create() => View(new ChargingDeviceCreateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.ChargingDeviceKey, "create")]
    public async Task<IActionResult> Create(ChargingDeviceCreateDto dto, CancellationToken ct) { … }

    // Details, Edit, Delete — diğer controller'ları örnek al.
}
```

### 25.11 View'lar

`Views/ChargingDevice/{Index,Create,Edit,Details}.cshtml` — diğer modülleri taban olarak kopyala.

### 25.12 wwwroot

```
wwwroot/css/chargingdevice/{index,create,edit,details}/style.css
wwwroot/js/chargingdevice/{ ... }/script.js  (gerekiyorsa)
```

### 25.13 Sidebar Menüsü

`_BaseLayout.cshtml`'de yeni menü item'ı:
```cshtml
@if (visiblePages.Contains(PageSeed.ChargingDeviceKey))
{
    <a class="@menuItemClass("ChargingDevice")" href="/ChargingDevice/Index">
        <span class="material-symbols-outlined">ev_station</span>
        <span class="menu-text">Şarj Cihazları</span>
    </a>
}
```

### 25.14 Derle + Test

```powershell
dotnet build feasibility.sln
dotnet run --project feasibility.app\feasibility.App.csproj
```

> Tek bir entity için harcanan ortalama süre: 30-45 dakika (standart copy/paste yaklaşımıyla).

---

## 26. Sık Karşılaşılan Sorunlar ve Çözümler (Troubleshooting)

### Q1: Rol Edit'te checkbox işaretliyorum ama kaydedilmiyor.

**Sebep:** Hidden `value="false"` + checkbox `value="true"` aynı isimde gönderildiğinde
ASP.NET Core bool binder ilk değeri alıyor ve hep `false` dönüyor.

**Çözüm:** Bu projede `_PermissionsPartial.cshtml`'den hidden field'lar **kaldırıldı**.
Sadece checkbox kullanılır — işaretsizse hiçbir şey gönderilmez, bool default `false` olur.

### Q2: Lokasyon silindi ama listede hâlâ görünüyor.

**Bu doğru davranış.** Soft delete sebebiyle silinmiş kayıt listede "Pasif" rozetiyle görünür.
Listelemede `Query(ignoreFilters: true)` kullanıldığı için filtre devre dışı.

Sadece sayım vb. yerlerde aktif (silinmemiş) kayıtların sayılması istenir → `Query()` default
filtre uygular.

### Q3: Migration başarısız oluyor: "There is already an object named 'X' in the database."

DB'de tablolar zaten var ama `__EFMigrationsHistory` tablosu kayıp olabilir. İki seçenek:
1. Veritabanını sıfırla (`DROP DATABASE` veya yeniden oluştur), `dotnet ef database update` çağır.
2. `__EFMigrationsHistory` tablosuna manuel `InitialClean` kaydı ekle (deneyimli kullanıcılar).

### Q4: Login sayfasında "JwtSettings:SecretKey ayarı bulunamadı" hatası.

`appsettings.json`'da `JwtSettings.SecretKey` boş. Doldur ve uygulamayı yeniden başlat.

### Q5: E-posta gönderilmiyor (şifre sıfırlama).

`MailSettings.PrivatePassword` Gmail için **uygulama şifresi** olmalı (normal Gmail şifresi
değil). Gmail hesabında "2 Adımlı Doğrulama" açık olmalı, sonra "Uygulama Şifresi" oluştur.

### Q6: AutoMapper "License key required" diyor.

`BusinessServiceRegistration.cs`'deki `cfg.LicenseKey = "..."` boş. Lucky Penny Software
lisans key'i ekle.

### Q7: Tarayıcıda haritada marker görünmüyor.

1. Lokasyonun `Latitude` ve `Longitude` 0 mı? `Index.cshtml`'deki filtre `Latitude != 0 && Longitude != 0` olanları gösterir.
2. Tab `map`'a tıklandı mı? Harita lazy init — sadece sekmeye tıklayınca yüklenir.
3. Internet bağlantısı var mı? Leaflet ve OpenStreetMap CDN ve tile servisi internet gerektirir.

### Q8: ActivityLog tablosu çok hızlı şişiyor.

Tüm POST/PUT/DELETE istekleri loglanıyor. Sınırlama yöntemleri:
- Index sayfasında `GetListAsync(500)` ile sadece son 500 gösterilir.
- Eski log'ları sil (cron / scheduled task ile). Şu an otomatik temizleme yok.

### Q9: SuperAdmin rolünü silmeye çalışıyorum, hata veriyor.

`RoleController.Delete` içinde `if (id == RoleSeed.SuperAdminRoleId)` kontrolü var. Bu rol
sistem rolüdür ve silinemez.

### Q10: Yeni rol oluşturdum ama o role atadığım kullanıcı login olunca menüde hiçbir şey görmüyor.

Yeni rolde **CanView** izni hiçbir sayfa için işaretlenmedi. `/Role/Edit/{id}` → matrisinden
ilgili sayfaların View'unu işaretle ve kaydet.

---

## 27. SOLID Prensipleri ve Tasarım Kararları

### 27.1 S — Single Responsibility

| Sınıf | Sorumluluğu |
|-------|-------------|
| `JwtService` | Yalnızca JWT token üretmek |
| `EmailService` | Yalnızca SMTP üzerinden e-posta atmak |
| `AuthService` | Auth iş akışı (login, forgot, verify, reset) — JWT'yi `JwtService`'e delege eder |
| `ActivityLogMiddleware` | HTTP isteklerini yakalayıp log entity'si hazırlamak |
| `GenericManager<T>` | CRUD orchestration — repository çağrısı + SaveChanges |
| `GenericRepository<T>` | Persistence detayları — DbSet, IgnoreQueryFilters, vb. |

### 27.2 O — Open/Closed

`IGenericService<T>` ve `GenericManager<T>` sayesinde yeni bir entity için **yeni servis
yazmadan** DI'a otomatik gelir. Open for extension (yeni T tipi), closed for modification.

Yeni audit alanı eklemek istiyorsan → `BaseEntity`'ye property ekle, configuration'a yansıt,
migration üret. `AppDbContext.ApplyAuditAndSoftDelete` zaten BaseEntity üzerinden çalıştığı
için audit logic'i değişmez.

### 27.3 L — Liskov Substitution

`AppRole : IdentityRole<Guid>` ve `AppUser : IdentityUser<Guid>` Identity'nin tüm contract'ını
korurlar. Kendi audit alanlarını eklerler ama Identity'nin beklediği member'ları kırmazlar.

### 27.4 I — Interface Segregation

Tek bir "büyük" `IService` yok. Her sorumluluk ayrı interface:
- `IJwtService` — sadece JWT
- `IAuthService` — sadece auth akışı
- `IRolePermissionService` — sadece yetkilendirme
- `IActivityLogService` — sadece log

Bir controller'ın yalnızca ihtiyacı olan servisleri inject etmesi sağlanır.

### 27.5 D — Dependency Inversion

Tüm bağımlılıklar **abstract** interface üzerinden. `LocationController` `IGenericService<Location>`
ister, hangi implementation gelecek olmaz. `Program.cs` boot anında concrete'leri DI'a kaydeder.

```csharp
services.AddScoped<IJwtService, JwtService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));
```

### 27.6 N-Tier Disiplini

Controller → Business → DataAccess → Database. **Her yön tek**. Controller `AppDbContext`'i
asla doğrudan kullanmaz. Business `HttpContext`'i `IHttpContextAccessor` üzerinden alır
(direkt referans almaz).

### 27.7 DRY (Don't Repeat Yourself)

- Audit logic tek noktada (`AppDbContext.ApplyAuditAndSoftDelete`)
- Soft delete tek noktada (`SaveChanges` override)
- Validation, mapping, authorization hepsi katmanlarına dağılmış, tekrarsız
- `_AuditPartial`, `_PermissionsPartial`, `_BaseLayout` ile UI tekrarı kaldırılmış

---

## 28. Güvenlik Notları

### 28.1 Saldırı Vektörleri ve Korumalar

| Saldırı | Korunma |
|---------|---------|
| **XSS** | Razor varsayılan olarak HTML encode eder. `@Html.Raw` yalnızca güvenilir kaynaklarda (JSON serializer çıktısı için). Cookie HttpOnly. |
| **CSRF** | Tüm POST formlarında `@Html.AntiForgeryToken()` + controller'da `[ValidateAntiForgeryToken]`. Cookie SameSite=Strict. |
| **SQL Injection** | EF Core parametrize sorgu kullanır. Raw SQL kullanılmıyor. |
| **Brute Force** | Identity Lockout: 5 başarısız giriş → 1 saat kilit. |
| **Şifre Açık Trafiği** | HTTPS zorunlu (`app.UseHttpsRedirection()` + `UseHsts()`). Cookie `Secure = true`. |
| **JWT Hijacking** | Cookie `HttpOnly + Secure + SameSite=Strict`. Token süresi 4 saat. |
| **Log Sızıntısı** | `/Login` ve `/Account` body'leri kaydedilmez. Form-urlencoded'da `password/token/secret` `***` maskelenir. |
| **Audit Tampering** | Audit alanları `SaveChanges` override'ında otomatik dolar; uygulama kodundan manuel set'i geri yazılır. |
| **Privileged Escalation** | SuperAdmin rolü ve kullanıcısı korunur (silinemez/adı değişmez). |

### 28.2 Production Öncesi Kontrol Listesi

- [ ] `JwtSettings.SecretKey` en az 32 karakter, kriptografik olarak güçlü.
- [ ] `appsettings.Development.json`'da test verisi varsa kaldırıldı.
- [ ] `MailSettings.PrivatePassword` env variable veya secret store'dan alınıyor.
- [ ] `ConnectionString` env variable veya secret store'dan alınıyor.
- [ ] HTTPS sertifikası kurulu, HSTS aktif.
- [ ] `Program.cs` `app.Environment.IsDevelopment()` kontrolü ile development sayfaları
      production'da gizli.
- [ ] SQL Server kullanıcısı yalnızca gerekli izinlere sahip (db_owner DEĞİL).
- [ ] Otomatik log temizliği (eski ActivityLog kayıtları) bir scheduled task ile yapılır.
- [ ] Süper admin şifresi production'da değiştirildi.
- [ ] CSP (Content Security Policy) header'ları eklendi (opsiyonel ama önerilir).
- [ ] Audit logları için yedekleme stratejisi var.

---

## 🎯 Sonuç

Bu README ile birlikte projenin **tüm katmanlarını, servislerini, akışlarını, yetkilendirme
mantığını, validation ve mapping pipeline'ını, soft delete davranışını, harita ve log
entegrasyonlarını** tek noktada bulabilirsin.

Sorun yaşadığında [Bölüm 26](#26-sık-karşılaşılan-sorunlar-ve-çözümler-troubleshooting)'ya bak.

Yeni özellik eklemeden önce [Bölüm 25](#25-geliştirici-rehberi--yeni-sayfa-nasıl-eklenir)'i takip et.

> **EKA Enerji ve Teknoloji — 2026**
