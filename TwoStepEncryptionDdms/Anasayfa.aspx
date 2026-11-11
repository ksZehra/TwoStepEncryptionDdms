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
</asp:Content>
