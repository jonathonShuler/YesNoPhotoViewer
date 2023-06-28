using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace YesNoPhotoViewer
{
    //WPF Application
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int selectedImageIndex = -1;
        string? selectedImageName = null;
        string? selectedImagePath = null;
        bool yesDirectoryChecked = false;
        bool noDirectoryChecked = false;
        DirectoryInfo? selectedImageParentDirectory;
        ArrayList images = new ArrayList();
        List<string> fileExtensions = new List<string>() { ".jpg", ".JPG", ".jpeg", ".png" };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectImageFromDialog(object sender, RoutedEventArgs e)
        {
            string? selectedImage = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
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
                ImageLabel.Content = "Image selection failed. Try again.";
            }
        }

        private void GetAllImages(string selectedImage)
        {
            images.Clear();
            yesDirectoryChecked = false;
            noDirectoryChecked = false;

            FileInfo selectedFileInfo = new FileInfo(selectedImage);
            selectedImageParentDirectory = selectedFileInfo.Directory;

            if (selectedImageParentDirectory != null)
            {
                foreach (FileInfo file in selectedImageParentDirectory.GetFiles())
                {
                    if (fileExtensions.Contains(file.Extension))
                    {
                        images.Add(file.FullName);
                    }
                }
            }

            if (images.Contains(selectedImage))
            {
                selectedImageIndex = images.IndexOf(selectedImage);
            }
        }

        private void ShowImage()
        {
            string? imagePath = null;
            if (selectedImageIndex > -1)
            {
                selectedImagePath = images[selectedImageIndex]?.ToString();
                if (selectedImagePath != null)
                {
                    imagePath = selectedImagePath;
                }

                if (ChooseImageLarge.Visibility != Visibility.Hidden)
                {
                    ChooseImageLarge.Visibility = Visibility.Hidden;
                }

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                if (imagePath != null)
                {
                    bitmapImage.UriSource = new Uri(imagePath);
                }

                bitmapImage.EndInit();

                MainImage.Source = bitmapImage;
                selectedImageName = bitmapImage.UriSource.Segments.Last<string>();
                ImageLabel.Content = selectedImageName;

                if (MainImage.Visibility != Visibility.Visible)
                {
                    MainImage.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (MainImage.Visibility != Visibility.Hidden)
                {
                    MainImage.Visibility = Visibility.Hidden;
                }

                if (ChooseImageLarge.Visibility != Visibility.Visible)
                {
                    ChooseImageLarge.Visibility = Visibility.Visible;
                }
                ImageLabel.Content = "";
            }
        }

        private void NextImage(object sender, RoutedEventArgs e)
        {
            if (MoveMode.IsChecked == true && selectedImageIndex > -1)
            {
                MoveToYesFolder();
            }
            else
            {
                if (selectedImageIndex < images.Count - 1)
                {
                    selectedImageIndex++;
                }
                ShowImage();
            }
        }

        private void MoveToYesFolder()
        {
            string sourceFile;
            string destinationFile;

            if (selectedImagePath != null && selectedImageParentDirectory != null && selectedImageName != null)
            {
                sourceFile = selectedImagePath;
                destinationFile = selectedImageParentDirectory.ToString() + @"\Yes\" + selectedImageName;

                if (!yesDirectoryChecked)
                {
                    manageDirectory("Yes");
                }


                if (images.Count == 0)
                {
                    images.RemoveAt(selectedImageIndex);
                    selectedImageIndex = -1;
                }
                else if (selectedImageIndex == images.Count - 1)
                {
                    images.RemoveAt(selectedImageIndex);
                    selectedImageIndex--;
                }
                else
                {
                    images.RemoveAt(selectedImageIndex);
                }

                ShowImage();

                try
                {
                    File.Move(sourceFile, destinationFile, false);
                }
                catch (IOException ex)
                {
                    Debug.WriteLine("Failed to move the image. Source: {0}. Destination: {1}.", sourceFile, destinationFile);
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        private void PreviousImage(object sender, RoutedEventArgs e)
        {
            if (MoveMode.IsChecked == true && selectedImageIndex > -1)
            {
                MoveToNoFolder();
            }
            else
            {
                if (selectedImageIndex > 0)
                {
                    selectedImageIndex--;
                }
                ShowImage();
            }
        }

        private void MoveToNoFolder()
        {
            string sourceFile;
            string destinationFile;

            if (selectedImagePath != null && selectedImageParentDirectory != null && selectedImageName != null)
            {
                sourceFile = selectedImagePath;
                destinationFile = selectedImageParentDirectory.ToString() + @"\No\" + selectedImageName;

                if (!noDirectoryChecked)
                {
                    manageDirectory("No");
                }

                if (images.Count == 0)
                {
                    images.RemoveAt(selectedImageIndex);
                    selectedImageIndex = -1;
                }
                else if (selectedImageIndex == images.Count - 1)
                {
                    images.RemoveAt(selectedImageIndex);
                    selectedImageIndex--;
                }
                else
                {
                    images.RemoveAt(selectedImageIndex);
                }

                ShowImage();

                try
                {
                    File.Move(sourceFile, destinationFile, false);
                }
                catch (IOException ex)
                {
                    Debug.WriteLine("Failed to move the image. Source: {0}. Destination: {1}.", sourceFile, destinationFile);
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        private void manageDirectory(string folderName)
        {
            if (Directory.Exists(selectedImageParentDirectory?.ToString() + @"\" + folderName))
            {
                Debug.WriteLine("The '{0}' folder already exists", folderName, "");

                if (folderName.Equals("No"))
                {
                    noDirectoryChecked = true;
                }
                else if (folderName.Equals("Yes"))
                {
                    yesDirectoryChecked = true;
                }
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(selectedImageParentDirectory?.ToString() + @"\" + folderName);
                    Debug.WriteLine("Successfully created the '{0}' folder", folderName, "");

                    if (folderName.Equals("No"))
                    {
                        noDirectoryChecked = true;
                    }
                    else if (folderName.Equals("Yes"))
                    {
                        yesDirectoryChecked = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to create the folder: {0}", ex.ToString());
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                NextImage(sender, e);
            }
            else if (e.Key == Key.Left)
            {
                PreviousImage(sender, e);
            }
        }
    }
}