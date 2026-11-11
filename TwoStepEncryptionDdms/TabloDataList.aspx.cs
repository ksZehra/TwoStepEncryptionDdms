using System;
using System.Web.UI.WebControls;
using TwoStepEncryptionDdms.Common;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class TabloDataList : TwoStepEncryptionDdms.Security.BasePage
    {
        readonly DatasetService datasets = new DatasetService();

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                GridTbl.Columns[4].Visible = CurrentUser.Has(Permission.Delete);
                GridTbl.Columns[3].Visible = CurrentUser.Has(Permission.Edit);
                GridTbl.Columns[2].Visible = CurrentUser.Has(Permission.New);
                GridTbl.Columns[1].Visible = CurrentUser.Has(Permission.View);
            }
            catch (Exception)
            {
                ClsMessage.ShowMessage("Beklenilmeyen bir hata oluştu!", this.Form);
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
            catch (Exception)
            {
                ClsMessage.ShowMessage("Beklenilmeyen bir hata oluştu!", this.Form);
            }
        }

        private void BindGrid(string aramaMetni = null)
        {
            try
            {
                GridTbl.DataSource = datasets.ListDatasets(aramaMetni);
                GridTbl.DataBind();
            }
            catch (Exception)
            {
                ClsMessage.ShowMessage("Kayıtlar listelenirken hata oluştu!", this.Form);
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
