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
using System.Globalization;
using System.Collections.Generic;

namespace File_Organizer
{
    public partial class MainWindow : Window
    {
        private bool isInitialized = false;
        public MainWindow()
        {
            InitializeComponent();
            isInitialized = true;
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

            switch (SortComboBox.SelectedIndex)
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
                    /*
                case 1:
                    SortFilesByName(PathTextBox.Text);
                    break; */
                case 1:
                    SortFilesBySize(PathTextBox.Text);
                    break;
                case 2:
                    SortFilesAlphabetically(PathTextBox.Text);
                    break;
            }

        }

        List<string> GetFiles(string directory)
        {
            List<string> allFiles = new List<string>();

            DirectoryInfo info = new DirectoryInfo(directory);
            List<string> folders = info.EnumerateDirectories("*.", System.IO.SearchOption.AllDirectories ).Select(x => x.FullName).ToList();

            allFiles.AddRange(Directory.GetFiles(directory));
            if (SortSubFolderCheckBox.IsChecked == true)
            {
                foreach (string folder in Directory.GetDirectories(directory))
                {
                    allFiles.AddRange(Directory.GetFiles(folder));
                }
            }

            return CheckForZipCheckBox(allFiles);
        }

        string CheckForCapitalCheckBox(string word)
        {
            return (CapitalizeFolderNamesCheckBox.IsChecked == true ? word[0].ToString().ToUpper() : word[0].ToString().ToLower()) + word.Substring(1);
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

            if (FileTypeThresholdSlider.Value > 0)
            {
                int count;
                List<string> discardList = new List<string>();
                foreach (string uniqueFileType in uniqueFileTypes)
                {
                    count = allFiles.FindAll(x => x.Split('.').Last() == uniqueFileType).Count();
                    if (count < Convert.ToInt32(SliderValueLabel.Content))
                    {
                        allFiles.RemoveAll(x => x.Split('.').Last() == uniqueFileType);
                        discardList.Add(uniqueFileType);
                    }
                }
                uniqueFileTypes.RemoveAll(x => discardList.Contains(x));
            }

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
                    if (!Directory.Exists($@"{directory}\\{file.Split(".").Last()}\\{file.Split(@"\").Last()}"))
                    {
                        Directory.Move(file, $@"{directory}\\{file.Split(".").Last()}\\{file.Split(@"\").Last()}");
                    }
                }
            }

            if (SortSubFolderCheckBox.IsChecked == true)
            {
                foreach (string folder in Directory.GetDirectories(directory))
                {
                    int? foundFiles = Directory.GetFileSystemEntries(folder).Count();
                    if (foundFiles == 0)
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
            Dictionary<string, long> allFiles = new Dictionary<string, long>();
            List<string> folders = new List<string>();

            foreach (string path in GetFiles(directory))
            {
                FileInfo fileInfo = new FileInfo(path);
                allFiles.Add(path, fileInfo.Length);
            }

            allFiles.OrderBy(x => x.Value);

            TextBox lastTextBox = new TextBox();
            foreach (TextBox textBox in FileSizeDistributionListBox.Items)
            {
                Directory.CreateDirectory($@"{directory}\{CheckForCapitalCheckBox($"Less Than {textBox.Text}")}");
                folders.Add(($@"{directory}\Less Than {textBox.Text}"));
                lastTextBox = textBox;
            }
            Directory.CreateDirectory($@"{directory}\{CheckForCapitalCheckBox($"More Than {lastTextBox.Text}")}");
            folders.Add($@"{directory}\More than {lastTextBox.Text}");

            foreach (KeyValuePair<string, long> values in allFiles)
            {
                if (values.Value > Convert.ToInt32(folders[^1].Split(" ").Last()))
                {
                    Directory.Move(values.Key, $@"{folders[^1]}\\{values.Key.Split(@"\").Last()}");
                }
                else
                {
                    for (int i = 0; i < folders.Count - 1; i++)
                    {
                        if (values.Value <= Convert.ToInt32(folders[i].Split(" ").Last()))
                        {
                            Directory.Move(values.Key, $@"{folders[i]}\\{values.Key.Split(@"\").Last()}");
                            break;
                        }
                    }
                }
            }

            if (SortSubFolderCheckBox.IsChecked == true)
            {
                foreach (string folder in Directory.GetDirectories(directory))
                {
                    string? foundFile = folders.Find(x => x.ToLower() == folder.ToLower());
                    if (foundFile == null)
                    {
                        Directory.Delete(folder);
                    }
                }
            }
        }

        private void SortFilesAlphabetically(string directory)
        {
            List<string> allFiles = GetFiles(directory);
            List<string> folderNames = new List<string>();

            foreach (string path in allFiles)
            {
                string lastLetter = (path.Split(@"\").Last()).ToCharArray()[0].ToString();
                if (!folderNames.Contains(lastLetter))
                {
                    folderNames.Add(lastLetter);
                }
            }

            foreach (string name in folderNames)
            {
                Directory.CreateDirectory($@"{directory}\\{CheckForCapitalCheckBox(name)}");
            }

            foreach (string path in allFiles)
            {
                string lastLetter = (path.Split(@"\").Last()).ToCharArray()[0].ToString();
                Directory.Move(path, $@"{directory}\\{lastLetter}\\{path.Split(@"\").Last()}");
            }
        }

        private void SortFilesRandomly(string directory)
        {

        }

        private List<string> CheckForZipCheckBox(List<string> stringList)
        {
            if (ExcludeZipCheckBox.IsChecked == true)
            {
                stringList.RemoveAll(x => x.Split('.').Last() == "zip");
            }
            return stringList;
        }

        private void UndoSortingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FileTypeThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderValueLabel.Content = ((int)FileTypeThresholdSlider.Value).ToString();
        }

        private void AddToFileSizeListBox_Click(object sender, RoutedEventArgs e)
        {
            TextBox textBox = new TextBox();
            textBox.FontSize = 10;
            textBox.BorderThickness = new Thickness(1);

            FileSizeDistributionListBox.Items.Add(textBox);
        }

        private void RemoveToFileSizeListBox_Click(object sender, RoutedEventArgs e)
        {
            FileSizeDistributionListBox.Items.Remove(^1);
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                switch (SortComboBox.SelectedIndex)
                {
                    case 0:
                        AmountThresholdStackPanel.IsEnabled = true;
                        SizeRangeStackPanel.IsEnabled = false;
                        break;
                        /*
                    case 1:
                        AmountThresholdStackPanel.IsEnabled = false;
                        SizeRangeStackPanel.IsEnabled = false;
                        break; */
                    case 1:
                        AmountThresholdStackPanel.IsEnabled = false;
                        SizeRangeStackPanel.IsEnabled = true;
                        break;
                    case 2:
                        AmountThresholdStackPanel.IsEnabled = false;
                        SizeRangeStackPanel.IsEnabled = false;
                        break;
                }
            }
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private bool isMaximized;
        private void fullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = isMaximized ? WindowState.Normal : WindowState.Maximized;
            isMaximized = !isMaximized;
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void SortFolderCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (SortFolderCheckBox.IsChecked == true)
            {
                SortSubFolderCheckBox.IsEnabled = true;
            }
            else
            {
                SortSubFolderCheckBox.IsEnabled = false;
                SortSubFolderCheckBox.IsChecked = false;
            }
        }
    }
}