/// <file>LayoutBuilder.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.</version>
/// <date>May 7th, 2026</date>

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
        private readonly frmMain _frmMain;

        // Layout constants
        private const int ROW_HEIGHT = 32;
        private const int ICON_SIZE = 22;
        private const int BUTTON_SIZE = 25;
        private const int HORIZONTAL_GAP = 10;
        private const int VERTICAL_GAP = 12;
        private const int RIGHT_PADDING = 4;
        private const int DATE_LABEL_WIDTH = 95;

        public enum LayoutType
        {
            Topics = 0,
            Today = 1,
            Week = 2,
            Finished = 3,
            Search = 4
        }

        private readonly LayoutType layoutType;

        public LayoutBuilder(frmMain mainForm, LayoutType layoutType)
        {
            _frmMain = mainForm;
            this.layoutType = layoutType;
        }

        /// <summary>
        /// Adds approve/edit/delete/unapprove buttons to the right panel.
        /// </summary>
        private void AddButtons(Panel rightPanel, Tasks task, LayoutType targetLayout)
        {
            FlowLayoutPanel flowLayoutPnlButtons = new FlowLayoutPanel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = false,
                Width = (BUTTON_SIZE * 3) + (HORIZONTAL_GAP * 2),
                Height = ROW_HEIGHT,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            Button btnApprove = CreateTaskButton(Properties.Resources.validate_task);
            Button btnEdit = CreateTaskButton(Properties.Resources.edit_task);
            Button btnDelete = CreateTaskButton(Properties.Resources.delete_task);
            Button btnUnapprove = CreateTaskButton(Properties.Resources.unapprove_task);

            // Adds a hand cursor to action buttons for better feedback
            btnApprove.Cursor = Cursors.Hand;
            btnEdit.Cursor = Cursors.Hand;
            btnDelete.Cursor = Cursors.Hand;
            btnUnapprove.Cursor = Cursors.Hand;

            AttachButtonEvents(btnApprove, btnEdit, btnDelete, btnUnapprove, task);

            if (targetLayout == LayoutType.Finished)
            {
                flowLayoutPnlButtons.Controls.Add(btnUnapprove);
            }

            else
            {
                flowLayoutPnlButtons.Controls.Add(btnApprove);
            }

            flowLayoutPnlButtons.Controls.Add(btnEdit);
            flowLayoutPnlButtons.Controls.Add(btnDelete);

            rightPanel.Controls.Add(flowLayoutPnlButtons);

            // In Today/Week layouts, no date column is displayed.
            // The right panel is shrinked so it only matches the width of the buttons.
            if (targetLayout == LayoutType.Today || targetLayout == LayoutType.Week)
            {
                rightPanel.Width = flowLayoutPnlButtons.Width + RIGHT_PADDING;
            }

            flowLayoutPnlButtons.Left = rightPanel.Width - flowLayoutPnlButtons.Width - RIGHT_PADDING;
            flowLayoutPnlButtons.Top = (ROW_HEIGHT - flowLayoutPnlButtons.Height) / 2;
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
                Left = 0,
                Top = 0,
                Width = DATE_LABEL_WIDTH,
                Height = ROW_HEIGHT,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.Black
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
                picIcon.BackgroundImage = Properties.Resources.birthday_cake;
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
        /// Wires all hover and click events for task buttons.
        /// </summary>
        private void AttachButtonEvents(Button btnApprove, Button btnEdit, Button btnDelete, Button btnUnapprove, Tasks task)
        {
            // Hover effects
            btnApprove.MouseEnter += (s, e) => btnApprove.BackgroundImage = Properties.Resources.validate_task_hover;
            btnApprove.MouseLeave += (s, e) => btnApprove.BackgroundImage = Properties.Resources.validate_task;

            btnEdit.MouseEnter += (s, e) => btnEdit.BackgroundImage = Properties.Resources.edit_task_hover;
            btnEdit.MouseLeave += (s, e) => btnEdit.BackgroundImage = Properties.Resources.edit_task;

            btnDelete.MouseEnter += (s, e) => btnDelete.BackgroundImage = Properties.Resources.delete_task_hover;
            btnDelete.MouseLeave += (s, e) => btnDelete.BackgroundImage = Properties.Resources.delete_task;

            btnUnapprove.MouseEnter += (s, e) => btnUnapprove.BackgroundImage = Properties.Resources.unapprove_task_hover;
            btnUnapprove.MouseLeave += (s, e) => btnUnapprove.BackgroundImage = Properties.Resources.unapprove_task;

            // Click events
            btnApprove.Click += (s, e) =>
            {
                string validationDate = DateTime.Today.ToString("yyyy-MM-dd");
                _frmMain.dbConn.ApproveTask(task.Id, validationDate);
                _frmMain.LoadTasks();
                if (task.Priorities_id >= 2)
                {
                    _frmMain.AskForCopyingTask(task);
                }
            };

            btnEdit.Click += (s, e) => new frmEditTask(_frmMain, task).ShowDialog();

            btnDelete.Click += (s, e) =>
            {
                DialogResult result = MessageBox.Show(LocalizationManager.GetString("areYouSureDeleteTheTask"),
                    LocalizationManager.GetString("confirmDeletion"), MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    _frmMain.dbConn.DeleteTask(task.Id);
                    _frmMain.LoadTasks();
                }
            };

            btnUnapprove.Click += (s, e) =>
            {
                _frmMain.dbConn.UnapproveTask(task.Id);
                _frmMain.LoadTasks();
            };
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

                    _frmMain.ToggleSelection(newTaskId, parentPanel);
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
                new frmEditTask(_frmMain, task).ShowDialog();
            };
        }

        /// <summary>
        /// Creates the left panel containing the icon and title.
        /// </summary>
        private Panel CreateLeftPanel(Panel rowPanel)
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
        }

        /// <summary>
        /// Creates the right panel containing date and action buttons.
        /// </summary>
        private Panel CreateRightPanel(Panel rowPanel, LayoutType targetLayout)
        {
            // Determines the width of the right-side panel.
            // If the target layout is Today or Week, the date column is omitted, so only the three buttons are included,
            // else the date column width is added before the buttons.
            int rightPanelWidth = (targetLayout == LayoutType.Today || targetLayout == LayoutType.Week)
                ? (BUTTON_SIZE * 3) + (HORIZONTAL_GAP * 2) + RIGHT_PADDING
                : DATE_LABEL_WIDTH + (BUTTON_SIZE * 3) + (HORIZONTAL_GAP * 2) + RIGHT_PADDING;

            return new Panel
            {
                // MinimumSize ensures the right panel always keeps enough space for the date (when present)
                // and the action buttons.
                // Unlike a fixed Width, MinimumSize prevents the left panel (set to Dock.Fill)
                // from shrinking it below this size, while still allowing the panel to grow if needed.
                MinimumSize = new Size(rightPanelWidth, ROW_HEIGHT),
                Height = ROW_HEIGHT,
                Dock = DockStyle.Right,
                BackColor = Color.Transparent
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
        public Button CreateTaskButton(Image imgButton)
        {
            Button btn = new Button
            {
                Size = new Size(BUTTON_SIZE, BUTTON_SIZE),
                BackgroundImage = imgButton,
                BackgroundImageLayout = ImageLayout.Zoom,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                UseVisualStyleBackColor = false,
                Top = (ROW_HEIGHT - BUTTON_SIZE) / 2,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            return btn;
        }

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

                Panel rightPanel = CreateRightPanel(rowPanel, targetLayout);
                Panel leftPanel = CreateLeftPanel(rowPanel);

                rowPanel.Controls.Add(leftPanel);
                rowPanel.Controls.Add(rightPanel);

                AddDateLabelIfNeeded(rightPanel, task, targetLayout, parsedDeadline);
                AddButtons(rightPanel, task, targetLayout);
                AddIcon(leftPanel, task, parsedDeadline);

                Label lblTitle = CreateTitleLabel(leftPanel, targetLayout);
                ApplyTitleText(lblTitle, task);

                AttachSelectionHandlers(rowPanel, lblTitle, task.Id);
                RegisterSelectableRow(targetPanel, rowPanel, lblTitle, task);

                currentPosY += ROW_HEIGHT + VERTICAL_GAP;
            }
        }

        /// <summary>
        /// Creates the title label for a task.
        /// </summary>
        /// <param name="leftPanel"> Left panel to which the label will be added. </param>
        /// <param name="targetLayout"> The layout type, used to determine padding. </param>
        private Label CreateTitleLabel(Panel leftPanel, LayoutType targetLayout)
        {
            Label lblTitle = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11),
                AutoSize = false,
                Padding = new Padding(ICON_SIZE + HORIZONTAL_GAP, 0, 0, 0)
            };

            leftPanel.Controls.Add(lblTitle);
            return lblTitle;
        }

        /// <summary>
        /// Returns true if the task is a dummy placeholder (no results or search error).
        /// </summary>
        private bool IsDummyTask(Tasks task) => task.Id < 0;

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
        /// Also clears selection when using the search layout.
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

            return null;
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
