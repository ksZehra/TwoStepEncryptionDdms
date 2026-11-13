using System;
using TwoStepEncryptionDdms.Logging;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using TwoStepEncryptionDdms.Common;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class TabloSchemaList : TwoStepEncryptionDdms.Security.BasePage
    {
        readonly DatasetService datasets = new DatasetService();

        static readonly string[] SortableColumns = { "TBLID", "TBLAD", "ISENCRYPT", "OLUSTURAN", "OLUSTURMATARIHI_SIRA", "GUNCELLEYEN", "GUNCELLEMETARIHI_SIRA" };

        string SortColumn
        {
            get { return (string)ViewState["SortColumn"] ?? ""; }
            set { ViewState["SortColumn"] = value; }
        }

        bool SortAscending
        {
            get { return ViewState["SortAscending"] == null || (bool)ViewState["SortAscending"]; }
            set { ViewState["SortAscending"] = value; }
        }

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
                    var successMsg = Session["SuccessToast"] as string;
                    if (!string.IsNullOrWhiteSpace(successMsg))
                    {
                        ClsMessage.ShowSuccess(successMsg, this.Form);
                        Session.Remove("SuccessToast");
                    }
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
                DataTable table = datasets.ListDatasets(aramaMetni ?? txtAra.Text);
                if (SortColumn.Length > 0)
                {
                    table.DefaultView.Sort = SortColumn + (SortAscending ? " ASC" : " DESC");
                }

                GridTbl.DataSource = table.DefaultView;
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

                ClsMessage.ShowSuccess("Tablo Silindi", this.Form);
            }
            catch (UserException ux)
            {
                ClsMessage.ShowError(ux.Message, this.Form);
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

                ClsMessage.ShowSuccess("Tablo Adı Güncellendi", this.Form);
            }
            catch (UserException)
            {
                ClsMessage.ShowError(hataMesaji, this.Form);
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

        protected void GridTbl_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                if (Array.IndexOf(SortableColumns, e.SortExpression) < 0)
                {
                    return;
                }

                SortAscending = SortColumn != e.SortExpression || !SortAscending;
                SortColumn = e.SortExpression;
                GridTbl.PageIndex = 0;
                BindGrid();
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Sıralama sırasında hata oluştu!", ex, this.Form);
            }
        }

        protected void GridTbl_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.Header || SortColumn.Length == 0)
                {
                    return;
                }

                foreach (TableCell cell in e.Row.Cells)
                {
                    LinkButton link = cell.Controls.OfType<LinkButton>().FirstOrDefault();
                    if (link != null && link.CommandArgument == SortColumn)
                    {
                        cell.CssClass += SortAscending ? " sorted-asc" : " sorted-desc";
                    }
                }
            }
            catch (Exception ex)
            {
                AppLog.Error(ex, "Tablo başlığı sıralama işareti");
            }
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
