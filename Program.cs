/// <file>Program.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.3</version>
/// <date>July 5th, 2026</date>

using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace LifeProManager
{
    static class Program
    {

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
        public static ApplicationContext appContext = null!;
        public static DBConnection DbConn = null!;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplyLocalization();

            // Initializes database before creating the DBConnection
            InitializeDatabase();

            // Creates the global DB connection after the DB is ready
            DbConn = new DBConnection();

            // Starts the application
            appContext = new ApplicationContext(new frmMain());
            Application.Run(appContext);
        }

        private static void ApplyLocalization()
        {
            string lang = Properties.Settings.Default.appLanguageCode;

            if (!string.IsNullOrEmpty(lang))
            {
                LocalizationManager.SetLanguage(lang);
            }
        }

        /// <summary>
        /// Initializes the database by checking if it exists and creating it if necessary. 
        /// If the database exists, it checks its integrity and recreates it if it's invalid.
        /// </summary>
        private static void InitializeDatabase()
        {
            string dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LifeProManager");

            Directory.CreateDirectory(dbFolder);

            string dbPath = Path.Combine(dbFolder, "LPM_DB.db");

            // If the database file does not exist
            if (!File.Exists(dbPath))
            {
                // Creates the empty database file
                using (var conn = new SqliteConnection($"Data Source={dbPath}"))
                {
                    conn.Open();
                }

                // Creates tables using a temporary DBConnection
                new DBConnection().CreateTablesAndInsertInitialData();
                return;
            }

            // If the database file exists, check its integrity
            DbConn = new DBConnection();
            bool dbValid = DbConn.CheckDBIntegrity();

            if (!dbValid)
            {
                new DBConnection().CreateTablesAndInsertInitialData();
            }
        }

        public static void SwitchMainForm(Form newForm)
        {
            if (appContext.MainForm is Form oldForm)
            {
                newForm.StartPosition = FormStartPosition.Manual;
                newForm.Left = oldForm.Left;
                newForm.Top = oldForm.Top;
                newForm.Width = oldForm.Width;
                newForm.Height = oldForm.Height;
                newForm.WindowState = oldForm.WindowState;

                appContext.MainForm = newForm;

                newForm.Show();
                oldForm.Close();
            }
        }
    }
}
