<%@ Page Title="Tablo Oluştur" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TabloNew.aspx.cs" Inherits="TwoStepEncryptionDdms.TabloNew" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .lblDuzenle {
            margin: 5px;
        }

        .btnSil {
            border-color: #ba1f1f !important;
        }

        table td, table th {
            height: 30px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <style>
        #divUst2 {
            display: none !important;
        }

        .sagAltIcerik {
            margin-top: 0 !important;
        }

        .cls-font {
            font: 12px Tahoma, Geneva, sans-serif !important;
        }

        td, th {
            font: 13px Tahoma, Geneva, sans-serif !important;
            padding: 3px !important;
        }

        th {
            font-weight: bold !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">

    <div class="col-md-12 col-lg-12 col-sm-12" style="margin: 0 !important; padding: 0px !important;">
        <div class="cls-divCerceve">
            <div class="cls-divHeader">
                <h4>Veri Kümesi Oluştur</h4>
            </div>

            <div class="alert alert-danger cls-font" role="alert" style="padding: 10px; line-height: 2">
                <p>Veri kümesi ve sütun isimlerini aşağıdaki yönergelere uygun olarak doldurunuz:</p>
                <p>- Veri kümesi ve sütun isimlerinde "_" ifadesi dışında özel karakter kullanılmamalıdır. Kullanılan özel karakterler geçersiz sayılarak işlem devam edecektir.</p>
                <p>- Veri kümsei veya sütun isimlerinde boşluk bırakılmamalıdır. Bırakılan boşluklar "_" karakteri ile değiştirilerek işlem devam edecektir.</p>
            </div>

            <div class="form-group row">
                <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 10%; padding-top: 6px; font: 13px Tahoma, Geneva, sans-serif !important;">Veri Kümesi Adı :</asp:Label>
                <div class="col-sm-10" style="width: 90%; padding-left: 0px;">
                    <asp:TextBox class="form-control cls-font" ID="txtTabloAdi" MaxLength="120" Height="28px" placeholder="Veri kümesi adını giriniz..." runat="server" ClientIDMode="Static"></asp:TextBox>
                </div>
            </div>
            <div class="form-group row">
                <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 10%; padding-top: 6px; font: 13px Tahoma, Geneva, sans-serif !important;">Anahtar (Key) :</asp:Label>
                <div class="col-sm-10" style="width: 90%; padding-left: 0px;">
                    <asp:TextBox class="form-control cls-font" ID="txtKey" MaxLength="32" Height="28px" placeholder="Anahtarı (key) 32 karakter olacak şekilde giriniz..." runat="server" ClientIDMode="Static"></asp:TextBox>
                </div>
            </div>
            <div class="form-group row">
                <asp:Label ID="lblKolonAdi" runat="server" Text="Sütun Adı :" class="col-sm-2 col-form-label" Style="width: 10%; padding-top: 6px; padding-right: 0px; font: 13px Tahoma, Geneva, sans-serif !important;"></asp:Label>
                <div class="col-sm-10" style="width: 90%; padding-left: 0px;">
                    <asp:TextBox class="form-control cls-font" ID="txtKolonAdi" MaxLength="50" Height="28px" placeholder="Sütun adını giriniz..." runat="server" ClientIDMode="Static"></asp:TextBox>
                </div>
            </div>
            <div class="form-group row">
                <asp:Label runat="server" class="col-sm-2 col-form-label" Style="width: 10%; padding-right: 0px; padding-top: 2px; font: 13px Tahoma, Geneva, sans-serif !important;">Şifrelensin Mi ?</asp:Label>
                <div class="col-sm-10" style="width: 90%; padding-left: 0px;">
                    <asp:CheckBox class="form-check-input" type="checkbox" ID="cbSifrele" runat="server" ClientIDMode="Static" Style="margin-top: 9px;" />
                </div>
            </div>
            <asp:Button ID="btnEkle" CssClass="btn btn-warning" Width="8%" runat="server" Text="Kolon Ekle" ClientIDMode="Static" OnClick="btnEkle_Click" />

            <div class="col-md-12 col-lg-12 col-sm-12" style="margin: 20px 0 20px 0 !important; padding: 0px !important; width: 100%;">
                <asp:ListView ID="listVwTblOlustur" runat="server" GroupPlaceholderID="gphListView" ItemPlaceholderID="iphListView"
                    OnItemDeleting="listVwTblOlustur_ItemDeleting"
                    OnItemEditing="listVwTblOlustur_ItemEditing" OnItemUpdating="listVwTblOlustur_ItemUpdating" OnItemCanceling="listVwTblOlustur_ItemCanceling">

                    <LayoutTemplate>
                        <table style="width: 100% !important;">
                            <tr style="background-color: lightgray;">
                                <th style="width: 65% !important; padding-bottom: 5px !important; padding-top: 5px !important; border: 1px solid gray; text-align: center;">Sütun Adı</th>
                                <th style="width: 15% !important; padding-bottom: 5px !important; padding-top: 5px !important; border: 1px solid gray; text-align: center;">Şifrelensin Mi</th>
                                <th style="width: 10% !important; padding-bottom: 5px !important; padding-top: 5px !important; border: 1px solid gray; text-align: center;">Sil</th>
                                <th style="width: 10% !important; padding-bottom: 5px !important; padding-top: 5px !important; border: 1px solid gray; text-align: center;">Düzenle</th>
                            </tr>
                            <asp:PlaceHolder runat="server" ID="gphListView"></asp:PlaceHolder>
                        </table>
                    </LayoutTemplate>
                    <GroupTemplate>
                        <tr>
                            <asp:PlaceHolder runat="server" ID="iphListView"></asp:PlaceHolder>
                        </tr>
                    </GroupTemplate>
                    <ItemTemplate>
                        <tr>
                            <td style="border: solid 1px; border-color: gray; border-radius: 4px; height: 10px; padding-left: 5px !important; text-align: center;">
                                <asp:Label ID="lblKolonAdi" Text='<%# Eval("KolonAdi") %>' runat="server" />
                            </td>
                            <td style="border: solid 1px; border-color: gray; border-radius: 4px; height: 10px; padding-left: 5px !important; text-align: center;">
                                <asp:Label ID="lbIsEncrypted" Text='<%# Eval("IsEncrypted") %>' runat="server" />
                            </td>
                            <td style="border: solid 1px; border-color: gray; padding: 1px 3px 1px 5px; text-align: center;">
                                <asp:Button ID="btnSil" CssClass="btn btn-danger btn-xs btnSil" runat="server" Text="Sil" CommandName="Delete" />
                            </td>
                            <td style="border: solid 1px; border-color: gray; padding: 1px 3px 1px 0; text-align: center;">
                                <asp:Button ID="btnDüzenle" CssClass="btn btn-success btn-xs" runat="server" Text="Düzenle" CommandName="Edit" />
                            </td>
                            <td></td>
                        </tr>
                    </ItemTemplate>

                    <EditItemTemplate>
                        <tr style="width: 100% !important;">
                            <td style="width: 65%; border: solid 1px gray;">
                                <asp:TextBox ID="txtEmpKolonAdi" runat="server" Height="25px" Width="99%" MaxLength="200" CssClass="lblDuzenle" Text='<%# Bind("KolonAdi") %>' />
                            </td>
                            <td style="width: 15%; border: solid 1px gray; text-align: center;">
                                <asp:Label ID="lbIsEncrypted" runat="server" Text='<%# Bind("IsEncrypted") %>'></asp:Label>
                            </td>
                            <td style="width: 10%; border: solid 1px gray; text-align: center;">
                                <asp:Button ID="btnGuncelle" CssClass="btn btn-info btn-xs" runat="server" Text="Güncelle" CommandName="Update" />
                            </td>
                            <td style="width: 10%; border: solid 1px gray; text-align: center;">
                                <asp:Button ID="btnIptal" CssClass="btn btn-warning btn-xs" runat="server" Text="İptal" CommandName="Cancel" />
                            </td>
                        </tr>
                    </EditItemTemplate>
                </asp:ListView>
            </div>

            <div class="form-group" style="margin-bottom: 20px; margin-top: 20px; padding: 0px !important; display: none;" id="divButtons" runat="server">
                <center>
                    <asp:Button ID="btnTblOlustur" runat="server" Text="Oluştur" Width="8%" CssClass="btn btn-success" ClientIDMode="Static" float="left" OnClick="btnTblOlustur_Click" />
                    <asp:Button ID="btnTemizle" runat="server" Text="Temizle" Width="8%" CssClass="btn btn-info" float="left" OnClick="btnTemizle_Click" />
                </center>
            </div>

        </div>
    </div>
    <script type="text/javascript">
        $('#btnTblOlustur').click(function () {
            if (document.getElementById("txtTabloAdi").value == "") {
                alert("Veri kümesi adı boş bırakılamaz!");
                return false
            }
        });

        $('#btnEkle').click(function () {
            if (document.getElementById("txtKolonAdi").value == "") {
                alert("Sütun adı boş bırakılamaz!");
                return false
            }
        });
    </script>
</asp:Content>
