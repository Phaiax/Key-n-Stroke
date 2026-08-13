using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;


namespace KeyNStroke
{
    class UIHelper
    {
        #region Helper to convert Drawing.Color to Windows.Media.Color
        public static MColor ToMediaColor(DColor color)
        {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static DColor ToDrawingColor(MColor color)
        {
            return DColor.FromArgb(color.A, color.R, color.G, color.B);
        }
        #endregion

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            if (parent is FrameworkElement)
                ((FrameworkElement)parent).ApplyTemplate();

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }

    public static class VistaFolderBrowser
    {
        public static string ShowDialog(IntPtr ownerHandle, string title, string initialPath = null)
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = title,
                    Filter = "Select Folder|*.this_is_a_folder_selection",
                    CheckFileExists = false,
                    CheckPathExists = true,
                    FileName = "Select Folder"
                };

                if (!string.IsNullOrEmpty(initialPath) && System.IO.Directory.Exists(initialPath))
                {
                    dialog.InitialDirectory = initialPath;
                }

                var fileDialogType = typeof(Microsoft.Win32.FileDialog);
                var setOptionMethod = fileDialogType.GetMethod("SetOption",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                if (setOptionMethod != null)
                {
                    setOptionMethod.Invoke(dialog, new object[] { 0x00000020, true }); // FOS_PICKFOLDERS
                }

                bool? result = dialog.ShowDialog();
                if (result == true)
                {
                    string path = dialog.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        return System.IO.Path.GetDirectoryName(path);
                    }
                    else if (System.IO.Directory.Exists(path))
                    {
                        return path;
                    }
                    else
                    {
                        string parent = System.IO.Path.GetDirectoryName(path);
                        if (!string.IsNullOrEmpty(parent) && System.IO.Directory.Exists(parent))
                        {
                            return parent;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.e("FOLDER_PICKER", ex.Message);
            }
            return null;
        }
    }
}
