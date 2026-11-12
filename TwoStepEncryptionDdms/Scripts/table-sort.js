(function (global) {
    var collator = new Intl.Collator('tr', { numeric: true, sensitivity: 'base' });

    // Header'a tıklanınca table'ı ascending/descending sıralar. Sort key'ler bir kez hesaplanır, row'lar tek seferde yeniden eklenir.
    global.makeSortable = function (selector, onSorted) {
        var table = document.querySelector(selector);
        if (!table || !table.tHead || !table.tBodies.length) {
            return;
        }

        var tbody = table.tBodies[0];
        var headers = table.tHead.rows[0].cells;

        Array.prototype.forEach.call(headers, function (th, index) {
            if (th.classList.contains('col-actions')) {
                return;
            }

            th.classList.add('sortable');
            th.setAttribute('aria-sort', 'none');
            th.tabIndex = 0;
            th.addEventListener('click', function () { sortBy(index, th); });
            th.addEventListener('keydown', function (e) {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    sortBy(index, th);
                }
            });
        });

        function sortBy(index, th) {
            var rows = Array.prototype.slice.call(tbody.rows);
            if (rows.length < 2 || rows[0].classList.contains('empty-row')) {
                return;
            }

            var ascending = th.getAttribute('aria-sort') !== 'ascending';
            Array.prototype.forEach.call(headers, function (h) {
                if (h.classList.contains('sortable')) {
                    h.setAttribute('aria-sort', 'none');
                }
            });

            var keys = rows.map(function (row) {
                return row.cells[index] ? row.cells[index].textContent.trim() : '';
            });
            var order = rows.map(function (row, i) { return i; }).sort(function (a, b) {
                var result = collator.compare(keys[a], keys[b]);
                return (ascending ? result : -result) || a - b;
            });

            var fragment = document.createDocumentFragment();
            order.forEach(function (i) { fragment.appendChild(rows[i]); });
            tbody.appendChild(fragment);

            th.setAttribute('aria-sort', ascending ? 'ascending' : 'descending');
            if (onSorted) {
                onSorted();
            }
        }
    };
})(window);
