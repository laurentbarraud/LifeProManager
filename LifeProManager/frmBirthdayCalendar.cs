/// <file>frmBirthdayCalendar.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.1</version>
/// <date>May 23th, 2026</date>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LifeProManager
{
    public partial class frmBirthdayCalendar : Form
    {
        private readonly Dictionary<Button, string> _buttonBaseResourceNames = new Dictionary<Button, string>();

        private DBConnection dbConn => Program.DbConn;

        public frmBirthdayCalendar()
        {
            InitializeComponent();
        }

        private void frmBirthdayCalendar_Load(object sender, EventArgs e)
        {
            LocalizationManager.LoadLocalizedStringsFor(this);

            // Fills the birthdays progressively
            CreateBirthdaysLayout(dbConn.ReadTask("WHERE Priorities_id == 4 AND Status_id == 1"));

            // Original image path mapping
            _buttonBaseResourceNames[cmdValidate] = "validate_task";

            // Hover events for all buttons
            cmdValidate.MouseEnter += Button_MouseEnter;
            cmdValidate.MouseLeave += Button_MouseLeave;
        }

        /// <summary>
        /// Handles the mouse-enter event for the validate button by replacing its background image
        /// with the corresponding hover version.
        /// </summary>
        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null)
            {
                return;
            }

            string baseName;

            if (!_buttonBaseResourceNames.TryGetValue(btn, out baseName))
            {
                return;
            }

            // Hover effect
            Image hoverImage = Properties.Resources.ResourceManager.GetObject(baseName + "_hover") as Image;

            if (hoverImage != null)
            {
                btn.BackgroundImage = hoverImage;
            }
        }

        /// <summary>
        /// Handles the mouse-leave event for the validate button by restoring its original background
        /// image. 
        /// </summary>
        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null)
            {
                return;
            }

            string baseName;

            if (!_buttonBaseResourceNames.TryGetValue(btn, out baseName))
            {
                return;
            }

            Image normalImage = Properties.Resources.ResourceManager.GetObject(baseName) as Image;

            if (normalImage != null)
            {
                btn.BackgroundImage = normalImage;
            }
        }

        private void cmdValidate_Click(object sender, EventArgs e)
        {
            this.Close();
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
        /// Builds the birthday calendar by grouping each entry into its month label.
        /// Parses dates safely, computes the age for the current year, and appends
        /// a fully localized birthday line to the correct month.
        /// </summary>
        public void CreateBirthdaysLayout(List<Tasks> listOfBirthdays)
        {
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

                // Append to the correct month label
                if (monthLabels.TryGetValue(month, out Label targetLabel))
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
            lblBirthdayCalendar.Text = LocalizationManager.GetString("lblBirthdayCalendarText");

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
