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
        int currentImageIndex = -1;
        ArrayList images = new ArrayList();
        List<string> fileExtensions = new List<string>() { "*.jpg", "*.png" };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectImageFromDialog(object sender, RoutedEventArgs e)
        {
            string? selectedImage = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if(openFileDialog.ShowDialog() == true)
            {
                selectedImage = openFileDialog.FileName;
            }

            if (selectedImage != null)
            {
                GetAllImages(selectedImage);
                ShowImage();
            }
            else
            {
                ImageName.Content = "Image selection failed. Try again.";
            }
        }

        private void GetAllImages(string selectedImage)
        {
            FileInfo selectedFileInfo = new FileInfo(selectedImage);
            DirectoryInfo? directory = selectedFileInfo.Directory;
            if (directory != null)
            {
                foreach(string fileExtension in fileExtensions)
                {
                    foreach (FileInfo file in directory.GetFiles(fileExtension))
                    {
                        images.Add(file.FullName);
                    }
                }
            }
            if (images.Contains(selectedImage))
            {
                currentImageIndex = images.IndexOf(selectedImage);
            }
        }

        private void ShowImage()
        {
            string? imagePath = null;
            if (currentImageIndex > -1)
            {
                var image = images[currentImageIndex];
                if (image != null)
                {
                    imagePath = image.ToString();
                }
            }

            if (ChooseImageLarge.Visibility != Visibility.Hidden)
            {
                ChooseImageLarge.Visibility = Visibility.Hidden;
            }

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            if (imagePath != null)
            {
                bitmapImage.UriSource = new Uri(imagePath);
            }
            bitmapImage.EndInit();

            MainImage.Source = bitmapImage;
            ImageName.Content = bitmapImage.UriSource.Segments.Last<string>();

            if (MainImage.Visibility != Visibility.Visible)
            {
                MainImage.Visibility = Visibility.Visible;
            }
        }

        private void YesToImage(object sender, RoutedEventArgs e)
        {
            if (currentImageIndex < images.Count -1)
            {
                currentImageIndex++;
                ShowImage();
            }
        }

        private void NoToImage(object sender, RoutedEventArgs e)
        {
            if (currentImageIndex > 0)
            {
                currentImageIndex--;
                ShowImage();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                YesToImage(sender, e);
            }
            else if (e.Key == Key.Left)
            {
                NoToImage(sender, e);
            }
        }
    }
}
