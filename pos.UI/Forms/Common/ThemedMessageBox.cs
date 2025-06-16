using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pos_system.pos.UI.Forms
{
    public partial class ThemedMessageBox : Form
    {
        public static void Show(string message, string title = "")
        {
            using (var msgBox = new ThemedMessageBox(message, title))
            {
                msgBox.ShowDialog();
            }
        }
    }
}
