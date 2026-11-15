using System;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using TwoStepEncryptionDdms.Logging;

namespace TwoStepEncryptionDdms.Common
{
    public static class ClsMessage
    {
        private static void ShowToast(string message, string type, HtmlForm form, string scriptMessage = "")
        {
            try
            {
                Label myLabel = new Label();
                string encoded = HttpUtility.JavaScriptStringEncode(message ?? "");
                myLabel.Text = "<script type='text/javascript'> $(function(){";
                myLabel.Text += "iqwerty.toast.toast('" + encoded + "', { type: '" + type + "' });";
                myLabel.Text += !string.IsNullOrWhiteSpace(scriptMessage) ? "\n " + scriptMessage : "";
                myLabel.Text += "});</script>";
                form.Controls.Add(myLabel);
            }
            catch { }
        }

        public static void ShowSuccess(string message, HtmlForm form, string scriptMessage = "")
        {
            ShowToast(message, "success", form, scriptMessage);
        }

        public static void ShowWarning(string message, HtmlForm form, string scriptMessage = "")
        {
            ShowToast(message, "warning", form, scriptMessage);
        }

        public static void ShowError(string message, HtmlForm form, string scriptMessage = "")
        {
            ShowToast(message, "error", form, scriptMessage);
        }

        public static void ShowError(string message, Exception ex, HtmlForm form, string scriptMessage = "")
        {
            string code = AppLog.Error(ex);
            ShowToast(message + " (Kod: " + code + ")", "error", form, scriptMessage);
        }
    }
}
