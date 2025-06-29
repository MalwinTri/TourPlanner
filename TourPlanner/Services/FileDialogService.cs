using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace TourPlanner.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string OpenImageFileDialog()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }

}
