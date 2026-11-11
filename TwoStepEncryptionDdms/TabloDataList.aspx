<%@ Page Title="Tablo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TabloDataList.aspx.cs" Inherits="TwoStepEncryptionDdms.TabloDataList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <style>
        .Font {
            height: 30px;
            font: 12px Tahoma, Geneva, sans-serif;
        }

        .Ara {
            width: 40%;
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

        td, th {
            padding: 3px !important;
        }
    </style>
    <div style="width: 100%; float: left;">
        <asp:TextBox ID="txtAra" CssClass="Font Ara" Width="65%" runat="server" MaxLength="200" placeholder=" Arama yapmak istediğiniz veri kümesinin adını giriniz..."></asp:TextBox>
        <asp:Button ID="btnAra" class="btn btn-warning btn-sm" Width="4%" runat="server" Text="Ara" OnClick="btnAra_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">

    <asp:GridView ID="GridTbl" Style="text-align: center; margin-bottom: 10px;" runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
        DataKeyNames="TBLAD" Width="100%" OnPageIndexChanging="GridTbl_PageIndexChanging"
        CssClass="clsGrid" HeaderStyle-BackColor="#DCDCDC" EmptyDataText="Kayıt bulunamadı..." EmptyDataRowStyle-Height="25px" EmptyDataRowStyle-BackColor="#DCDCDC"
        PagerSettings-Position="Bottom" PagerStyle-Height="25px">
        <Columns>
            <asp:TemplateField HeaderText="Id" ItemStyle-Width="6%" ItemStyle-Height="30px" HeaderStyle-Height="35px">
                <ItemTemplate>
                    <asp:Label ID="lbTblId" runat="server" Text='<%# Eval("TBLID") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Kayıt Görüntüle" ItemStyle-Width="6%" Visible="false">
                <ItemTemplate>
                    <asp:HyperLink ID="Hlink1" runat="server"
                        NavigateUrl='<%# "~/DataView.aspx?tblAd="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLAD").ToString()) %>'>
                        <img src="Themes/img/view7.png" alt="Kayıt Görüntüle" style="width:21px;height:21px;" />
                    </asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Kayıt Ekle" ItemStyle-Width="6%" Visible="false">
                <ItemTemplate>
                    <asp:HyperLink ID="Hlink2" runat="server"
                        NavigateUrl='<%# "~/DataEditNew.aspx?tblId="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLID").ToString())+"&tblAd="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLAD").ToString()) %>'>
                        <img src="Themes/img/add5.png" alt="Kayıt Ekle" style="width:24px;height:24px;" />
                    </asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Kayıt Güncelle" ItemStyle-Width="6%" Visible="false">
                <ItemTemplate>
                    <asp:HyperLink ID="Hlink3" runat="server"
                        NavigateUrl='<%# "~/DataEditList.aspx?tblId="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLID").ToString())+"&tblAd="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLAD").ToString())+"&type="+HttpUtility.UrlEncode("edit") %>'>
                       <img src="Themes/img/edit12.png" alt="Kayıt Güncelle" style="width:22px;height:22px;" />
                    </asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Kayıt Sil" ItemStyle-Width="6%" Visible="false">
                <ItemTemplate>
                    <asp:HyperLink ID="Hlink4" runat="server"
                        NavigateUrl='<%# "~/DataEditList.aspx?tblId="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLID").ToString())+"&tblAd="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLAD").ToString())+"&type="+HttpUtility.UrlEncode("delete") %>'>
                       <img src="Themes/img/del16.png" alt="Kayıt Sil" style="width:23px;height:23px;" />
                    </asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
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
        </Columns>
        <PagerSettings Mode="Numeric" FirstPageText="İlk" LastPageText="Son" />
    </asp:GridView>
</asp:Content>
