/// <file>frmBirthdayCalendar.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.3</version>
/// <date>July 5th, 2026</date>

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace LifeProManager
{
    public partial class frmBirthdayCalendar : Form
    {
        private readonly Dictionary<Button, string> _buttonBaseResourceNames = new Dictionary<Button, string>();

        private DBConnection dbConn => Program.DbConn;

        private frmMain? _frmMain = null;

        public frmBirthdayCalendar(Form callingForm)
        {
            _frmMain = callingForm as frmMain;
            InitializeComponent();
            LoadLocalizedStrings();
            ApplyStoredColors();
        }

        private void frmBirthdayCalendar_Load(object sender, EventArgs e)
        {

            // Fills the birthdays progressively
            CreateBirthdaysLayout(dbConn.ReadTask("WHERE Priorities_id == 4 AND Status_id == 1"));

            // Original image path mapping
            _buttonBaseResourceNames[cmdValidate] = "validate";

            // Hover events for the validate button
            cmdValidate.MouseEnter += Button_MouseEnter;
            cmdValidate.MouseLeave += Button_MouseLeave;
        }

        /// <summary>
        /// Applies stored theme colors and adjusts each month’s
        /// appearance based on the current date.
        /// </summary>
        internal void ApplyStoredColors()
        {
            // Window background
            this.BackColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfBirthdayCalendarBackground);

            // Base colors
            Color SquaresPrimaryColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfBirthdayCalendarSquaresPrimary);
            Color SquaresSecondaryColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfBirthdayCalendarSquaresSecondary);
            Color SquaresTertiaryColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfBirthdayCalendarSquaresTertiary);

            Color calendarTextColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfBirthdayCalendarText);

            // Labels grouped by month index
            Label[] monthDataLabels =
            {
                lblJanuaryData, lblFebruaryData, lblMarchData, lblAprilData, lblMayData, lblJuneData,
                lblJulyData, lblAugustData, lblSeptemberData, lblOctoberData, lblNovemberData, lblDecemberData
            };

            Label[] monthTitleLabels =
            {
                lblJanuary, lblFebruary, lblMarch, lblApril, lblMay, lblJune,
                lblJuly, lblAugust, lblSeptember, lblOctober, lblNovember, lblDecember
            };

            int currentMonth = DateTime.Now.Month;

            for (int i = 0; i < 12; i++)
            {
                Label dataLabel = monthDataLabels[i];
                Label monthTitleLabel = monthTitleLabels[i];

                // Picks the base color depending on the month index
                Color baseColor;

                if (i == 0 || i == 3 || i == 5 || i == 10)
                {
                    baseColor = SquaresPrimaryColor;
                }
                
                else if (i == 1 || i == 6 || i == 8 || i == 11)
                {
                    baseColor = SquaresSecondaryColor;
                }
                
                else
                {
                    baseColor = SquaresTertiaryColor;
                }

                if (i + 1 == currentMonth)
                {
                    // Current month: slightly brighter (+5%)
                    baseColor = Color.FromArgb(
                        Math.Min(255, (int)(baseColor.R * 1.05)),
                        Math.Min(255, (int)(baseColor.G * 1.05)),
                        Math.Min(255, (int)(baseColor.B * 1.05))
                    );
                }

                // Applies final color
                dataLabel.BackColor = baseColor;
                dataLabel.ForeColor = calendarTextColor;

                monthTitleLabel.ForeColor = calendarTextColor;
            }

            this.Invalidate();
        }

        /// <summary>
        /// Builds a fully localized birthday line using the current UI culture.
        /// Generates the correct ordinal, applies singular/plural rules,
        /// and assembles a natural sentence such as:
        /// "1st - John will turn 42.", 
        /// "le 1er - Jean aura 42 ans.", 
        /// "1.º - Juan cumplirá 42 años."
        /// </summary>
        /// <param name="birthdayDate"></param>
        /// <param name="firstName"></param>
        /// <param name="ageReached"></param>
        /// <returns></returns>
        private string BuildBirthdayLine(DateTime birthdayDate, string firstName, int ageReached)
        {
            string currentLanguageCode = LocalizationManager.GetCurrentLanguageCode();
            int nbDay = birthdayDate.Day;

            string dayOrdinal = GetDayOrdinal(nbDay, currentLanguageCode);
            string yearsOld = GetYearsOldText(ageReached, currentLanguageCode);

            switch (currentLanguageCode)
            {
                case "fr":
                    return $"le {dayOrdinal} - {firstName} ({ageReached} {yearsOld})";

                case "es":
                    return $"{dayOrdinal} - {firstName} ({ageReached} {yearsOld})";

                default: // en
                    return $"{dayOrdinal} - {firstName} ({ageReached})";
            }
        }

        /// <summary>
        /// Changes the button's background image to the hover version 
        /// when the mouse enters the button area.
        /// </summary>
        private void Button_MouseEnter(object? sender, EventArgs? e)
        {
            if (sender is Button btn)
            {
                UIHover.HandleMouseEnter(btn, _buttonBaseResourceNames);
            }
        }

        /// <summary>
        /// Changes back the button's background image to the normal version 
        /// when the mouse leaves the button area.
        /// </summary>
        private void Button_MouseLeave(object? sender, EventArgs? e)
        {
            if (sender is Button btn)
            {
                UIHover.HandleMouseLeave(btn, _buttonBaseResourceNames);
            }
        }
        private void cmdValidate_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Builds the birthday calendar by grouping each entry into its month label.
        /// Parses dates safely, computes the age for the current year, and appends
        /// a fully localized birthday line to the correct month.
        /// </summary>
        public void CreateBirthdaysLayout(List<Tasks> listOfBirthdays)
        {
            // Clear all month labels before filling them
            lblJanuaryData.Text = "";
            lblFebruaryData.Text = "";
            lblMarchData.Text = "";
            lblAprilData.Text = "";
            lblMayData.Text = "";
            lblJuneData.Text = "";
            lblJulyData.Text = "";
            lblAugustData.Text = "";
            lblSeptemberData.Text = "";
            lblOctoberData.Text = "";
            lblNovemberData.Text = "";
            lblDecemberData.Text = "";

            int currentYear = DateTime.Now.Year;

            // Maps month numbers to their corresponding UI labels
            var monthLabels = new Dictionary<int, Label>
            {
                { 1, lblJanuaryData },
                { 2, lblFebruaryData },
                { 3, lblMarchData },
                { 4, lblAprilData },
                { 5, lblMayData },
                { 6, lblJuneData },
                { 7, lblJulyData },
                { 8, lblAugustData },
                { 9, lblSeptemberData },
                { 10, lblOctoberData },
                { 11, lblNovemberData },
                { 12, lblDecemberData }
            };

            foreach (Tasks task in listOfBirthdays)
            {
                // Safe date parsing (avoids format issues)
                if (!DateTime.TryParse(task.Deadline, out DateTime birthdayDate))
                {
                    continue;
                }

                if (birthdayDate.Year != currentYear)
                {
                    continue;
                }

                // Birth year stored in Description
                int yearOfBirth;
                int.TryParse(task.Description, out yearOfBirth);

                int ageReached = currentYear - yearOfBirth;
                int month = birthdayDate.Month;

                // Appends to the correct month label
                if (monthLabels.TryGetValue(month, out Label? targetLabel))
                {
                    targetLabel.Text += BuildBirthdayLine(birthdayDate, task.Title, ageReached) + "\n";
                }
            }
        }

        /// <summary>
        /// Get the correct ordinal representation of a day number based on the culture.
        /// </summary>
        /// <param name="day"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        private string GetDayOrdinal(int day, string culture)
        {
            switch (culture)
            {
                case "fr":
                    return day == 1 ? "1er" : day.ToString();

                case "es":
                    return $"{day}.º";

                default: // en
                    if (day % 10 == 1 && day != 11)
                    {
                        return $"{day}st";
                    }

                    else if (day % 10 == 2 && day != 12)
                    {
                        return $"{day}nd";
                    }

                    else if (day % 10 == 3 && day != 13)
                    {
                        return $"{day}rd";
                    }

                    else
                    {
                        return $"{day}th";
                    }
            }
        }

        /// <summary>
        /// Gets the correct ordinal suffix for a day number based on the culture.
        /// </summary>
        /// <param name="age"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        private string GetYearsOldText(int age, string culture)
        {
            switch (culture)
            {
                case "fr":
                    return age <= 1 ? "an" : "ans";

                case "es":
                    return age <= 1 ? "año" : "años";

                default: // en
                    return ""; // no years old suffix in English
            }
        }

        /// <summary>
        /// Loads all the localized strings for the UI elements based on the current language setting.
        /// </summary>
        public void LoadLocalizedStrings()
        {
            // --- Window title ---
            this.Text = LocalizationManager.GetString("BirthdayCalendar");

            // --- Labels ---
            lblJanuary.Text = LocalizationManager.GetString("lblJanuaryText");
            lblFebruary.Text = LocalizationManager.GetString("lblFebruaryText");
            lblMarch.Text = LocalizationManager.GetString("lblMarchText");
            lblApril.Text = LocalizationManager.GetString("lblAprilText");
            lblMay.Text = LocalizationManager.GetString("lblMayText");
            lblJune.Text = LocalizationManager.GetString("lblJuneText");
            lblJuly.Text = LocalizationManager.GetString("lblJulyText");
            lblAugust.Text = LocalizationManager.GetString("lblAugustText");
            lblSeptember.Text = LocalizationManager.GetString("lblSeptemberText");
            lblOctober.Text = LocalizationManager.GetString("lblOctoberText");
            lblNovember.Text = LocalizationManager.GetString("lblNovemberText");
            lblDecember.Text = LocalizationManager.GetString("lblDecemberText");
        }
    }
}
