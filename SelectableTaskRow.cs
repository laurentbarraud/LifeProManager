/// <file>frmSelectableTaskRow.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.3</version>
/// <date>July 4th, 2026</date>

using System;
using System.Windows.Forms;

namespace LifeProManager
{
    public class SelectableTaskRow
    {
        public int TaskId;
        public Label? TitleLabel;
        public int Priority;
        public string Description = "";
        public Panel? RowPanel;
    }
}
