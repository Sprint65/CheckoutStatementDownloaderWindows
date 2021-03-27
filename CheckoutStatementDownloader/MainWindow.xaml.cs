using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


//Using System.Net.Http directive which will enable HttpClient.
using System.Net.Http;
using System.Text.RegularExpressions;

namespace CheckoutStatementDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static HttpClient client = new HttpClient();
        static string baseUrl = "https://api.checkout.com/";
        // Example Statement ID: 210312B207657


        public MainWindow()
        {

            InitializeComponent();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
        }


        private async void enterButton_Click(object sender, RoutedEventArgs e)
        {
            string statementId = this.statementIdText.Text;

            if (statementId.Length == 0)
            {
                // If input is empty prompt user to enter ID
                MessageBox.Show("Please enter a statement ID", "Statement ID required!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string statementData = await FetchStatementCSV(statementId);

            // If downloaded statement is null then tell user there was an error
            if (statementData == null)
            {
                MessageBox.Show("Please ensure your API Key is correct. This can be changed in the app settings by going to: File > Settings", "Error Downloading Statement!", MessageBoxButton.OK , MessageBoxImage.Error);
                return;
            }

            // Check to see if CSV contains data
            if (CheckIfDownloadedCsvIsEmpty(statementData))
            {
                Debug.WriteLine("File is empty");
                MessageBox.Show("Please ensure you have entered the correct statement ID.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                await SaveCsvFileToDisk(statementId, statementData);
            }
        }

        // Fetch statement using the Checkout.com API
        private async Task<string> FetchStatementCSV(String id)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Properties.Settings.Default.apiKey);
            string apiURL = $"reporting/statements/{id}/payments/download";
            HttpResponseMessage response = await client.GetAsync(apiURL);
            Debug.WriteLine("Response: ");
            Debug.WriteLine(response);

            if (response.IsSuccessStatusCode)
            {
                String resultCSV = await response.Content.ReadAsStringAsync();
                return resultCSV;
            } 
            else if (response.StatusCode.ToString() == "Unauthorized")
            {
                Debug.WriteLine("Authorisation Error!");
            }
            Debug.WriteLine("Status Code:");
            Debug.WriteLine(response.StatusCode);
            return null;
        }

        private async Task SaveCsvFileToDisk(string id, string resultCSV)
        {
                string folderPath = Properties.Settings.Default.downloadFolderLocation;
                string fileName = id + ".csv";
                string filePath = Path.Combine(folderPath, fileName);

                // Create folder if it doesn't already exist
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                Debug.WriteLine("File Path: " + filePath);

                    try
                    {
                        // Save data in Documents folder under new subfolder 'Statements'
                        using (StreamWriter outputFile = new StreamWriter(filePath))
                        {
                            await outputFile.WriteAsync(resultCSV);

                            MessageBox.Show("Statement downloaded to " + Properties.Settings.Default.downloadFolderLocation, "Success!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            this.statementIdText.Text = "";
                            Process.Start("explorer.exe", Properties.Settings.Default.downloadFolderLocation);
                        }
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.ToString(), "Error saving file to folder!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
        }

        private static bool CheckIfDownloadedCsvIsEmpty(String csvData)
        {
            int count = 0;

            Regex regex = new Regex(@"\n");

            Match match = regex.Match(csvData);

            while (match.Success)
            {
                count++;
                match = match.NextMatch();

                if (count > 1) return false;
            }

            //Debug.WriteLine("Count: ");
            //Debug.WriteLine(count);
            return true;

        }

        private void OpenSettingsPoppupWindow(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsPoppupWindow = new SettingsWindow();
            settingsPoppupWindow.Show();
        }

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
