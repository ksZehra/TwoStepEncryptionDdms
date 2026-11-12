using System;
using TwoStepEncryptionDdms.Logging;
using System.Web.UI.WebControls;
using TwoStepEncryptionDdms.Common;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class TabloSchemaList : TwoStepEncryptionDdms.Security.BasePage
    {
        readonly DatasetService datasets = new DatasetService();

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                GridTbl.Columns[1].Visible = CurrentUser.Has(Permission.Delete);
                lnkYeniVeriKumesi.Visible = CurrentUser.Has(Permission.New);
                GridTbl.Columns[2].Visible = CurrentUser.Has(Permission.Edit);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Beklenilmeyen bir hata oluştu!", ex, this.Form);
            }
        }

        protected override Permission RequiredPermission
        {
            get { return Permission.List; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    this.BindGrid();
                }
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Beklenilmeyen bir hata oluştu!", ex, this.Form);
            }
        }

        private void BindGrid(string aramaMetni = null)
        {
            try
            {
                GridTbl.DataSource = datasets.ListDatasets(aramaMetni);
                GridTbl.DataBind();
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Kayıtlar listelenirken hata oluştu!", ex, this.Form);
            }
        }

        protected void GridTbl_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            EnsurePermission(Permission.Delete);

            try
            {
                datasets.DeleteDataset(GridTbl.DataKeys[e.RowIndex].Value.ToString());
                AppLog.Audit("DatasetDeleted", GridTbl.DataKeys[e.RowIndex].Value.ToString());
                BindGrid();

                ClsMessage.ShowMessage("Tablo Silindi", this.Form);
            }
            catch (UserException ux)
            {
                ClsMessage.ShowMessage(ux.Message, this.Form);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Tablo silinirken hata oluştu!", ex, this.Form);
            }
        }

        protected void GridTbl_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            EnsurePermission(Permission.Edit);

            const string hataMesaji = "Tablo adı güncellenemedi. Ad geçersiz veya zaten kullanılıyor olabilir.";
            try
            {
                string eskiTblAd = GridTbl.DataKeys[e.RowIndex].Value.ToString();
                GridViewRow row = GridTbl.Rows[e.RowIndex];
                string yeniTableName = ((TextBox)row.Cells[3].Controls[0]).Text.Trim().Replace(' ', '_');

                datasets.RenameDataset(eskiTblAd, yeniTableName, CurrentUser.Id, CurrentUser.FullName);

                GridTbl.EditIndex = -1;
                BindGrid();

                ClsMessage.ShowMessage("Tablo Adı Güncellendi", this.Form);
            }
            catch (UserException)
            {
                ClsMessage.ShowMessage(hataMesaji, this.Form);
            }
            catch (Exception ex)
            {
                GridTbl.EditIndex = -1;
                ClsMessage.ShowError(hataMesaji, ex, this.Form);
            }
        }

        protected void GridTbl_RowEditing(object sender, GridViewEditEventArgs e)
        {
            EnsurePermission(Permission.Edit);

            GridTbl.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void GridTbl_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridTbl.EditIndex = -1;
            BindGrid();
        }

        protected void GridTbl_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridTbl.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void btnAra_Click(object sender, EventArgs e)
        {
            BindGrid(txtAra.Text);
        }
    }
}
