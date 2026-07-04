/// <file>Tasks.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.3</version>
/// <date>July 5th, 2026</date>


namespace LifeProManager
{
    /// <summary>
    /// This class handles the values for each task.
    /// </summary>
    public class Tasks
    {
        private int id;
        private string title = "";
        private string description = "";
        private string deadline = DateTime.Today.ToString("yyyy-MM-dd");
        private string validationDate = DateTime.Today.ToString("yyyy-MM-dd");
        private int priorities_id;
        private int lists_id;
        private int status_id;

        public int Id
        {
            get => id;
            set => id = value;
        }

        public string Title
        {
            get => title;
            set => title = value ?? "";
        }

        public string Description
        {
            get => description;
            set => description = value ?? "";
        }

        public string Deadline
        {
            get => deadline;
            set => deadline = value ?? DateTime.Today.ToString("yyyy-MM-dd");
        }

        public string ValidationDate
        {
            get => validationDate;
            set => validationDate = value ?? DateTime.Today.ToString("yyyy-MM-dd");
        }

        public int Priorities_id
        {
            get => priorities_id;
            set => priorities_id = value;
        }

        public int Lists_id
        {
            get => lists_id;
            set => lists_id = value;
        }

        public int Status_id
        {
            get => status_id;
            set => status_id = value;
        }
    }
}
