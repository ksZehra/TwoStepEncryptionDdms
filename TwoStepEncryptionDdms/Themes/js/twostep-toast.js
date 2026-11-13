
'use strict';

function mergeOptions(initialOptions, customOptions) {
    const merged = customOptions || {};
    for (const prop in initialOptions) {
        if (merged.hasOwnProperty(prop)) {
            if (initialOptions[prop] !== null && initialOptions[prop].constructor === Object) {
                merged[prop] = mergeOptions(initialOptions[prop], merged[prop]);
            }
        } else {
            merged[prop] = initialOptions[prop];
        }
    }
    return merged;
}

function stylize(element, styles) {
    Object.keys(styles).forEach((style) => {
        element.style[style] = styles[style];
    });
}

const toast = (() => {
    const TOAST_ANIMATION_SPEED = 300;
    const TOAST_GAP = 10;

    const TYPE_STYLES = {
        success: { bg: '#166534', icon: '#ffffff' },
        error: { bg: '#b91c1c', icon: '#ffffff' },
        warning: { bg: '#b45309', icon: '#ffffff' },
        info: { bg: '#1d4ed8', icon: '#ffffff' },
    };

    const DEFAULT_SETTINGS = {
        type: 'info',
        duration: 4000,
        closeOnClick: true,
    };

    const ICONS = {
        success: 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="black" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpolyline points="20 6 9 17 4 12"%3E%3C/polyline%3E%3C/svg%3E',
        error: 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="black" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"%3E%3Ccircle cx="12" cy="12" r="10"%3E%3C/circle%3E%3Cline x1="15" y1="9" x2="9" y2="15"%3E%3C/line%3E%3Cline x1="9" y1="9" x2="15" y2="15"%3E%3C/line%3E%3C/svg%3E',
        warning: 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="black" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"%3E%3C/path%3E%3Cline x1="12" y1="9" x2="12" y2="13"%3E%3C/line%3E%3Cline x1="12" y1="17" x2="12.01" y2="17"%3E%3C/line%3E%3C/svg%3E',
        info: 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="black" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"%3E%3Ccircle cx="12" cy="12" r="10"%3E%3C/circle%3E%3Cline x1="12" y1="16" x2="12" y2="12"%3E%3C/line%3E%3Cline x1="12" y1="8" x2="12.01" y2="8"%3E%3C/line%3E%3C/svg%3E',
    };

    const CONTAINER_ID = 'twostep-toast-container';

    function getContainer() {
        let container = document.getElementById(CONTAINER_ID);
        if (!container) {
            container = document.createElement('div');
            container.id = CONTAINER_ID;
            Object.assign(container.style, {
                position: 'fixed',
                bottom: '20px',
                right: '20px',
                zIndex: '100000',
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'flex-end',
                gap: TOAST_GAP + 'px',
                pointerEvents: 'none',
            });
            document.body.appendChild(container);
        }
        return container;
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text === null || text === undefined ? '' : String(text);
        return div.innerHTML;
    }

    function showToast(text, options) {
        const _options = mergeOptions(DEFAULT_SETTINGS, options || {});
        const typeStyle = TYPE_STYLES[_options.type] || TYPE_STYLES.info;

        const container = getContainer();

        const toastEl = document.createElement('div');
        Object.assign(toastEl.style, {
            background: typeStyle.bg,
            color: '#ffffff',
            padding: '10px 14px',
            borderRadius: '8px',
            boxShadow: '0 10px 25px -5px rgba(0, 0, 0, 0.15), 0 8px 10px -6px rgba(0, 0, 0, 0.1)',
            border: '1px solid rgba(255, 255, 255, 0.25)',
            minWidth: '260px',
            maxWidth: '440px',
            fontFamily: '"Segoe UI", system-ui, -apple-system, "Segoe UI", Roboto, Helvetica, Arial, sans-serif',
            fontSize: '13px',
            lineHeight: '1.5',
            display: 'flex',
            alignItems: 'center',
            gap: '10px',
            position: 'relative',
            overflow: 'hidden',
            opacity: '0',
            transform: 'translateX(40px) translateY(10px)',
            pointerEvents: 'auto',
            cursor: 'pointer',
        });

        // Icon
        const iconEl = document.createElement('div');
        iconEl.style.flex = '0 0 auto';
        iconEl.style.width = '18px';
        iconEl.style.height = '18px';
        iconEl.style.maskImage = "url('" + ICONS[_options.type] + "')";
        iconEl.style.maskPosition = 'center';
        iconEl.style.maskSize = 'contain';
        iconEl.style.maskRepeat = 'no-repeat';
        iconEl.style.backgroundColor = typeStyle.icon;
        iconEl.style.WebkitMaskImage = iconEl.style.maskImage;
        iconEl.style.WebkitMaskPosition = iconEl.style.maskPosition;
        iconEl.style.WebkitMaskSize = iconEl.style.maskSize;
        iconEl.style.WebkitMaskRepeat = iconEl.style.maskRepeat;
        toastEl.appendChild(iconEl);

        // Message
        const msgEl = document.createElement('div');
        msgEl.style.flex = '1 1 auto';
        msgEl.style.minWidth = '0';
        msgEl.innerHTML = escapeHtml(text);
        toastEl.appendChild(msgEl);

        // Close button
        const closeEl = document.createElement('button');
        closeEl.type = 'button';
        closeEl.innerHTML = '&times;';
        Object.assign(closeEl.style, {
            background: 'none',
            border: 'none',
            color: 'rgba(255, 255, 255, 0.7)',
            fontSize: '20px',
            lineHeight: '1',
            padding: '0',
            marginLeft: '8px',
            cursor: 'pointer',
            flex: '0 0 auto',
        });
        closeEl.onclick = function (e) {
            e.stopPropagation();
            hideToast(toastEl);
        };
        toastEl.appendChild(closeEl);

        // Progress bar
        const progressEl = document.createElement('div');
        Object.assign(progressEl.style, {
            position: 'absolute',
            left: '0',
            bottom: '0',
            height: '3px',
            background: 'rgba(255, 255, 255, 0.45)',
            width: '100%',
            transformOrigin: 'left',
            transform: 'scaleX(1)',
            transition: 'transform ' + (_options.duration / 1000) + 'ms linear',
        });
        toastEl.appendChild(progressEl);

        container.appendChild(toastEl);

        // Animate in
        setTimeout(function () {
            toastEl.style.transition = 'all ' + TOAST_ANIMATION_SPEED + 'ms cubic-bezier(0.16, 1, 0.3, 1)';
            toastEl.style.opacity = '1';
            toastEl.style.transform = 'translateX(0) translateY(0)';
            progressEl.style.transform = 'scaleX(0)';
        }, 10);

        // Auto hide
        if (_options.duration !== 0) {
            setTimeout(function () { hideToast(toastEl); }, _options.duration);
        }

        // Click to dismiss
        if (_options.closeOnClick) {
            toastEl.onclick = function () { hideToast(toastEl); };
        }

        return {
            hide: function () { hideToast(toastEl); },
        };
    }

    function hideToast(toastEl) {
        if (!toastEl || !toastEl.parentNode) {
            return;
        }
        toastEl.style.transition = 'all ' + TOAST_ANIMATION_SPEED + 'ms ease';
        toastEl.style.opacity = '0';
        toastEl.style.transform = 'translateX(40px) translateY(10px)';
        setTimeout(function () {
            if (toastEl.parentNode) {
                toastEl.parentNode.removeChild(toastEl);
            }
        }, TOAST_ANIMATION_SPEED);
    }

    return {
        toast: function (text, options) {
            return showToast(text, options);
        },
    };
})();

window.iqwerty = window.iqwerty || {};
window.iqwerty.toast = toast;
