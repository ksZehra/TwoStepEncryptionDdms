using System;
using System.Text;
using System.Web;
using TwoStepEncryptionDdms.Common;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class DataEditList : TwoStepEncryptionDdms.Security.BasePage
    {
        readonly RecordService records = new RecordService();
        string tblAd = "", tblId = "", parKey = "", type = "";
        int satirId = 0;

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

                string gelenType = Request.QueryString["type"];
                type = (gelenType == "edit" || gelenType == "delete" || gelenType == "delBtn") ? gelenType : "";

                int gelenSatirId;
                satirId = int.TryParse(Request.QueryString["satirId"], out gelenSatirId) ? gelenSatirId : 0;

                // Key URL'de taşınmaz; k=1 ise bu session'da verify edilip hatırlanan key kullanılır.
                if (Request.QueryString["k"] == "1" && !string.IsNullOrWhiteSpace(tblAd))
                {
                    parKey = TableKeyService.Recall(Context, tblAd) ?? "";
                }

                if (string.IsNullOrWhiteSpace(tblAd))
                {
                    throw new UserException("Boş parametre bulunmaktadır!");
                }

                EnsurePermission(type == "edit" ? Permission.Edit : Permission.Delete);

                if (!IsPostBack && !IsCallback)
                {
                    loadPageKeyControls();

                    if (type == "delBtn" && satirId > 0)
                    {
                        // State değiştiren GET isteği olduğu için session'a bağlı CSRF token zorunludur.
                        if (!AuthService.ValidateCsrfToken(Context, Request.QueryString["csrf"]))
                        {
                            throw new UserException("Geçersiz istek! Lütfen silme işlemini listeden tekrar başlatınız.");
                        }
                        dataDelete();
                        if (string.IsNullOrWhiteSpace(parKey))
                        {
                            listele();
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(parKey))
                    {
                        btnKey.Visible = false;
                        tbKey.Enabled = false;
                        divAlert.Visible = true;
                        tbKey.Text = parKey;
                        listele();
                    }
                }
            }
            catch (UserException ux)
            {
                ClsMessage.ShowMessage(ux.Message, this.Form);
            }
            catch (Exception)
            {
                ClsMessage.ShowMessage("Kayıt listelenirken hata oluştu!", this.Form);
            }
        }

        private void loadPageKeyControls()
        {
            records.EnsureRegistered(tblAd);

            if (!records.HasKey(tblAd))
            {
                divKey.Visible = false;
                listele();
            }
        }

        private void dataDelete()
        {
            EnsurePermission(Permission.Delete);

            records.DeleteRecord(tblAd, satirId);

            ClsMessage.ShowMessage("Kayıt silindi.", this.Form);
        }

        private void listele()
        {
            tabloBaslik.InnerText = tblAd;

            RecordList list = records.GetRecords(tblAd, divKey.Visible, tbKey.Text, null, null);

            ltList.Text = BuildTable(list);
            divContainer.Visible = true;
        }

        private string BuildTable(RecordList list)
        {
            var html = new StringBuilder();
            html.Append("<table style='border: 1px solid lightGray; margin-bottom:0px;' id= 'table-id' class='table table-striped table-class'>");
            html.Append("<thead><tr>");
            html.Append("<th>").Append(type == "edit" ? "GÜNCELLE" : "SİL").Append("</th>");

            foreach (RecordColumn column in list.Columns)
            {
                html.Append("<th>").Append(HttpUtility.HtmlEncode(column.Name)).Append("</th>");
            }
            html.Append("</tr></thead><tbody>");

            foreach (string[] cells in list.Rows)
            {
                html.Append("<tr>");

                string actionButton = "";
                string urlString = "tblId=" + HttpUtility.UrlEncode(tblId) + "&tblAd=" + HttpUtility.UrlEncode(tblAd) + "&satirId=" + HttpUtility.UrlEncode(cells[0]) + "&k=1";

                if (type == "edit")
                {
                    actionButton = "<Button type='button' style='border:transparent;' class='redirect' redirect='" + "DataEditNew.aspx?" + urlString + "'><img src='Themes/img/edit2.png' alt style='width:18px; height:18px;'/></Button>";
                }
                else if (type == "delete" || type == "delBtn")
                {
                    urlString += "&type=delBtn&csrf=" + AuthService.GetCsrfToken(Context);
                    actionButton = "<Button type='button' style='border:transparent;' class='redirect' redirect='" + "DataEditList.aspx?" + urlString + "'><img src='Themes/img/del8.png' alt style='width:18px; height:18px;'/></Button>";
                }

                html.Append("<td>").Append(actionButton).Append("</td>");

                for (int j = 0; j < cells.Length; j++)
                {
                    html.Append("<td").Append(list.Columns[j].IsEncrypted ? " style='background-color:#e6f0f4;'" : "").Append(">");
                    html.Append(HttpUtility.HtmlEncode(cells[j])).Append("</td>");
                }

                html.Append("</tr>");
            }

            html.Append("</tbody></table>");
            return html.ToString();
        }

        protected void btnKey_Click(object sender, EventArgs e)
        {
            try
            {
                if (records.VerifyKey(Context, tblAd, tbKey.Text))
                {
                    TableKeyService.Remember(Context, tblAd, tbKey.Text);
                    btnKey.Visible = false;
                    tbKey.Enabled = false;
                    divAlert.Visible = true;
                    listele();
                }
                else
                {
                    throw new UserException("Anahtar bilgisi yanlış! Lütfen tekrar deneyiniz.");
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
    }
}
