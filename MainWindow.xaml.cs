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

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
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

        private void SortFilesButton_Click(object sender, RoutedEventArgs e)
        {
            
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
                    SortByFileType(directory);
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            
        }

        private void SortByFileType(string directory)
        {
            List<string> uniqueFileTypes = new List<string>();
            foreach (string folder in Directory.GetDirectories(directory))
            {
                foreach (string file in Directory.GetFiles(folder))
                {
                    string fileType = file.Split(".").Last();
                    if (!uniqueFileTypes.Contains(fileType))
                    {
                        uniqueFileTypes.Add(fileType);
                    }
                }
            }
            foreach (string fileType in uniqueFileTypes)
            {
                TreeViewItem fileTypeFolder = new TreeViewItem();
                fileTypeFolder.Header = fileType;
                PreviewTreeView.Items.Add(fileTypeFolder);
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