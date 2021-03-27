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
        // Replace with user set key * MUST BE REMOVED *
        static string apiKey = "sk_e2099101-ffb1-49db-b240-37cd96508044";
        // Set a variable to the Documents path.
        static string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Statements");
        // Example Statement ID: 210312B207657


        public MainWindow()
        {

            InitializeComponent();

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(apiKey);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void GetSettings()
        {
            apiKey = Properties.Settings.Default.apiKey;
            folderPath = Properties.Settings.Default.downloadFolderLocation;
        }

        private async void enterButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(content);
            string statementId = this.statementIdText.Text;

            if (statementId.Length == 0)
            {
                // If input is empty prompt user to enter ID
                MessageBox.Show("Please enter a statement ID", "Error!");
                return;
            }
            string statementData = await FetchStatementCSV(statementId);

            // If downloaded statement is null then tell user there was an error
            if (statementData == null)
            {
                MessageBox.Show("Error downloading statement", "Error!");
                return;
            }

            // Check to see if CSV contains data
            if (CheckIfDownloadedCsvIsEmpty(statementData))
            {
                Debug.WriteLine("File is empty");
                MessageBox.Show("Please ensure you have entered the correct statement ID.", "Error!");
            }
            else
            {
                bool successfulDownload = await SaveCsvFileToDisk(statementId, statementData);

                if(successfulDownload)
                {
                    MessageBox.Show("Statement downloaded to " + folderPath, "Success!");
                    this.statementIdText.Text = "";
                    Process.Start("explorer.exe", folderPath);
                }
            }
        }

        // Fetch statement using the Checkout.com API
        private async Task<string> FetchStatementCSV(String id)
        {
            string apiURL = $"reporting/statements/{id}/payments/download";
            HttpResponseMessage response = await client.GetAsync(apiURL);
            Debug.WriteLine("Response: ");
            Debug.WriteLine(response);

            if (response.IsSuccessStatusCode)
            {
                String resultCSV = await response.Content.ReadAsStringAsync();
                return resultCSV;
            }

            return null;
              
        }

        private async Task<bool> SaveCsvFileToDisk(string id, string resultCSV)
        {
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
                            return true;
                        }
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.ToString(), "Error!");
                        return false;
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
