/// <file>frmCustomizeAppColors.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.3</version>
/// <date>July 4th, 2026</date>

using System;
using System.Configuration;

namespace LifeProManager
{
    public partial class frmCustomizeAppColors : Form
    {
        private readonly Dictionary<Button, string> _buttonBaseResourceNames = new Dictionary<Button, string>();
        private readonly frmMain? _frmMain;

        public frmCustomizeAppColors(Form callingForm)
        {
            // Allows to re-use the methods of frmMain
            _frmMain = callingForm as frmMain;
            InitializeComponent();
                       
            ApplyStoredColors();
            ApplyStoredColorsToButtonsBackground();

            cmdValidate.BackColor = this.BackColor;
        }

        /// <summary>
        /// Loads the preset themes into the ComboBox and restores 
        /// the last selected theme when the form is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmCustomizeAppColors_Load(object sender, EventArgs e)
        {
            LoadLocalizedStrings();

            // Loads the preset themes in the combobox
            cboPreSetThemes.Items.Clear();

            // Original images path mapping
            _buttonBaseResourceNames[cmdValidate] = "validate";

            // Hover events for validate button
            cmdValidate.MouseEnter += Button_MouseEnter;
            cmdValidate.MouseLeave += Button_MouseLeave;

            LoadThemesIntoComboBox();
            RestoreThemeSelection();
        }

        /// <summary>
        /// Applies one of the predefined color themes based on the ComboBox index.
        /// </summary>
        private void ApplyPresetTheme(int indexComboBox)
        {
            // Light Theme
            if (indexComboBox == 0)
            {
                Properties.Settings.Default.colorOfMainWindowBackground = "#FBFBFB";
                Properties.Settings.Default.colorOfRightPanelBackground = "#FBFBFB";
                Properties.Settings.Default.colorOfSecondaryWindowsBackground = "#F7F7F7";
                Properties.Settings.Default.colorOfPanelsBackground = "#FFFFFF";
                Properties.Settings.Default.colorOfInputFieldsBackground = "#FFFFFF";
                Properties.Settings.Default.colorOfLblTaskDescriptionBackground = "#FCFAF3";
                Properties.Settings.Default.colorOfInputFieldsText = "#1E1E1E";
                Properties.Settings.Default.colorOfTaskTitlesText = "#2A2A2A";
                Properties.Settings.Default.colorOfLblTaskDescriptionText = "#6E6E6E";
                Properties.Settings.Default.colorOfValidateButton = "#D0D0D0";
                
                Properties.Settings.Default.colorOfBirthdayCalendarBackground = "#FBFBFB";
                Properties.Settings.Default.colorOfBirthdayCalendarText = "#2A2A2A";
                Properties.Settings.Default.colorOfBirthdayCalendarSquaresPrimary = "#FFFFFF";
                Properties.Settings.Default.colorOfBirthdayCalendarSquaresSecondary = "#F5F5F5";
                Properties.Settings.Default.colorOfBirthdayCalendarSquaresTertiary = "#EBEBEB";
            }

            // Dark Theme
            if (indexComboBox == 1)
            {
                // Inspired by Google dark mode: deep charcoal, soft contrasts
                Properties.Settings.Default.colorOfMainWindowBackground = "#202124";
                Properties.Settings.Default.colorOfRightPanelBackground = "#202124";
                Properties.Settings.Default.colorOfSecondaryWindowsBackground = "#303134";
                Properties.Settings.Default.colorOfPanelsBackground = "#202124";
                Properties.Settings.Default.colorOfInputFieldsBackground = "#303134";
                Properties.Settings.Default.colorOfLblTaskDescriptionBackground = "#303134";
                Properties.Settings.Default.colorOfInputFieldsText = "#E8EAED";
                Properties.Settings.Default.colorOfTaskTitlesText = "#E8EAED";
                Properties.Settings.Default.colorOfLblTaskDescriptionText = "#E8EAED";
                Properties.Settings.Default.colorOfValidateButton = "#62686C";
                
                Properties.Settings.Default.colorOfBirthdayCalendarBackground = "#202124";
                Properties.Settings.Default.colorOfBirthdayCalendarText = "#E8EAED";
                Properties.Settings.Default.colorOfBirthdayCalendarSquaresPrimary = "#2A2C2F";
                Properties.Settings.Default.colorOfBirthdayCalendarSquaresSecondary = "#35383C";
                Properties.Settings.Default.colorOfBirthdayCalendarSquaresTertiary = "#40444A";
            }

            // Blue Theme
            if (indexComboBox == 2)
            {
                Properties.Settings.Default.colorOfMainWindowBackground = "#F5F7FA";
                Properties.Settings.Default.colorOfRightPanelBackground = "#F5F7FA";
                Properties.Settings.Default.colorOfSecondaryWindowsBackground = "#E6EBEF";
                Properties.Settings.Default.colorOfPanelsBackground = "#FFFFFF";
                Properties.Settings.Default.colorOfInputFieldsBackground = "#FFFFFF";
                Properties.Settings.Default.colorOfLblTaskDescriptionBackground = "#F1E9D2";
                Properties.Settings.Default.colorOfInputFieldsText = "#1E1E1E";
                Properties.Settings.Default.colorOfTaskTitlesText = "#2F4F57";
                Properties.Settings.Default.colorOfLblTaskDescriptionText = "#728FA7";
                Properties.Settings.Default.colorOfValidateButton = "#B6C0C1";
                
                Properties.Settings.Default.colorOfBirthdayCalendarBackground = "#F5F7FA";
                Properties.Settings.Default.colorOfBirthdayCalendarText = "#2F4F57";
                Properties.Settings.Default.colorOfBirthdayCalendarSquaresPrimary = "#DFF3EF";
                Properties.Settings.Default.colorOfBirthdayCalendarSquaresSecondary = "#E7EEF8";
                Properties.Settings.Default.colorOfBirthdayCalendarSquaresTertiary = "#F7F9E8";
            }

            // Saves selected theme index
            Properties.Settings.Default.selectedThemeIndex = indexComboBox;
            Properties.Settings.Default.Save();

            // Refreshes UI
            _frmMain?.ApplyStoredColors();
            _frmMain?.LoadTasks();
        }

        /// <summary>
        /// Applies the stored theme colors by reading the hex values from application settings
        /// and updating the controls of this window accordingly.
        /// </summary>
        internal void ApplyStoredColors()
        {
            Color secondaryWindowsBackgroundColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfSecondaryWindowsBackground);
            Color inputFieldsBackground = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsBackground);
            Color inputFieldsForeground = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsText);
            Color taskTitlesTextColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfTaskTitlesText);
            Color panelsBackgroundColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfPanelsBackground);

            // Background of the window
            this.BackColor = secondaryWindowsBackgroundColor;

            void ApplyThemeToControl(Control ctrl)
            {
                if (ctrl is ComboBox)
                {
                    ctrl.BackColor = inputFieldsBackground;
                    ctrl.ForeColor = inputFieldsForeground;
                }

                else if (ctrl is Label || ctrl is CheckBox)
                {
                    ctrl.ForeColor = taskTitlesTextColor;
                }

                else if (ctrl is Panel)
                {
                    ctrl.BackColor = panelsBackgroundColor;
                }

                foreach (Control childCtrl in ctrl.Controls)
                {
                    ApplyThemeToControl(childCtrl);
                }
            }

            foreach (Control ctrl in this.Controls)
            {
                ApplyThemeToControl(ctrl);
            }   


            // Main window Color buttons
            cmdColorOfMainWindowBackground.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfMainWindowBackground);

            cmdColorOfRightPanelBackground.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfRightPanelBackground);

            cmdColorOfInputFieldsBackground.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsBackground);

            cmdColorOfLblTaskDescriptionBackground.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfLblTaskDescriptionBackground);

            cmdColorOfTaskTitlesText.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfTaskTitlesText);

            cmdColorOfInputFieldsText.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsText);

            cmdColorOfLblTaskDescriptionText.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfLblTaskDescriptionText);

            cmdColorOfValidateButton.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfValidateButton);

            cmdColorOfSecondaryWindowsBackground.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfSecondaryWindowsBackground);

            // Birthday calendar color buttons
            cmdColorOfBirthdayCalendarBackground.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfBirthdayCalendarBackground);

            cmdColorOfBirthdayCalendarSquaresPrimary.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfBirthdayCalendarSquaresPrimary);

            cmdColorOfBirthdayCalendarSquaresSecondary.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfBirthdayCalendarSquaresSecondary);

            cmdColorOfBirthdayCalendarSquaresTertiary.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfBirthdayCalendarSquaresTertiary);

            cmdColorOfBirthdayCalendarText.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfBirthdayCalendarText);

            // Labels text color
            Color labelsColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfTaskTitlesText);

            lblColorOfMainWindowBackground.ForeColor = labelsColor;
            lblColorOfRightPanelBackground.ForeColor = labelsColor;
            lblColorOfInputFieldsBackground.ForeColor = labelsColor;
            lblColorOfLblTaskDescriptionBackground.ForeColor = labelsColor;
            lblColorOfTaskTitlesBackground.ForeColor = labelsColor;
            lblColorOfTaskTitlesText.ForeColor = labelsColor;
            lblColorOfInputFieldsText.ForeColor = labelsColor;
            lblColorOfLblTaskDescriptionText.ForeColor = labelsColor;
            lblColorOfValidateButton.ForeColor = labelsColor;
            lblColorOfSecondaryWindowsBackground.ForeColor = labelsColor;
            lblColorOfBirthdayCalendarBackground.ForeColor = labelsColor;
            lblColorOfBirthdayCalendarSquaresPrimary.ForeColor = labelsColor;
            lblColorOfBirthdayCalendarSquaresSecondary.ForeColor = labelsColor;
            lblColorOfBirthdayCalendarSquaresTertiary.ForeColor = labelsColor;
            lblColorOfBirthdayCalendarText.ForeColor = labelsColor;

            // ComboBox
            cboPreSetThemes.BackColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsBackground);

            cboPreSetThemes.ForeColor =
                ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsText);

            // Forces redraw
            this.Invalidate();
        }

        /// <summary>
        /// Applies the stored color settings to the background of all color-selection buttons.
        /// This method relies on the naming convention:
        /// Setting:  colorOfXxxxx
        /// Button:   cmdColorOfXxxxx
        /// </summary>
        private void ApplyStoredColorsToButtonsBackground()
        {
            var settings = Properties.Settings.Default;

            // Iterates through all user settings
            foreach (SettingsProperty prop in settings.Properties)
            {
                string settingName = prop.Name;

                // Only process color settings
                if (!settingName.StartsWith("colorOf"))
                {
                    continue;
                }

                // Builds the expected button name (cmdColorOfXxxxx)
                string buttonName = "cmd" + char.ToUpper(settingName[0]) + settingName.Substring(1);

                // Searches for the button in all nested containers
                Control[] foundControl = this.Controls.Find(buttonName, true);

                if (foundControl.Length == 0)
                {
                    continue;
                }

                if (foundControl[0] is Button btn)
                {
                    string? colorHex = settings[settingName] as string;

                    if (!string.IsNullOrEmpty(colorHex))
                    {
                        try
                        {
                            // Converts the stored hex string to a Color and apply it
                            btn.BackColor = System.Drawing.ColorTranslator.FromHtml(colorHex);
                        }
                        catch
                        {
                            // Ignores invalid color values to avoid runtime errors
                        }
                    }
                }
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

        /// <summary>
        /// Applies the stored colors for the selected preset theme 
        /// when the user changes the selection in the ComboBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboPreSetThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPreSetThemes.SelectedIndex < 0)
            {
                return;
            }

            // Applies the preset theme by writing all the color settings to the application settings
            ApplyPresetTheme(cboPreSetThemes.SelectedIndex);

            // Refreshes the color buttons in this window
            ApplyStoredColors();

            // Refreshes Validate buttons in the main window
            _frmMain?.RefreshAllValidateButtons();
        }

        /// <summary>
        /// Opens a color picker, saves the selected color in application settings,
        /// updates the preview button, switches the theme preset to "Custom",
        /// and reloads only the UI zone impacted by this color.
        /// </summary>
        internal void ChangeColor(string settingKey, Button previewButton)
        {
            using (ColorDialog colorDlg = new ColorDialog())
            {
                // Preselects current color
                string currentColorHex = Properties.Settings.Default[settingKey]?.ToString() ?? "#FFFFFF";
                Color currentColor = ColorTranslator.FromHtml(currentColorHex);
                colorDlg.Color = currentColor;
                colorDlg.FullOpen = true;

                // User cancelled
                if (colorDlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                // Converts each RGB component to 2-character hexadecimal (X2).
                // Example: 5 → "05", 12 → "0C", 255 → "FF".
                // Allows to get a valid HTML color code in the #RRGGBB format.
                string newColorHex = $"#{colorDlg.Color.R:X2}{colorDlg.Color.G:X2}{colorDlg.Color.B:X2}";

                // Saves new color
                Properties.Settings.Default[settingKey] = newColorHex;
                Properties.Settings.Default.Save();

                // Updates preview square
                previewButton.BackColor = colorDlg.Color;
                
                MarkThemeAsCustom();

                // Reloads only the impacted UI zone
                _frmMain?.ReloadUI(settingKey);
            }
        }

        private void cmdColorOfBirthdayCalendarBackground_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfBirthdayCalendarBackground", cmdColorOfBirthdayCalendarBackground);
        }

        private void cmdColorOfBirthdayCalendarSquaresPrimary_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfBirthdayCalendarSquaresPrimary", cmdColorOfBirthdayCalendarSquaresPrimary);
        }

        private void cmdColorOfBirthdayCalendarSquaresSecondary_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfBirthdayCalendarSquaresSecondary", cmdColorOfBirthdayCalendarSquaresSecondary);
        }

        private void cmdColorOfBirthdayCalendarSquaresTertiary_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfBirthdayCalendarSquaresTertiary", cmdColorOfBirthdayCalendarSquaresTertiary);
        }

        private void cmdColorOfBirthdayCalendarText_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfBirthdayCalendarText", cmdColorOfBirthdayCalendarText);
        }

        private void cmdColorOfInputFieldsBackground_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfInputFieldsBackground", cmdColorOfInputFieldsBackground);
        }

        private void cmdColorOfInputFieldsText_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfInputFieldsText", cmdColorOfInputFieldsText);
        }

        private void cmdColorOfLblTaskDescriptionBackground_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfLblTaskDescriptionBackground", cmdColorOfLblTaskDescriptionBackground);
        }

        private void cmdColorOfLblTaskDescriptionText_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfLblTaskDescriptionText", cmdColorOfLblTaskDescriptionText);
        }

        private void cmdColorOfMainWindowBackground_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfMainWindowBackground", cmdColorOfMainWindowBackground);
        }

        private void cmdColorOfRightPanelBackground_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfRightPanelBackground", cmdColorOfRightPanelBackground);
        }

        private void cmdColorOfSecondaryWindowsBackground_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfSecondaryWindowsBackground", cmdColorOfSecondaryWindowsBackground);
        }

        private void cmdColorOfTaskTitlesBackground_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfTaskTitlesBackground", cmdColorOfTaskTitlesBackground);
        }

        private void cmdColorOfTaskTitlesText_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfTaskTitlesText", cmdColorOfTaskTitlesText);
        }

        private void cmdColorOfValidateButton_Click(object sender, EventArgs e)
        {
            ChangeColor("colorOfValidateButton", cmdColorOfValidateButton);
        }

        private void cmdValidate_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Loads all the localized strings for the UI elements based on the current language setting.
        /// </summary>
        public void LoadLocalizedStrings()
        {
            // --- Window title ---
            this.Text = LocalizationManager.GetString("CustomizeAppColors");

            // --- Labels ---
            lblColorOfMainWindowBackground.Text = LocalizationManager.GetString("ColorOfMainWindowBackground");
            lblColorOfRightPanelBackground.Text = LocalizationManager.GetString("ColorOfRightPanelBackground");
            lblColorOfSecondaryWindowsBackground.Text = LocalizationManager.GetString("ColorOfSecondaryWindowsBackground");
            lblColorOfTaskTitlesBackground.Text = LocalizationManager.GetString("ColorOfTaskTitlesBackground");
            lblColorOfInputFieldsBackground.Text = LocalizationManager.GetString("ColorOfInputFieldsBackground");
            lblColorOfLblTaskDescriptionBackground.Text = LocalizationManager.GetString("ColorOfPnlTaskDescriptionBackground");
            lblColorOfTaskTitlesText.Text = LocalizationManager.GetString("ColorOfTaskTitles");
            lblColorOfInputFieldsText.Text  = LocalizationManager.GetString("ColorOfInputFieldsText");
            lblColorOfLblTaskDescriptionText.Text = LocalizationManager.GetString("ColorOfPnlTaskDescription");
            lblColorOfValidateButton.Text = LocalizationManager.GetString("ColorOfValidateButton");
            lblColorOfBirthdayCalendarBackground.Text = LocalizationManager.GetString("ColorOfBirthdayCalendarBackground");
            lblColorOfBirthdayCalendarSquaresPrimary.Text = LocalizationManager.GetString("ColorOfBirthdayCalendarSquaresPrimary");
            lblColorOfBirthdayCalendarSquaresSecondary.Text = LocalizationManager.GetString("ColorOfBirthdayCalendarSquaresSecondary");
            lblColorOfBirthdayCalendarSquaresTertiary.Text = LocalizationManager.GetString("ColorOfBirthdayCalendarSquaresTertiary");
            lblColorOfBirthdayCalendarText.Text = LocalizationManager.GetString("ColorOfBirthdayCalendarText");
            lblSelectPreSetTheme.Text = LocalizationManager.GetString("SelectPreSetTheme");
        }

        /// <summary>
        /// Loads the localized theme names into the ComboBox.
        /// </summary>
        private void LoadThemesIntoComboBox()
        {
            cboPreSetThemes.Items.Clear();
            cboPreSetThemes.Items.Add(LocalizationManager.GetString("LightTheme"));
            cboPreSetThemes.Items.Add(LocalizationManager.GetString("DarkTheme"));
            cboPreSetThemes.Items.Add(LocalizationManager.GetString("BlueTheme"));
        }

        /// <summary>
        /// Marks the current theme as custom after a manual color change.
        /// </summary>
        private void MarkThemeAsCustom()
        {
            // Saves custom state
            Properties.Settings.Default.selectedThemeIndex = 99;
            Properties.Settings.Default.Save();

            // Updates ComboBox display
            cboPreSetThemes.SelectedIndex = -1;
            cboPreSetThemes.Text = LocalizationManager.GetString("Custom");
        }

        /// <summary>
        /// Restores the ComboBox selection based on the saved theme index.
        /// </summary>
        private void RestoreThemeSelection()
        {
            int savedSelectedThemeIndex = Properties.Settings.Default.selectedThemeIndex;

            if (savedSelectedThemeIndex >= 0 && savedSelectedThemeIndex <= 2)
            {
                cboPreSetThemes.SelectedIndex = savedSelectedThemeIndex;
            }
            else
            {
                cboPreSetThemes.SelectedIndex = -1;
                cboPreSetThemes.Text = LocalizationManager.GetString("Custom");
            }
        }
    }
}
