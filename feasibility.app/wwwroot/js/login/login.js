document.addEventListener('DOMContentLoaded', function () {
    var timerEl = document.getElementById('codeTimer');
    var resendBtn = document.getElementById('resendBtn');
    if (!timerEl) return;

    var seconds = parseInt(timerEl.dataset.seconds || '60', 10);

    function tick() {
        if (seconds <= 0) {
            timerEl.textContent = 'Süre doldu';
            if (resendBtn) resendBtn.disabled = false;
            return;
        }
        var min = Math.floor(seconds / 60);
        var sec = seconds % 60;
        timerEl.textContent = min + ':' + (sec < 10 ? '0' + sec : sec);
        seconds -= 1;
        window.setTimeout(tick, 1000);
    }
    if (resendBtn) resendBtn.disabled = true;
    tick();
});
