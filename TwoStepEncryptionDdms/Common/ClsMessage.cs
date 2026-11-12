using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using TwoStepEncryptionDdms.Logging;


namespace TwoStepEncryptionDdms.Common
{
    public class ClsMessage
    {
        public ClsMessage() { }

        public static void ShowMessage(string message, HtmlForm form, string scriptMessage = "")
        {
            try
            {
                Label myLabel = new Label();
                myLabel.Text = "<script type='text/javascript'> $(function(){";
                myLabel.Text += "iqwerty.toast.toast('" + HttpUtility.JavaScriptStringEncode(message ?? "") + "');";
                myLabel.Text += !string.IsNullOrWhiteSpace(scriptMessage) ? "\n " + scriptMessage : "";
                myLabel.Text += "});</script>";
                form.Controls.Add(myLabel);
            }
            catch { }
        }

        public static void ShowError(string message, Exception ex, HtmlForm form, string scriptMessage = "")
        {
            string code = AppLog.Error(ex);
            ShowMessage(message + " (Kod: " + code + ")", form, scriptMessage);
        }
    }
}
