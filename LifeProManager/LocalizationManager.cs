/// <file>LocalizationManager.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.1</version>
/// <date>May 23th, 2026</date>

using System;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace LifeProManager
{
    /// <summary>
    /// Central localization manager for WinForms.
    /// Automatically loads culture-specific satellite files
    /// based on the current UI culture.
    /// </summary>
    public static class LocalizationManager
    {
        // Single ResourceManager for the neutral resource file
        private static readonly ResourceManager _resourceManager =
            new ResourceManager("LifeProManager.strings", typeof(LocalizationManager).Assembly);

        // Stores the current language code
        private static string _currentLanguageCode = "en";

        /// <summary>
        /// Sets the current language and updates thread cultures.
        /// </summary>
        public static void SetLanguage(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                languageCode = "en";
            }

            _currentLanguageCode = languageCode;

            CultureInfo culture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Gets the current language code.
        /// </summary>
        public static string GetCurrentLanguageCode()
        {
            return _currentLanguageCode;
        }

        /// <summary>
        /// Returns a localized string for the given key.
        /// Uses the current UI culture and falls back to English if missing.
        /// Returns the key itself if nothing is found.
        /// </summary>
        public static string GetString(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            try
            {
                // Tries to get the value using the current UI culture
                string localizedText = _resourceManager.GetString(key, CultureInfo.CurrentUICulture);
                
                if (!string.IsNullOrEmpty(localizedText))
                {
                    return localizedText;
                }

                // Fallback to English explicitly
                localizedText = _resourceManager.GetString(key, new CultureInfo("en"));
                
                if (!string.IsNullOrEmpty(localizedText))
                {
                    return localizedText;
                }
            }
            catch
            {
                // Ignores missing keys or culture errors.
            }

            // Last resort: returns the key itself.
            return key;
        }

        /// <summary>
        /// Applies localization to a specific form by calling its LoadLocalizedStrings() method.
        /// </summary>
        public static void LoadLocalizedStringsFor(Form selectedForm)
        {
            if (selectedForm is frmMain frmMain)
            {
                frmMain.LoadLocalizedStrings();
            }
            else if (selectedForm is frmAddTask frmAddTask)
            {
                frmAddTask.LoadLocalizedStrings();
            }
            else if (selectedForm is frmAddTopic frmAddTopic)
            {
                frmAddTopic.LoadLocalizedStrings();
            }

            else if (selectedForm is frmBirthdayCalendar frmBirthdayCalendar)
            {
                frmBirthdayCalendar.LoadLocalizedStrings();
            }

            else if (selectedForm is frmEditTask frmEditTask)
            {
                frmEditTask.LoadLocalizedStrings();
            }

            else if (selectedForm is frmAbout frmAbout)
            {
                frmAbout.LoadLocalizedStrings();
            }
        }
    }
}

