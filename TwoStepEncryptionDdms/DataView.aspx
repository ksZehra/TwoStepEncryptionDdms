<%@ Page Title="Kayıtlar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataView.aspx.cs" Inherits="TwoStepEncryptionDdms.DataView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <div class="page-heading">
        <div>
            <h1 class="page-title" runat="server" id="pageTitle">Kayıtlar</h1>
            <p class="page-subtitle">Veri kümesindeki kayıtları görüntüleyin, ekleyin ve güncelleyin.</p>
        </div>
        <div class="page-actions">
            <a class="btn btn-default" href="Record.aspx">Veri Kümesi Listesine Dön</a>
            <button type="button" class="btn btn-primary" id="btnNewRecord" runat="server" clientidmode="Static" visible="false">Yeni Kayıt</button>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">
    <div class="key-card" id="divKey" runat="server">
        <div class="key-card-icon" aria-hidden="true">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
                <rect x="4" y="10" width="16" height="10" rx="2"></rect>
                <path d="M8 10V7a4 4 0 0 1 8 0v3"></path>
            </svg>
        </div>
        <div class="key-card-body">
            <h2 class="key-card-title">Bu Veri Kümesi Şifreli</h2>
            <p class="key-card-text">Kayıtlarla çalışmak için 32 karakterlik anahtarı girin. Güvenliğiniz için anahtar sayfa her açıldığında yeniden istenir.</p>
            <div class="key-form">
                <asp:TextBox class="form-control" ID="tbKey" TextMode="Password" MaxLength="32" placeholder="32 karakterlik anahtar" runat="server" ClientIDMode="Static" autocomplete="new-password" data-reveal="true"></asp:TextBox>
                <asp:Button ID="btnKey" CssClass="btn btn-primary" runat="server" Text="Kontrol Et" OnClick="btnKey_Click" ClientIDMode="Static" />
            </div>
            <div class="key-status">
                <span class="badge badge-success">Anahtar doğrulandı</span>
                <span>Bu sayfada kayıtları görüntüleyebilir, ekleyebilir ve güncelleyebilirsiniz.</span>
            </div>
        </div>
    </div>

    <asp:HiddenField runat="server" ID="hfFormMode" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="hfRecordId" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="hfSaveData" ClientIDMode="Static" />

    <div class="card record-form" id="divForm" runat="server" clientidmode="Static" visible="false">
        <div class="card-header">
            <h2 class="card-title" id="formTitle">Yeni kayıt</h2>
            <span class="card-note">Şifreli alanlar mavi olarak işaretlenir</span>
        </div>
        <div class="card-body">
            <asp:Literal runat="server" ID="ltFields"></asp:Literal>

            <div class="form-actions">
                <button type="button" class="btn btn-default" id="btnFormCancel">Vazgeç</button>
                <asp:Button ID="btnSave" CssClass="btn btn-primary" runat="server" Text="Ekle" OnClick="btnSave_Click" ClientIDMode="Static" />
            </div>
        </div>
    </div>

    <div class="card" id="divContainer" runat="server" visible="false">
        <div class="card-header">
            <h2 class="card-title" runat="server" id="tabloBaslik"></h2>
            <span class="card-note">Şifreli sütunlar mavi olarak işaretlenir</span>
        </div>
        <div class="card-body">
            <div class="search-panel">
                <div class="form-grid">
                    <div class="form-field">
                        <asp:Label runat="server" CssClass="field-label" AssociatedControlID="tbSutunAdi">Sütun Adı</asp:Label>
                        <asp:TextBox class="form-control" ID="tbSutunAdi" MaxLength="120" placeholder="Sütun adını giriniz..." runat="server" ClientIDMode="Static"></asp:TextBox>
                    </div>
                    <div class="form-field">
                        <asp:Label runat="server" CssClass="field-label" AssociatedControlID="tbKelime">Aranacak Kelime</asp:Label>
                        <asp:TextBox class="form-control" ID="tbKelime" MaxLength="120" placeholder="Aramak istediğiniz kelimeyi giriniz..." runat="server" ClientIDMode="Static"></asp:TextBox>
                    </div>
                </div>
                <div class="search-actions">
                    <asp:Button ID="btnKelimeAra" CssClass="btn btn-default btn-sm" runat="server" Text="Ara" ClientIDMode="Static" OnClick="btnKelimeAra_Click" />
                    <asp:Button ID="btnTemizle" runat="server" Text="Temizle" CssClass="btn btn-default btn-sm" OnClick="btnTemizle_Click" />
                </div>
            </div>

            <div class="table-toolbar">
                <label for="maxRows" class="table-toolbar-label">Kayıt sayısı</label>
                <select class="form-control input-sm" id="maxRows">
                    <option value="5000">Tüm Kayıtları Gör</option>
                    <option value="5">5</option>
                    <option value="10">10</option>
                    <option value="20">20</option>
                    <option value="50">50</option>
                    <option value="100">100</option>
                </select>
            </div>

            <div class="table-responsive">
                <asp:Literal ID="ltList" runat="server"></asp:Literal>
            </div>

            <div class="pagination-container">
                <nav>
                    <ul class="pagination">
                        <li data-page="prev">
                            <span aria-hidden="true">‹</span>
                            <span class="sr-only">Önceki sayfa</span>
                        </li>
                        <li data-page="next" id="prev">
                            <span aria-hidden="true">›</span>
                            <span class="sr-only">Sonraki sayfa</span>
                        </li>
                    </ul>
                </nav>
            </div>
        </div>
    </div>

    <script src="Scripts/table-sort.js"></script>
    <script type="text/javascript">
$(function () {
    $('#btnKey').click(function () {
        if (document.getElementById('tbKey').value == '') {
            iqwerty.toast.toast('Anahtar bilgisi boş bırakılamaz.', { type: 'warning' });
            return false;
        }
    });

    var form = $('#divForm');
    var inputs = form.find('.record-input');

    function openForm(mode, row) {
        $('#hfFormMode').val(mode);
        $('#hfRecordId').val(row ? row.data('id') : '');
        $('#formTitle').text(mode === 'edit' ? 'Kayıt #' + row.data('id') + ' güncelle' : 'Yeni kayıt');
        $('#btnSave').val(mode === 'edit' ? 'Güncelle' : 'Ekle');
        $('.data-table tr.is-editing').removeClass('is-editing');

        inputs.each(function () {
            var cell = row ? row.find('td[data-col="' + $(this).data('col') + '"]') : null;
            $(this).val(cell && cell.length ? cell.text() : '');
        });

        if (row) {
            row.addClass('is-editing');
        }
        form.show();
        inputs.first().focus();
        form[0].scrollIntoView({ block: 'nearest', behavior: 'smooth' });
    }

    function closeForm() {
        $('#hfFormMode').val('');
        $('#hfRecordId').val('');
        $('.data-table tr.is-editing').removeClass('is-editing');
        form.hide();
    }

    $('#btnNewRecord').click(function () { openForm('new'); });
    $('#btnFormCancel').click(closeForm);
    $(document).on('click', '.js-edit', function () { openForm('edit', $(this).closest('tr')); });

    $('#btnSave').click(function () {
        var saveVal = '';
        inputs.each(function () {
            saveVal += encodeURIComponent($(this).data('col')) + '#' + encodeURIComponent($(this).val()) + '|';
        });
        $('#hfSaveData').val(saveVal);
    });

    var encoder = new TextEncoder();
    inputs.filter('[data-maxbytes]').on('input', function () {
        var max = parseInt($(this).attr('data-maxbytes'), 10);
        var chars = Array.from(this.value);
        while (chars.length && encoder.encode(chars.join('')).length > max) {
            chars.pop();
        }
        var trimmed = chars.join('');
        if (trimmed !== this.value) {
            this.value = trimmed;
        }
    });

    // Postback sonrası form durumu korunur; hata alınırsa kullanıcının girdiği değerler kaybolmaz.
    var mode = $('#hfFormMode').val();
    if (mode) {
        var row = mode === 'edit' ? $('.data-table tr[data-id="' + $('#hfRecordId').val() + '"]') : null;
        openForm(mode, row && row.length ? row : null);
        $.each(($('#hfSaveData').val() || '').split('|'), function (i, pair) {
            var parts = pair.split('#');
            if (parts.length === 2) {
                inputs.filter(function () { return $(this).data('col') === decodeURIComponent(parts[0]); }).val(decodeURIComponent(parts[1]));
            }
        });
    } else {
        form.hide();
    }
    $('#hfSaveData').val('');
});

getPagination('#table-id');
makeSortable('#table-id', function () { $('#maxRows').change(); });

function getPagination(table) {
    var lastPage = 1;
    $(table).addClass('is-paginated');

    $('#maxRows').on('change', function () {
        lastPage = 1;
        $('.pagination').find('li').slice(1, -1).remove();

        var maxRows = parseInt($(this).val());
        var totalRows = $(table + ' tbody tr').length;
        $('.pagination-container').toggle(!(maxRows == 5000 || totalRows === 0 || totalRows <= maxRows));

        var trnum = 0;
        $(table + ' tr:gt(0)').each(function () {
            trnum++;
            $(this).toggle(trnum <= maxRows);
        });

        if (totalRows > maxRows) {
            var pagenum = Math.ceil(totalRows / maxRows);
            for (var i = 1; i <= pagenum; i++) {
                $('.pagination #prev').before('<li data-page="' + i + '"><span>' + i + '<span class="sr-only">(current)</span></span></li>').show();
            }
        }
        $('.pagination [data-page="1"]').addClass('active');

        $('.pagination li').on('click', function (evt) {
            evt.stopImmediatePropagation();
            evt.preventDefault();
            var pageNum = $(this).attr('data-page');
            var maxRows = parseInt($('#maxRows').val());

            if (pageNum == 'prev') {
                if (lastPage == 1) return;
                pageNum = --lastPage;
            }
            if (pageNum == 'next') {
                if (lastPage == $('.pagination li').length - 2) return;
                pageNum = ++lastPage;
            }

            lastPage = pageNum;
            $('.pagination li').removeClass('active');
            $('.pagination [data-page="' + lastPage + '"]').addClass('active');
            limitPagging();

            var trIndex = 0;
            $(table + ' tr:gt(0)').each(function () {
                trIndex++;
                $(this).toggle(!(trIndex > maxRows * pageNum || trIndex <= maxRows * pageNum - maxRows));
            });
        });
        limitPagging();
    }).val(10).change();
}

function limitPagging() {
    if ($('.pagination li').length <= 7) return;

    var active = parseInt($('.pagination li.active').attr('data-page'));
    if (active <= 3) {
        $('.pagination li:gt(10)').hide();
        $('.pagination li:lt(10)').show();
    } else {
        $('.pagination li:gt(0)').hide();
        for (var i = active - 2; i <= active + 2; i++) {
            $('.pagination [data-page="' + i + '"]').show();
        }
    }
    $('.pagination [data-page="next"]').show();
}
</script>
</asp:Content>
