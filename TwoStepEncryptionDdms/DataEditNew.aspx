<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataEditNew.aspx.cs" Inherits="TwoStepEncryptionDdms.DataEditNew" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <div class="page-heading">
        <div>
            <h1 class="page-title"><span runat="server" id="spnDataset"></span>Kayıt Formu</h1>
            <p class="page-subtitle">Veri kümesine yeni kayıt ekleyin veya mevcut kaydı güncelleyin.</p>
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
            <h2 class="key-card-title">Bu veri kümesi şifreli</h2>
            <p class="key-card-text">Kayıt eklemek için 32 karakterlik anahtarı girin.</p>
            <div class="key-form">
                <asp:TextBox class="form-control" ID="tbKey" MaxLength="32" placeholder="32 karakterlik anahtar" runat="server" ClientIDMode="Static" autocomplete="off"></asp:TextBox>
                <asp:Button ID="btnKey" CssClass="btn btn-primary" runat="server" Text="Kontrol Et" OnClick="btnKey_Click" ClientIDMode="Static" />
            </div>
            <div class="key-status">
                <span class="badge badge-success">Anahtar doğrulandı</span>
                <span>Şifreli alanlar bu oturumda çözülebilir.</span>
            </div>
        </div>
    </div>

    <div class="card" runat="server" id="divTablo" visible="false">
        <div class="card-header">
            <h2 class="card-title" runat="server" id="tabloBaslik"></h2>
            <span class="card-note">Şifreli alanlar mavi olarak işaretlenir</span>
        </div>
        <div class="card-body">
            <div class="notice notice-info" runat="server" id="divAlert">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                    <circle cx="12" cy="12" r="9"></circle>
                    <path d="M12 11v5"></path>
                    <path d="M12 8h.01"></path>
                </svg>
                <div>Mavi renkli alanlara eklenen veriler şifrelenerek kaydedilecektir.</div>
            </div>

            <asp:HiddenField runat="server" ID="hfTable" ClientIDMode="Static"></asp:HiddenField>
            <asp:Literal runat="server" ID="ltList"></asp:Literal>

            <div class="form-actions">
                <asp:Button ID="btnKayitGir" CssClass="btn btn-primary" runat="server" Text="Ekle" OnClick="btnKayitGir_Click" ClientIDMode="Static" />
                <asp:HiddenField runat="server" ID="hfSaveData" ClientIDMode="Static" />
            </div>
        </div>
    </div>

    <script type="text/javascript">

        $(function () {

            $('#btnKey').click(function () {
                if (document.getElementById("tbKey").value == "") {
                    alert("Görüntülemek istediğiniz tablonun içerisinde şifrelenmesini istediğiniz alanlar olduğu için 'anahtar' bilgisi boş bırakılamaz!");
                    return false
                }
            });

            $('#btnKayitGir').click(function () {
                $('#hfSaveData').val('');
                var saveVal = '';
                $('.cst-lbl').each(function () {
                    saveVal = saveVal + $(this).attr('id') + '#' + $(this).val() + '|';
                });
                $('#hfSaveData').val(saveVal);
            });
        });
    </script>

</asp:Content>
