/// <file>Lists.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.</version>
/// <date>May 7th, 2026</date>

namespace LifeProManager
{
    /// <class>This class handles the lists of topics</class>
    public class Lists
    {
        private int id;
        private string title;

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

        /// <summary> 
        /// Override of the ToString method to display the title of the list
        /// in the comboboxes of the application 
        /// </summary>
        public override string ToString()
        {
            return Title;
        }
    }
}
