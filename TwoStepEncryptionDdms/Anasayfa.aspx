<%@ Page Title="Anasayfa" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Anasayfa.aspx.cs" Inherits="TwoStepEncryptionDdms.Anasayfa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <div class="page-heading">
        <div>
            <h1 class="page-title">Genel Bakış</h1>
            <p class="page-subtitle">Sistemdeki veri kümeleri ve iki adımlı şifreleme özeti.</p>
        </div>
        <div class="page-actions">
            <a class="btn btn-default <%= CanList ? "" : "hidden" %>" href="TabloSchemaList.aspx">Veri Kümeleri</a>
            <a class="btn btn-primary <%= CanCreate ? "" : "hidden" %>" href="TabloNew.aspx">Yeni Veri Kümesi</a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">

    <%--*************Yorum satırına alınan yerler AES algoritmasının farklı modlarda performans testi için kullanılmıştır!*************--%>
    <%--<div class="form-group row">
        <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 30%; padding-top: 6px; color: darkred; font-weight: bold !important; font: 13px Tahoma, Geneva, sans-serif bold !important;">SIFRESIZ_TABLO :</asp:Label>
        <div class="col-sm-10" style="width: 30%; padding-left: 0px; padding-top: 7.5px; font: 13px Tahoma, Geneva, sans-serif !important; color: darkred;">
            <asp:Label runat="server" ID="lbSifresiz"></asp:Label>
        </div>
    </div>
    <div class="form-group row">
        <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 30%; padding-top: 6px; color: darkred; font-weight: bold !important; font: 13px Tahoma, Geneva, sans-serif bold !important;">ECB_TEK_SIFRELEME :</asp:Label>
        <div class="col-sm-10" style="width: 30%; padding-left: 0px; padding-top: 7.5px; font: 13px Tahoma, Geneva, sans-serif !important; color: darkred;">
            <asp:Label runat="server" ID="lbEcbTek"></asp:Label>
        </div>
    </div>
    <div class="form-group row">
        <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 30%; padding-top: 6px; color: darkred; font-weight: bold !important; font: 13px Tahoma, Geneva, sans-serif bold !important;">CBC_TYPE1_TEK_SIFRELEME_NOHMAC :</asp:Label>
        <div class="col-sm-10" style="width: 30%; padding-left: 0px; padding-top: 7.5px; font: 13px Tahoma, Geneva, sans-serif !important; color: darkred;">
            <asp:Label runat="server" ID="lbCbcTek"></asp:Label>
        </div>
    </div>
    <div class="form-group row">
        <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 30%; padding-top: 6px; color: darkred; font-weight: bold !important; font: 13px Tahoma, Geneva, sans-serif bold !important;">CFB_TYPE4_TEK_SIFRELEME :</asp:Label>
        <div class="col-sm-10" style="width: 30%; padding-left: 0px; padding-top: 7.5px; font: 13px Tahoma, Geneva, sans-serif !important; color: darkred;">
            <asp:Label runat="server" ID="lbCfbTek"></asp:Label>
        </div>
    </div>
    <div class="form-group row">
        <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 30%; padding-top: 6px; color: darkred; font-weight: bold !important; font: 13px Tahoma, Geneva, sans-serif bold !important;">ECB_CIFT_SIFRELEME :</asp:Label>
        <div class="col-sm-10" style="width: 30%; padding-left: 0px; padding-top: 7.5px; font: 13px Tahoma, Geneva, sans-serif !important; color: darkred;">
            <asp:Label runat="server" ID="lbEcbCift"></asp:Label>
        </div>
    </div>

    <div class="form-group row">
        <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 30%; padding-top: 6px; color: darkred; font-weight: bold !important; font: 13px Tahoma, Geneva, sans-serif bold !important;">CBC_CIFT_SIFRELEME :</asp:Label>
        <div class="col-sm-10" style="width: 30%; padding-left: 0px; padding-top: 7.5px; font: 13px Tahoma, Geneva, sans-serif !important; color: darkred;">
            <asp:Label runat="server" ID="lbCbcCift"></asp:Label>
        </div>
    </div>
    <div class="form-group row">
        <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 30%; padding-top: 6px; color: darkred; font-weight: bold !important; font: 13px Tahoma, Geneva, sans-serif bold !important;">CFB_CIFT_SIFRELEME :</asp:Label>
        <div class="col-sm-10" style="width: 30%; padding-left: 0px; padding-top: 7.5px; font: 13px Tahoma, Geneva, sans-serif !important; color: darkred;">
            <asp:Label runat="server" ID="lbCfbCift"></asp:Label>
        </div>
    </div>
    <div class="form-group row">
        <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 30%; padding-top: 6px; color: darkred; font-weight: bold !important; font: 13px Tahoma, Geneva, sans-serif bold !important;">TEK_ADIMLI_DB_SIFRELEME :</asp:Label>
        <div class="col-sm-10" style="width: 30%; padding-left: 0px; padding-top: 7.5px; font: 13px Tahoma, Geneva, sans-serif !important; color: darkred;">
            <asp:Label runat="server" ID="lbTekAdimDb"></asp:Label>
        </div>
    </div>--%>

    <div class="notice notice-warning <%= DashboardAvailable ? "hidden" : "" %>">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <path d="M12 9v4"></path>
            <path d="M12 17h.01"></path>
            <path d="M10.3 4.2 2.6 18a2 2 0 0 0 1.7 3h15.4a2 2 0 0 0 1.7-3L13.7 4.2a2 2 0 0 0-3.4 0Z"></path>
        </svg>
        <div>Özet verileri şu anda yüklenemedi. Diğer işlemler etkilenmez.</div>
    </div>

    <div class="stat-grid">
        <div class="stat-card">
            <span class="stat-label">Veri Kümesi</span>
            <span class="stat-value"><%= DashboardAvailable ? DatasetCount.ToString("N0") : "—" %></span>
            <span class="stat-meta">Sisteme kayıtlı toplam veri kümesi</span>
        </div>
        <div class="stat-card">
            <span class="stat-label">Şifreli Veri Kümesi</span>
            <span class="stat-value"><%= DashboardAvailable ? EncryptedDatasetCount.ToString("N0") : "—" %></span>
            <span class="stat-meta">En az bir şifreli sütun içeren küme</span>
        </div>
        <div class="stat-card">
            <span class="stat-label">Toplam Kayıt</span>
            <span class="stat-value"><%= DashboardAvailable ? RecordCount.ToString("N0") : "—" %></span>
            <span class="stat-meta">Kayıtlı veri kümelerindeki satır sayısı</span>
        </div>
    </div>

    <div class="dashboard-grid">
        <div class="card">
            <div class="card-header">
                <h2 class="card-title">İki adımlı şifreleme akışı</h2>
                <span class="card-note">AES-256 + RSA-2048</span>
            </div>
            <div class="card-body">
                <div class="flow-list">
                    <div class="flow-step">
                        <span class="flow-number">01</span>
                        <h3 class="flow-title">AES ile şifrele</h3>
                        <p class="flow-text">Şifrelenecek her alan, 256 bit anahtarla AES kullanılarak şifrelenir. Varsayılan mod CFB'dir.</p>
                    </div>
                    <div class="flow-step">
                        <span class="flow-number">02</span>
                        <h3 class="flow-title">RSA ile ikinci katman</h3>
                        <p class="flow-text">AES çıktısı, SQL Server tarafında ENCRYPTBYASYMKEY ve RSA-2048 anahtarıyla yeniden şifrelenir.</p>
                    </div>
                    <div class="flow-step">
                        <span class="flow-number">03</span>
                        <h3 class="flow-title">Geri çöz</h3>
                        <p class="flow-text">Okuma sırasında önce DECRYPTBYASYMKEY, ardından AES çözme uygulanır. Anahtar olmadan veri okunamaz.</p>
                    </div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-header">
                <h2 class="card-title">Hızlı işlemler</h2>
            </div>
            <div class="card-body">
                <ul class="quick-list">
                    <li class="<%= CanCreate ? "" : "hidden" %>">
                        <a href="TabloNew.aspx">
                            Yeni veri kümesi oluştur
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m9 18 6-6-6-6"></path></svg>
                        </a>
                    </li>
                    <li class="<%= CanList ? "" : "hidden" %>">
                        <a href="TabloSchemaList.aspx">
                            Veri kümelerini yönet
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m9 18 6-6-6-6"></path></svg>
                        </a>
                    </li>
                    <li class="<%= CanList ? "" : "hidden" %>">
                        <a href="TabloDataList.aspx">
                            Kayıtları görüntüle
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m9 18 6-6-6-6"></path></svg>
                        </a>
                    </li>
                    <li>
                        <a href="Personel.aspx">
                            Hesap bilgilerim
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m9 18 6-6-6-6"></path></svg>
                        </a>
                    </li>
                </ul>

                <div class="system-chips" aria-label="Şifreleme bileşenleri">
                    <span class="system-chip">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m5 12 4 4L19 6"></path></svg>
                        AES-256
                    </span>
                    <span class="system-chip">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m5 12 4 4L19 6"></path></svg>
                        RSA-2048
                    </span>
                    <span class="system-chip">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m5 12 4 4L19 6"></path></svg>
                        SQL Server
                    </span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
