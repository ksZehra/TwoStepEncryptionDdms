using System;
using System.Text;
using System.Web;
using TwoStepEncryptionDdms.Common;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class DataView : TwoStepEncryptionDdms.Security.BasePage
    {
        readonly RecordService records = new RecordService();
        string tblAd = "";

        protected override Permission RequiredPermission
        {
            get { return Permission.View; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                tblAd = Request.QueryString["tblAd"];

                if (string.IsNullOrWhiteSpace(tblAd))
                {
                    throw new UserException("Boş parametre bulunmaktadır!");
                }

                if (!IsPostBack && !IsCallback)
                {
                    loadPageKeyControls();
                }
            }
            catch (UserException ux)
            {
                ClsMessage.ShowMessage(ux.Message, this.Form);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Kayıt listelenirken hata oluştu!", ex, this.Form);
            }
        }

        private void loadPageKeyControls()
        {
            try
            {
                records.EnsureRegistered(tblAd);
                spnDataset.InnerText = tblAd + " - ";

                if (!records.HasKey(tblAd))
                {
                    divKey.Visible = false;
                    listele();
                }
            }
            catch (ArgumentException)
            {
                throw new UserException("Veri kümesi bulunamadı!");
            }
        }

        private void listele(string parKolondi = "", string parAranacakKelime = "")
        {
            tabloBaslik.InnerText = tblAd;

            RecordList list = records.GetRecords(tblAd, divKey.Visible, tbKey.Text, parKolondi, parAranacakKelime);

            ltList.Text = BuildTable(list);
            divContainer.Visible = true;
        }

        private static string BuildTable(RecordList list)
        {
            var html = new StringBuilder();
            html.Append("<table id='table-id' class='data-table'>");
            html.Append(" <thead><tr>");

            foreach (RecordColumn column in list.Columns)
            {
                html.Append(column.IsEncrypted ? "<th class='col-encrypted' title='Şifrelenmiş sütun'>" : "<th>");
                html.Append(HttpUtility.HtmlEncode(column.Name)).Append("</th>");
            }
            html.Append("</tr></thead><tbody>");

            foreach (string[] cells in list.Rows)
            {
                html.Append("<tr>");
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

            html.Append(" </tbody></table>");
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
            catch (Exception ex)
            {
                ClsMessage.ShowError("Hata oluştu!", ex, this.Form);
            }
        }

        protected void btnKelimeAra_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbSutunAdi.Text) || string.IsNullOrWhiteSpace(tbKelime.Text))
                {
                    throw new UserException("Arama yapabilmek için \"Sütün Adı\" ve \"Aranacak Kelime\" alanlarının doldurulması zorunludur!");
                }

                listele(tbSutunAdi.Text.Trim(), tbKelime.Text.TrimEnd());
            }
            catch (UserException ux)
            {
                ClsMessage.ShowMessage(ux.Message, this.Form);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Arama sırasında hata oluştu!", ex, this.Form);
            }
        }

        protected void btnTemizle_Click(object sender, EventArgs e)
        {
            try
            {
                tbKelime.Text = "";
                tbSutunAdi.Text = "";
                listele();
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Hata oluştu!", ex, this.Form);
            }
        }
    }
}
