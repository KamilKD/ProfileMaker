using System;
using System.IO;
using System.Windows;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace ProfileMaker.Utils
{
    /// <summary>
    /// Класс загрузки файла
    /// </summary>
    public class DialogService
    {
        public string FilePath { get; set; }
        public string DirectoryPath { get; set; }
        public string FileName { get; set; }

        public bool OpenExcelDialog()
        {
            var openFileDialog = new OpenFileDialog();
            // Пока загружаем только файлы еxcel
            openFileDialog.Filter = "файл Excel|*.xlsx";
            openFileDialog.InitialDirectory = DirectoryPath == null
            ? Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
            : DirectoryPath;
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                DirectoryPath = FilePath.Substring(0, FilePath.LastIndexOf('\\') - 1);
                FileName = openFileDialog.SafeFileName;
                return true;
            }

            return false;
        }

        public bool SaveDxfDialog()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "dxf";
            saveFileDialog.AddExtension = true;
            saveFileDialog.InitialDirectory = DirectoryPath == null
            ? Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
            : DirectoryPath;
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                DirectoryPath = FilePath.Substring(0, FilePath.LastIndexOf('\\') - 1);
                return true;
            }
            return false;
        }


        public bool SaveExcelDialog()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = DirectoryPath == null
            ? Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
            : DirectoryPath;
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                DirectoryPath = FilePath.Substring(0, FilePath.LastIndexOf('\\') - 1);
                FileName = saveFileDialog.SafeFileName;
                return true;
            }
            return false;
        }
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
