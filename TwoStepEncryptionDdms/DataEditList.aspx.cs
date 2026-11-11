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
            html.Append("<table id='table-id' class='data-table'>");
            html.Append("<thead><tr>");
            html.Append("<th class='col-actions'>").Append(type == "edit" ? "Düzenle" : "Sil").Append("</th>");

            foreach (RecordColumn column in list.Columns)
            {
                html.Append(column.IsEncrypted ? "<th class='col-encrypted' title='Şifrelenmiş sütun'>" : "<th>");
                html.Append(HttpUtility.HtmlEncode(column.Name)).Append("</th>");
            }
            html.Append("</tr></thead><tbody>");

            foreach (string[] cells in list.Rows)
            {
                html.Append("<tr>");

                string actionButton = "";
                string urlString = "tblId=" + HttpUtility.UrlEncode(tblId) + "&tblAd=" + HttpUtility.UrlEncode(tblAd) + "&satirId=" + HttpUtility.UrlEncode(cells[0]) + "&k=1";

                if (type == "edit")
                {
                    actionButton = "<button type='button' class='btn btn-xs btn-default redirect' redirect='" + HttpUtility.HtmlAttributeEncode("DataEditNew.aspx?" + urlString) + "'>Düzenle</button>";
                }
                else if (type == "delete" || type == "delBtn")
                {
                    urlString += "&type=delBtn&csrf=" + AuthService.GetCsrfToken(Context);
                    actionButton = "<button type='button' class='btn btn-xs btn-danger-outline redirect' redirect='" + HttpUtility.HtmlAttributeEncode("DataEditList.aspx?" + urlString) + "'>Sil</button>";
                }

                html.Append("<td class='col-actions'>").Append(actionButton).Append("</td>");

                for (int j = 0; j < cells.Length; j++)
                {
                    html.Append(list.Columns[j].IsEncrypted ? "<td class='cell-encrypted'>" : "<td>");
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
