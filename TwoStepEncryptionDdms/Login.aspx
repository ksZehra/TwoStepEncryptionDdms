<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TwoStepEncryptionDdms.Login" %>

<!DOCTYPE html>
<html lang="tr">
<head runat="server">
    <title>Giriş Yap</title>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/Site.css" rel="stylesheet" />

    <script src="Scripts/jquery-3.3.1.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <script src="Themes/js/toast.min.js"></script>
</head>
<body class="auth-body">
    <div class="auth-shell">
        <div class="auth-brand">
            <span class="brand-mark" aria-hidden="true">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
                    <rect x="4" y="10" width="16" height="10" rx="2"></rect>
                    <path d="M8 10V7a4 4 0 0 1 8 0v3"></path>
                </svg>
            </span>
            <div>
                <div class="auth-brand-name">Şifreli Dinamik Veri Kayıt Sistemi</div>
                <div class="auth-brand-note">İki adımlı şifreleme</div>
            </div>
        </div>

        <div class="auth-card">
            <h1 class="auth-title">Giriş yap</h1>
            <p class="auth-subtitle">Devam etmek için hesabınıza giriş yapın.</p>

            <form id="form1" runat="server" autocomplete="off">
                <asp:Label ID="lblMesaj" runat="server" CssClass="form-alert"></asp:Label>

                <div class="form-group">
                    <asp:Label ID="lblKAdi" runat="server" Text="Kullanıcı Adı" AssociatedControlID="txtKAdi"></asp:Label>
                    <asp:TextBox ID="txtKAdi" runat="server" CssClass="form-control" MaxLength="100" ClientIDMode="Static" placeholder="Kullanıcı adınız" autocomplete="username"></asp:TextBox>
                </div>

                <div class="form-group">
                    <asp:Label ID="lblPassword" runat="server" Text="Şifre" AssociatedControlID="txtPassword"></asp:Label>
                    <div class="password-field">
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" MaxLength="200" ClientIDMode="Static" placeholder="Şifreniz" autocomplete="current-password"></asp:TextBox>
                        <button type="button" class="password-toggle" data-target="txtPassword" aria-label="Şifreyi göster">Göster</button>
                    </div>
                </div>

                <div class="checkbox-inline auth-checkbox">
                    <asp:CheckBox ID="chboxBeniHatirla" runat="server" ClientIDMode="Static" />
                    <asp:Label ID="lblChboxBeniHatirla" runat="server" Text="Beni Hatırla"></asp:Label>
                </div>

                <div class="auth-actions">
                    <asp:Button ID="btnGirisYap" runat="server" Text="Giriş Yap" CssClass="btn btn-primary btn-block" OnClick="btnGirisYap_Click" />
                    <asp:Button ID="btnEmailAktif" runat="server" Text="Hesabını Aktif Et" CssClass="btn btn-default btn-block" OnClick="btnEmailAktif_Click" Visible="false" />
                </div>

                <div class="auth-footer">
                    Hesabınız yok mu?
                    <asp:Button ID="btnKayitOl" runat="server" Text="Kayıt Ol" CssClass="btn btn-link btn-sm" BorderStyle="None" OnClick="btnKayitOl_Click" />
                </div>
            </form>
        </div>
    </div>

    <script type="text/javascript">
        $('#btnGirisYap').click(function () {
            if (document.getElementById("txtKAdi").value == "") {
                alert("Kullanıcı adı boş bırakılamaz!");
                return false
            }
            else if (document.getElementById("txtPassword").value == "") {
                alert("Şifre boş bırakılamaz!");
                return false
            }
        });

        $('.password-toggle').on('click', function () {
            var targetId = $(this).data('target');
            var $input = $('#' + targetId);
            var show = $input.attr('type') === 'password';

            $input.attr('type', show ? 'text' : 'password');
            $(this).text(show ? 'Gizle' : 'Göster');
        });
    </script>
</body>
</html>
