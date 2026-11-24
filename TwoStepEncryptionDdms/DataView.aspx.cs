using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using TwoStepEncryptionDdms.Common;
using TwoStepEncryptionDdms.Logging;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class DataView : TwoStepEncryptionDdms.Security.BasePage, IPostBackEventHandler
    {
        readonly RecordService records = new RecordService();
        string tblAd = "";
        string intent = "";

        protected override Permission RequiredPermission
        {
            get { return Permission.List; }
        }

        bool CanView { get { return CurrentUser.Has(Permission.View); } }
        bool CanNew { get { return CurrentUser.Has(Permission.New); } }
        bool CanEdit { get { return CurrentUser.Has(Permission.Edit); } }
        bool CanDelete { get { return CurrentUser.Has(Permission.Delete); } }
        bool CanSeeRecords { get { return CanView || CanEdit || CanDelete; } }

        bool TableHasKey
        {
            get { return ViewState["TableHasKey"] as bool? ?? false; }
            set { ViewState["TableHasKey"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DatasetLink link = DatasetLink.Read(Request);
                tblAd = link.TblAd;
                intent = link.Type;

                if (string.IsNullOrWhiteSpace(tblAd))
                {
                    throw new UserException("Veri kümesi bulunamadı.");
                }

                if (!CanSeeRecords && !CanNew)
                {
                    EnsurePermission(Permission.View);
                }

                pageTitle.InnerText = tblAd;
                Title = tblAd + " - Kayıtlar";

                if (!IsPostBack && !IsCallback)
                {
                    LoadWorkspace();
                }
            }
            catch (UserException ux)
            {
                ClsMessage.ShowError(ux.Message, this.Form);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Kayıtlar yüklenirken hata oluştu.", ex, this.Form);
            }
        }

        private void LoadWorkspace()
        {
            try
            {
                records.EnsureRegistered(tblAd);
            }
            catch (ArgumentException)
            {
                throw new UserException("Veri kümesi bulunamadı.");
            }

            TableHasKey = records.HasKey(tblAd);

            if (!TableHasKey)
            {
                divKey.Visible = false;
                Unlock();
            }
            else
            {
                // Sayfa her açıldığında key yeniden istenir; önceki girişten kalan key session'dan silinir.
                TableKeyService.Forget(Context, tblAd);
            }
        }

        // Key bu session'da doğrulandıktan sonra form ve liste açılır; aynı veri kümesi için tekrar sorulmaz.
        private void Unlock()
        {
            btnKey.Visible = false;
            tbKey.Enabled = false;

            if (CanNew || CanEdit)
            {
                BuildForm();
                divForm.Visible = true;
            }

            btnNewRecord.Visible = CanNew;

            if (CanSeeRecords)
            {
                BindRecords();
            }

            if (intent == "new" && CanNew && !IsPostBack)
            {
                hfFormMode.Value = "new";
            }
        }

        private string GetVerifiedKey()
        {
            if (!TableHasKey)
            {
                return "";
            }

            string key = TableKeyService.Recall(Context, tblAd);
            if (string.IsNullOrEmpty(key))
            {
                throw new UserException("Önce veri kümesi anahtarını doğrulayın.");
            }

            return key;
        }

        private void BuildForm()
        {
            IList<EditField> fields = records.GetEditFields(tblAd, "", "");
            int encryptedLimit = RecordService.GetEncryptedInputLimit();

            var html = new StringBuilder();
            html.Append("<div class=\"field-grid\">");

            for (int i = 0; i < fields.Count; i++)
            {
                EditField field = fields[i];
                string inputId = "fld" + i;

                html.Append("<div class=\"field\">");
                html.Append("<label class=\"field-label\" for=\"").Append(inputId).Append("\">");
                html.Append(HttpUtility.HtmlEncode(field.Name));
                if (field.IsEncrypted)
                {
                    html.Append(" <span class=\"badge badge-encrypted\">Şifreli</span>");
                }
                html.Append("</label>");

                html.Append("<input type=\"text\" maxlength=\"200\" autocomplete=\"off\" class=\"form-control record-input\" id=\"").Append(inputId)
                    .Append("\" data-col=\"").Append(HttpUtility.HtmlAttributeEncode(field.Name)).Append("\"");
                if (field.IsEncrypted && encryptedLimit > 0)
                {
                    html.Append(" data-maxbytes=\"").Append(encryptedLimit).Append("\"");
                }
                html.Append(" /></div>");
            }

            html.Append("</div>");
            ltFields.Text = html.ToString();
        }

        private void BindRecords(string searchColumn = "", string searchTerm = "")
        {
            tabloBaslik.InnerText = tblAd;

            RecordList list = records.GetRecords(tblAd, TableHasKey, GetVerifiedKey(), searchColumn, searchTerm);

            ltList.Text = BuildTable(list);
            divContainer.Visible = true;
        }

        private string BuildTable(RecordList list)
        {
            string csrf = CanDelete ? AuthService.GetCsrfToken(Context) : "";

            var html = new StringBuilder();
            // İlk column KAYITID'dir; işlem column'larından önce gösterilir.
            html.Append("<table id='table-id' class='data-table'><thead><tr><th class='col-no'>")
                .Append(HttpUtility.HtmlEncode(list.Columns[0].Name)).Append("</th>");
            if (CanEdit)
            {
                html.Append("<th class='col-actions'>Güncelle</th>");
            }
            if (CanDelete)
            {
                html.Append("<th class='col-actions'>Sil</th>");
            }

            for (int c = 1; c < list.Columns.Count; c++)
            {
                RecordColumn column = list.Columns[c];
                html.Append(column.IsEncrypted ? "<th class='col-encrypted' title='Şifrelenmiş sütun'>" : "<th>");
                html.Append(HttpUtility.HtmlEncode(column.Name)).Append("</th>");
            }
            html.Append("</tr></thead><tbody>");

            foreach (string[] cells in list.Rows)
            {
                html.Append("<tr data-id='").Append(HttpUtility.HtmlAttributeEncode(cells[0])).Append("'>");
                html.Append("<td class='col-no'>").Append(HttpUtility.HtmlEncode(cells[0])).Append("</td>");

                if (CanEdit)
                {
                    html.Append("<td class='col-actions'><button type='button' class='btn btn-xs btn-default action-icon icon-edit js-edit' title='Güncelle' aria-label='Güncelle'>Güncelle</button></td>");
                }
                if (CanDelete)
                {
                    string deleteScript = "if (confirm('Bu kaydı silmek istediğinize emin misiniz?')) { " + ClientScript.GetPostBackEventReference(this, "delete:" + cells[0] + ":" + csrf) + "; }";
                    html.Append("<td class='col-actions'><button type='button' class='btn btn-xs btn-danger-outline action-icon icon-delete' title='Sil' aria-label='Sil' onclick=\"")
                        .Append(HttpUtility.HtmlAttributeEncode(deleteScript)).Append("\">Sil</button></td>");
                }

                for (int j = 1; j < cells.Length; j++)
                {
                    html.Append(list.Columns[j].IsEncrypted ? "<td class='cell-encrypted'" : "<td");
                    html.Append(" data-col='").Append(HttpUtility.HtmlAttributeEncode(list.Columns[j].Name)).Append("'>");
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

        protected void btnKey_Click(object sender, EventArgs e)
        {
            try
            {
                string enteredKey = tbKey.Text;
                tbKey.Text = "";

                if (!records.VerifyKey(Context, tblAd, enteredKey))
                {
                    throw new UserException("Anahtar bilgisi yanlış. Lütfen tekrar deneyin.");
                }

                TableKeyService.Remember(Context, tblAd, enteredKey);
                Unlock();
            }
            catch (UserException ux)
            {
                ClsMessage.ShowError(ux.Message, this.Form);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Anahtar doğrulanırken hata oluştu.", ex, this.Form);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string recordId = "";
                if (hfFormMode.Value == "edit")
                {
                    int id;
                    if (!int.TryParse(hfRecordId.Value, out id) || id <= 0)
                    {
                        throw new UserException("Güncellenecek kayıt bulunamadı.");
                    }
                    recordId = id.ToString();
                }

                EnsurePermission(recordId == "" ? Permission.New : Permission.Edit);

                records.SaveRecord(Context, tblAd, recordId, GetVerifiedKey(), ParsePostedValues(hfSaveData.Value));
                AppLog.Audit(recordId == "" ? "RecordAdded" : "RecordUpdated", tblAd + (recordId == "" ? "" : " #" + recordId));

                // Yeni kayıtta form açık ve boş kalır ki art arda kayıt girilebilsin.
                hfSaveData.Value = "";
                if (recordId != "")
                {
                    hfFormMode.Value = "";
                    hfRecordId.Value = "";
                }

                if (CanSeeRecords)
                {
                    BindRecords();
                }

                ClsMessage.ShowSuccess(recordId == "" ? "Kayıt eklendi." : "Kayıt güncellendi.", this.Form);
            }
            catch (UserException ux)
            {
                ClsMessage.ShowError(ux.Message, this.Form);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Kayıt kaydedilirken hata oluştu.", ex, this.Form);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                EnsurePermission(Permission.Delete);

                string[] parts = (eventArgument ?? "").Split(':');
                int recordId;
                if (parts.Length != 3
                    || parts[0] != "delete"
                    || !int.TryParse(parts[1], out recordId)
                    || recordId <= 0
                    || !AuthService.ValidateCsrfToken(Context, parts[2]))
                {
                    throw new UserException("Geçersiz istek. Lütfen sayfayı yenileyip tekrar deneyin.");
                }

                records.DeleteRecord(tblAd, recordId);
                AppLog.Audit("RecordDeleted", tblAd + " #" + recordId);

                hfFormMode.Value = "";
                hfRecordId.Value = "";
                BindRecords();
                ClsMessage.ShowSuccess("Kayıt silindi.", this.Form);
            }
            catch (UserException ux)
            {
                ClsMessage.ShowError(ux.Message, this.Form);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Kayıt silinirken hata oluştu.", ex, this.Form);
            }
        }

        protected void btnKelimeAra_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbSutunAdi.Text) || string.IsNullOrWhiteSpace(tbKelime.Text))
                {
                    throw new UserException("Arama için sütun adı ve aranacak kelime girilmelidir.");
                }

                BindRecords(tbSutunAdi.Text.Trim(), tbKelime.Text.TrimEnd());
            }
            catch (UserException ux)
            {
                ClsMessage.ShowWarning(ux.Message, this.Form);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Arama sırasında hata oluştu.", ex, this.Form);
            }
        }

        protected void btnTemizle_Click(object sender, EventArgs e)
        {
            try
            {
                tbKelime.Text = "";
                tbSutunAdi.Text = "";
                BindRecords();
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Hata oluştu.", ex, this.Form);
            }
        }

        // UI "column#value|column#value" biçiminde gönderir; her parça URL-encode edilir, böylece value içindeki '#' ve '|' korunur.
        private static IList<KeyValuePair<string, string>> ParsePostedValues(string posted)
        {
            var values = new List<KeyValuePair<string, string>>();
            foreach (string group in (posted ?? "").TrimEnd('|').Split('|'))
            {
                if (group.Length == 0)
                {
                    continue;
                }
                string[] parts = group.Split(new[] { '#' }, 2);
                values.Add(new KeyValuePair<string, string>(Uri.UnescapeDataString(parts[0]), parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : ""));
            }
            return values;
        }
    }
}
