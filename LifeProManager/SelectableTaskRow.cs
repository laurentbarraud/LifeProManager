/// <file>frmSelectableTaskRow.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.2</version>
/// <date>May 29th, 2026</date>

using System;
using System.Windows.Forms;

namespace LifeProManager
{
    public class SelectableTaskRow
    {
        public int TaskId;
        public Label TitleLabel;
        public int Priority;
        public string Description;
        public Panel RowPanel;     // Keeps the selected row visible in scrolling
    }

}
