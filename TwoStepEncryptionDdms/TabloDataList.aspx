<%@ Page Title="Tablo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TabloDataList.aspx.cs" Inherits="TwoStepEncryptionDdms.TabloDataList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <div class="page-heading">
        <div>
            <h1 class="page-title">Kayıtlar</h1>
            <p class="page-subtitle">Veri kümelerindeki kayıtları görüntüleyin, ekleyin, güncelleyin veya silin.</p>
        </div>
        <div class="page-actions">
            <div class="search-box">
                <asp:TextBox ID="txtAra" CssClass="form-control" runat="server" MaxLength="200" placeholder="Veri kümesi adı ara..."></asp:TextBox>
                <asp:Button ID="btnAra" CssClass="btn btn-primary" runat="server" Text="Ara" OnClick="btnAra_Click" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">

    <div class="card">
        <div class="card-header">
            <h2 class="card-title">Veri kümesi kayıtları</h2>
            <span class="card-note">Sayfa boyutu: 10</span>
        </div>
        <div class="table-responsive">
    <asp:GridView ID="GridTbl" runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
        DataKeyNames="TBLAD" OnPageIndexChanging="GridTbl_PageIndexChanging"
        CssClass="data-table" GridLines="None" EmptyDataText="Kayıt bulunamadı."
        PagerSettings-Position="Bottom" PagerStyle-CssClass="grid-pager">
        <Columns>
            <asp:TemplateField HeaderText="Id" ItemStyle-Width="6%" ItemStyle-Height="30px" HeaderStyle-Height="35px">
                <ItemTemplate>
                    <asp:Label ID="lbTblId" runat="server" Text='<%# Eval("TBLID") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Görüntüle" ItemStyle-Width="7%" ItemStyle-CssClass="col-actions" Visible="false">
                <ItemTemplate>
                    <asp:HyperLink ID="Hlink1" runat="server" CssClass="action-link" ToolTip="Kayıtları görüntüle"
                        NavigateUrl='<%# "~/DataView.aspx?tblAd="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLAD").ToString()) %>'>Görüntüle</asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Ekle" ItemStyle-Width="5%" ItemStyle-CssClass="col-actions" Visible="false">
                <ItemTemplate>
                    <asp:HyperLink ID="Hlink2" runat="server" CssClass="action-link" ToolTip="Yeni kayıt ekle"
                        NavigateUrl='<%# "~/DataEditNew.aspx?tblId="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLID").ToString())+"&tblAd="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLAD").ToString()) %>'>Ekle</asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Güncelle" ItemStyle-Width="7%" ItemStyle-CssClass="col-actions" Visible="false">
                <ItemTemplate>
                    <asp:HyperLink ID="Hlink3" runat="server" CssClass="action-link" ToolTip="Kayıt güncelle"
                        NavigateUrl='<%# "~/DataEditList.aspx?tblId="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLID").ToString())+"&tblAd="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLAD").ToString())+"&type="+HttpUtility.UrlEncode("edit") %>'>Güncelle</asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Sil" ItemStyle-Width="5%" ItemStyle-CssClass="col-actions" Visible="false">
                <ItemTemplate>
                    <asp:HyperLink ID="Hlink4" runat="server" CssClass="action-link action-danger" ToolTip="Kayıt sil"
                        NavigateUrl='<%# "~/DataEditList.aspx?tblId="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLID").ToString())+"&tblAd="+HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "TBLAD").ToString())+"&type="+HttpUtility.UrlEncode("delete") %>'>Sil</asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TBLAD" HeaderText="Veri Kümesi"></asp:BoundField>
            <asp:TemplateField HeaderText="Şifreli Mi" ItemStyle-Width="10%">
                <ItemTemplate>
                    <asp:Label ID="lbSifreliMi" runat="server" CssClass='<%# Convert.ToString(Eval("ISENCRYPT")) == "Evet" ? "badge badge-encrypted" : "badge badge-plain" %>' Text='<%# Eval("ISENCRYPT") %>'></asp:Label>
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
        </div>
    </div>
</asp:Content>
