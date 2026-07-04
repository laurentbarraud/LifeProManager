/// <file>UIHover.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.3</version>
/// <date>July 5th, 2026</date>

using System;

namespace LifeProManager
{
    internal static class UIHover
    {
        /// <summary>
        /// Handle mouse enter event for buttons, changing the background image to the hover version.
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="map"></param>
        public static void HandleMouseEnter(Button btn, Dictionary<Button, string> map)
        {
            // If the key exists, baseName gets the value.
            // If not, baseName is null and TryGetValue returns false.
            if (map.TryGetValue(btn, out string? baseName))
            {
                Image? hoverImage = Properties.Resources.ResourceManager.GetObject(baseName + "Hover") as Image;

                if (hoverImage != null)
                {
                    btn.BackgroundImage = hoverImage;
                }
            }
        }

        /// <summary>
        /// Handle mouse enter event for buttons on the main form, 
        /// with special behavior for certain buttons when Ctrl or Shift is held down.
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="map"></param>
        public static void HandleMouseEnterMainForm(Button btn, Dictionary<Button, string> map)
        {
            if (!map.TryGetValue(btn, out string? baseName) || baseName is null)
            {
                return;
            }

            bool ctrlHold = Control.ModifierKeys == Keys.Control;
            bool shiftHold = Control.ModifierKeys == Keys.Shift;

            // Super-hover for delete button when Ctrl or Shift is held down
            if (btn.Name == "cmdDeleteTopic" && (ctrlHold || shiftHold))
            {
                Image? superHover = Properties.Resources.deleteTrashSuperHover;

                if (superHover != null)
                {
                    btn.BackgroundImage = superHover;
                }

                return;
            }

            // Super-hover for search button when Ctrl is held down
            if (btn.Name == "cmdSearch" && ctrlHold)
            {
                Image? searchSuperHover = Properties.Resources.searchSuperHover;

                if (searchSuperHover != null)
                {
                    btn.BackgroundImage = searchSuperHover;
                }

                return;
            }

            // Normal hover
            Image? hoverImage = Properties.Resources.ResourceManager.GetObject(baseName + "Hover") as Image;

            if (hoverImage != null)
            {
                btn.BackgroundImage = hoverImage;
            }
        }

        /// <summary>
        /// Handles mouse leave event for buttons, changing the background image back to the normal version.
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="map"></param>
        public static void HandleMouseLeave(Button btn, Dictionary<Button, string> map)
        {
            if (map.TryGetValue(btn, out string? baseName))
            {
                Image? normalImage = Properties.Resources.ResourceManager.GetObject(baseName) as Image;

                if (normalImage != null)
                {
                    btn.BackgroundImage = normalImage;
                }
            }
        }
    }
}
