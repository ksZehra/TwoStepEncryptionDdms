<%@ Page Title="Kayıtlar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Record.aspx.cs" Inherits="TwoStepEncryptionDdms.Record" %>

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
            <h2 class="card-title">Veri Kümeleri</h2>
            <span class="card-note">Sayfa boyutu: 10</span>
        </div>
        <div class="table-responsive">
            <asp:GridView ID="GridTbl" runat="server" AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True" OnSorting="GridTbl_Sorting" OnRowDataBound="GridTbl_RowDataBound" PageSize="10"
                DataKeyNames="TBLAD" OnPageIndexChanging="GridTbl_PageIndexChanging"
                CssClass="data-table" GridLines="None" EmptyDataText="Kayıt bulunamadı."
                PagerSettings-Position="Bottom" PagerStyle-CssClass="grid-pager">
                <Columns>
                    <asp:TemplateField HeaderText="ID" SortExpression="TBLID" ItemStyle-CssClass="col-no" HeaderStyle-CssClass="col-no">
                        <ItemTemplate>
                            <asp:Label ID="lbTblId" runat="server" Text='<%# Eval("TBLID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="İşlem Yap" ItemStyle-CssClass="col-actions" HeaderStyle-CssClass="col-actions" Visible="false">
                        <ItemTemplate>
                            <asp:HyperLink ID="Hlink1" runat="server" CssClass="btn btn-xs btn-default action-icon icon-open" ToolTip="Kayıtları görüntüle ve düzenle"
                                NavigateUrl='<%# TwoStepEncryptionDdms.Security.DatasetLink.Build("~/DataView.aspx", DataBinder.Eval(Container.DataItem, "TBLID"), DataBinder.Eval(Container.DataItem, "TBLAD")) %>'>Aç</asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Kayıt Ekle" ItemStyle-CssClass="col-actions" HeaderStyle-CssClass="col-actions" Visible="false">
                        <ItemTemplate>
                            <asp:HyperLink ID="Hlink2" runat="server" CssClass="btn btn-xs btn-default action-icon icon-add" ToolTip="Yeni kayıt ekle"
                                NavigateUrl='<%# TwoStepEncryptionDdms.Security.DatasetLink.Build("~/DataView.aspx", DataBinder.Eval(Container.DataItem, "TBLID"), DataBinder.Eval(Container.DataItem, "TBLAD"), "new") %>'>Ekle</asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TBLAD" HeaderText="Veri Kümesi" SortExpression="TBLAD"></asp:BoundField>
                    <asp:TemplateField HeaderText="Şifreli mi" SortExpression="ISENCRYPT" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lbSifreliMi" runat="server" CssClass='<%# Convert.ToString(Eval("ISENCRYPT")) == "Evet" ? "badge badge-encrypted" : "badge badge-plain" %>' Text='<%# Eval("ISENCRYPT") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Oluşturan" SortExpression="OLUSTURAN" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lbOlusturan" runat="server" Text='<%# Eval("OLUSTURAN") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Oluşturma Tarihi" SortExpression="OLUSTURMATARIHI_SIRA" ItemStyle-Width="10%">
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
