<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataEditNew.aspx.cs" Inherits="TwoStepEncryptionDdms.DataEditNew" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <style type="text/css">
        .lblSessionCss {
            float: left !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">
    <style type="text/css">
        .alert-info {
            background-color: #d9edf7;
            border-color: #bce8f1;
            color: #3a87ad;
            padding: 7px 0 2px 0 !important;
            margin: 0 0 20px 0;
        }
    </style>

    <div style="width: 100%; border: 1px solid gray; padding: 10px 0 10px 0; margin-bottom: 10px;" runat="server" id="divKey">
        <div class="form-group row" style="padding-left: 1% !important;">
            <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 4.5%; padding-top: 5px;">Anahtar:</asp:Label>
            <div class="col-sm-10" style="width: 95%;">
                <asp:TextBox class="form-control" ID="tbKey" MaxLength="32" placeholder="32 karakter olacak şekilde anahtarı giriniz..." Width="100%" Height="30px" runat="server" ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>
        <center>
            <asp:Button ID="btnKey" class="btn btn-success" runat="server" Text="Kontrol Et" Width="150px" OnClick="btnKey_Click" ClientIDMode="Static" />
        </center>
    </div>

    <div style="width: 100%;" runat="server" id="divTablo" visible="false">
        <center>
            <div class="panel-content" style="border: 1px solid gray; padding: 10px;">

                <div class="alert alert-info" runat="server" id="divAlert" style="padding: 10px !important;">
                    <label>Mavi renkli alanlara eklenen veriler şifrelenerek kaydedilecektir.</label>
                </div>

                <h4 style="font-weight: bold; margin-top: 0; margin-bottom: 10px;" runat="server" id="tabloBaslik"></h4>

                <asp:HiddenField runat="server" ID="hfTable" ClientIDMode="Static"></asp:HiddenField>
                <asp:Literal runat="server" ID="ltList"></asp:Literal>

                <asp:Button ID="btnKayitGir" class="btn btn-info" runat="server" Text="Ekle" Width="150px" OnClick="btnKayitGir_Click" ClientIDMode="Static" />
                <asp:HiddenField runat="server" ID="hfSaveData" ClientIDMode="Static" />
            </div>
        </center>
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
