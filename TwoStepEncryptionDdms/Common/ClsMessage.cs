using System;
using System.Web;
using System.Web.UI.HtmlControls;
using TwoStepEncryptionDdms.Logging;

namespace TwoStepEncryptionDdms.Common
{
    public static class ClsMessage
    {
        private static void ShowToast(string message, string type, HtmlForm form, string scriptMessage = "")
        {
            // Controls.Add yerine startup script kullanılır; form içinde <%= %> code block varsa Controls değiştirilemez.
            string encoded = HttpUtility.JavaScriptStringEncode(message ?? "");
            string script = "$(function(){ iqwerty.toast.toast('" + encoded + "', { type: '" + type + "' });"
                + (string.IsNullOrWhiteSpace(scriptMessage) ? "" : "\n " + scriptMessage)
                + "});";
            form.Page.ClientScript.RegisterStartupScript(typeof(ClsMessage), Guid.NewGuid().ToString("N"), script, true);
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
