using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Lab4_ADONET_SQLite
{
    /// <summary>
    /// Minimal browser host from the previous lab, extended with
    /// the exact LAB4 Database and Help menus.
    /// </summary>
    public sealed class BrowserForm : Form
    {
        private readonly SQLiteHandler database = new SQLiteHandler();
        private readonly WebBrowser browser = new WebBrowser();
        private readonly TextBox addressBox = new TextBox();

        private ToolStripMenuItem connectItem;
        private ToolStripMenuItem disconnectItem;
        private ToolStripMenuItem addKeywordItem;
        private ToolStripMenuItem viewKeywordsItem;

        public BrowserForm()
        {
            Text = "LAB4 Browser";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;

            MenuStrip menu = BuildMenu();
            FlowLayoutPanel navigation = BuildNavigationBar();

            browser.Dock = DockStyle.Fill;
            browser.ScriptErrorsSuppressed = true;

            Controls.Add(browser);
            Controls.Add(navigation);
            Controls.Add(menu);
            MainMenuStrip = menu;

            disconnectItem.Enabled = false;
            addKeywordItem.Enabled = false;
            viewKeywordsItem.Enabled = false;

            FormClosed += (sender, e) => database.Dispose();
            Shown += (sender, e) => NavigateHome();
        }

        private MenuStrip BuildMenu()
        {
            MenuStrip menu = new MenuStrip();

            ToolStripMenuItem databaseMenu =
                new ToolStripMenuItem("Database");

            connectItem = new ToolStripMenuItem(
                "Connect", null, Connect_Click);
            disconnectItem = new ToolStripMenuItem(
                "Disconnect", null, Disconnect_Click);
            addKeywordItem = new ToolStripMenuItem(
                "AddKeyword", null, AddKeyword_Click);
            viewKeywordsItem = new ToolStripMenuItem(
                "ViewKeywords", null, ViewKeywords_Click);

            databaseMenu.DropDownItems.Add(connectItem);
            databaseMenu.DropDownItems.Add(disconnectItem);
            databaseMenu.DropDownItems.Add(new ToolStripSeparator());
            databaseMenu.DropDownItems.Add(addKeywordItem);
            databaseMenu.DropDownItems.Add(viewKeywordsItem);

            ToolStripMenuItem helpMenu =
                new ToolStripMenuItem("Help");
            helpMenu.DropDownItems.Add(
                new ToolStripMenuItem("About", null, About_Click));

            menu.Items.Add(databaseMenu);
            menu.Items.Add(helpMenu);
            return menu;
        }

        private FlowLayoutPanel BuildNavigationBar()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 36,
                Padding = new Padding(4),
                WrapContents = false
            };

            addressBox.Width = 560;
            addressBox.Text = "https://www.google.com";

            Button goButton = new Button { Text = "Go", Width = 55 };
            Button backButton = new Button { Text = "Back", Width = 65 };
            Button forwardButton = new Button { Text = "Forward", Width = 75 };
            Button homeButton = new Button { Text = "Home", Width = 65 };

            goButton.Click += (sender, e) => NavigateFromAddressBox();
            addressBox.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    NavigateFromAddressBox();
                }
            };

            backButton.Click += (sender, e) =>
            {
                if (browser.CanGoBack)
                    browser.GoBack();
            };

            forwardButton.Click += (sender, e) =>
            {
                if (browser.CanGoForward)
                    browser.GoForward();
            };

            homeButton.Click += (sender, e) => NavigateHome();

            browser.DocumentCompleted += (sender, e) =>
            {
                if (browser.Url != null)
                    addressBox.Text = browser.Url.ToString();

                backButton.Enabled = browser.CanGoBack;
                forwardButton.Enabled = browser.CanGoForward;
            };

            panel.Controls.Add(addressBox);
            panel.Controls.Add(goButton);
            panel.Controls.Add(backButton);
            panel.Controls.Add(forwardButton);
            panel.Controls.Add(homeButton);
            return panel;
        }

        private void NavigateHome()
        {
            NavigateTo("https://www.google.com");
        }

        private void NavigateFromAddressBox()
        {
            string address = addressBox.Text.Trim();

            if (address.Length == 0)
                return;

            if (!address.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !address.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                address = "https://" + address;
            }

            NavigateTo(address);
        }

        private void NavigateTo(string address)
        {
            Uri uri;

            if (Uri.TryCreate(address, UriKind.Absolute, out uri))
                browser.Navigate(uri);
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            try
            {
                database.ConnectToDb();
                connectItem.Enabled = false;
                disconnectItem.Enabled = true;
                addKeywordItem.Enabled = true;
                viewKeywordsItem.Enabled = true;
                MessageBox.Show("Connected to the SQLite database.");
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            database.DisconnectFromDb();
            connectItem.Enabled = true;
            disconnectItem.Enabled = false;
            addKeywordItem.Enabled = false;
            viewKeywordsItem.Enabled = false;
            MessageBox.Show("Disconnected from the SQLite database.");
        }

        private void AddKeyword_Click(object sender, EventArgs e)
        {
            try
            {
                using (KeywordDialog dialog = new KeywordDialog())
                {
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                        return;

                    database.InsertKeyword(dialog.Keyword);
                    MessageBox.Show("Keyword added.");
                }
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        private void ViewKeywords_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> keywords = database.GetAllKeywords();

                string text = keywords.Count == 0
                    ? "No keywords saved."
                    : string.Join(Environment.NewLine, keywords);

                MessageBox.Show(text, "Keywords");
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "LAB4: ADO.NET and SQLite",
                "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private static void ShowDatabaseError(Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Database error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}

