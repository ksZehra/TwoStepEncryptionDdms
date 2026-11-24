(function () {
    var EYE = '<svg class="icon-show" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12Z"></path><circle cx="12" cy="12" r="3"></circle></svg>';
    var EYE_OFF = '<svg class="icon-hide" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M10.6 5.1A10.4 10.4 0 0 1 12 5c6.5 0 10 7 10 7a17.6 17.6 0 0 1-2.6 3.6"></path><path d="M6.6 6.6A17.4 17.4 0 0 0 2 12s3.5 7 10 7a9.7 9.7 0 0 0 5.4-1.6"></path><path d="M9.9 9.9a3 3 0 0 0 4.2 4.2"></path><path d="m2 2 20 20"></path></svg>';

    function setVisible(input, button, visible) {
        input.type = visible ? 'text' : 'password';
        button.classList.toggle('is-visible', visible);
        var label = visible ? 'Gizle' : 'Göster';
        button.setAttribute('aria-label', label);
        button.title = label;
        button.setAttribute('aria-pressed', visible ? 'true' : 'false');
    }

    // data-reveal işaretli her input'a göz ikonu eklenir; input başlangıçta maskelenir.
    function init() {
        var inputs = document.querySelectorAll('input[data-reveal]');
        Array.prototype.forEach.call(inputs, function (input) {
            if (input.parentNode.classList.contains('password-field')) {
                return;
            }

            var wrapper = document.createElement('div');
            wrapper.className = 'password-field';
            input.parentNode.insertBefore(wrapper, input);
            wrapper.appendChild(input);

            var button = document.createElement('button');
            button.type = 'button';
            button.className = 'password-toggle';
            button.setAttribute('aria-controls', input.id);
            button.innerHTML = EYE + EYE_OFF;
            wrapper.appendChild(button);

            setVisible(input, button, false);
            button.addEventListener('click', function () {
                setVisible(input, button, input.type === 'password');
                input.focus();
            });
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
