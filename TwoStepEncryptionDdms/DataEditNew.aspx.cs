using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using TwoStepEncryptionDdms.Common;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class DataEditNew : TwoStepEncryptionDdms.Security.BasePage
    {
        readonly RecordService records = new RecordService();
        string tblAd = "", tblId = "", satirId = "", parKey = "";

        protected override Permission RequiredPermission
        {
            get { return Permission.List; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                tblAd = Request.QueryString["tblAd"];
                int tblIdSayi;
                tblId = int.TryParse(Request.QueryString["tblId"], out tblIdSayi) ? tblIdSayi.ToString() : "";

                if (Request.QueryString["satirId"] != null)
                {
                    int gelenSatirId;
                    satirId = int.TryParse(Request.QueryString["satirId"], out gelenSatirId) ? gelenSatirId.ToString() : "";
                    btnKayitGir.Text = "Güncelle";

                    if (!string.IsNullOrWhiteSpace(tblAd))
                    {
                        parKey = TableKeyService.Recall(Context, tblAd) ?? "";
                    }
                }

                EnsurePermission(string.IsNullOrWhiteSpace(satirId) ? Permission.New : Permission.Edit);

                if (string.IsNullOrWhiteSpace(tblAd) || string.IsNullOrWhiteSpace(tblId))
                {
                    btnKayitGir.Visible = false;
                    btnKey.Visible = false;
                    throw new UserException("Boş parametre bulunmaktadır!");
                }

                if (!IsPostBack && !IsCallback)
                {
                    if (string.IsNullOrWhiteSpace(satirId))
                    {
                        loadPageKeyControls();
                    }
                    else if (string.IsNullOrWhiteSpace(parKey) && records.HasKey(tblAd))
                    {
                        btnKayitGir.Visible = false;
                        throw new UserException("Anahtar bilgisi bu oturumda bulunamadı. Lütfen kayıt listesine dönüp anahtarı tekrar giriniz.");
                    }
                    getTableColums();
                }

                if (!String.IsNullOrWhiteSpace(satirId))
                {
                    divKey.Visible = false;
                    divTablo.Visible = true;
                    divAlert.Visible = string.IsNullOrWhiteSpace(parKey) ? false : true;
                }
            }
            catch (UserException ux)
            {
                ClsMessage.ShowMessage(ux.Message, this.Form);
            }
            catch (Exception)
            {
                ClsMessage.ShowMessage("Beklenmeyen bir hata oluştu!", this.Form);
            }
        }

        private void loadPageKeyControls()
        {
            records.EnsureRegistered(tblAd);

            if (!records.HasKey(tblAd))
            {
                divTablo.Visible = true;
                divKey.Visible = false;
                divAlert.Visible = false;
            }
        }

        private void getTableColums()
        {
            IList<EditField> fields = records.GetEditFields(tblAd, satirId, parKey);

            tabloBaslik.InnerText = tblAd;

            bool editing = !string.IsNullOrWhiteSpace(satirId);
            var html = new StringBuilder();
            html.Append("<table style='font-family: Lato, sans-serif;'>");
            html.Append("<tr>");
            html.Append("</tr>");
            html.Append("<tr>");
            foreach (EditField field in fields)
            {
                html.Append("<td style='padding-right:10px;'>");
                html.Append(@"<div class=""form-group""> <label class=""form-label"" style='height:30px; padding-top:5px; font-size:14px; ");
                html.Append(field.IsEncrypted ? " color:#3a87ad;' " : "' ");
                html.Append(@" ' for=""simpleinput"">").Append(HttpUtility.HtmlEncode(field.Name)).Append(@"</label> </div>");
                html.Append(" </td>");

                html.Append("<td style='padding-right:10px;'>");
                html.Append(@"<div class=""form-group"">");
                html.Append(@"<input type='text' maxlength='200' style='height:30px; width:600px; font-size:14px;' id=""").Append(HttpUtility.HtmlAttributeEncode(field.Name)).Append(@""" ");
                if (editing)
                {
                    html.Append(@" value=""").Append(HttpUtility.HtmlAttributeEncode(field.Value)).Append(@""" ");
                }
                html.Append(@" class=""form-control cst-lbl"" runat=""server"" />
                                     </div>");
                html.Append("</td>");

                html.Append("</tr>");
            }
            html.Append("</table>");

            ltList.Text = html.ToString();
        }

        protected void btnKey_Click(object sender, EventArgs e)
        {
            try
            {
                if (records.VerifyKey(Context, tblAd, tbKey.Text))
                {
                    TableKeyService.Remember(Context, tblAd, tbKey.Text);
                    divTablo.Visible = true;
                    btnKey.Visible = false;
                    tbKey.Enabled = false;
                }
                else
                {
                    throw new UserException("Anahtar bilgisi yanlış! Lütfen tekrar deneyiniz!");
                }
            }
            catch (UserException ux)
            {
                ClsMessage.ShowMessage(ux.Message, this.Form);
            }
            catch (Exception)
            {
                ClsMessage.ShowMessage("Hata oluştu!", this.Form);
            }
        }

        protected void btnKayitGir_Click(object sender, EventArgs e)
        {
            bool hataVar = false;
            try
            {
                EnsurePermission(string.IsNullOrWhiteSpace(satirId) ? Permission.New : Permission.Edit);

                string aesKey = string.IsNullOrWhiteSpace(satirId) ? tbKey.Text : parKey;
                records.SaveRecord(Context, tblAd, satirId, aesKey, ParsePostedValues(hfSaveData.Value));
            }
            catch (UserException ux)
            {
                hataVar = true;
                ClsMessage.ShowMessage(ux.Message, this.Form);
            }
            catch (Exception)
            {
                hataVar = true;
                ClsMessage.ShowMessage("Kayıt kaydedilirken hata oluştu!", this.Form);
            }
            finally
            {
                if (!hataVar)
                {
                    ClsMessage.ShowMessage(!string.IsNullOrWhiteSpace(satirId) ? "Kayıt güncellendi." : "Kayıt eklendi.", this.Form);

                    if (!string.IsNullOrWhiteSpace(satirId))
                    {
                        Response.Redirect("DataEditList.aspx?tblId=" + HttpUtility.UrlEncode(tblId) + "&tblAd=" + HttpUtility.UrlEncode(tblAd) + "&type=edit&k=1");
                    }
                }
            }
        }

        // UI "column#value|column#value" biçiminde gönderir; value içindeki '#' korunur.
        private static IList<KeyValuePair<string, string>> ParsePostedValues(string posted)
        {
            var values = new List<KeyValuePair<string, string>>();
            foreach (string group in posted.TrimEnd('|').Split('|'))
            {
                string[] parts = group.Split(new[] { '#' }, 2);
                values.Add(new KeyValuePair<string, string>(parts[0], parts.Length > 1 ? parts[1] : ""));
            }
            return values;
        }
    }
}
