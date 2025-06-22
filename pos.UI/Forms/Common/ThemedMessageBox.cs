using System;
using System.Linq;
using pos_system.pos.UI.Forms;
using pos_system;
using pos_system.pos;
using pos_system.pos.UI;

namespace pos_system.pos.UI.Forms.Common
{
    public partial class ThemedMessageBox : Form
    {
        public static void Show(string message, string title = "")
        {
            using (var msgBox = new pos_system.pos.UI.Forms.Common.ThemedMessageBox(message, title))
            {
                msgBox.ShowDialog();
            }
        }
    }
}
