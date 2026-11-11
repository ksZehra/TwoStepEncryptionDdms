<%@ Page Title="Tablo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TabloSchemaList.aspx.cs" Inherits="TwoStepEncryptionDdms.TabloSchemaList" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <style>
        .Font {
            height: 30px;
            font: 12px Tahoma, Geneva, sans-serif;
        }

        .Ara {
            width: 60% !important;
            border: 0.5px solid grey;
        }

        .clsGrid {
            font: 12px Tahoma, Geneva, sans-serif;
            line-height: normal;
            background-color: White;
            border: 0.5px solid grey;
            box-sizing: border-box;
            border-spacing: 0;
        }

        input[type=text] {
            width: 95%;
        }

        td, th {
            padding: 3px !important;
        }
    </style>
    <div style="width: 100%; float: left;">
        <asp:TextBox ID="txtAra" CssClass="Font Ara" Width="65%" runat="server" MaxLength="200" placeholder=" Arama yapmak istediğiniz veri kümesinin adını giriniz..."></asp:TextBox>
        <asp:Button ID="btnAra" class="btn btn-warning btn-sm" Width="4%" runat="server" Text="Ara" OnClick="btnAra_Click" />
        <asp:Button ID="btnTabloEkle" class="btn btn-info btn-sm" ClientIDMode="Static" runat="server" Text="Tablo Ekle" Visible="false" Width="10%" />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">
    <asp:GridView ID="GridTbl" Style="text-align: center; margin-bottom: 10px;" runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
        OnRowDeleting="GridTbl_RowDeleting" DataKeyNames="TBLAD" OnRowEditing="GridTbl_RowEditing" OnRowUpdating="GridTbl_RowUpdating"
        OnRowCancelingEdit="GridTbl_RowCancelingEdit" OnPageIndexChanging="GridTbl_PageIndexChanging" Width="100%" CssClass="clsGrid" HeaderStyle-BackColor="#DCDCDC"
        EmptyDataText="Kayıt bulunamadı..." EmptyDataRowStyle-Height="25px" EmptyDataRowStyle-BackColor="#DCDCDC" PagerSettings-Position="Bottom" PagerStyle-Height="25px">
        <Columns>
            <asp:TemplateField HeaderText="Id" ItemStyle-Width="6%" ItemStyle-Height="30px" HeaderStyle-Height="35px">
                <ItemTemplate>
                    <asp:Label ID="lbTblId" runat="server" Text='<%# Eval("TBLID") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowDeleteButton="true" ItemStyle-Width="8%" HeaderText="Sil" DeleteText="<img src='Themes/img/del5.png' style='width:21px;height:21px;' alt='Sil'" Visible="false" />
            <asp:CommandField ShowEditButton="true" ItemStyle-Width="8%" HeaderText="Düzenle" EditText="<img src='Themes/img/edit16.jpg' style='width:21px;height:21px;' alt='Düzenle'" Visible="false" />
            <asp:BoundField DataField="TBLAD" HeaderText="Veri Kümesi"></asp:BoundField>
            <asp:TemplateField HeaderText="Şifreli Mi" ItemStyle-Width="10%">
                <ItemTemplate>
                    <asp:Label ID="lbSifreliMi" runat="server" Text='<%# Eval("ISENCRYPT") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Oluşturan" ItemStyle-Width="10%">
                <ItemTemplate>
                    <asp:Label ID="lbOlusturan" runat="server" Text='<%# Eval("OLUSTURAN") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Oluşturma Tarihi" ItemStyle-Width="10%">
                <ItemTemplate>
                    <asp:Label ID="lbOlusturmaTar" runat="server" Text='<%# Eval("OLUSTURMATARIHI") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Güncelleyen" ItemStyle-Width="10%">
                <ItemTemplate>
                    <asp:Label ID="lbGuncelleyen" runat="server" Text='<%# Eval("GUNCELLEYEN") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Güncelleme Tarihi" ItemStyle-Width="10%">
                <ItemTemplate>
                    <asp:Label ID="lbGuncellemeTar" runat="server" Text='<%# Eval("GUNCELLEMETARIHI") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <%-- <asp:TemplateField HeaderText="Kayıt Ekle" ItemStyle-Width="6%" Visible="false">
                <ItemTemplate>
                     <asp:HyperLink runat="server" NavigateUrl='<%# string.Format("~/DataEditNew.aspx?tblId={0}&tblAd={1}",
                    HttpUtility.UrlEncode(Eval("TBLID").ToString()), HttpUtility.UrlEncode(Eval("TBLAD").ToString())) %>'
                        Text="Yeni Kayıt Ekle" />
                    <asp:HyperLink ID="Hlink2" runat="server"
                        NavigateUrl='<%# "~/DataEditNew.aspx?tblId="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLID").ToString())+"&tblAd="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLAD").ToString()) %>'>
                        Kayıt Ekle
                        <img src="Themes/img/add5.png" alt="Kayıt Ekle" style="width:26px;height:26px;" />
                    </asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>--%>
        </Columns>
        <PagerSettings Mode="NumericFirstLast" FirstPageText="İlk" LastPageText="Son" />
        <%--        <PagerSettings Mode="NumericFirstLast" FirstPageText="İlk" LastPageText="Son"/>--%>
    </asp:GridView>

    <%-- ************Devexpress*************--%>
    <%--    <dx:ASPxGridView runat="server" ID="myGrid" ClientIDMode="Static" Width="100%" KeyFieldName="TBLID"
        OnRowDeleting="myGrid_RowDeleting" SettingsPager-PageSize="10">
        Styles-Header-HorizontalAlign="Center"
        <Columns>
             <dx:GridViewDataHyperLinkColumn Caption="Sil" FieldName="TBLID" Name="Sil" CellStyle-HorizontalAlign="Center" Width="20px" Settings-AllowSort="False" Settings-AllowAutoFilter="False" Settings-AllowDragDrop="False">
                <PropertiesHyperLinkEdit ImageUrl="Themes/img/del5.png" Text="Tabloyu Sil"></PropertiesHyperLinkEdit>
            </dx:GridViewDataHyperLinkColumn>
            <dx:GridViewCommandColumn Caption="Sil" ShowDeleteButton="true" ToolTip="Tabloyu Sil" Width="5%" AllowDragDrop="False">
            </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn Caption="Tablo Id" FieldName="TBLID" CellStyle-HorizontalAlign="Center"></dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Tablo" FieldName="TBLAD"></dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Şifreli Mi" FieldName="ISENCRYPT"></dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Oluşturan" FieldName="OLUSTURAN"></dx:GridViewDataTextColumn>
            <dx:GridViewDataDateColumn Caption="Oluşturma Tarihi" FieldName="OLUSTURMATARIHI"></dx:GridViewDataDateColumn>
            <dx:GridViewDataTextColumn Caption="Güncelleyen" FieldName="GUNCELLEYEN"></dx:GridViewDataTextColumn>
            <dx:GridViewDataDateColumn Caption="Güncelleme Tarihi" FieldName="GUNCELLEMETARIHI"></dx:GridViewDataDateColumn>
        </Columns>
        <Settings ShowFilterRow="true" ShowFilterRowMenu="true" ShowHeaderFilterButton="true" ShowHeaderFilterBlankItems="true" />
        <SettingsText ConfirmDelete="Silmek istediğinizden emin misiniz?" EmptyDataRow="Kayıt bulunamadı..." HeaderFilterCancelButton="Vazgeç" HeaderFilterOkButton="Tamam" HeaderFilterSelectAll="Tümünü Seç" />
        <SettingsPager Summary-AllPagesText=" {1} ({2} Kayıtlar)" Summary-Text="{1} Sayfanın {0}.Sayfası,  (Kayıt Sayısı: {2})"></SettingsPager>
    </dx:ASPxGridView>--%>
</asp:Content>
