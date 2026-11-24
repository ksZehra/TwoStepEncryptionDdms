<%@ Page Title="Veri Kümeleri" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TableSchemaList.aspx.cs" Inherits="TwoStepEncryptionDdms.TableSchemaList" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <div class="page-heading">
        <div>
            <h1 class="page-title">Veri Kümeleri</h1>
            <p class="page-subtitle">Kayıtlı veri kümelerini görüntüleyin, yeniden adlandırın veya silin.</p>
        </div>
        <div class="page-actions">
            <div class="search-box">
                <asp:TextBox ID="txtAra" CssClass="form-control" runat="server" MaxLength="200" placeholder="Veri kümesi adı ara..."></asp:TextBox>
                <asp:Button ID="btnAra" CssClass="btn btn-default" runat="server" Text="Ara" OnClick="btnAra_Click" />
            </div>
            <a id="lnkYeniVeriKumesi" runat="server" class="btn btn-primary" href="TableNew.aspx">
                <svg viewBox="0 0 24 24" width="14" height="14" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" aria-hidden="true">
                    <line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
                Yeni Veri Kümesi Oluştur
            </a>
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
                OnRowDeleting="GridTbl_RowDeleting" DataKeyNames="TBLAD" OnRowEditing="GridTbl_RowEditing" OnRowUpdating="GridTbl_RowUpdating"
                OnRowCancelingEdit="GridTbl_RowCancelingEdit" OnPageIndexChanging="GridTbl_PageIndexChanging" CssClass="data-table"
                GridLines="None" EmptyDataText="Kayıt bulunamadı." PagerSettings-Position="Bottom" PagerStyle-CssClass="grid-pager">
                <Columns>
                    <asp:TemplateField HeaderText="ID" SortExpression="TBLID" ItemStyle-CssClass="col-no" HeaderStyle-CssClass="col-no">
                        <ItemTemplate>
                            <asp:Label ID="lbTblId" runat="server" Text='<%# Eval("TBLID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowDeleteButton="true" ButtonType="Link" ItemStyle-CssClass="col-actions" HeaderStyle-CssClass="col-actions" HeaderText="Kümeyi Sil" DeleteText="Veri kümesini ve tüm kayıtlarını sil" Visible="false" ControlStyle-CssClass="btn btn-xs btn-default action-icon icon-delete" />
                    <asp:CommandField ShowEditButton="true" ButtonType="Link" ItemStyle-CssClass="col-actions" HeaderStyle-CssClass="col-actions" HeaderText="Yeniden Adlandır" EditText="Veri kümesinin adını değiştir" UpdateText="Kaydet" CancelText="Vazgeç" Visible="false" ControlStyle-CssClass="btn btn-xs btn-default action-icon icon-edit" />
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
                    <asp:TemplateField HeaderText="Güncelleyen" SortExpression="GUNCELLEYEN" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lbGuncelleyen" runat="server" Text='<%# Eval("GUNCELLEYEN") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Güncelleme Tarihi" SortExpression="GUNCELLEMETARIHI_SIRA" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lbGuncellemeTar" runat="server" Text='<%# Eval("GUNCELLEMETARIHI") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerSettings Mode="NumericFirstLast" FirstPageText="İlk" LastPageText="Son" />
            </asp:GridView>
        </div>
    </div>

</asp:Content>
