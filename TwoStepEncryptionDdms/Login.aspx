<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TwoStepEncryptionDdms.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Giriş Yap</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>

</head>
<body>
    <div class="container">
        <h2>Giriş Yap</h2>
        <form id="form1" runat="server" autocomplete="off">
            <div class="form-group">
                <asp:Label ID="lblMesaj" runat="server" Font-Size="Medium" ForeColor="Red"></asp:Label>
            </div>
            <div class="form-group">
                <asp:Label ID="lblKAdi" runat="server" Text="Kullanıcı Adı"></asp:Label>
                <asp:TextBox ID="txtKAdi" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label ID="lblPassword" runat="server" Text="Sifre"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" MaxLength="200"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:CheckBox ID="chboxBeniHatirla" runat="server" />
                <asp:Label ID="lblChboxBeniHatirla" runat="server" Text="Beni Hatırla"></asp:Label>
            </div>
            <div class="form-group">
                <asp:Button ID="btnGirisYap" runat="server" Text="Giriş Yap" CssClass="btn btn-info" OnClick="btnGirisYap_Click" Width="150px" />
                <asp:Button ID="btnEmailAktif" runat="server" Text="Hesabını Aktif Et" CssClass="btn btn-group-default" OnClick="btnEmailAktif_Click" Width="150px" Visible="false" />
                <asp:Button ID="btnKayitOl" runat="server" Text="Kayıt Ol" CssClass="btn btn-warning" Width="150px" BorderStyle="Solid" OnClick="btnKayitOl_Click" />
            </div>
        </form>
    </div>
</body>

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
</script>
</html>
