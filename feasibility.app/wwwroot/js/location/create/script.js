document.addEventListener('DOMContentLoaded', function () {
    var citySelect = document.getElementById('citySelect');
    var districtSelect = document.getElementById('districtSelect');
    var latInput = document.getElementById('latitudeInput');
    var lngInput = document.getElementById('longitudeInput');
    var mapEl = document.getElementById('locationMap');

    if (!citySelect || !districtSelect) return;

    var savedCity = citySelect.dataset.selected || citySelect.value || '';
    var savedDistrict = districtSelect.dataset.selected || districtSelect.value || '';

    var defaultCenter = [39.0, 35.0];
    var defaultZoom = 6;
    var savedLat = parseFloat(latInput.value);
    var savedLng = parseFloat(lngInput.value);
    var hasInitialPoint = !isNaN(savedLat) && !isNaN(savedLng) && (savedLat !== 0 || savedLng !== 0);

    var map = L.map(mapEl, {
        center: hasInitialPoint ? [savedLat, savedLng] : defaultCenter,
        zoom: hasInitialPoint ? 14 : defaultZoom,
        scrollWheelZoom: true
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; OpenStreetMap'
    }).addTo(map);

    var marker = null;
    function placeMarker(lat, lng, zoom) {
        if (marker) {
            marker.setLatLng([lat, lng]);
        } else {
            marker = L.marker([lat, lng], { draggable: true }).addTo(map);
            marker.on('dragend', function () {
                var ll = marker.getLatLng();
                updateInputs(ll.lat, ll.lng);
            });
        }
        if (typeof zoom === 'number') {
            map.setView([lat, lng], zoom);
        } else {
            map.panTo([lat, lng]);
        }
    }

    function updateInputs(lat, lng) {
        latInput.value = lat.toFixed(6);
        lngInput.value = lng.toFixed(6);
    }

    if (hasInitialPoint) {
        placeMarker(savedLat, savedLng, 14);
    }

    map.on('click', function (e) {
        placeMarker(e.latlng.lat, e.latlng.lng);
        updateInputs(e.latlng.lat, e.latlng.lng);
    });

    function syncFromInputs() {
        var lat = parseFloat(latInput.value);
        var lng = parseFloat(lngInput.value);
        if (!isNaN(lat) && !isNaN(lng)) placeMarker(lat, lng);
    }
    latInput.addEventListener('change', syncFromInputs);
    lngInput.addEventListener('change', syncFromInputs);

    var geocodeAbort = null;
    function geocode(query) {
        if (geocodeAbort) geocodeAbort.abort();
        geocodeAbort = new AbortController();
        var url = 'https://nominatim.openstreetmap.org/search?format=json&limit=1&countrycodes=tr&q=' + encodeURIComponent(query);
        return fetch(url, { signal: geocodeAbort.signal, headers: { 'Accept-Language': 'tr' } })
            .then(function (r) { return r.json(); })
            .then(function (results) { return results && results[0]; })
            .catch(function () { return null; });
    }

    function updateMapFromCityDistrict(force) {
        var city = citySelect.value;
        var district = districtSelect.value;
        if (!city) return;
        var query = district ? (district + ', ' + city + ', Türkiye') : (city + ', Türkiye');
        geocode(query).then(function (result) {
            if (!result) return;
            var lat = parseFloat(result.lat);
            var lng = parseFloat(result.lon);
            if (isNaN(lat) || isNaN(lng)) return;
            if (force || !marker) {
                placeMarker(lat, lng, district ? 13 : 10);
                updateInputs(lat, lng);
            } else {
                map.setView([lat, lng], district ? 13 : 10);
            }
        });
    }

    fetch('/data/il-ilce.json')
        .then(function (r) { return r.json(); })
        .then(function (data) {
            Object.keys(data).sort().forEach(function (city) {
                var opt = new Option(city, city, city === savedCity, city === savedCity);
                citySelect.add(opt);
            });

            function populateDistricts(city, selectedDistrict) {
                districtSelect.innerHTML = '<option value="">-- İlçe Seç --</option>';
                if (city && data[city]) {
                    data[city].forEach(function (d) {
                        var opt = new Option(d, d, d === selectedDistrict, d === selectedDistrict);
                        districtSelect.add(opt);
                    });
                }
            }

            if (savedCity) populateDistricts(savedCity, savedDistrict);
            if (!hasInitialPoint && savedCity) updateMapFromCityDistrict(true);

            citySelect.addEventListener('change', function () {
                populateDistricts(citySelect.value, '');
                updateMapFromCityDistrict(true);
            });
            districtSelect.addEventListener('change', function () {
                updateMapFromCityDistrict(true);
            });
        });
});
