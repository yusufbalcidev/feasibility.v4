document.addEventListener('DOMContentLoaded', function () {
    var toggleBtn = document.getElementById('sidebarToggle');
    var closeBtn = document.getElementById('closeSidebarBtn');
    var overlay = document.getElementById('mobile-overlay');
    var body = document.body;

    function toggleSidebar() {
        if (window.innerWidth <= 768) {
            body.classList.toggle('sidebar-open-mobile');
            overlay.classList.toggle('hidden');
        } else {
            body.classList.toggle('sidebar-collapsed');
        }
    }

    if (toggleBtn) toggleBtn.addEventListener('click', toggleSidebar);
    if (closeBtn) closeBtn.addEventListener('click', toggleSidebar);
    if (overlay) overlay.addEventListener('click', toggleSidebar);

    window.addEventListener('resize', function () {
        if (window.innerWidth > 768) {
            body.classList.remove('sidebar-open-mobile');
            if (overlay) overlay.classList.add('hidden');
        }
    });

    // Confirm-on-delete handler (used by tables)
    document.querySelectorAll('form[data-confirm]').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            var message = form.getAttribute('data-confirm') || 'Bu işlemi gerçekleştirmek istiyor musunuz?';
            alertify.confirm('Onay', message,
                function () { form.removeAttribute('data-confirm'); form.submit(); },
                function () { });
        });
    });
});
(function () {
    var sidebarBtn = document.getElementById('sidebarThemeToggle');

    function apply(theme) {
        var isDark = theme === 'dark';
        if (isDark) {
            document.documentElement.setAttribute('data-theme', 'dark');
        } else {
            document.documentElement.removeAttribute('data-theme');
        }
        if (sidebarBtn) {
            var sIcon = sidebarBtn.querySelector('.material-symbols-outlined');
            var sText = sidebarBtn.querySelector('.menu-text');
            if (sIcon) sIcon.textContent = isDark ? 'light_mode' : 'dark_mode';
            if (sText) sText.textContent = isDark ? 'Açık Tema' : 'Koyu Tema';
        }
    }

    apply(localStorage.getItem('theme') === 'dark' ? 'dark' : 'light');

    function handleClick() {
        var next = document.documentElement.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
        localStorage.setItem('theme', next);
        apply(next);
    }

    if (sidebarBtn) sidebarBtn.addEventListener('click', handleClick);
})();