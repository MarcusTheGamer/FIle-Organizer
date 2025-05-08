using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace File_Sorter___Organizer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void BrowseFoldersButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog();
            dialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string directory = dialog.FolderName;
                PathTextBox.Text = directory;
                ValidateDirectory(directory);
                InitializeTreeView(directory);
            }
        }

        private void InitializeTreeView(string directory)
        {
            PathTreeView.Items.Clear();

            string[] folders = Directory.GetDirectories(directory);
            foreach (string folder in folders)
            {
                TreeViewItem parentItem = new TreeViewItem();
                parentItem.Header = folder.Split(@"\").Last();
                foreach (string file in Directory.GetFiles(folder))
                {
                    TreeViewItem childItem = new TreeViewItem();
                    childItem.Header = file.Split(@"\").Last();
                    parentItem.Items.Add(childItem);
                }
                PathTreeView.Items.Add(parentItem);
            }
            foreach (string file in Directory.GetFiles(directory))
            {
                TreeViewItem fileItem = new TreeViewItem();
                fileItem.Header = file.Split(@"\").Last();
                PathTreeView.Items.Add(fileItem);
            }
        }

        private void ValidateDirectory(string directory)
        {
            SortFilesButton.IsEnabled = Directory.Exists(directory) ? true : false;
        }

        private void EnableAllButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeAllParameterStates(true);
        }

        private void DisableAllButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeAllParameterStates(false);
        }

        private void ChangeAllParameterStates(bool state)
        {
            foreach (CheckBox checkBox in ParameterStackPanel.Children)
            {
                checkBox.IsChecked = state;
            }
        }

        private void ApplyParametersButton_Click(object sender, RoutedEventArgs e)
        {
            InitializePreviewTreeView(PathTextBox.Text);
        }

        private void InitializePreviewTreeView(string directory)
        {
            
            switch(SortComboBox.SelectedIndex)
            {
                case 0:
                    PreviewSortByFileType(directory);
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            
        }
        
        private void PreviewSortByFileType(string directory)
        {
            PreviewTreeView.Items.Clear();

            List<string> allFiles = new List<string>();
            List<string> uniqueFileTypes = new List<string>();

            // Gets all files in base directory
            allFiles.AddRange(Directory.GetFiles(directory));
            foreach (string folder in Directory.GetDirectories(directory))
            {
                allFiles.AddRange(Directory.GetFiles(folder));
            }

            foreach (string file in allFiles)
            {
                uniqueFileTypes.Add(file.Split(".").Last());
            }
            uniqueFileTypes = uniqueFileTypes.Distinct().ToList();
            
            // Makes treeview folders from unique file types
            foreach (string fileType in uniqueFileTypes)
            {

                TreeViewItem fileTypeFolder = new TreeViewItem();
                fileTypeFolder.Header = fileType;
                foreach (string file in allFiles)
                {
                    if (file.Split(".").Last() == fileType)
                    {
                        TreeViewItem fileTypeFile = new TreeViewItem();
                        fileTypeFile.Header = file.Split(@"\").Last();
                        fileTypeFolder.Items.Add(fileTypeFile);
                    }
                }

                PreviewTreeView.Items.Add(fileTypeFolder);
            }
        }
        private void SortFilesButton_Click(object sender, RoutedEventArgs e)
        {
            switch (SortComboBox.SelectedIndex)
            {
                case 0:
                    SortFilesByType(PathTextBox.Text);
                    break;
                case 1:
                    SortFilesByName(PathTextBox.Text);
                    break;
                case 2:
                    SortFilesBySize(PathTextBox.Text);
                    break;
                case 3:
                    SortFilesRandomly(PathTextBox.Text);
                    break;
            }

        }

        List<string> GetFiles(string directory)
        {
            List<string> allFiles = new List<string>();

            allFiles.AddRange(Directory.GetFiles(directory));
            if (SortSubFolderCheckBox.IsChecked == true)
            {
                foreach (string folder in Directory.GetDirectories(directory))
                {
                    allFiles.AddRange(Directory.GetFiles(folder));
                }
            }

            return allFiles;
        }

        string CheckForCapitalCheckBox(string word)
        {
            return (CapitalizeFolderNamesCheckBox.IsChecked == true ? word[0].ToString().ToUpper() : word[0].ToString()) + word.Substring(1);
        }

        private void SortFilesByType(string directory)
        {
            List<string> allFiles = new List<string>();
            List<string> uniqueFileTypes = new List<string>();

            allFiles = GetFiles(directory);

            foreach (string file in allFiles)
            {
                uniqueFileTypes.Add(file.Split(".").Last());
            }
            uniqueFileTypes = uniqueFileTypes.Distinct().ToList();


            foreach (string fileType in uniqueFileTypes)
            {
                if (!Directory.Exists($@"{directory}\\{fileType}"))
                {
                    Directory.CreateDirectory($@"{directory}\\{CheckForCapitalCheckBox(fileType)}");
                }
            }

            foreach (string file in allFiles)
            {
                if (file.Split(@"\").Last().Split(".").Last() != file.Split(@"\")[^2].ToLower())
                {
                    Directory.Move(file, $@"{directory}\\{file.Split(".").Last()}\\{file.Split(@"\").Last()}");
                }
            }

            if (SortSubFolderCheckBox.IsChecked == true)
            {
                foreach (string folder in Directory.GetDirectories(directory))
                {
                    string? foundFile = uniqueFileTypes.Find(x => x == folder.Split(@"\").Last().ToLower());
                    if (foundFile == null)
                    {
                        Directory.Delete(folder);
                    }
                }
            }
        }

        private void SortFilesByName(string directory)
        {

        }

        private void SortFilesBySize(string directory)
        {
            Dictionary<string,long> allFiles = new Dictionary<string,long>();

            foreach (string path in GetFiles(directory))
            {
                FileInfo fileInfo = new FileInfo(path);
                allFiles.Add(path, fileInfo.Length);
            }

            allFiles.OrderBy(x => x.Value);

            Directory.CreateDirectory($@"{directory}\\Less than 50MB");
            Directory.CreateDirectory($@"{directory}\\Less than 500MB");
            Directory.CreateDirectory($@"{directory}\\More than 1GB");

            foreach (KeyValuePair<string, long> values in allFiles)
            {
                if (values.Value <= 50000000)
                {
                    Directory.Move(values.Key, $@"{directory}\\Less than 50MB\\{values.Key.Split(@"\").Last()}");
                }
                else if (values.Value <= 500000000)
                {
                    Directory.Move(values.Key, $@"{directory}\\Less than 500MB\\{values.Key.Split(@"\").Last()}");
                }
                else
                {
                    Directory.Move(values.Key, $@"{directory}\\More than 1GB\\{values.Key.Split(@"\").Last()}");
                }
            }
        }

        private void SortFilesRandomly(string directory)
        {

        }

        private void UndoSortingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FileTypeThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderValueLabel.Content = ((int)FileTypeThresholdSlider.Value).ToString();
        }
    }
}