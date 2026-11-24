using System;
using TwoStepEncryptionDdms.Logging;
using System.Text;
using System.Web;
using System.Web.UI;
using TwoStepEncryptionDdms.Common;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class DataEditList : TwoStepEncryptionDdms.Security.BasePage, IPostBackEventHandler
    {
        readonly RecordService records = new RecordService();
        string tblAd = "", tblId = "", parKey = "", type = "";

        protected override Permission RequiredPermission
        {
            get { return Permission.List; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DatasetLink link = DatasetLink.Read(Request);
                tblAd = link.TblAd;
                int tblIdSayi;
                tblId = int.TryParse(link.TblId, out tblIdSayi) ? tblIdSayi.ToString() : "";
                type = (link.Type == "edit" || link.Type == "delete") ? link.Type : "";

                // Key URL'de taşınmaz; RecallKey ise bu session'da verify edilip hatırlanan key kullanılır.
                if (link.RecallKey && !string.IsNullOrWhiteSpace(tblAd))
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
                    var successMsg = Session["SuccessToast"] as string;
                    if (!string.IsNullOrWhiteSpace(successMsg))
                    {
                        ClsMessage.ShowSuccess(successMsg, this.Form);
                        Session.Remove("SuccessToast");
                    }

                    loadPageKeyControls();

                    if (!string.IsNullOrWhiteSpace(parKey))
                    {
                        btnKey.Visible = false;
                        tbKey.Enabled = false;
                        divAlert.Visible = true;
                        listele();
                    }
                }
            }
            catch (UserException ux)
            {
                ClsMessage.ShowError(ux.Message, this.Form);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Kayıt listelenirken hata oluştu!", ex, this.Form);
            }
        }

        private void loadPageKeyControls()
        {
            records.EnsureRegistered(tblAd);
            spnDataset.InnerText = tblAd + " - ";

            if (!records.HasKey(tblAd))
            {
                divKey.Visible = false;
                listele();
            }
        }

        private void dataDelete(int recordId)
        {
            records.DeleteRecord(tblAd, recordId);
            AppLog.Audit("RecordDeleted", tblAd + " #" + recordId);

            ClsMessage.ShowSuccess("Kayıt silindi.", this.Form);
        }

        private string GetVerifiedKey()
        {
            if (!divKey.Visible)
            {
                return "";
            }

            string key = TableKeyService.Recall(Context, tblAd);
            if (string.IsNullOrEmpty(key))
            {
                throw new UserException("Önce veri kümesi anahtarını doğrulayın!");
            }

            return key;
        }

        private void listele()
        {
            tabloBaslik.InnerText = tblAd;

            RecordList list = records.GetRecords(tblAd, divKey.Visible, GetVerifiedKey(), null, null);

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
                string editUrl = DatasetLink.Build("DataEditNew.aspx", tblId, tblAd, satirId: cells[0], recallKey: true);

                if (type == "edit")
                {
                    actionButton = "<button type='button' class='btn btn-xs btn-default action-icon icon-edit redirect' title='Düzenle' aria-label='Düzenle' redirect='" + HttpUtility.HtmlAttributeEncode(editUrl) + "'>Düzenle</button>";
                }
                else if (type == "delete")
                {
                    string deleteScript = "if (confirm('Bu kaydı silmek istediğinize emin misiniz?')) { " + ClientScript.GetPostBackEventReference(this, "delete:" + cells[0] + ":" + AuthService.GetCsrfToken(Context)) + "; }";
                    actionButton = "<button type='button' class='btn btn-xs btn-danger-outline action-icon icon-delete' title='Sil' aria-label='Sil' onclick=\"" + HttpUtility.HtmlAttributeEncode(deleteScript) + "\">Sil</button>";
                }

                html.Append("<td class='col-actions'>").Append(actionButton).Append("</td>");

                for (int j = 0; j < cells.Length; j++)
                {
                    html.Append(list.Columns[j].IsEncrypted ? "<td class='cell-encrypted'>" : "<td>");
                    html.Append(HttpUtility.HtmlEncode(cells[j])).Append("</td>");
                }

                html.Append("</tr>");
            }

            if (list.Rows.Count == 0)
            {
                html.Append("<tr class='empty-row'><td colspan='99'>Kayıt bulunamadı.</td></tr>");
            }

            html.Append("</tbody></table>");
            return html.ToString();
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            EnsurePermission(Permission.Delete);

            try
            {
                string[] parts = (eventArgument ?? "").Split(':');
                int recordId;
                if (parts.Length != 3
                    || parts[0] != "delete"
                    || !int.TryParse(parts[1], out recordId)
                    || recordId <= 0
                    || !AuthService.ValidateCsrfToken(Context, parts[2]))
                {
                    throw new UserException("Geçersiz istek! Lütfen silme işlemini listeden tekrar başlatınız.");
                }

                dataDelete(recordId);
                listele();
            }
            catch (UserException ux)
            {
                ClsMessage.ShowError(ux.Message, this.Form);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Kayıt silinirken hata oluştu!", ex, this.Form);
            }
        }

        protected void btnKey_Click(object sender, EventArgs e)
        {
            try
            {
                string enteredKey = tbKey.Text;
                tbKey.Text = "";

                if (records.VerifyKey(Context, tblAd, enteredKey))
                {
                    TableKeyService.Remember(Context, tblAd, enteredKey);
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
                ClsMessage.ShowError(ux.Message, this.Form);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Hata oluştu!", ex, this.Form);
            }
        }
    }
}
