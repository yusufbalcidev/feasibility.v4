# Fizibilite (Amortisman) Hesaplama Mimarisi

Bu döküman, fizibilite sonuç tablosunun **arka planda nasıl hesaplandığını**, hangi entity'lerin rol aldığını ve formüllerin tam zincirini anlatır.

> Tüm hesaplama mantığı tek bir yerde toplanmıştır: [`FeasibilityAmortizationManager.BuildDetail()`](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L196). Hem kayıtlı çalışmanın detayı (`GetDetailAsync`) hem de form üzerinde anlık önizleme (`CalculatePreviewAsync`) aynı `BuildDetail` fonksiyonunu çağırır — yani ekranda gördüğünüz "anlık" sonuç ile DB'den gelen sonuç **birebir aynı koddan** üretilir.

---

## 1. Veri Modeli (Entity Yapısı)

Hesaplamayı besleyen üç tablo (entity) vardır. İlişki: **Study → DeviceLine → YearProjection** (1-N-N).

### 1.1. `Study` — Çalışmanın ana kütüğü
[`Study.cs`](../feasibility.entity/Entities/FeasibilityAmortization/Study.cs)

Bir fizibilite çalışmasının üst seviye (global) girdileridir. Sonuç tablosundaki tüm hatlar bu ortak parametreleri paylaşır.

| Alan | Anlamı | Sonuçtaki rolü |
|------|--------|----------------|
| `UsdRate`, `EurRate` | Kur | TL ↔ USD çevrimleri, ROI hesabı |
| `InflationTl/Usd/Eur` | Yıllık enflasyon % | İleriki yılların kur projeksiyonu (`ProjectedUsdRate`) |
| `HasRent`, `MonthlyRentTl` | Aylık kira (TL gölge alan) | Yıllık sabit gider |
| `ContractMonths`, `ContractStartDate` | Sözleşme süresi/başlangıcı | Bilgilendirme / yıl aralığı |
| `MonthlyLostDaysPercent` | Aylık kayıp gün % | `uptimeFactor` → ciro & elektrik düşürücü |
| `PostWarrantyMaintenanceCostTl` | Garanti sonrası bakım (yıllık) | Yıllık sabit gider |
| `AdvertisingRevenueTl` | Reklam geliri (yıllık) | Sabit giderden **düşülür** (gelir) |
| `StationUnitCostTl` | Tek seferlik istasyon birim bedeli | Yatırım + geri kazanılabilir kısım |
| `ProviderEntryFeeTl` | Sağlayıcı giriş bedeli | Yatırım (batık/sunk) |
| `InfrastructureCostTl` | Altyapı bedeli | Yatırım (batık/sunk) |
| `HasLoan`, `LoanAmount`, `LoanAnnualInterestRate`, `LoanTermMonths` | Kredi parametreleri | Kredi taksiti, faiz, BSM, öz kaynak |

> **`*Tl` "gölge" alanları:** Kullanıcı tutarları TRY/USD/EUR girebilir. Kayıt anında `ApplyTlShadows` her tutarı `ToTl()` ile TL'ye sabitler ([satır 701-718](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L701)). Hesaplama **daima TL gölge alanı** üzerinden döner; böylece kur sonradan değişse de geçmiş tutar bozulmaz.

### 1.2. `DeviceLine` — Cihaz hattı (istasyon satırı)
[`DeviceLine.cs`](../feasibility.entity/Entities/FeasibilityAmortization/DeviceLine.cs)

Formdaki istasyon tablosunun her bir satırı. AC/DC tipinde bir cihaz grubu.

| Alan | Anlamı |
|------|--------|
| `DeviceType` | AC / DC |
| `DeviceCount` | Bu hattaki cihaz adedi |
| `SocketCount` | Cihaz başına soket sayısı |
| `DailyChargesPerSocket` | Soket başına günlük şarj sayısı |
| `AvgKwh` | Şarj başına ortalama kWh |
| `SalePriceTl` | kWh satış fiyatı (TL) |
| `PurchasePriceTl` | kWh alış (elektrik maliyeti) fiyatı (TL) |
| `UnitLocationCostTl` | Cihaz başına istasyon/lokasyon bedeli (TL gölge) |
| `AgreementGenre` | **Sözleşme türü:** `Profit` (Kâr) / `Revenue` (Ciro) |
| `AgreementRate` | **Oran** (0–1 ondalık; formdaki "Oran %" / 100) |

### 1.3. `YearProjection` — Yıl bazlı projeksiyon
[`YearProjection.cs`](../feasibility.entity/Entities/FeasibilityAmortization/YearProjection.cs)

Her hattın her yıl için fiyat/kur projeksiyonu. H1 = yılın ilk yarısı (1–5. ay mantığı), H2 = ikinci yarısı.

| Alan | Anlamı |
|------|--------|
| `Year` | Yıl |
| `DailyChargePerSocket` | O yıl için günlük şarj (0 ise hattaki taban değer) |
| `SalePriceH1/H2` | Yarıyıl bazlı kWh satış fiyatı |
| `PurchasePriceH1/H2` | Yarıyıl bazlı kWh alış fiyatı |
| `UsdRate` | O yıla ait kur (bilgi amaçlı) |

> `EffectivePrices()` kuralı: YearProjection'daki değer **> 0 ise onu**, değilse `DeviceLine`'daki taban değeri kullanır ([satır 720-732](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L720)). Yani yıl bazlı boş bırakılan alanlar otomatik taban fiyata düşer.

---

## 2. Çıktı Modeli (`FeasibilityAmortizationDetailDto`)

Sonuç tablosunu besleyen DTO: [`FeasibilityAmortizationDetailDto.cs`](../feasibility.entity/Dtos/FeasibilityAmortization/FeasibilityAmortizationDetailDto.cs).

Başlıca bölümler:
- **Yatırım özeti** → `TotalInvestmentTl/Usd`, `EquityInvestmentUsd`, `RecoverableInvestmentUsd`, `SunkInvestmentUsd`
- **Kredi tablosu** → `LoanSchedule` (`LoanScheduleRowDto`)
- **Hat detayları** → `DeviceLines` (`DeviceLineDetailDto`), her hatta yıllık & aylık kırılım
- **Yıl özetleri** → `YearSummaries` (`YearSummaryDto`) — sonuç tablosunun ana satırları
- **Geri ödeme** → `PaybackYears`, `RoiPercent`, `SunkPaybackYears`, `SunkAmortization`

---

## 3. Hesaplama Zinciri (Adım Adım)

Aşağıdaki tüm akış [`BuildDetail()`](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L196) içinde döner.

### 3.1. Yatırım toplamı
```
oneTimeTl = StationUnitCostTl + ProviderEntryFeeTl + InfrastructureCostTl
deviceTl  = Σ (aktif hat: UnitLocationCostTl × DeviceCount)
totalTl   = oneTimeTl + deviceTl
totalUsd  = totalTl / UsdRate
```
> **Önemli:** `deviceTl` ve diğer toplamlar **yalnızca `IsDeleted == false` (aktif) hatlardan** hesaplanır. Silinmiş hatlar yatırıma da, ciroya da girmez. (Bkz. geçmiş bug notu: silinmiş hatların yatırıma dahil edilmesi.)

### 3.2. Sabit giderler (yıllık)
```
annualRent        = HasRent ? MonthlyRentTl × 12 : 0
annualMaintenance = PostWarrantyMaintenanceCostTl
annualAdvertising = AdvertisingRevenueTl        // gelir, eksi olarak girer
```

### 3.3. Kredi (varsa) — basit faiz + BSM
[satır 207-239](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L207)
```
loanDays          = LoanTermMonths × 30
loanTotalInterest = LoanAmount × LoanAnnualInterestRate × loanDays / 36000   // basit faiz, oran % cinsinden
loanTotalBsm      = loanTotalInterest × 0.05                                  // %5 BSM/KKDF
loanTotalRepay    = LoanAmount + loanTotalInterest + loanTotalBsm
monthlyLoanPayment= loanTotalRepay / LoanTermMonths
```
Aylık taksit tablosu (`LoanSchedule`) eşit anapara + eşit faiz + eşit BSM olarak dağıtılır; kalan anapara her ay düşer.

### 3.4. Kayıp gün faktörü
```
uptimeFactor = 1 − clamp(MonthlyLostDaysPercent, 0, 100) / 100
```
Bu faktör hem ciroyu hem elektrik maliyetini orantılı düşürür (cihaz boşta kalınca ikisi de azalır).

### 3.5. Hat × Yıl üretimi ve cirosu → `ComputeLineYear()`
[satır 734-741](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L734)

Yıl, sabit `h1Days = 151`, `h2Days = 214` (toplam 365 gün) olarak iki yarıya bölünür:
```
eH1 = SocketCount × daily × 151 × AvgKwh × uptimeFactor     // H1 satılan kWh
eH2 = SocketCount × daily × 214 × AvgKwh × uptimeFactor     // H2 satılan kWh

annualRevenue   = eH1 × saleH1  + eH2 × saleH2              // yıllık ciro (TL)
annualElectric  = eH1 × buyH1   + eH2 × buyH2              // yıllık elektrik maliyeti (TL)
```
> Burada `daily`, `saleH1/H2`, `buyH1/H2` değerleri `EffectivePrices()` ile YearProjection veya taban değerden seçilir.

### 3.6. Komisyon (pay) — "Oran %" ve "Sözleşme Türü" burada devreye girer
[satır 276-278](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L276)
```
grossMargin = annualRevenue − annualElectric

commission =
    AgreementGenre == Revenue (Ciro)  →  AgreementRate × annualRevenue
    AgreementGenre == Profit  (Kâr)   →  AgreementRate × grossMargin
```
- **Oran % yükseldikçe → komisyon artar → net kâr ve ROI düşer → amortisman süresi uzar.**
- Aynı oran, "Ciro" seçiliyse genelde daha yüksek komisyon üretir (cironun tamamına uygulanır); "Kâr" seçiliyse sadece elektrik düşülmüş marja uygulanır.

Hat bazında net marj:
```
AnnualNetMarginTl = grossMargin − commission
```

### 3.7. Toplamlar
```
totalRevTl  = Σ annualRevenue (tüm aktif hatlar)
totalElecTl = Σ annualElectric
totalCommTl = Σ commission
```

### 3.8. Yıllık net & ROI
[satır 400-403](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L400)
```
annualFixedCosts = annualRent + annualMaintenance − annualAdvertising + annualLoanPayment
annualNetTl      = totalRevTl − totalElecTl − totalCommTl − annualFixedCosts
annualNetUsd     = annualNetTl / UsdRate
ROI %            = round(annualNetUsd / totalUsd × 100, 1)
```

---

## 4. Sonuç Tablosunun Satırları — `YearSummaries`

Sonuç tablosunun **ana gövdesi** budur. Çalışmadaki tüm yıllar (`allYears`) için döngü çalışır ([satır 427-466](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L427)).

Her yıl için:
```
yRevTl  = Σ (o yılın hat cirosu)
yElecTl = Σ (o yılın hat elektrik maliyeti)
yCommTl = Σ (o yılın hat komisyonu)            // yine Oran × (Ciro veya Marj)

yLoanPmt = o yıl içinde kalan kredi taksiti     // LoanPaymentForYear()
yFixed   = annualRent + annualMaintenance − annualAdvertising + yLoanPmt
yNetTl   = yRevTl − yElecTl − yCommTl − yFixed
yNetUsd  = round(yNetTl / ProjectedUsdRate(year), 0)

cumUsd  += yNetUsd                              // kümülatif bakiye
```

| Sütun (`YearSummaryDto`) | Formül |
|--------------------------|--------|
| `TotalRevenueTl` | yıllık toplam ciro |
| `TotalElectricityCostTl` | yıllık toplam elektrik maliyeti |
| `TotalCommissionTl` | yıllık toplam komisyon (**Oran etkisi**) |
| `FixedCostsTl` | kira + bakım − reklam + kredi |
| `NetProfitTl` | `yNetTl` |
| `NetProfitUsd` | `yNetTl / ProjectedUsdRate` |
| `CumulativeBalanceUsd` | öz kaynaktan başlayıp her yıl net eklenen bakiye |

> **Kur projeksiyonu** (`ProjectedUsdRate`, [satır 252-259](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L252)): taban yıldan sonraki her yıl için kur, `UsdRate × (1 + InflationUsd/100)^offset` ile büyütülür. Böylece ileriki yılların TL→USD çevrimi enflasyona göre ayarlanır.

---

## 5. Geri Ödeme (Amortisman) Süreleri

### 5.1. Öz kaynak geri ödemesi — `PaybackYears`
[satır 423-481](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L423)
```
loanUsd   = LoanAmount / UsdRate
equityUsd = totalUsd − loanUsd                 // krediyle finanse edilmeyen, cepten çıkan kısım
cumUsd başlangıç = −equityUsd
```
Kümülatif bakiye (`CumulativeBalanceUsd`) ilk kez **≥ 0** olduğu yıl geri ödeme yılıdır; yıl içi kesir doğrusal interpolasyonla (`need / NetProfitUsd`) eklenir.

### 5.2. Batık yatırım amortismanı — `SunkPaybackYears` / `SunkAmortization`
[satır 483-519](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L483)

Yatırım ikiye ayrılır:
```
stationTl = StationUnitCostTl + deviceTl       // taşınabilir / geri kazanılabilir donanım
sunkUsd   = totalUsd − stationUsd               // batık (giriş bedeli + altyapı gibi geri alınamayan)
```
`SunkAmortization` tablosu, her yılın net kârıyla **sadece batık tutarın** ne zaman geri kazanıldığını izler; karşılandığı yıl `IsPaybackYear = true` olur.

| Alan (`SunkAmortizationRowDto`) | Anlamı |
|------|--------|
| `NetProfitUsd` | o yılın net kârı |
| `RecoveredCumulativeUsd` | o yıla kadar geri kazanılan kümülatif |
| `RemainingUsd` | kalan batık tutar |
| `IsPaybackYear` | batığın kapandığı yıl |

---

## 6. Hat Detayı: Aylık Kırılım — `MonthlyBreakdownYears`

Her hat için 12 aylık ayrıntı üretilir ([satır 315-376](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L315)). Bu, sonuç ekranındaki hat-açılır detay tablosunu besler.

Her ay (`daysPerMonth` gerçek gün sayısıyla):
```
lostDays   = days × MonthlyLostDaysPercent / 100
netOpDays  = days − lostDays
isH1       = (ay ≤ 5)                            // ilk 5 ay H1 fiyatı, kalanı H2
saleKwh    = SocketCount × daily × AvgKwh × netOpDays
revTl      = saleKwh × salePrice
elecTl     = saleKwh × buyPrice
commTl     = Oran × (Ciro ise revTl, Kâr ise revTl−elecTl)
rentTl     = MonthlyRentTl / toplam cihaz adedi  // kira cihaz başına eşit dağıtılır
loanTl     = (vade içindeyse) hattın kredi payı
grossTl    = revTl − commTl − rentTl − elecTl − loanTl
```

> **Kredi taksitinin hatlara dağıtımı** ([satır 310-313](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L310)): taksit cihaz adedine değil, **hattın yatırım büyüklüğü oranında** (`lineInvestment / deviceTl`) paylaştırılır — pahalı DC hattı daha çok taşır. Bu pay sadece aylık brüt kâr gösteriminde düşülür; toplam net kâr/ROI değişmez.

TL→USD çevrimi her ay için `ProjectedUsdRate(year)` ile yapılır.

---

## 7. Özet Akış Şeması

```
Study (global girdiler) ───┐
                           ├──► BuildDetail()
DeviceLine (hatlar) ───────┤        │
  └─ YearProjection ───────┘        │
                                    ▼
        ┌────────────── EffectivePrices() ──► daily/sale/buy fiyatları
        │
        ▼
   ComputeLineYear()  ──► annualRevenue, annualElectric
        │
        ▼
   grossMargin = ciro − elektrik
   commission  = Oran × (Ciro? ciro : grossMargin)        ◄── "Oran %" + "Sözleşme Türü"
        │
        ├─► Hat detayı (yıllık + aylık kırılım)
        │
        ▼
   YearSummaries (yıl yıl: ciro, elektrik, komisyon, sabit gider, net)
        │
        ├─► annualNetTl ─► ROI %
        ├─► CumulativeBalanceUsd ─► PaybackYears (öz kaynak)
        └─► SunkAmortization ─► SunkPaybackYears (batık)
```

---

## 8. Kritik Noktalar / Tuzaklar

1. **Sadece aktif hatlar (`!IsDeleted`)** her toplamda kullanılır — yatırım, ciro, komisyon hepsi. Silinmiş hatların hesaba sızması geçmişte yatırımı şişiren bir bug'a yol açmıştı.
2. **TL gölge alanları** üzerinden hesaplama yapılır; kullanıcının girdiği orijinal para birimi sadece görüntüleme/yeniden yükleme içindir.
3. **`AgreementRate` DB'de ondalıktır (0–1)**; formdaki "Oran %" gösterimi `× 100` / `/ 100` ile dönüştürülür ([preload satır 605](../feasbility.business/Concrete/FeasibilityAmortizationManager.cs#L605), [kayıt `agreementRate = s.oran / 100`]).
4. **Kredi toplam net kârı değiştirmez**, sadece zamanlama (aylık brüt kâr ve öz kaynak/payback) üzerinde etkilidir.
5. **Önizleme = Kayıt:** `CalculatePreviewAsync` ile `GetDetailAsync` aynı `BuildDetail`'i çağırdığı için ekranda gördüğünüz anlık sonuç, kaydedince birebir korunur.
