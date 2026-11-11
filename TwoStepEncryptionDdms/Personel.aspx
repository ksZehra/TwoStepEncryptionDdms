<%@ Page Title="Hesabım" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Personel.aspx.cs" Inherits="TwoStepEncryptionDdms.Personel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <div class="page-heading">
        <div>
            <h1 class="page-title">Hesabım</h1>
            <p class="page-subtitle">Profil bilgilerinizi görüntüleyin ve şifrenizi yönetin.</p>
        </div>
        <div class="page-actions">
            <button type="button" class="btn btn-default" id="btnSifreDegistir">Şifre Değiştir</button>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">
    <div class="profile-grid">
        <div class="card">
            <div class="card-header">
                <h2 class="card-title">Kişisel bilgiler</h2>
            </div>
            <div class="card-body">
                <dl class="detail-list">
                    <div class="detail-row">
                        <dt>Ad</dt>
                        <dd><asp:Label ID="lbAd" runat="server" Text=""></asp:Label></dd>
                    </div>
                    <div class="detail-row">
                        <dt>Soyad</dt>
                        <dd><asp:Label ID="lbSoyad" runat="server" Text=""></asp:Label></dd>
                    </div>
                    <div class="detail-row">
                        <dt>Kullanıcı adı</dt>
                        <dd><asp:Label ID="lbUserName" runat="server" Text=""></asp:Label></dd>
                    </div>
                    <div class="detail-row">
                        <dt>E-posta</dt>
                        <dd><asp:Label ID="lbEposta" runat="server" Text=""></asp:Label></dd>
                    </div>
                </dl>
            </div>
        </div>

        <div class="card">
            <div class="card-header">
                <h2 class="card-title">Hesap durumu</h2>
            </div>
            <div class="card-body">
                <dl class="detail-list">
                    <div class="detail-row">
                        <dt>Profil</dt>
                        <dd><asp:Label ID="lbProfil" runat="server" Text=""></asp:Label></dd>
                    </div>
                    <div class="detail-row">
                        <dt>Durum</dt>
                        <dd><span class="badge badge-plain"><asp:Label ID="lbDurum" runat="server" Text=""></asp:Label></span></dd>
                    </div>
                    <div class="detail-row">
                        <dt>Yetkiler</dt>
                        <dd><asp:Label ID="lbYetki" runat="server" Text=""></asp:Label></dd>
                    </div>
                </dl>
            </div>
        </div>
    </div>

    <!-- Şifre değiştirme modalı -->
    <div class="modal fade" id="sifreModal" tabindex="-1" role="dialog" aria-labelledby="sifreModalTitle">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="sifreModalTitle">Şifre Değiştir</h4>
                </div>
                <div class="modal-body">
                    <div class="notice notice-info" style="margin-bottom: 14px;">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                            <circle cx="12" cy="12" r="9"></circle>
                            <path d="M12 11v5"></path>
                            <path d="M12 8h.01"></path>
                        </svg>
                        <div>
                            <strong>Şifre kuralları</strong>
                            <ul class="notice-list">
                                <li>En az bir küçük harf ve bir büyük harf içermelidir.</li>
                                <li>En az 8, en fazla 128 karakter olmalıdır.</li>
                                <li>En az bir sayı ve bir özel karakter içermelidir.</li>
                            </ul>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label1" runat="server" Text="Mevcut şifreniz" AssociatedControlID="txtMevcutSifre"></asp:Label>
                        <asp:TextBox ID="txtMevcutSifre" runat="server" TextMode="Password" ClientIDMode="Static" MinLength="1" MaxLength="128" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label2" runat="server" Text="Yeni şifre" AssociatedControlID="txtYeniSifre"></asp:Label>
                        <asp:TextBox ID="txtYeniSifre" runat="server" TextMode="Password" ClientIDMode="Static" MinLength="8" MaxLength="128" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group mb-0">
                        <asp:Label ID="lblEmail" runat="server" Text="Yeni şifre tekrar" AssociatedControlID="txtYeniSifreTekrar"></asp:Label>
                        <asp:TextBox ID="txtYeniSifreTekrar" TextMode="Password" runat="server" ClientIDMode="Static" MinLength="8" MaxLength="128" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnVazgec" class="btn btn-default" data-dismiss="modal">Vazgeç</button>
                    <asp:Button ID="btnKaydet" runat="server" Text="Kaydet" CssClass="btn btn-primary" ClientIDMode="Static" OnClick="btnKaydet_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        $('#btnKaydet').click(function (event) {
            var mevcutSifre = $("#txtMevcutSifre").val();
            var sifre1 = $("#txtYeniSifre").val();
            var sifre2 = $("#txtYeniSifreTekrar").val();

            var isNumber = /[0-9]+/;
            var isUpper = /[A-Z]+/;
            var isLower = /[a-z]+/;
            var isOzelKarakter = /[!@#$%^&*()_+=\[{\]};:<>|./?,-]/;

            var errorMsg = "";
            var hataVar = false;

            if (mevcutSifre == '' || sifre1 == '' || sifre2 == '') {
                errorMsg = "Bütün alanların doldurulması zorunludur!";
                hataVar = true;
            }
            else if (sifre1 != sifre2) {
                errorMsg = "Lütfen şifrelerin aynı girildiğinden emin olunuz!";
                hataVar = true;
            }
            else if (sifre1.length < 8 || sifre1.length > 128) {
                errorMsg = "Şifreniz en az 8, en fazla 128 karakter olmalıdır!";
                hataVar = true;
            }
            else if (!isLower.test(sifre1)) {
                errorMsg = "Şifreniz en az bir küçük harf içermelidir!";
                hataVar = true;
            }
            else if (!isUpper.test(sifre1)) {
                errorMsg = "Şifreniz en az bir büyük harf içermelidir!";
                hataVar = true;
            }
            else if (!isNumber.test(sifre1)) {
                errorMsg = "Şifreniz en az bir sayısal ifade içermelidir!";
                hataVar = true;
            }
            else if (!isOzelKarakter.test(sifre1)) {
                errorMsg = "Şifreniz en az bir özel karakter içermelidir!";
                hataVar = true;
            }

            if (hataVar) {
                iqwerty.toast.toast(errorMsg);
                return false;
            }
            else {
                return true;
            }
        });

        $('#btnSifreDegistir').click(function (event) {
            $('#sifreModal').modal('show');
        });

        $('#btnVazgec').click(function (event) {
            $('#sifreModal').modal('hide');
        });
    </script>
</asp:Content>
