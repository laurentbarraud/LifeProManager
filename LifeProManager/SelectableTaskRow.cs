/// <file>frmSelectableTaskRow.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.1</version>
/// <date>May 23th, 2026</date>

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
