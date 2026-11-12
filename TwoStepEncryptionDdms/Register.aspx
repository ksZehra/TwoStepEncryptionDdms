<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="TwoStepEncryptionDdms.Register" %>

<!DOCTYPE html>
<html lang="tr">
<head runat="server">
    <title>Kayıt Ol</title>
    <link rel="icon" type="image/svg+xml" href="data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 32 32'%3E%3Crect width='32' height='32' rx='7' fill='%232563eb'/%3E%3Crect x='8' y='15' width='16' height='11' rx='2' fill='white'/%3E%3Cpath d='M12 15v-3a4 4 0 0 1 8 0v3' fill='none' stroke='white' stroke-width='2'/%3E%3C/svg%3E" />
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/Site.css" rel="stylesheet" />

    <script src="Scripts/jquery-3.3.1.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <script src="Themes/js/toast.min.js"></script>
</head>
<body class="auth-body">
    <div class="auth-shell auth-shell-wide">
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

        <div class="auth-card auth-card-wide">
            <h1 class="auth-title">Kayıt ol</h1>
            <p class="auth-subtitle">Yeni bir kullanıcı hesabı oluşturun.</p>

            <form id="form1" runat="server" autocomplete="off">
                <asp:Label ID="lblMessage" runat="server" CssClass="form-alert"></asp:Label>

                <div class="form-grid">
                    <div class="form-group">
                        <asp:Label ID="lblAd" runat="server" Text="Ad" AssociatedControlID="txtAd"></asp:Label>
                        <asp:TextBox ID="txtAd" runat="server" MaxLength="50" CssClass="form-control" ClientIDMode="Static" placeholder="Adınız"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblSoyad" runat="server" Text="Soyad" AssociatedControlID="txtSoyad"></asp:Label>
                        <asp:TextBox ID="txtSoyad" runat="server" MaxLength="50" CssClass="form-control" ClientIDMode="Static" placeholder="Soyadınız"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblEmail" runat="server" Text="E-posta" AssociatedControlID="txtEmail"></asp:Label>
                        <asp:TextBox ID="txtEmail" runat="server" MaxLength="250" TextMode="Email" CssClass="form-control" ClientIDMode="Static" placeholder="ornek@eposta.com"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblKullaniciAdi" runat="server" Text="Kullanıcı Adı" AssociatedControlID="txtKullaniciAdi"></asp:Label>
                        <asp:TextBox ID="txtKullaniciAdi" runat="server" MaxLength="100" CssClass="form-control" ClientIDMode="Static" placeholder="Kullanıcı adınız"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblSifre" runat="server" Text="Şifre" AssociatedControlID="txtSifre"></asp:Label>
                        <div class="password-field">
                            <asp:TextBox ID="txtSifre" runat="server" TextMode="Password" MaxLength="128" MinLength="8" CssClass="form-control" ClientIDMode="Static" placeholder="Şifreniz"></asp:TextBox>
                            <button type="button" class="password-toggle" data-target="txtSifre" aria-label="Şifreyi göster">Göster</button>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblTekrarSifre" runat="server" Text="Tekrar Şifre" AssociatedControlID="txtTekrarSifre"></asp:Label>
                        <div class="password-field">
                            <asp:TextBox ID="txtTekrarSifre" runat="server" TextMode="Password" MaxLength="128" MinLength="8" CssClass="form-control" ClientIDMode="Static" placeholder="Şifrenizi tekrar girin"></asp:TextBox>
                            <button type="button" class="password-toggle" data-target="txtTekrarSifre" aria-label="Şifreyi göster">Göster</button>
                        </div>
                    </div>
                </div>

                <div class="notice notice-info">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                        <circle cx="12" cy="12" r="9"></circle>
                        <path d="M12 11v5"></path>
                        <path d="M12 8h.01"></path>
                    </svg>
                    <div>Şifreniz en az bir küçük harf, bir büyük harf, bir sayı ve bir özel karakter içermelidir; 8–128 karakter arasında olmalıdır.</div>
                </div>

                <div class="auth-actions">
                    <asp:Button ID="btnKayitOl" runat="server" Text="Kayıt Ol" CssClass="btn btn-primary btn-block" OnClick="btnKayitOl_Click" />
                    <asp:Button ID="btnGiris" runat="server" Text="Giriş Yap" CssClass="btn btn-default btn-block" OnClick="btnGiris_Click" />
                </div>
            </form>
        </div>
    </div>

    <script type="text/javascript">
        $('#btnKayitOl').click(function (event) {
            var ad = $("#txtAd").val();
            var soyad = $("#txtSoyad").val();
            var email = $("#txtEmail").val();
            var KULLANICIADI = $("#txtKullaniciAdi").val();
            var sifre1 = $("#txtSifre").val();
            var sifre2 = $("#txtTekrarSifre").val();

            if (ad == '' || soyad == '' || email == '' || KULLANICIADI == '' || sifre1 == '' || sifre2 == '') {
                alert("Bütün alanların doldurulması zorunludur!");
                return false
            }
            else if (sifre1 != sifre2) {
                alert("Lütfen şifre alanlarının aynı girildiğinden emin olunuz!");
                return false
            }
            else if (sifre1.length < 8 || sifre1.length > 128) {
                alert("Şifreniz en az 8, en fazla 128 karakterden oluşabilir!");
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
