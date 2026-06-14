/* Tarife Fizibilitesi - ortak form mantığı (Create + Edit) */
(function () {
    'use strict';

    // ---- Sabitler / yardımcılar ----
    var ST_TYPES = ["AC", "DC"];
    var CCY = ["TRY", "USD", "EUR"];
    var CCY_LABELS = { USD: '$', TRY: 'TL', EUR: '€' };
    var ccyMap = { 'TRY': 0, 'USD': 1, 'EUR': 2 };
    var ccyRev = ['TRY', 'USD', 'EUR'];
    var deviceTypeMap = { 'AC': 0, 'DC': 1 };

    var fmt = function (n, d) { d = d === undefined ? 2 : d; return isFinite(n) ? n.toLocaleString('tr-TR', { minimumFractionDigits: d, maximumFractionDigits: d }) : '-'; };
    var parse = function (v) { v = ('' + v).replace(/\./g, '').replace(',', '.'); var n = parseFloat(v); return isNaN(n) ? 0 : n; };
    var isEmptyVal = function (v) { return v === '' || v === null || v === undefined || (typeof v === 'number' && isNaN(v)); };
    var fmtCell = function (v, d) { return isEmptyVal(v) ? '' : fmt(+v, d); };
    var parseCell = function (v) { if (('' + v).trim() === '') return ''; var n = parse(v); return n < 0 ? 0 : n; };
    var showToast = function (msg, isErr) {
        if (window.alertify) alertify[isErr ? 'error' : 'success'](msg);
        else if (isErr) alert(msg);
    };
    var clampInt = function (v, min, max) { v = Math.round(v); if (v < min) v = min; if (v > max) v = max; return v; };
    var clampNum = function (v, min, max) { if (v < min) v = min; if (v > max) v = max; return v; };

    // ---- Kur / Enflasyon ----
    var FX = [
        { code: "USD/TRY", unit: 1, name: "ABD DOLARI", buy: _fx.usdBuy, sell: _fx.usdSell },
        { code: "EUR/TRY", unit: 1, name: "EURO",       buy: _fx.eurBuy, sell: _fx.eurSell }
    ];
    var INF = [
        { k: "TL",  v: (_tlInf  !== null ? _tlInf  : 23)  },
        { k: "EUR", v: (_eurInf !== null ? _eurInf : 2.3) },
        { k: "USD", v: (_usdInf !== null ? _usdInf : 2.9) }
    ];
    // ---- Fiyatlandırma sabitleri (KDV / komisyon) ----
    var PRICING = { vat: 20, commission: 2.7 };

    var USD = function () { return FX[0].sell; };
    var EUR = function () { return FX[1].sell; };
    var USD_PER_EUR = function () { return FX[1].sell / FX[0].sell; };

    function renderFx() {
        document.getElementById('fxBody').innerHTML = FX.map(function (f) {
            return '<tr>' +
                '<td class="mono" style="font-weight:600">' + f.code + '</td>' +
                '<td class="text-center mono">' + f.unit + '</td>' +
                '<td>' + f.name + '</td>' +
                '<td class="text-right mono">' + fmt(f.buy, 3) + '</td>' +
                '<td class="text-right mono">' + fmt(f.sell, 3) + '</td>' +
                '</tr>';
        }).join('');
    }
    function renderInf() {
        document.getElementById('infBody').innerHTML = INF.map(function (item, idx) {
            return '<tr>' +
                '<td style="font-weight:600;text-transform:uppercase">' + item.k + '</td>' +
                '<td class="text-right"><input class="fiz-cell-in" value="' + fmt(item.v, 2) + '" onchange="PricingForm.setInf(' + idx + ',this)"></td>' +
                '</tr>';
        }).join('');
    }
    function setInf(idx, el) {
        var n = clampNum(parse(el.value), 0, 100);
        INF[idx].v = n; el.value = fmt(n, 2);
    }
    function renderPricing() {
        var body = document.getElementById('pricingBody');
        if (!body) return;
        body.innerHTML =
            '<tr><td style="font-weight:600">KDV Oranı</td>' +
            '<td class="text-right"><input class="fiz-cell-in" value="' + fmt(PRICING.vat, 2) + '" onchange="PricingForm.setPricing(\'vat\',this)"></td></tr>' +
            '<tr><td style="font-weight:600">Banka / eMSP Komisyon Oranı</td>' +
            '<td class="text-right"><input class="fiz-cell-in" value="' + fmt(PRICING.commission, 2) + '" onchange="PricingForm.setPricing(\'commission\',this)"></td></tr>';
    }
    function setPricing(key, el) {
        var max = key === 'commission' ? 99 : 100;
        var n = clampNum(parse(el.value), 0, max);
        PRICING[key] = n; el.value = fmt(n, 2);
    }
    function refreshFx() {
        fetch('/FeasibilityPricing/GetRates')
            .then(function (r) { return r.json(); })
            .then(function (d) {
                FX[0].buy = d.usdBuy; FX[0].sell = d.usdSell;
                FX[1].buy = d.eurBuy; FX[1].sell = d.eurSell;
                renderFx(); renderStations(); recalcCapex();
                showToast('Kur verileri güncellendi.');
            })
            .catch(function () { showToast('Kur verisi alınamadı.', true); });
    }

    function toTl(v, ccy) {
        if (ccy === 'USD') return v * USD();
        if (ccy === 'EUR') return v * EUR();
        return v;
    }
    function convText(v, ccy) {
        if (ccy === 'USD') {
            return 'TL <span class="calc">' + fmt(v * USD(), 2) + '</span>' +
                   ' &nbsp;&middot;&nbsp; EUR <span class="calc">' + fmt(v / USD_PER_EUR(), 2) + '</span>';
        } else if (ccy === 'TRY') {
            return '$ <span class="calc">' + fmt(v / USD(), 4) + '</span>' +
                   ' &nbsp;&middot;&nbsp; EUR <span class="calc">' + fmt(v / EUR(), 4) + '</span>';
        } else {
            return '$ <span class="calc">' + fmt(v * USD_PER_EUR(), 2) + '</span>' +
                   ' &nbsp;&middot;&nbsp; TL <span class="calc">' + fmt(v * EUR(), 2) + '</span>';
        }
    }

    // ---- CAPEX alanları ----
    var CAPEX = [
        { id: 'hardware', label: 'Donanım Maliyeti',         ccy: 'USD', val: 0 },
        { id: 'infra',    label: 'Altyapı Kurulum Maliyeti', ccy: 'USD', val: 0 },
        { id: 'opex',     label: 'Yıllık Sabit OPEX',        ccy: 'USD', val: 0 }
    ];

    function capexFieldHtml(f) {
        var pre = CCY_LABELS[f.ccy] || 'TL';
        var ccyOpts = CCY.map(function (c) {
            return '<option value="' + c + '"' + (c === f.ccy ? ' selected' : '') + '>' + c + '</option>';
        }).join('');
        return '<div class="fiz-field">' +
            '<label>' + f.label + '</label>' +
            '<div class="fiz-input-row">' +
                '<div class="fiz-input-wrap" style="flex:1;">' +
                    '<span class="fiz-pre" id="' + f.id + '_pre">' + pre + '</span>' +
                    '<input class="fiz-ctl" id="' + f.id + '" value="' + fmt(f.val, 2) + '">' +
                '</div>' +
                '<select class="fiz-ccy-sel" id="' + f.id + '_ccy" onchange="PricingForm.setCapexCcy(\'' + f.id + '\',this.value)">' + ccyOpts + '</select>' +
            '</div>' +
            '<div class="fiz-conv" id="' + f.id + '_conv"></div>' +
        '</div>';
    }
    function renderCapex() {
        document.getElementById('capexGrid').innerHTML = CAPEX.map(capexFieldHtml).join('');
        CAPEX.forEach(function (f) {
            var el = document.getElementById(f.id);
            if (!el) return;
            el.oninput = function (e) { e.target.classList.remove('bad'); var fl = e.target.closest('.fiz-field'); if (fl) fl.classList.remove('bad'); recalcCapex(); };
            el.onblur = function (e) { var n = clampNum(parse(e.target.value), 0, 100000000); e.target.value = fmt(n, 2); recalcCapex(); };
        });
        recalcCapex();
    }
    function setCapexCcy(id, newCcy) {
        var f = CAPEX.find(function (x) { return x.id === id; });
        if (!f) return;
        f.ccy = newCcy;
        var preEl = document.getElementById(id + '_pre');
        if (preEl) preEl.textContent = CCY_LABELS[newCcy] || newCcy;
        recalcCapex();
    }
    function recalcCapex() {
        CAPEX.forEach(function (f) {
            var el = document.getElementById(f.id); if (!el) return;
            var v = parse(el.value);
            var convEl = document.getElementById(f.id + '_conv');
            if (convEl) convEl.innerHTML = convText(v, f.ccy);
        });
    }

    // ---- İstasyonlar ----
    var stations = [];

    function usedTypes(except) { return stations.filter(function (s, i) { return i !== except; }).map(function (s) { return s.type; }); }
    function nextType() { return ST_TYPES.find(function (t) { return !usedTypes(-1).includes(t); }); }

    function addStation() {
        var t = nextType();
        if (!t) { showToast('Tüm istasyon türleri eklendi.', true); return; }
        var defRoi = t === 'DC' ? 16 : 10;
        stations.push({ type: t, soket: 0, dailyKwh: 0, economicLife: 0, discountRate: 0, targetProfit: defRoi, ebmLow: 0, ebmHigh: 0 });
        renderStations();
    }
    function delStation(i) { stations.splice(i, 1); renderStations(); }

    function renderStations() {
        var wrap = document.getElementById('stWrap');
        if (stations.length === 0) {
            wrap.innerHTML = '<div class="fiz-empty"><span class="material-symbols-outlined">table_rows</span><p>Henüz istasyon eklenmedi. Başlamak için "İstasyon Türü Ekle"ye tıklayın.</p></div>';
        } else {
            var rows = stations.map(function (s, i) {
                var typeOpt = ST_TYPES.map(function (o) {
                    return '<option ' + (o === s.type ? 'selected' : '') + ' ' + (usedTypes(i).includes(o) ? 'disabled' : '') + '>' + o + '</option>';
                }).join('');
                return '<tr data-row="' + i + '">' +
                    '<td><select class="fiz-cell-sel" onchange="PricingForm.setSt(' + i + ',\'type\',this.value)">' + typeOpt + '</select></td>' +
                    '<td><input class="fiz-cell-in" value="' + fmtCell(s.soket, 0) + '" onchange="PricingForm.setNum(' + i + ',\'soket\',this,1,1000,true)"></td>' +
                    '<td><input class="fiz-cell-in" value="' + fmtCell(s.dailyKwh, 2) + '" onchange="PricingForm.setNum(' + i + ',\'dailyKwh\',this,0,10000,false)"></td>' +
                    '<td><input class="fiz-cell-in" value="' + fmtCell(s.economicLife, 0) + '" onchange="PricingForm.setNum(' + i + ',\'economicLife\',this,1,50,true)"></td>' +
                    '<td><input class="fiz-cell-in" value="' + fmtCell(s.discountRate, 2) + '" onchange="PricingForm.setNum(' + i + ',\'discountRate\',this,0,100,false)"></td>' +
                    '<td><input class="fiz-cell-in" value="' + fmtCell(s.targetProfit, 2) + '" onchange="PricingForm.setNum(' + i + ',\'targetProfit\',this,10,16,false)"></td>' +
                    '<td><input class="fiz-cell-in" value="' + fmtCell(s.ebmLow, 4) + '" onchange="PricingForm.setNum(' + i + ',\'ebmLow\',this,0,100,false)"></td>' +
                    '<td><input class="fiz-cell-in" value="' + fmtCell(s.ebmHigh, 4) + '" onchange="PricingForm.setNum(' + i + ',\'ebmHigh\',this,0,100,false)"></td>' +
                    '<td class="text-right mono">' + (s.ebmLow !== '' && s.ebmHigh !== '' ? fmt((+s.ebmLow + +s.ebmHigh) / 2, 4) : '—') + '</td>' +
                    '<td class="text-center"><div style="display:flex;gap:4px;justify-content:center;">' +
                        '<button class="fiz-icon-btn del" title="Satırı sil" onclick="PricingForm.delStation(' + i + ')"><span class="material-symbols-outlined">delete</span></button>' +
                    '</div></td></tr>';
            }).join('');
            wrap.innerHTML = '<table class="table"><thead><tr>' +
                '<th>İstasyon Türü</th>' +
                '<th class="text-right">Soket Adeti</th>' +
                '<th class="text-right">Soket Başı Günlük Kullanım (kWh)</th>' +
                '<th class="text-right">Ekonomik Ömür (yıl)</th>' +
                '<th class="text-right">İskonto Oranı (%)</th>' +
                '<th class="text-right">Hedef Kâr Marjı / ROI (%)</th>' +
                '<th class="text-right">EBM En Düşük (USD/kWh)</th>' +
                '<th class="text-right">EBM En Yüksek (USD/kWh)</th>' +
                '<th class="text-right">EBM Ortalama (oto)</th>' +
                '<th class="text-center">İşlemler</th>' +
                '</tr></thead><tbody>' + rows + '</tbody></table>';
        }
        document.getElementById('addSt').disabled = stations.length >= ST_TYPES.length;
        document.getElementById('stCount').textContent = stations.length;
    }
    function setSt(i, k, v) { stations[i][k] = v; renderStations(); }
    function setNum(i, k, el, min, max, integer) {
        var raw = parseCell(el.value);
        if (raw !== '') raw = integer ? clampInt(raw, min, max) : clampNum(raw, min, max);
        stations[i][k] = raw;
        renderStations();
    }

    // ---- Validasyon ----
    function markBad(id, bad) {
        var el = document.getElementById(id);
        if (el) el.classList.toggle('bad', !!bad);
    }
    function validateForm() {
        var errors = 0;
        var name = document.getElementById('fname').value.trim();
        markBad('f-name', !name); if (!name) errors++;
        if (stations.length === 0) { showToast('En az bir istasyon ekleyiniz.', true); errors++; }
        stations.forEach(function (s) {
            if (!(+s.soket >= 1) || !(+s.dailyKwh > 0) || !(+s.economicLife >= 1)) errors++;
            if (!(+s.targetProfit >= 10) || !(+s.targetProfit <= 16)) errors++;
            if (!(+s.ebmLow > 0) || !(+s.ebmHigh > 0)) errors++;
            else if (+s.ebmLow > +s.ebmHigh) { showToast('En düşük EBM, en yüksek EBM\'den büyük olamaz.', true); errors++; }
        });
        if (errors > 0) showToast('Lütfen zorunlu alanları doldurunuz (' + errors + ' eksik/hatalı).', true);
        return errors === 0;
    }

    // ---- Payload / kaydet ----
    function collectPayload() {
        function capVal(id) { var el = document.getElementById(id); return el ? parse(el.value) : 0; }
        function capCcy(id) { var f = CAPEX.find(function (x) { return x.id === id; }); return f ? (ccyMap[f.ccy] || 0) : 0; }

        return {
            feasibilityName: document.getElementById('fname').value.trim(),
            kind:            0,
            saveAsNewVersion: false,
            baseVersion:      _preload ? (_preload.version || 1) : 1,
            usdRate:         FX[0].sell,
            eurRate:         FX[1].sell,
            inflationTl:     INF[0].v,
            inflationUsd:    INF[2].v,
            inflationEur:    INF[1].v,
            vatRate:         PRICING.vat,
            commissionRate:  PRICING.commission,
            stations: stations.map(function (s) {
                return {
                    deviceType:         deviceTypeMap[s.type] !== undefined ? deviceTypeMap[s.type] : 0,
                    socketCount:        +s.soket || 0,
                    dailyKwhPerSocket:  +s.dailyKwh || 0,
                    economicLifeYears:  +s.economicLife || 0,
                    discountRate:       +s.discountRate || 0,
                    targetProfitMargin: +s.targetProfit || 0,
                    hardwareCost:           capVal('hardware'),
                    hardwareCostCurrency:   capCcy('hardware'),
                    infrastructureCost:         capVal('infra'),
                    infrastructureCostCurrency: capCcy('infra'),
                    annualOpex:             capVal('opex'),
                    annualOpexCurrency:     capCcy('opex'),
                    // Ortalama EBM girilmez; (en düşük + en yüksek) / 2 olarak hesaplanır (sunucu da doğrular).
                    gridElectricityCost:     ((+s.ebmLow || 0) + (+s.ebmHigh || 0)) / 2,
                    gridElectricityCostLow:  +s.ebmLow || 0,
                    gridElectricityCostHigh: +s.ebmHigh || 0
                };
            })
        };
    }

    function setSaveLoading(on) {
        var btn = document.getElementById('saveBtn');
        if (!btn) return;
        if (on) {
            btn.dataset.html = btn.innerHTML;
            btn.disabled = true;
            btn.classList.add('is-loading');
            btn.innerHTML = '<span class="material-symbols-outlined fiz-spin">progress_activity</span> Kaydediliyor…';
        } else {
            btn.disabled = false;
            btn.classList.remove('is-loading');
            if (btn.dataset.html) btn.innerHTML = btn.dataset.html;
        }
    }

    // Create modunda: doğrudan yeni kayıt oluşturur (versiyon seçimi yok).
    function submitForm() {
        if (!validateForm()) return;
        var payload = collectPayload();
        setSaveLoading(true);
        fetch('/FeasibilityPricing/Save', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        })
        .then(function (r) {
            if (!r.ok) return r.text().then(function (t) {
                var msg = 'Sunucu hatası (' + r.status + ')';
                try { msg = JSON.parse(t).message || msg; } catch (e) {}
                throw new Error(msg);
            });
            return r.json();
        })
        .then(function (data) {
            window.location.href = '/FeasibilityPricing/View/' + data.id;
        })
        .catch(function (err) {
            setSaveLoading(false);
            showToast(err.message || 'Kayıt sırasında hata oluştu.', true);
        });
    }

    // ---- Kaydetme yöntemi modalı (yalnızca Edit modunda) ----
    var BASE_VERSION = (_preload && _preload.version) ? _preload.version : 1;

    function openSaveModal() {
        if (!validateForm()) return;
        // Create modunda modal yok; doğrudan kaydet.
        if (_mode !== 'edit') { submitForm(); return; }
        var upd = document.querySelector('input[name="saveMode"][value="update"]');
        if (upd) upd.checked = true;
        document.getElementById('save-overlay').classList.add('show');
    }
    function closeSaveModal() {
        var ov = document.getElementById('save-overlay');
        if (ov) ov.classList.remove('show');
    }

    function setConfirmLoading(on) {
        var btn = document.getElementById('confirmSaveBtn');
        if (!btn) return;
        if (on) {
            btn.dataset.html = btn.innerHTML;
            btn.disabled = true;
            btn.innerHTML = '<span class="material-symbols-outlined fiz-spin">progress_activity</span> Kaydediliyor…';
        } else {
            btn.disabled = false;
            if (btn.dataset.html) btn.innerHTML = btn.dataset.html;
        }
    }

    function confirmSave() {
        var modeEl = document.querySelector('input[name="saveMode"]:checked');
        var mode = modeEl ? modeEl.value : 'update';

        if (mode === 'new') {
            var payload = collectPayload();
            payload.saveAsNewVersion = true;
            payload.baseVersion      = BASE_VERSION;
            setConfirmLoading(true);
            fetch('/FeasibilityPricing/Save', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            })
            .then(function (r) {
                if (!r.ok) return r.text().then(function (t) {
                    var msg = 'Sunucu hatası (' + r.status + ')';
                    try { msg = JSON.parse(t).message || msg; } catch (e) {}
                    throw new Error(msg);
                });
                return r.json();
            })
            .then(function (data) {
                showToast('Yeni fizibilite versiyonu oluşturuldu.');
                setTimeout(function () { window.location.href = '/FeasibilityPricing/Edit/' + data.id; }, 1000);
            })
            .catch(function (err) {
                setConfirmLoading(false);
                showToast(err.message || 'Kayıt sırasında hata oluştu.', true);
            });
            return;
        }

        var updPayload = collectPayload();
        setConfirmLoading(true);
        fetch('/FeasibilityPricing/Update?id=' + encodeURIComponent(_studyId), {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(updPayload)
        })
        .then(function (r) {
            if (!r.ok) return r.text().then(function (t) {
                var msg = 'Sunucu hatası (' + r.status + ')';
                try { msg = JSON.parse(t).message || msg; } catch (e) {}
                throw new Error(msg);
            });
            return r.json();
        })
        .then(function (data) {
            window.location.href = '/FeasibilityPricing/View/' + data.id;
        })
        .catch(function (err) {
            setConfirmLoading(false);
            showToast(err.message || 'Güncelleme sırasında hata oluştu.', true);
        });
    }

    function resetAll() {
        document.getElementById('fname').value = '';
        document.getElementById('nameCt').textContent = '0';
        stations = [];
        CAPEX.forEach(function (f) { f.val = 0; f.ccy = 'USD'; });
        PRICING.vat = 20; PRICING.commission = 2.7;
        renderStations(); renderCapex(); renderPricing();
    }

    // ---- Edit preload ----
    function applyPreload() {
        if (!_preload) return;
        document.getElementById('fname').value = _preload.feasibilityName || '';
        document.getElementById('nameCt').textContent = (_preload.feasibilityName || '').length;

        if (_preload.usdRate) FX[0].sell = _preload.usdRate;
        if (_preload.eurRate) FX[1].sell = _preload.eurRate;
        if (_preload.inflTl  != null) INF[0].v = _preload.inflTl;
        if (_preload.inflEur != null) INF[1].v = _preload.inflEur;
        if (_preload.inflUsd != null) INF[2].v = _preload.inflUsd;

        if (_preload.vatRate        != null) PRICING.vat = _preload.vatRate;
        if (_preload.commissionRate != null) PRICING.commission = _preload.commissionRate;

        stations = (_preload.stations || []).map(function (st) {
            return {
                type: st.type || 'AC',
                soket: st.soket || 0,
                dailyKwh: st.dailyKwh || 0,
                economicLife: st.economicLife || 0,
                discountRate: st.discountRate || 0,
                targetProfit: st.targetProfit != null ? st.targetProfit : (st.type === 'DC' ? 16 : 10),
                ebmLow:  st.ebmLow  || 0,
                ebmHigh: st.ebmHigh || 0
            };
        });

        var first = (_preload.stations || [])[0];
        if (first) {
            CAPEX[0].val = first.hardware || 0;
            CAPEX[0].ccy = ccyRev[first.hardwareCcy] || 'TRY';
            CAPEX[1].val = first.infra || 0;
            CAPEX[1].ccy = ccyRev[first.infraCcy] || 'TRY';
            CAPEX[2].val = first.opex || 0;
            CAPEX[2].ccy = ccyRev[first.opexCcy] || 'USD';
        }
    }

    function toggleCard(id) { document.getElementById(id).classList.toggle('collapsed'); }

    // ---- Tab / sonuç önizleme ----
    function switchTab(t) {
        document.querySelectorAll('.fiz-tab').forEach(function (b) { b.classList.toggle('active', b.dataset.tab === t); });
        document.getElementById('pane-form').classList.toggle('active', t === 'form');
        document.getElementById('pane-results').classList.toggle('active', t === 'results');
    }

    function runInjectedScripts(container) {
        container.querySelectorAll('script').forEach(function (old) {
            var s = document.createElement('script');
            if (old.src) { s.src = old.src; }
            else { s.textContent = old.textContent; }
            old.parentNode.replaceChild(s, old);
        });
    }

    // Ortak hesaplama isteği. silent=true ise sekme değiştirmez ve hata toast'u göstermez
    // (Edit açılışında kayıtlı veriyle sonucu önceden hazırlamak için).
    function fetchResult(payload, silent) {
        var empty   = document.getElementById('resultsEmpty');
        var content = document.getElementById('resultsContent');
        if (!silent) content.innerHTML = '<div class="fiz-empty"><span class="material-symbols-outlined">hourglass_top</span><p>Hesaplanıyor…</p></div>';
        if (empty) empty.style.display = 'none';

        return fetch('/FeasibilityPricing/Calculate', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        })
        .then(function (r) {
            if (!r.ok) return r.text().then(function (t) {
                var msg = 'Sunucu hatası (' + r.status + ')';
                try { msg = JSON.parse(t).message || msg; } catch (e) {}
                throw new Error(msg);
            });
            return r.text();
        })
        .then(function (html) {
            content.innerHTML = html;
            runInjectedScripts(content);
        })
        .catch(function (err) {
            content.innerHTML = '';
            if (empty) empty.style.display = '';
            if (!silent) showToast(err.message || 'Hesaplama sırasında hata oluştu.', true);
        });
    }

    function calculate() {
        if (!validateForm()) { switchTab('form'); return; }
        switchTab('results');
        fetchResult(collectPayload(), false);
    }

    // Edit açılışında: doğrulama/sekme değişimi yapmadan sonucu sessizce hazırla.
    function calculatePreview() {
        fetchResult(collectPayload(), true);
    }

    // ---- Açılış ----
    function init() {
        applyPreload();
        renderFx();
        renderInf();
        renderPricing();
        renderCapex();
        renderStations();

        // Düzenleme modunda kayıtlı veriyle sonuç sekmesini hazır göster (sekme değiştirmeden).
        if (_mode === 'edit' && _preload && (_preload.stations || []).length > 0) {
            calculatePreview();
        }
    }

    // dış dünyaya açılan API
    window.PricingForm = {
        setInf: setInf, setSt: setSt, setNum: setNum, delStation: delStation,
        setCapexCcy: setCapexCcy, setPricing: setPricing
    };
    window.refreshFx = refreshFx;
    window.addStation = addStation;
    window.submitForm = submitForm;
    window.openSaveModal = openSaveModal;
    window.closeSaveModal = closeSaveModal;
    window.confirmSave = confirmSave;
    window.resetAll = resetAll;
    window.toggleCard = toggleCard;
    window.switchTab = switchTab;
    window.calculate = calculate;

    document.addEventListener('DOMContentLoaded', function () {
        init();
        var ov = document.getElementById('save-overlay');
        if (ov) ov.addEventListener('click', function (e) {
            if (e.target.id === 'save-overlay') closeSaveModal();
        });
    });
})();
