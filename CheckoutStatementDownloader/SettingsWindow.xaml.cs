using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CheckoutStatementDownloader
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
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
            this.Hide();
        }
    }
}
