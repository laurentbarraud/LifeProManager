/// <file>Program.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.</version>
/// <date>May 7th, 2026</date>

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace LifeProManager
{
    static class Program
    {
        public static ApplicationContext appContext;
        public static DBConnection DbConn;

        /// <summary> 
        /// Main entry point of the application. 
        /// </summary> 
        [STAThread] 
        static void Main() 
        { 
            Application.EnableVisualStyles(); 
            Application.SetCompatibleTextRenderingDefault(false); 
            
            // Applies the UI language before creating any form.
            ApplyLocalization(); 
            
            // Creates the single global database connection for the entire application.
            DbConn = new DBConnection(); 
            
            // Ensures the database file exists and is valid before launching the UI.
            InitializeDatabase();

            // Uses an ApplicationContext instead of running a form directly.
            appContext = new ApplicationContext(new frmMain());
            Application.Run(appContext); 
        }

        /// <summary> 
        /// Applies the saved UI culture before any form is created. 
        /// </summary> 
        private static void ApplyLocalization() 
        { 
            string lang = Properties.Settings.Default.appLanguageCode; 
            
            if (!string.IsNullOrEmpty(lang)) 
            { 
                 LocalizationManager.SetLanguage(lang);
            } 
        }

        /// <summary> 
        /// Ensures the database file exists and is valid. 
        /// If missing or invalid, it is recreated.
        /// </summary> 
        private static void InitializeDatabase() 
        { 
            string dbPath = Path.Combine(Application.StartupPath, "LPM_DB.db");
            
            // If the file does not exist, creates it and initializes tables.
            if (!File.Exists(dbPath)) 
            { 
                MessageBox.Show( LocalizationManager.GetString("databaseNotFound"), 
                    LocalizationManager.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConn.CreateFile(); 
                DbConn.CreateTablesAndInsertInitialData(); 
                return; 
            } 
            
            // If file exists, check integrity.
            bool dbValid = DbConn.CheckDBIntegrity(); 
            
            if (!dbValid) 
            { 
                MessageBox.Show( LocalizationManager.GetString("databaseCorrupted"), 
                    LocalizationManager.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error); 
                DbConn.CreateTablesAndInsertInitialData(); 
            } 
        }

        /// <summary>
        /// Helper method to switch the main form.
        /// Replaces the visible UI with a new form without closing the application.
        /// Preserves size, position, and window state.
        /// </summary>
        public static void SwitchMainForm(Form newForm) 
        { 
            Form oldForm = appContext.MainForm; 
            appContext.MainForm = newForm;

            // Copies size and position
            newForm.StartPosition = FormStartPosition.Manual;
            newForm.Left = oldForm.Left;
            newForm.Top = oldForm.Top;
            newForm.Width = oldForm.Width;
            newForm.Height = oldForm.Height;

            // Copies window state (Normal / Maximized)
            newForm.WindowState = oldForm.WindowState;

            // Replaces the main form in the ApplicationContext
            appContext.MainForm = newForm;

            newForm.Show(); 
            oldForm.Close(); 
        }
    }
}

