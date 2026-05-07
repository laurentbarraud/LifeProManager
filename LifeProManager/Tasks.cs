/// <file>Tasks.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.</version>
/// <date>May 7th, 2026</date>


namespace LifeProManager
{
    /// <class>This class handles the values for each task</class>
    public class Tasks
    {
        private int id;
        private string title;
        private string description;
        private string deadline;
        private string validationDate;
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
            set => title = value; 
        }

        public string Description 
        { 
            get => description; 
            set => description = value; 
        }

        public string Deadline 
        { 
            get => deadline; 
            set => deadline = value; 
        }

        public string ValidationDate 
        { 
            get => validationDate; 
            set => validationDate = value; 
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
        { get => status_id; 
          set => status_id = value; 
        }
    }
}
