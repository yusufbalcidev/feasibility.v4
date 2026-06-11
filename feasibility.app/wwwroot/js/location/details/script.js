document.addEventListener('DOMContentLoaded', function () {
    var mapEl = document.getElementById('locationMap');
    if (!mapEl) return;

    var lat = parseFloat(mapEl.dataset.lat);
    var lng = parseFloat(mapEl.dataset.lng);
    if (isNaN(lat) || isNaN(lng)) return;

    var map = L.map(mapEl, {
        center: [lat, lng],
        zoom: 17,
        scrollWheelZoom: false
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; OpenStreetMap'
    }).addTo(map);

    L.marker([lat, lng]).addTo(map);

    map.on('focus', function () { map.scrollWheelZoom.enable(); });
    map.on('blur', function () { map.scrollWheelZoom.disable(); });
});
