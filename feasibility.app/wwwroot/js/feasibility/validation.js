/*
 * Amortisman fizibilitesi — ortak validasyon katmanı (Create + Edit ortak kullanır).
 *
 * Amaç: kullanıcı saçma/sınır dışı değer girince (örn. yüzde alanına 150) sonucu
 * patlatmadan, TÜM ihlalleri tek seferde toplayıp her alanın HEMEN ALTINDA
 * (.fiz-err / .fiz-cell-in.bad) inline göstermek.
 *
 * Sınırlar iş kararıdır; tek yerde FizBounds'ta tutulur.
 */
(function (global) {
    'use strict';

    // Tek doğruluk kaynağı: alan sınırları. [min, max] dahil aralık.
    var FizBounds = {
        // Yüzdeler
        komisyonOran:   { min: 0,    max: 99,          label: 'Sözleşme komisyon oranı', unit: '%' },
        kayipGun:       { min: 0,    max: 99,          label: 'Aylık kayıp gün',         unit: '%' },
        faizOran:       { min: 0,    max: 100,         label: 'Yıllık faiz oranı',       unit: '%' },
        enflasyon:      { min: 0,    max: 100,         label: 'Enflasyon',               unit: '%' },
        zam:            { min: 0,    max: 100,         label: 'Otomatik zam',            unit: '%' },
        // Kur
        kur:            { min: 0,    max: 10000,       label: 'Döviz kuru',              unit: '', exclusiveMin: true },
        // Tutarlar (para)
        tutar:          { min: 0,    max: 999999999,   label: 'Tutar',                   unit: '' },
        fiyat:          { min: 0,    max: 999999999,   label: 'Fiyat',                   unit: '', exclusiveMin: true },
        // Adet / süre (tam sayı)
        adet:           { min: 1,    max: 9999,        label: 'Adet',                    unit: '', integer: true },
        soket:          { min: 1,    max: 9999,        label: 'Soket adeti',             unit: '', integer: true },
        sozlesmeAy:     { min: 1,    max: 600,         label: 'Sözleşme süresi',         unit: 'ay', integer: true },
        vadeAy:         { min: 1,    max: 600,         label: 'Vade',                    unit: 'ay', integer: true },
        // Şarjlanma
        gunlukSarj:     { min: 0,    max: 1000,        label: 'Günlük soket başı şarjlanma', unit: '', exclusiveMin: true },
        ortKwh:         { min: 0,    max: 100000,      label: 'Ortalama şarjlanma (kWh)',    unit: '', exclusiveMin: true }
    };

    function fmtLimit(n) {
        return isFinite(n) ? n.toLocaleString('tr-TR') : '' + n;
    }

    // Bir sayıyı sınıra çeker (canlı giriş düzeltmesi için). Boş/NaN ise min döner.
    function clampNum(value, key) {
        var b = FizBounds[key];
        if (!b) return value;
        var n = typeof value === 'number' ? value : parseFloat(('' + value).replace(/\./g, '').replace(',', '.'));
        if (!isFinite(n)) return b.min;
        if (b.integer) n = Math.trunc(n);
        if (n < b.min) n = b.min;
        if (n > b.max) n = b.max;
        return n;
    }

    // Tek bir değeri sınıra göre doğrular. Geçerliyse null, değilse hata mesajı döner.
    // required=false ise boş değere izin verir (boşsa null döner).
    function checkValue(value, key, opts) {
        opts = opts || {};
        var b = FizBounds[key];
        if (!b) return null;

        var raw = ('' + (value === undefined || value === null ? '' : value)).trim();
        if (raw === '') {
            return opts.required ? (b.label + ' zorunludur.') : null;
        }

        var n = typeof value === 'number' ? value : parseFloat(raw.replace(/\./g, '').replace(',', '.'));
        if (!isFinite(n)) return b.label + ' geçerli bir sayı olmalıdır.';

        if (b.integer && Math.trunc(n) !== n) return b.label + ' tam sayı olmalıdır.';

        var lo = b.min, hi = b.max;
        var belowMin = b.exclusiveMin ? (n <= lo) : (n < lo);
        if (belowMin) {
            return b.exclusiveMin
                ? (b.label + ' ' + fmtLimit(lo) + ' değerinden büyük olmalıdır.')
                : (b.label + ' en az ' + fmtLimit(lo) + (b.unit ? ' ' + b.unit : '') + ' olmalıdır.');
        }
        if (n > hi) {
            return b.label + ' en fazla ' + fmtLimit(hi) + (b.unit ? ' ' + b.unit : '') + ' olabilir.';
        }
        return null;
    }

    /* ---------- Inline hata gösterimi ---------- */

    // Bir .fiz-field'ı hatalı işaretler ve mesajı alanın altındaki .fiz-err'e yazar.
    function markField(fieldId, message) {
        var field = document.getElementById(fieldId);
        if (!field) return;
        field.classList.add('bad');
        var err = field.querySelector('.fiz-err');
        if (err && message) err.textContent = message;
    }

    // İstasyon tablosundaki bir hücreyi hatalı işaretler ve satırı vurgular.
    // Mesaj, satırın altında tek bir özet hücresinde toplanır.
    function markCell(rowIdx, key) {
        var row = document.querySelector('tr[data-row="' + rowIdx + '"]');
        if (!row) return;
        row.classList.add('fiz-row-bad');
        var cell = row.querySelector('input[data-key="' + key + '"]');
        if (cell) cell.classList.add('bad');
    }

    // Önceki tüm validasyon izlerini temizler.
    function clearValidation() {
        document.querySelectorAll('.fiz-field.bad').forEach(function (f) { f.classList.remove('bad'); });
        document.querySelectorAll('.fiz-cell-in.bad').forEach(function (c) { c.classList.remove('bad'); });
        document.querySelectorAll('.fiz-row-bad').forEach(function (r) { r.classList.remove('fiz-row-bad'); });
        document.querySelectorAll('.fiz-empty.bad').forEach(function (e) { e.classList.remove('bad'); });
        document.querySelectorAll('.fiz-cell-errbox').forEach(function (e) { e.remove(); });
    }

    /*
     * Tüm formu doğrular. İhlallerin HEPSİNİ toplar (ilkinde durmaz),
     * her birini ilgili alanın altında inline gösterir.
     *
     * cfg = {
     *   stations: [...],            // istasyon satırları (zorunlu)
     *   requireContractMonths: bool // Create'te zorunlu, Edit'te opsiyonel
     * }
     * Dönüş: { ok: bool, count: number }
     */
    function validateFeasibility(cfg) {
        cfg = cfg || {};
        var stations = cfg.stations || [];
        clearValidation();

        var errors = [];          // { type:'field'|'cell', id/row/key, message }
        var firstBadCard = null;
        function markCard(id) {
            if (!firstBadCard) firstBadCard = id;
            var c = document.getElementById(id);
            if (c) c.classList.remove('collapsed');
        }

        var byId = function (id) { var el = document.getElementById(id); return el ? el.value : ''; };

        // 01 — Temel bilgiler
        if (!byId('loc'))            { markField('f-loc',  'Lokasyon seçiniz.'); errors.push({ type: 'field', id: 'f-loc',  message: 'Lokasyon seçiniz.' }); markCard('card-01'); }
        if (!('' + byId('fname')).trim()) { markField('f-name', 'Fizibilite adı zorunludur.'); errors.push({ type: 'field', id: 'f-name', message: 'Fizibilite adı zorunludur.' }); markCard('card-01'); }
        if (!byId('sozlesmeBaslangic')) { markField('f-startdate', 'Sözleşme başlangıç tarihi zorunludur.'); errors.push({ type: 'field', id: 'f-startdate', message: 'Sözleşme başlangıç tarihi zorunludur.' }); markCard('card-01'); }

        // 02 — Enflasyon (TL/USD/EUR) — tabloda input yoksa atla
        ['TL', 'USD', 'EUR'].forEach(function () {});
        if (Array.isArray(global.INF)) {
            global.INF.forEach(function (item) {
                var msg = checkValue(item.v, 'enflasyon');
                if (msg) { errors.push({ type: 'inf', message: item.k + ' enflasyonu: ' + msg }); markCard('card-02'); }
            });
        }

        // 03 — En az bir istasyon
        if (stations.length === 0) {
            var card03 = document.getElementById('card-03');
            if (card03) {
                card03.classList.remove('collapsed');
                var emptyBox = document.querySelector('#stWrap .fiz-empty');
                if (emptyBox) {
                    emptyBox.classList.add('bad');
                    var p = emptyBox.querySelector('p');
                    if (p) p.textContent = 'En az bir istasyon türü eklemelisiniz. Başlamak için aşağıdaki "İstasyon Türü Ekle" butonuna tıklayın.';
                }
                card03.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
            return { ok: false, count: 1 };
        }

        // 03 — Her istasyon satırı: alan -> sınır eşlemesi
        var stRules = [
            { key: 'adet',    bound: 'adet' },
            { key: 'soket',   bound: 'soket' },
            { key: 'daily',   bound: 'gunlukSarj' },
            { key: 'avgkwh',  bound: 'ortKwh' },
            { key: 'pTL',     bound: 'fiyat' },
            { key: 'alisKwh', bound: 'fiyat' },
            { key: 'bedel',   bound: 'tutar' },
            { key: 'oran',    bound: 'komisyonOran' }
        ];
        stations.forEach(function (s, i) {
            var rowMsgs = [];
            stRules.forEach(function (r) {
                var bound = FizBounds[r.bound];
                // Pozitif zorunlu alanlar (adet/soket/şarj/fiyat/bedel) boş geçilemez; oran/komisyon 0 olabilir.
                var required = r.bound !== 'komisyonOran';
                var msg = checkValue(s[r.key], r.bound, { required: required });
                if (msg) { markCell(i, r.key); rowMsgs.push(msg); }
            });

            // Alış fiyatı satış fiyatından yüksek olamaz.
            var satis = parseFloat('' + s.pTL);
            var alis  = parseFloat('' + s.alisKwh);
            if (isFinite(satis) && isFinite(alis) && alis > satis) {
                markCell(i, 'alisKwh');
                rowMsgs.push('Alış fiyatı (TL/kWh), satış fiyatından (' + satis.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' TL) yüksek olamaz.');
            }

            if (rowMsgs.length) {
                errors.push({ type: 'cell', row: i });
                markCard('card-03');
            }
        });

        // 04 — Sözleşme süresi (her zaman zorunlu)
        var sozMsg = checkValue(byId('sozlesmeAy'), 'sozlesmeAy', { required: true });
        if (sozMsg) { markField('f-sozlesme', sozMsg); errors.push({ type: 'field', id: 'f-sozlesme', message: sozMsg }); markCard('card-04'); }

        // 04 — Aylık kayıp gün
        var kayipMsg = checkValue(byId('kayipGunYuzde'), 'kayipGun');
        if (kayipMsg) { markField('f-kayipgun', kayipMsg); errors.push({ type: 'field', id: 'f-kayipgun', message: kayipMsg }); markCard('card-04'); }

        // 04 — Tutar alanları
        // required: kira/reklam checkbox'a bağlı (açıksa zorunlu), bakim/giris/altyapi opsiyonel (boş geçilebilir)
        [
            { id: 'kira',    bound: 'tutar', guard: 'kiraVar',   required: true  },
            { id: 'reklam',  bound: 'tutar', guard: 'reklamVar', required: true  },
            { id: 'bakim',   bound: 'tutar',                     required: false },
            { id: 'giris',   bound: 'tutar',                     required: false },
            { id: 'altyapi', bound: 'tutar',                     required: false }
        ].forEach(function (f) {
            var el = document.getElementById(f.id);
            if (!el) return;
            if (f.guard) { var g = document.getElementById(f.guard); if (g && !g.checked) return; }
            var msg = checkValue(el.value, f.bound, { required: f.required });
            if (msg) { el.classList.add('bad'); errors.push({ type: 'finput', id: f.id, message: msg }); markCard('card-04'); appendInputError(el, msg); }
        });

        // 05 — Kredi (yalnızca kredi işaretliyse)
        var krediVar = document.getElementById('krediVar');
        if (krediVar && krediVar.checked) {
            var loanChecks = [
                { id: 'krediTutar', bound: 'tutar',    required: true },
                { id: 'faizOran',   bound: 'faizOran', required: true },
                { id: 'vadeAy',     bound: 'vadeAy',   required: true }
            ];
            loanChecks.forEach(function (c) {
                var el = document.getElementById(c.id);
                if (!el) return;
                var msg = checkValue(el.value, c.bound, { required: c.required });
                if (msg) { el.classList.add('bad'); errors.push({ type: 'finput', id: c.id, message: msg }); markCard('card-05'); appendInputError(el, msg); }
            });
        }

        var ok = errors.length === 0;
        if (!ok && firstBadCard) {
            var card = document.getElementById(firstBadCard);
            if (card) card.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
        return { ok: ok, count: errors.length };
    }

    // Bir input'un hemen altına inline hata satırı ekler (kira/bakım/kredi gibi
    // .fiz-err'i olmayan alanlar için).
    function appendInputError(inputEl, message) {
        var field = inputEl.closest('.fiz-field');
        if (!field) return;
        var err = field.querySelector('.fiz-err');
        if (err) { field.classList.add('bad'); err.textContent = message; return; }
        // .fiz-err yoksa dinamik bir tane oluştur.
        var box = document.createElement('span');
        box.className = 'fiz-err fiz-cell-errbox';
        box.style.display = 'block';
        box.textContent = message;
        field.appendChild(box);
    }

// İstasyon satırında alış/satış fiyatı tutarlılığını anlık kontrol eder.
    // satis veya alis boş/geçersizse uyarı vermez.
    // Dönüş: ihlal varsa hata mesajı string, yoksa null.
    function checkAlisVsSatis(satis, alis) {
        var s = parseFloat('' + satis);
        var a = parseFloat('' + alis);
        if (!isFinite(s) || !isFinite(a)) return null;
        if (a > s) {
            return 'Alış fiyatı (' + a.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) +
                   ' TL/kWh), satış fiyatından (' + s.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) +
                   ' TL/kWh) yüksek olamaz.';
        }
        return null;
    }

    // Dışa aç
    global.FizValidation = {
        bounds: FizBounds,
        clamp: clampNum,
        check: checkValue,
        clear: clearValidation,
        validate: validateFeasibility,
        checkAlisVsSatis: checkAlisVsSatis
    };
})(window);
