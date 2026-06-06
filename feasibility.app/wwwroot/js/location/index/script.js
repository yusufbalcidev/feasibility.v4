document.addEventListener('DOMContentLoaded', function () {
    var tabBtns = document.querySelectorAll('.tab-btn');
    var panes = document.querySelectorAll('.tab-pane');
    var map = null;

    function activate(tabName) {
        tabBtns.forEach(function (b) { b.classList.toggle('active', b.dataset.tab === tabName); });
        panes.forEach(function (p) { p.classList.toggle('active', p.id === 'tab-' + tabName); });
        if (tabName === 'map') initMap();
    }

    tabBtns.forEach(function (b) {
        b.addEventListener('click', function () { activate(b.dataset.tab); });
    });

    function initMap() {
        if (map) {
            setTimeout(function () { map.invalidateSize(); }, 100);
            return;
        }

        var dataEl = document.getElementById('locations-data');
        var locations = [];
        try { locations = JSON.parse(dataEl.textContent || '[]'); } catch (e) { locations = []; }

        map = L.map('locationsMap', {
            center: [39.0, 35.0],
            zoom: 6,
            scrollWheelZoom: true
        });

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; OpenStreetMap'
        }).addTo(map);

        if (!locations.length) return;

        var bounds = [];
        locations.forEach(function (loc) {
            var marker = L.marker([loc.lat, loc.lng]).addTo(map);
            marker.bindPopup(
                '<strong>' + escapeHtml(loc.name) + '</strong><br/>' +
                escapeHtml(loc.city) + ' / ' + escapeHtml(loc.district) +
                (loc.type ? '<br/><em>' + escapeHtml(loc.type) + '</em>' : '') +
                '<br/><a href="/Location/Details/' + loc.id + '">Detayı görüntüle →</a>'
            );
            bounds.push([loc.lat, loc.lng]);
        });

        if (bounds.length === 1) {
            map.setView(bounds[0], 13);
        } else {
            map.fitBounds(bounds, { padding: [40, 40] });
        }
    }

    function escapeHtml(value) {
        if (value == null) return '';
        return String(value)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }
});
