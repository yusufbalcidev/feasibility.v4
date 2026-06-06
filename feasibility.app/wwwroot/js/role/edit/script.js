document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.perm-toggle-all').forEach(function (toggle) {
        toggle.addEventListener('change', function () {
            var targetClass = toggle.dataset.target;
            document.querySelectorAll('.permission-check.' + targetClass).forEach(function (cb) {
                cb.checked = toggle.checked;
            });
        });
    });
});
