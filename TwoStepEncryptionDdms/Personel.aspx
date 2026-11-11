<%@ Page Title="Hesabım" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Personel.aspx.cs" Inherits="TwoStepEncryptionDdms.Personel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <button type="button" class="btn btn-warning" id="btnSifreDegistir">
        Şifre Değiştir
    </button>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">
    <style>
        td {
            font: 13px Tahoma, Geneva, sans-serif;
        }

        .boldText {
            font-weight: bold;
            height: 40px;
            min-width: 60px;
            margin: 10px 0 10px 0;
        }

        .black_overlay {
            display: none;
            position: absolute;
            top: 0%;
            left: 0%;
            width: 100%;
            height: 100%;
            background-color: black;
            z-index: 1001;
            -moz-opacity: 0.7;
            opacity: .70;
            filter: alpha(opacity=70);
        }

        .modal-content {
            font: 13px Tahoma, Geneva, sans-serif;
        }
    </style>

    <div class="cls-divCerceve">
        <div class="cls-divHeader">
            <h4>Personel Bilgileri</h4>
        </div>

        <table style="background: white; border-radius: 10px; width: 100%;">
            <tr>
                <td class="boldText" style="width: 7%;">Ad:</td>
                <td style="width: 43%;">
                    <asp:Label ID="lbAd" runat="server" Text=""></asp:Label>
                </td>
                <td class="boldText" style="width: 4%;">Soyad:</td>
                <td style="width: 46%;">
                    <asp:Label ID="lbSoyad" runat="server" Text=""></asp:Label></td>
            </tr>
            <tr>
                <td class="boldText">Kullanıcı Adı:</td>
                <td>
                    <asp:Label ID="lbUserName" runat="server" Text=""></asp:Label></td>
                <td class="boldText">E-posta:</td>
                <td>
                    <asp:Label ID="lbEposta" runat="server" Text=""></asp:Label></td>
            </tr>
            <tr>
                <td class="boldText">Profil:</td>
                <td>
                    <asp:Label ID="lbProfil" runat="server" Text=""></asp:Label></td>
                <td class="boldText">Durum:</td>
                <td>
                    <asp:Label ID="lbDurum" runat="server" Text=""></asp:Label></td>
            </tr>
            <tr>
                <td class="boldText">Yetkileri:</td>
                <td>
                    <asp:Label ID="lbYetki" runat="server" Text=""></asp:Label></td>
            </tr>
        </table>
    </div>

    <!-- Modal -->
    <div id="fadeContent" style="position: absolute; top: 10%; left: 25%; width: 50%; height: 50%; display: none; z-index: 1002;">

        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title text-dark" id="modalSifremiUnuttum">Şifre Değiştir</h4>
                </div>

                <div class="col-12" style="padding: 10px 15px 0 15px;">
                    <div class="page-custom-info w-100 mt-4">
                        <b>Şifre Kuralları:</b>
                        <div class="text-sm my-2" style="padding-top: 5px;">
                            <div>- En az bir küçük harf içermelidir</div>
                            <div>- En az bir büyük harf içermelidir</div>
                            <div>- En az 6 en fazla 12 karakterden oluşmalıdır</div>
                            <div>- En az bir sayısal değer içermelidir</div>
                            <div>- En az bir özel karakter içermelidir</div>
                        </div>
                    </div>
                </div>

                <div class="modal-body">
                    <div class="form-group">
                        <asp:Label ID="Label1" runat="server" Text="Mevcut şifreniz"></asp:Label>
                        <asp:TextBox ID="txtMevcutSifre" runat="server" TextMode="Password" ClientIDMode="Static" MinLength="6" MaxLength="200" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="Label2" runat="server" Text="Yeni Şifre"></asp:Label>
                        <asp:TextBox ID="txtYeniSifre" runat="server" TextMode="Password" ClientIDMode="Static" MinLength="6" MaxLength="200" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblEmail" runat="server" Text="Yeni Şifre Tekrar"></asp:Label>
                        <asp:TextBox ID="txtYeniSifreTekrar" TextMode="Password" runat="server" ClientIDMode="Static" MinLength="6" MaxLength="200" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnKaydet" runat="server" Text="Kaydet" class="btn btn-success" ClientIDMode="Static" OnClick="btnKaydet_Click" />
                    <button type="button" id="btnVazgec" class="btn btn-info" data-dismiss="modal">Vazgeç</button>
                </div>
            </div>
        </div>
    </div>

    <div id="fade" class="black_overlay"></div>

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
            var hatVar = false;

            if (mevcutSifre == '' || sifre1 == '' || sifre2 == '') {
                errorMsg = "Bütün alanların doldurulması zorunludur!";
                hataVar = true;
            }
            else if (sifre1 != sifre2) {
                errorMsg = "Lütfen şifrelerin aynı girildiğinden emin olunuz!";
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
                errorMsg = "Şifreniz en bir özel karakter içermelidir!";
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
            $('#fadeContent').fadeIn();
            $('#fade').fadeIn();
        });

        $('#btnVazgec').click(function (event) {
            $('#fadeContent').fadeOut();
            $('#fade').fadeOut();
        });
    </script>
</asp:Content>
