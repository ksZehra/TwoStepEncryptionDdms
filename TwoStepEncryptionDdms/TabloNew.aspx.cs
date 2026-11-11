using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using TwoStepEncryptionDdms.Common;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class TabloNew : TwoStepEncryptionDdms.Security.BasePage
    {
        readonly DatasetService datasets = new DatasetService();

        protected override Permission RequiredPermission
        {
            get { return Permission.New; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    ViewState["Vs_Kolon"] = NewColumnTable();
                    this.BindGrid();
                }
            }
            catch (Exception)
            {
                ClsMessage.ShowMessage("Beklenmeyen bir hata oluştu!", this.Form);
            }
        }

        private static DataTable NewColumnTable()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[2] { new DataColumn("KolonAdi"), new DataColumn("IsEncrypted") });
            return dt;
        }

        private static bool GecerliAdMi(string ad)
        {
            return SqlIdentifier.IsValid(ad);
        }

        protected void BindGrid()
        {
            listVwTblOlustur.DataSource = (DataTable)ViewState["Vs_Kolon"];
            listVwTblOlustur.DataBind();
        }

        protected void btnEkle_Click(object sender, EventArgs e)
        {
            string kolonAdi = txtKolonAdi.Text.Trim().Replace(" ", "_");
            if (!GecerliAdMi(kolonAdi))
            {
                ClsMessage.ShowMessage("Kolon adı yalnızca harf, rakam ve alt çizgi içerebilir!", this.Form);
                return;
            }

            DataTable dt = (DataTable)ViewState["Vs_Kolon"];
            dt.Rows.Add(kolonAdi, cbSifrele.Checked ? "Evet" : "Hayır");
            ViewState["Vs_Kolon"] = dt;
            if (dt.Rows.Count > 0) { divButtons.Style.Add("display", "block"); }
            cbSifrele.Checked = false;
            this.BindGrid();
            txtKolonAdi.Text = string.Empty;
        }

        protected void listVwTblOlustur_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            DataTable dt = (DataTable)ViewState["Vs_Kolon"];
            dt.Rows.RemoveAt(e.ItemIndex);
            ViewState["Vs_Kolon"] = dt;
            this.BindGrid();
        }

        protected void listVwTblOlustur_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            listVwTblOlustur.EditIndex = e.NewEditIndex;
            listVwTblOlustur.DataSource = (DataTable)ViewState["Vs_Kolon"];
            listVwTblOlustur.DataBind();
        }

        protected void listVwTblOlustur_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            int ind = e.ItemIndex;
            string yeniDeger = "";

            TextBox txt = (listVwTblOlustur.Items[e.ItemIndex].FindControl("txtEmpKolonAdi")) as TextBox;

            if (txt != null)
                yeniDeger = txt.Text.Trim().Replace(" ", "_");

            if (!GecerliAdMi(yeniDeger))
            {
                ClsMessage.ShowMessage("Kolon adı yalnızca harf, rakam ve alt çizgi içerebilir!", this.Form);
                return;
            }

            listVwTblOlustur.EditIndex = -1;
            lblKolonAdi.Text = yeniDeger;
            DataTable dt = (DataTable)ViewState["Vs_Kolon"];
            dt.Rows[ind][0] = yeniDeger;
            ViewState["Vs_Kolon"] = dt;
            this.BindGrid();

            listVwTblOlustur.DataSource = (DataTable)ViewState["Vs_Kolon"];
            listVwTblOlustur.DataBind();
        }

        protected void listVwTblOlustur_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            listVwTblOlustur.EditIndex = -1;
            listVwTblOlustur.DataSource = (DataTable)ViewState["Vs_Kolon"];
            listVwTblOlustur.DataBind();
        }

        protected void btnTblOlustur_Click(object sender, EventArgs e)
        {
            bool yonlendir = false;
            try
            {
                DataTable dtKolonlar = (DataTable)ViewState["Vs_Kolon"];
                var columns = new List<DatasetColumn>();
                foreach (DataRow dr in dtKolonlar.Rows)
                {
                    columns.Add(new DatasetColumn(Convert.ToString(dr["KolonAdi"]), Convert.ToString(dr["IsEncrypted"]) == "Evet"));
                }

                string tableName = txtTabloAdi.Text.Trim().Replace(" ", "_");
                datasets.CreateDataset(CurrentUser.Id, CurrentUser.FullName, tableName, columns, txtKey.Text);

                listVwTblOlustur.Items.Clear();
                listVwTblOlustur.DataBind();
                ViewState["Vs_Kolon"] = NewColumnTable();
                txtTabloAdi.Text = "";
                txtKey.Text = "";

                yonlendir = true;
            }
            catch (UserException ux)
            {
                ClsMessage.ShowMessage(ux.Message, this.Form);
            }
            catch (Exception)
            {
                ClsMessage.ShowMessage("Tablo oluşturulurken hata oluştu!", this.Form);
            }

            if (yonlendir) { Response.Redirect("TabloSchemaList.aspx"); }
        }

        protected void btnTemizle_Click(object sender, EventArgs e)
        {
            listVwTblOlustur.Items.Clear();
            listVwTblOlustur.DataBind();

            ViewState["Vs_Kolon"] = NewColumnTable();
            this.BindGrid();

            txtTabloAdi.Text = "";
            txtKolonAdi.Text = "";
            txtKey.Text = "";
        }
    }
}
