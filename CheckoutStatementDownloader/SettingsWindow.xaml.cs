using System;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;

namespace CheckoutStatementDownloader
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void LoadUserSettings(object sender, RoutedEventArgs e)
        {
            this.apiKeyInput.Text = Properties.Settings.Default.apiKey;
            this.folderLocationInput.Text = Properties.Settings.Default.downloadFolderLocation;
        }

        private void UpdateUserSettings(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.apiKey = this.apiKeyInput.Text;
            Properties.Settings.Default.downloadFolderLocation = this.folderLocationInput.Text;
            Properties.Settings.Default.Save();
            this.Hide();
        }

        private void OpenFolderSelectionWindow(object sender, RoutedEventArgs e)
        {
            DialogResult selectedFolder = folderBrowserDialog.ShowDialog();

            Debug.WriteLine(selectedFolder);

            if (selectedFolder.ToString() == "OK")
            {
                this.folderLocationInput.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
