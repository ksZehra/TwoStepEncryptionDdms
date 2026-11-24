<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Register Src="~/Controls/CreditFooter.ascx" TagPrefix="uc" TagName="CreditFooter" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        // customErrors ResponseRewrite ile gelindiğinde de doğru status code dönsün.
        Response.StatusCode = 500;
        Response.TrySkipIisCustomErrors = true;
    }
</script>

<!DOCTYPE html>
<html lang="tr">
<head runat="server">
    <title>Bir hata oluştu</title>
    <link rel="icon" type="image/svg+xml" href="data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 32 32'%3E%3Crect width='32' height='32' rx='7' fill='%232563eb'/%3E%3Crect x='8' y='15' width='16' height='11' rx='2' fill='white'/%3E%3Cpath d='M12 15v-3a4 4 0 0 1 8 0v3' fill='none' stroke='white' stroke-width='2'/%3E%3C/svg%3E" />
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="robots" content="noindex">

    <asp:PlaceHolder runat="server">
        <link href="<%= ResolveUrl("~/Content/bootstrap.min.css") %>" rel="stylesheet" />
        <link href="<%= TwoStepEncryptionDdms.Common.AssetVersion.Url("~/Content/Site.css") %>" rel="stylesheet" />
    </asp:PlaceHolder>
</head>
<body class="auth-body">
    <div class="auth-shell">
        <div class="auth-brand auth-brand-center">
            <span class="brand-mark" aria-hidden="true">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
                    <rect x="4" y="10" width="16" height="10" rx="2"></rect>
                    <path d="M8 10V7a4 4 0 0 1 8 0v3"></path>
                </svg>
            </span>
            <div>
                <div class="auth-brand-name">Şifreli Dinamik Veri Kayıt Sistemi</div>
                <div class="auth-brand-note">İki adımlı şifreleme</div>
            </div>
        </div>

        <div class="auth-card error-card">
            <div class="error-code error-code-danger">500</div>
            <h1 class="auth-title">Beklenmeyen bir hata oluştu</h1>
            <p class="auth-subtitle">İsteğiniz işlenirken bir sorun çıktı. Birkaç dakika sonra tekrar deneyin; sorun sürerse sistem yöneticinize haber verin.</p>
            <div class="auth-actions">
                <a class="btn btn-primary btn-block" href="<%= ResolveUrl("~/Dashboard") %>">Ana sayfaya dön</a>
            </div>
        </div>
    </div>

    <uc:CreditFooter runat="server" />
</body>
</html>
