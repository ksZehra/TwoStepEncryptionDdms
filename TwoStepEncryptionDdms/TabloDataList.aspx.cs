using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using TwoStepEncryptionDdms.Common;
using TwoStepEncryptionDdms.Logging;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class TabloDataList : TwoStepEncryptionDdms.Security.BasePage
    {
        readonly DatasetService datasets = new DatasetService();

        static readonly string[] SortableColumns = { "TBLID", "TBLAD", "ISENCRYPT", "OLUSTURAN", "OLUSTURMATARIHI_SIRA" };

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
                GridTbl.Columns[4].Visible = CurrentUser.Has(Permission.Delete);
                GridTbl.Columns[3].Visible = CurrentUser.Has(Permission.Edit);
                GridTbl.Columns[2].Visible = CurrentUser.Has(Permission.New);
                GridTbl.Columns[1].Visible = CurrentUser.Has(Permission.View);
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
                this.BindGrid();
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
