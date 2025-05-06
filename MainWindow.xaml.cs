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
using static System.Net.WebRequestMethods;
using Microsoft.VisualBasic.FileIO;

namespace File_Sorter___Organizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
                /*
                foreach (string file in Directory.GetFiles(folder))
                {
                    allFiles.Add(file);
                    string fileType = file.Split(".").Last();
                    if (!uniqueFileTypes.Contains(fileType))
                    {
                        uniqueFileTypes.Add(fileType);
                    }
                }
                */
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
            SortFiles(PathTextBox.Text);
        }

        private void SortFiles(string directory)
        {
            List<string> allFiles = new List<string>();
            List<string> uniqueFileTypes = new List<string>();
            
            allFiles.AddRange(Directory.GetFiles(directory));
            if (SortSubFolderCheckBox.IsChecked == true)
            {
                foreach (string folder in Directory.GetDirectories(directory))
                {
                    allFiles.AddRange(Directory.GetFiles(folder));
                }
            }

            foreach (string file in allFiles)
            {
                uniqueFileTypes.Add(file.Split(".").Last());
            }
            uniqueFileTypes = uniqueFileTypes.Distinct().ToList();


            foreach (string fileType in uniqueFileTypes)
            {
                if (!Directory.Exists($@"{directory}\\{fileType}"))
                {
                    string firstLetter = CapitalizeFolderNamesCheckBox.IsChecked == true ? fileType[0].ToString().ToUpper() : fileType[0].ToString();
                    Directory.CreateDirectory($@"{directory}\\{firstLetter + fileType.Substring(1)}");
                }
            }

            foreach (string file in allFiles)
            {
                if (file.Split(@"\").Last().Split(".").Last() != file.Split(@"\")[^2].ToLower())
                {
                    Directory.Move(file, $@"{directory}\\{file.Split(".").Last()}\\{file.Split(@"\").Last()}");
                }
            }

            /*
            foreach (string fileType in uniqueFileTypes)
            {
                Directory.CreateDirectory($@"{directory}\\{fileType}");

                foreach (string file in allFiles)
                {
                    if (file.Split(".").Last() == fileType)
                    {
                        Directory.Move(file, $@"{directory}\\{fileType}\\{file.Split(@"\").Last()}");
                    }
                }
            }
            */

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

        private void UndoSortingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FileTypeThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderValueLabel.Content = ((int)FileTypeThresholdSlider.Value).ToString();
        }
    }
}