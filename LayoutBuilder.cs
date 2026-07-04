/// <file>LayoutBuilder.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.3</version>
/// <date>July 5th, 2026</date>

using System;

namespace LifeProManager
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    public class LayoutBuilder
    {
        // Private members

        // Reference to the main form for event handling
        private readonly frmMain _frmMain;

        // Layout constants
        private const int ROW_HEIGHT = 32;
        private const int ICON_SIZE = 22;
        internal const int BUTTON_SIZE = 28;
        private const int HORIZONTAL_GAP = 10;
        private const int VERTICAL_GAP = 12;
        private const int DATE_LABEL_WIDTH = 95;

        private readonly LayoutType layoutType;

        /// <summary>
        /// Returns true if the task is a dummy placeholder (no results or search error).
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool IsDummyTask(Tasks task) => task.Id < 0;

        // Public properties and enum
        public enum LayoutType
        {
            Topics = 0,
            Today = 1,
            Week = 2,
            Finished = 3,
            Search = 4
        }

        public LayoutBuilder(frmMain mainForm, LayoutType layoutType)
        {
            _frmMain = mainForm;
            this.layoutType = layoutType;
        }

        /// <summary>
        /// Adds the appropriate action button to the right panel of a task row.
        /// </summary>
        private void AddButton(Panel rightPanel, Tasks task, LayoutType targetLayout, Panel rowPanel)
        {
            // Determines which icon to use depending on the layout type
            Button btn;

            if (targetLayout == LayoutType.Finished)
            {
                btn = CreateTaskButton(Properties.Resources.validateFilled);
                btn.Enabled = false;
                btn.Cursor = Cursors.Default;
            }
            
            else
            {
                btn = CreateTaskButton(Properties.Resources.validateTask);
                btn.Cursor = Cursors.Hand;

                _frmMain.ApplyValidateButtonColor(btn);

                Image? normalImage = btn.Image;

                btn.MouseEnter += (s, e) =>
                {
                    btn.Image = Properties.Resources.validateTaskHover;
                };

                btn.MouseLeave += (s, e) =>
                {
                    btn.Image = normalImage;
                };

                btn.Click += (s, e) =>
                {
                    _frmMain.ToggleSelection(task.Id);
                    _frmMain.ValidateTask_Click(s, e);
                };
            }

            // Prevents WinForms from stretching the button vertically or horizontally
            btn.MinimumSize = new Size(BUTTON_SIZE, BUTTON_SIZE);
            btn.MaximumSize = new Size(BUTTON_SIZE, BUTTON_SIZE);

            rightPanel.Controls.Add(btn);

            // Ensures the button is always visible above other controls
            btn.BringToFront();

            btn.Dock = DockStyle.Right;
            btn.Margin = new Padding(0, 0, 20, 0);
        }

        /// <summary>
        /// Adds a date label to the right panel when required by the layout.
        /// </summary>
        private void AddDateLabelIfNeeded(Panel rightPanel, Tasks task, LayoutType targetLayout, DateTime deadline)
        {
            if (targetLayout == LayoutType.Today || targetLayout == LayoutType.Week)
            {
                return;
            }

            Label lblDate = new Label
            {
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                Padding = new Padding(0, 0, 8, 0),
                AutoSize = false,
                Width = DATE_LABEL_WIDTH + 10
            };

            if (targetLayout == LayoutType.Topics)
            {
                string langCode = LocalizationManager.GetCurrentLanguageCode();
                CultureInfo definedCulture = new CultureInfo(langCode);

                string dateCulturalFormat = (definedCulture.TwoLetterISOLanguageName == "fr"
                                             || definedCulture.TwoLetterISOLanguageName == "es")
                    ? "dd/MM/yyyy"
                    : "MM/dd/yyyy";

                lblDate.Text = deadline.ToString(dateCulturalFormat);
            }


            else if (targetLayout == LayoutType.Finished && DateTime.TryParse(task.ValidationDate, out DateTime validationDate))
            {
                string langCode = LocalizationManager.GetCurrentLanguageCode();
                CultureInfo definedCulture = new CultureInfo(langCode);
                string dateCulturalFormat = (definedCulture.TwoLetterISOLanguageName == "fr" || definedCulture.TwoLetterISOLanguageName == "es")
                    ? "dd/MM/yyyy"
                    : "MM/dd/yyyy";

                lblDate.Text = validationDate.ToString(dateCulturalFormat);
            }

            rightPanel.Controls.Add(lblDate);
        }

        /// <summary>
        /// Adds a dummy row (no results / search error) to the panel.
        /// </summary>
        private void AddDummyTaskRow(Panel targetPanel, Tasks task, ref int currentY)
        {
            Label lblDummyTask = new Label
            {
                Text = task.Title,
                AutoSize = true,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                Left = 20,
                Top = currentY,
                Cursor = Cursors.Hand
            };

            lblDummyTask.Click += (s, e) => _frmMain.TriggerTodayClick();

            targetPanel.Controls.Add(lblDummyTask);
            currentY += ROW_HEIGHT + VERTICAL_GAP;
        }

        /// <summary>
        /// Adds the appropriate icon to the left panel based on priority and deadline.
        /// </summary>
        private void AddIcon(Panel leftPanel, Tasks task, DateTime deadline)
        {
            PictureBox picIcon = new PictureBox
            {
                Size = new Size(ICON_SIZE, ICON_SIZE),
                Dock = DockStyle.Left,
                Margin = new Padding(0, (ROW_HEIGHT - ICON_SIZE) / 2, 10, 0),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.Transparent
            };

            if (task.Priorities_id == 4)
            {
                picIcon.BackgroundImage = Properties.Resources.birthdayCake;
            }
            else if (deadline < DateTime.Today)
            {
                picIcon.BackgroundImage = Properties.Resources.clock;
            }
            else if (task.Priorities_id % 2 != 0)
            {
                picIcon.BackgroundImage = Properties.Resources.important;
            }

            leftPanel.Controls.Add(picIcon);
        }

        /// <summary>
        /// Applies the correct title text, including birthday age logic.
        /// </summary>
        private void ApplyTitleText(Label lbl, Tasks task)
        {
            // If it's a birthday task and parsing succeeds
            if (task.Priorities_id == 4 && int.TryParse(task.Description, out int birthYear))
            {
                int ageReached = DateTime.Now.Year - birthYear;

                // Displays the first name and the age they'll reach this year
                lbl.Text = $"{task.Title} ({ageReached})";
            }
            else
            {
                lbl.Text = task.Title;
            }
        }

        /// <summary>
        /// Wires click and double‑click events for task selection and editing.
        /// </summary>
        private void AttachSelectionHandlers(Panel rowPanel, Label lblTitle, int taskId)
        {
            // Local function used by both click handlers
            void HandleSelectionClick()
            {
                // Determines the owning task list panel explicitly
                if (rowPanel.Parent is Panel parentPanel)
                {
                    int newTaskId;

                    if (_frmMain.IsTaskSelected(taskId))
                    {
                        // Clicking the already selected task unselects it
                        newTaskId = -1;
                    }
                    else
                    {
                        // Selects this task
                        newTaskId = taskId;
                    }

                    _frmMain.ToggleSelection(newTaskId);
                }
            }

            // Click on the title label selects/unselects the task
            lblTitle.Click += (s, e) =>
            {
                HandleSelectionClick();
            };

            // Click on the row panel selects/unselects the task
            rowPanel.Click += (s, e) =>
            {
                HandleSelectionClick();
            };

            // Double‑click on the title opens the edit dialog
            lblTitle.DoubleClick += (s, e) =>
            {
                var task = _frmMain.dbConn.ReadTaskById(taskId);
                
                if (task == null)
                {
                    return;
                }

                new frmEditTask(_frmMain, task).ShowDialog();
            };
        }

        /// <summary>
        /// Creates the left panel containing the icon and title.
        /// </summary>
        private Panel CreateLeftPanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
        }

        /// <summary>
        /// Creates the right panel containing the date label (when applicable)
        /// and the action button or filled validation icon depending on the layout.
        /// </summary>
        private Panel CreateRightPanel(LayoutType targetLayout)
        {
            // The right panel will automatically size itself based on its children
            // (date label and button). This avoids reserving extra blank space.
            return new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Height = ROW_HEIGHT,
                Dock = DockStyle.Right,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 5, 0)
            };
        }

        /// <summary>
        /// Creates the main row panel for a task.
        /// </summary>
        private Panel CreateRowPanel(Panel targetPanel, int y)
        {
            return new Panel
            {
                Left = 10,
                Top = y,
                Width = targetPanel.ClientSize.Width - 20,
                Height = ROW_HEIGHT,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
        }

        /// <summary>
        /// Creates a task button with standard styling.
        /// </summary>
        /// <param name="imgButton">The image to display on the button.</param>
        public Button CreateTaskButton(Image imgButton)
        {
            Button btn = new Button
            {
                Size = new Size(BUTTON_SIZE, BUTTON_SIZE),
                AutoSize = false,
                Image = imgButton,
                ImageAlign = ContentAlignment.MiddleCenter,
                Text = string.Empty,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                TabStop = false,
                BackColor = Color.Transparent,
                UseVisualStyleBackColor = false,
                FlatStyle = FlatStyle.Flat,
            };

            // Removes all borders and background painting
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;

            // Prevents WinForms from stretching the button
            btn.MinimumSize = new Size(BUTTON_SIZE, BUTTON_SIZE);
            btn.MaximumSize = new Size(BUTTON_SIZE, BUTTON_SIZE);

            return btn;
        }

        /// <summary>
        /// Orchestrates the construction of the task layout for the specified panel.
        /// Delegates UI creation, button logic, date formatting, icon selection and event wiring
        /// to dedicated helper methods for clarity and maintainability.
        /// </summary>
        /// <summary>
        /// Orchestrates the construction of the task layout for the specified panel.
        /// Delegates UI creation, button logic, date formatting, icon selection and event wiring
        /// to dedicated helper methods for clarity and maintainability.
        /// </summary>
        public void CreateTasksLayout(List<Tasks> tasksList, LayoutType targetLayout)
        {
            Panel targetPanel = ResolveTargetPanel(targetLayout);

            if (targetPanel == null)
            {
                return;
            }

            ResetSelectionState(targetPanel);
            targetPanel.Controls.Clear();

            int currentPosY = 10;

            foreach (var task in tasksList)
            {
                if (IsDummyTask(task))
                {
                    // Dummy rows only allowed in Today panel
                    if (targetLayout == LayoutType.Today)
                    {
                        AddDummyTaskRow(targetPanel, task, ref currentPosY);
                    }
                    continue;
                }

                bool parsingOfDeadLineSucceeded = TryParseDeadline(task, out DateTime parsedDeadline);

                if (!parsingOfDeadLineSucceeded)
                {
                    continue;
                }

                Panel rowPanel = CreateRowPanel(targetPanel, currentPosY);
                targetPanel.Controls.Add(rowPanel);

                Panel rightPanel = CreateRightPanel(targetLayout);
                Panel leftPanel = CreateLeftPanel();

                rowPanel.Controls.Add(leftPanel);
                rowPanel.Controls.Add(rightPanel);

                AddDateLabelIfNeeded(rightPanel, task, targetLayout, parsedDeadline);
                AddButton(rightPanel, task, targetLayout, rowPanel);
                AddIcon(leftPanel, task, parsedDeadline);

                Label lblTitle = CreateTitleLabel(leftPanel);
                ApplyTitleText(lblTitle, task);

                AttachSelectionHandlers(rowPanel, lblTitle, task.Id);

                // Attach the handler to the title label instead of the row panel
                // to prevent the context menu from opening unintentionally.
                lblTitle.MouseDown += LblTitleTask_MouseDown;

                RegisterSelectableRow(targetPanel, rowPanel, lblTitle, task);

                currentPosY += ROW_HEIGHT + VERTICAL_GAP;
            }
        }

        /// <summary>
        /// Creates the title label for a task.
        /// </summary>
        /// <param name="leftPanel"> Left panel to which the label will be added. </param>
        private Label CreateTitleLabel(Panel leftPanel)
        {
            Label lblTitle = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfTaskTitlesText),
                Font = new Font("Segoe UI", 11),
                AutoSize = false,
                Padding = new Padding(ICON_SIZE + HORIZONTAL_GAP, 0, 15, 0)
            };

            leftPanel.Controls.Add(lblTitle);
            return lblTitle;
        }

        /// <summary>
        /// Finds the SelectableTaskRow associated with a given title label.
        /// This allows right‑click logic to retrieve the correct task.
        /// </summary>
        private SelectableTaskRow? FindRowByLabel(Label lbl)
        {
            // The parent of the label is the left panel, and its parent is the row panel.
            Panel? rowPanel = lbl.Parent?.Parent as Panel;
            
            if (rowPanel == null)
            {
                return null;
            }

            // The parent of the row panel is the task list panel (Today, Week, etc.)
            Panel? parentPanel = rowPanel.Parent as Panel;
            
            if (parentPanel == null)
            {
                return null;
            }

            // Searches for the SelectableTaskRow in the selection structure for this panel.
            if (_frmMain.selectionByPanel.TryGetValue(parentPanel, out var panelRows))
            {
                foreach (var row in panelRows)
                {
                    // If the title label matches, we found the corresponding row.
                    if (row.TitleLabel == lbl)
                    {
                        return row;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Handles right‑clicks on a task title label.
        /// Selects the corresponding task, configures the context menu based on the
        /// parent panel and opens the menu at the mouse location. 
        /// This ensures the context menu only appears when a task is explicitly 
        /// selected by the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblTitleTask_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            // Ensures sender is a Label
            if (sender is not Label lblTask)
            {
                return;
            }

            // Retrieves the SelectableTaskRow associated with this label.
            if (FindRowByLabel(lblTask) is SelectableTaskRow associatedRow)
            {
                // Retrieves the parent panel of the row using pattern matching.
                // Pattern matching ensures both RowPanel and its Parent are non‑null
                // before accessing them, preventing any potential null‑reference issues.
                // This guarantees that the correct panel is used when selecting the task.
                if (associatedRow.RowPanel is Panel rowPanel &&
                    rowPanel.Parent is Panel associatedRowParentPanel)
                {
                    _frmMain.ToggleSelection(associatedRow.TaskId);
                }
            }

            // Determines the parent panel
            if (lblTask.Parent?.Parent?.Parent is not Panel parentPanel)
            {
                return;
            }

            // Finished layout: shows relevant contextual menu options
            if (parentPanel == _frmMain.pnlFinished)
            {
                _frmMain.ValidateTask.Visible = false;
                _frmMain.ReassignTask.Visible = true;
                _frmMain.EditTask.Visible = true;
                _frmMain.DeleteTask.Visible = true;
            }
            
            else
            {
                // Others layouts
                _frmMain.ValidateTask.Visible = true;
                _frmMain.ReassignTask.Visible = false;
                _frmMain.EditTask.Visible = true;
                _frmMain.DeleteTask.Visible = true;
            }

            // Opens the context menu at the mouse location.
            _frmMain.cmsTasksOptions.Show(lblTask, e.Location);
        }

        /// <summary>
        /// Registers the row in the selection structure for the panel.
        /// </summary>
        private void RegisterSelectableRow(Panel targetPanel, Panel rowPanel, Label lblTitle, Tasks task)
        {
            _frmMain.selectionByPanel[targetPanel].Add(new SelectableTaskRow
            {
                TaskId = task.Id,
                TitleLabel = lblTitle,
                Priority = task.Priorities_id,
                Description = task.Description,
                RowPanel = rowPanel
            });
        }

        /// <summary>
        /// Clears selection state and resets the selectable rows list for the given panel.
        /// Ensures the panel entry exists in the selection dictionary.
        /// </summary>
        private void ResetSelectionState(Panel targetPanel)
        {
            // Ensures the dictionary contains an entry for this panel
            if (!_frmMain.selectionByPanel.ContainsKey(targetPanel))
            {
                _frmMain.selectionByPanel[targetPanel] = new List<SelectableTaskRow>();
            }
            else
            {
                _frmMain.selectionByPanel[targetPanel].Clear();
            }

            // Resets internal selection state and hides the description label
            _frmMain.ResetSelection();
        }

        /// <summary>
        /// Returns the panel corresponding to the requested layout.
        /// </summary>
        private Panel ResolveTargetPanel(LayoutType targetLayout)
        {
            if (targetLayout == LayoutType.Today)
            {
                return _frmMain.pnlToday;
            }

            if (targetLayout == LayoutType.Week)
            {
                return _frmMain.pnlWeek;
            }

            if (targetLayout == LayoutType.Topics)
            {
                return _frmMain.pnlTopics;
            }

            if (targetLayout == LayoutType.Finished)
            {
                return _frmMain.pnlFinished;
            }

            if (targetLayout == LayoutType.Search)
            {
                _frmMain.ResetSelection();
                return _frmMain.pnlToday;
            }

            // Default fallback to Today panel if the layout type is unrecognized
            return _frmMain.pnlToday;
        }



        /// <summary>
        /// Parses the task deadline into a DateTime.
        /// Dummy tasks are always accepted even without a valid deadline.
        /// Real tasks must have a valid, non-empty deadline.
        /// </summary>
        /// <returns>True if the parse succeeds or it's a dummy task</returns>
        private bool TryParseDeadline(Tasks task, out DateTime deadline)
        {
            // Dummy tasks: no deadline required
            if (IsDummyTask(task))
            {
                deadline = DateTime.MinValue;
                return true;
            }

            // Real tasks: deadline must exist
            if (string.IsNullOrWhiteSpace(task.Deadline))
            {
                deadline = DateTime.MinValue;
                return false;
            }

            // Real tasks: deadline must be valid
            if (DateTime.TryParse(task.Deadline, out deadline))
            {
                deadline = deadline.Date;
                return true;
            }

            // Invalid deadline for a real task is rejected
            deadline = DateTime.MinValue;
            return false;
        }
    }
}
