using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YesNoPhotoViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath;
        ArrayList images = new ArrayList();
        List<string> fileExtensions = new List<string>() { "*.jpg", "*.png" };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetImageSource(object sender, RoutedEventArgs e)
        {
            //FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if(openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
            }

            if (filePath != null)
            {
                ChooseImageLarge.Visibility = Visibility.Hidden;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(filePath);
                bitmapImage.EndInit();
                MainImage.Source = bitmapImage;
                MainImage.Visibility = Visibility.Visible;
                GetAllImages();
            }
        }

        private void GetAllImages()
        {
            FileInfo selectedFileInfo = new FileInfo(filePath);
            DirectoryInfo? directory = selectedFileInfo.Directory;
            if (directory != null)
            {
                foreach(string fileExtension in fileExtensions)
                {
                    foreach (FileInfo file in directory.GetFiles(fileExtension))
                    {
                        images.Add(file);
                    }
                }
            }

            Console.WriteLine(images);
        }

        private void YesToImage(object sender, RoutedEventArgs e)
        {

        }

        private void NoToImage(object sender, RoutedEventArgs e)
        {

        }
    }
}
