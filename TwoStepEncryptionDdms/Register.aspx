<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="TwoStepEncryptionDdms.Register" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Kayıt Ol</title>
    <meta charset="utf-8">

    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
</head>
<body>
    <div class="container">
        <h2>Kayıt Ol</h2>
        <form id="form1" runat="server">
            <div class="form-group">
                <asp:Label ID="lblMessage" runat="server" Font-Size="Medium" ForeColor="Red"></asp:Label>
            </div>
            <div class="form-group">
                <asp:Label ID="lblAd" runat="server" Text="Ad"></asp:Label>
                <asp:TextBox ID="txtAd" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label ID="lblSoyad" runat="server" Text="Soyad"></asp:Label>
                <asp:TextBox ID="txtSoyad" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label ID="lblEmail" runat="server" Text="Email"></asp:Label>
                <asp:TextBox ID="txtEmail" runat="server" MaxLength="250" TextMode="Email" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label ID="lblKullaniciAdi" runat="server" Text="Kullanıcı Adı"></asp:Label>
                <asp:TextBox ID="txtKullaniciAdi" runat="server" MaxLength="100" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label ID="lblSifre" runat="server" Text="Şifre"></asp:Label>
                <asp:TextBox ID="txtSifre" runat="server" TextMode="Password" MaxLength="200" MinLength="6" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label ID="lblTekrarSifre" runat="server" Text="Tekrar Şifre"></asp:Label>
                <asp:TextBox ID="txtTekrarSifre" runat="server" TextMode="Password" MaxLength="200" MinLength="6" CssClass="form-control"></asp:TextBox>
            </div>

            <div class="col-12 form-group">
                <b>Şifreniz en az bir küçük harf, en az bir büyük harf, en az bir sayısal değer, en az bir özel karakter içermelidir, en az 6 en fazla 12 karakterden oluşmalıdır!</b>
            </div>

            <div class="form-group">
                <asp:Button ID="btnKayitOl" runat="server" Text="Kayıt Ol" CssClass="btn btn-info" float="left" Width="145px" OnClick="btnKayitOl_Click" />
                <asp:Button ID="btnGiriş" runat="server" Text="Giriş Yap" CssClass="btn btn-warning" OnClick="btnGiriş_Click" float="left" Width="145px" />
            </div>

        </form>
    </div>
</body>

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
        else {
            if ($('#txtSifre').length < 6 && $('#txtSifre').length > 200) {
                alert("Şifreniz en az 6 en fazla 200 karakterden oluşabilir!");
                return false
            }
        }
    });  
</script>
</html>
