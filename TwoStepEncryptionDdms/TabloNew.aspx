<%@ Page Title="Tablo Oluştur" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TabloNew.aspx.cs" Inherits="TwoStepEncryptionDdms.TabloNew" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <div class="page-heading">
        <div>
            <h1 class="page-title">Yeni Veri Kümesi</h1>
            <p class="page-subtitle">Sütunları tanımlayın ve şifreleme anahtarını belirleyin.</p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">

    <div class="notice notice-info">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <circle cx="12" cy="12" r="9"></circle>
            <path d="M12 11v5"></path>
            <path d="M12 8h.01"></path>
        </svg>
        <div>
            <strong>Adlandırma kuralları</strong>
            <ul class="notice-list">
                <li>Veri kümesi ve sütun adlarında yalnızca harf, rakam ve alt çizgi (_) kullanılabilir.</li>
                <li>Boşluklar otomatik olarak alt çizgiye dönüştürülür.</li>
                <li>Şifrelenecek sütun varsa 32 karakterlik anahtar zorunludur.</li>
            </ul>
        </div>
    </div>

    <div class="card">
        <div class="card-header">
            <h2 class="card-title">Veri kümesi bilgileri</h2>
        </div>
        <div class="card-body">
            <div class="form-grid">
                <div class="form-field">
                    <asp:Label runat="server" AssociatedControlID="txtTabloAdi" CssClass="field-label">Veri Kümesi Adı</asp:Label>
                    <asp:TextBox class="form-control" ID="txtTabloAdi" MaxLength="120" placeholder="Örn. PERSONEL" runat="server" ClientIDMode="Static" autocomplete="off"></asp:TextBox>
                </div>
                <div class="form-field">
                    <asp:Label runat="server" AssociatedControlID="txtKey" CssClass="field-label">Anahtar (Key)</asp:Label>
                    <asp:TextBox class="form-control" ID="txtKey" MaxLength="32" placeholder="32 karakterlik anahtar" runat="server" ClientIDMode="Static" autocomplete="off"></asp:TextBox>
                    <span class="field-hint">Yalnızca şifrelenecek sütun varsa kullanılır.</span>
                </div>
            </div>
        </div>
    </div>

    <div class="card">
        <div class="card-header">
            <h2 class="card-title">Sütunlar</h2>
            <span class="card-note">Önce sütunu ekleyin, sonra veri kümesini oluşturun.</span>
        </div>
        <div class="card-body">
            <div class="form-grid">
                <div class="form-field">
                    <asp:Label ID="lblKolonAdi" runat="server" Text="Sütun Adı" AssociatedControlID="txtKolonAdi" CssClass="field-label"></asp:Label>
                    <asp:TextBox class="form-control" ID="txtKolonAdi" MaxLength="50" placeholder="Örn. AD" runat="server" ClientIDMode="Static" autocomplete="off"></asp:TextBox>
                </div>
                <div class="form-field">
                    <label class="checkbox-inline" for="cbSifrele">
                        <asp:CheckBox ID="cbSifrele" runat="server" ClientIDMode="Static" />
                        Şifrelensin mi?
                    </label>
                    <span class="field-hint">Şifreli sütunlar AES-256 ve RSA-2048 ile korunur.</span>
                </div>
            </div>

            <asp:Button ID="btnEkle" CssClass="btn btn-primary column-add-action" runat="server" Text="Sütun Ekle" ClientIDMode="Static" OnClick="btnEkle_Click" />

            <div class="table-responsive" style="margin-top: 16px;">
                <asp:ListView ID="listVwTblOlustur" runat="server" ItemPlaceholderID="iphListView"
                    OnItemDeleting="listVwTblOlustur_ItemDeleting"
                    OnItemEditing="listVwTblOlustur_ItemEditing" OnItemUpdating="listVwTblOlustur_ItemUpdating" OnItemCanceling="listVwTblOlustur_ItemCanceling">

                    <LayoutTemplate>
                        <table class="data-table">
                            <thead>
                                <tr>
                                    <th>Sütun Adı</th>
                                    <th>Şifrelensin Mi</th>
                                    <th class="col-actions">Sil</th>
                                    <th class="col-actions">Düzenle</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:PlaceHolder runat="server" ID="iphListView"></asp:PlaceHolder>
                            </tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:Label ID="lblKolonAdi" Text='<%# Eval("KolonAdi") %>' runat="server" />
                            </td>
                            <td>
                                <asp:Label ID="lbIsEncrypted" Text='<%# Eval("IsEncrypted") %>' runat="server" CssClass='<%# Convert.ToString(Eval("IsEncrypted")) == "Evet" ? "badge badge-encrypted" : "badge badge-plain" %>' />
                            </td>
                            <td class="col-actions">
                                <asp:Button ID="btnSil" CssClass="btn btn-xs btn-danger-outline action-icon-input icon-delete" ToolTip="Sil" runat="server" Text="Sil" CommandName="Delete" />
                            </td>
                            <td class="col-actions">
                                <asp:Button ID="btnDüzenle" CssClass="btn btn-xs btn-default action-icon-input icon-edit" ToolTip="Düzenle" runat="server" Text="Düzenle" CommandName="Edit" />
                            </td>
                        </tr>
                    </ItemTemplate>

                    <EditItemTemplate>
                        <tr class="is-editing">
                            <td>
                                <asp:TextBox ID="txtEmpKolonAdi" runat="server" MaxLength="200" CssClass="form-control input-sm" Text='<%# Bind("KolonAdi") %>' />
                            </td>
                            <td>
                                <asp:Label ID="lbIsEncrypted" runat="server" Text='<%# Bind("IsEncrypted") %>'></asp:Label>
                            </td>
                            <td class="col-actions">
                                <asp:Button ID="btnGuncelle" CssClass="btn btn-xs btn-primary action-icon-input icon-save" ToolTip="Kaydet" runat="server" Text="Kaydet" CommandName="Update" />
                            </td>
                            <td class="col-actions">
                                <asp:Button ID="btnIptal" CssClass="btn btn-xs btn-default action-icon-input icon-cancel" ToolTip="Vazgeç" runat="server" Text="Vazgeç" CommandName="Cancel" />
                            </td>
                        </tr>
                    </EditItemTemplate>
                </asp:ListView>
            </div>

            <div class="form-actions" id="divButtons" runat="server" style="display: none;">
                <asp:Button ID="btnTemizle" runat="server" Text="Temizle" CssClass="btn btn-default" OnClick="btnTemizle_Click" />
                <asp:Button ID="btnTblOlustur" runat="server" Text="Oluştur" CssClass="btn btn-primary" ClientIDMode="Static" OnClick="btnTblOlustur_Click" />
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
