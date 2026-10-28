using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab4_ADONET_SQLite
{
    /// <summary>
    /// Modal keyword entry form. OK returns DialogResult.OK;
    /// Cancel returns DialogResult.Cancel.
    /// </summary>
    public sealed class KeywordDialog : Form
    {
        private readonly TextBox textBoxKeyword;

        public string Keyword { get; private set; }

        public KeywordDialog()
        {
            Text = "Add keyword";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(360, 130);

            Label label = new Label
            {
                Text = "Keyword:",
                AutoSize = true,
                Location = new Point(12, 19)
            };

            textBoxKeyword = new TextBox
            {
                Location = new Point(90, 15),
                Width = 245
            };

            Button buttonOk = new Button
            {
                Text = "OK",
                Location = new Point(170, 75)
            };
            buttonOk.Click += ButtonOk_Click;

            Button buttonCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(255, 75),
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(label);
            Controls.Add(textBoxKeyword);
            Controls.Add(buttonOk);
            Controls.Add(buttonCancel);

            AcceptButton = buttonOk;
            CancelButton = buttonCancel;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Keyword = textBoxKeyword.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
