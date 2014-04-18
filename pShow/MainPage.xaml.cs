using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using pShow.Resources;

namespace pShow
{
    /// <summary>
    /// The page responsible for displaying the available albums with preview.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// The page constructor.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            ContentPanel.Children.Clear();
            foreach (MediaSource source in MediaSource.GetAvailableMediaSources()) {

                using (MediaLibrary mediaLib = new MediaLibrary(source))
                {
                    foreach (PictureAlbum album in mediaLib.RootPictureAlbum.Albums)
                    {
                        if (album.Pictures.Count != 0)
                        {
                            StackPanel albumInfo = buildAlbumInformation(album);
                            App.insertInGrid(ContentPanel, albumInfo);
                        }
                    }
                }
            }
            BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Build a localized application bar.
        /// </summary>
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.5;

            ApplicationBarIconButton settingsButton = new ApplicationBarIconButton(new Uri("/Resources/feature.settings.png", UriKind.Relative));
            settingsButton.Text = AppResources.SettingsTitle;
            settingsButton.Click += openSettings;
            ApplicationBar.Buttons.Add(settingsButton);
        }

        /// <summary>
        /// Fill a UI element with picture album information.
        /// </summary>
        /// <param name="pAlbum">
        /// A PictureAlbum element for the picture album to get information from.
        /// </param>
        /// <returns>
        /// A StackPanel element where the information was added.
        /// </returns>
        private StackPanel buildAlbumInformation(PictureAlbum pAlbum)
        {
            StackPanel infoPanel = new StackPanel();
            infoPanel.Margin = new System.Windows.Thickness(5);
            // Get first picture from album as album preview
            Image albumImage = new Image();
            BitmapImage albumPic = new BitmapImage();
            albumPic.SetSource(pAlbum.Pictures[0].GetThumbnail());
            albumImage.Source = albumPic;
            albumImage.Stretch = System.Windows.Media.Stretch.UniformToFill;

            // Get album name
            TextBlock albumName = new TextBlock();
            albumName.Text = pAlbum.Name;
            albumName.Tag = pAlbum.GetHashCode();
            albumName.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            // Build button to navigate to desired album
            Button albumButton = new Button();
            albumButton.BorderThickness = new Thickness(0);
            albumButton.Content = albumImage;
            albumButton.Tag = pAlbum.Name;
            albumButton.Click += openAlbum;
                        
            // Add picture and album name to panel
            infoPanel.Children.Add(albumButton);
            infoPanel.Children.Add(albumName);
            
            return infoPanel;
        }

        /// <summary>
        /// Navigate to the album details page for the chosen album.
        /// </summary>
        private void openAlbum(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            NavigationService.Navigate(new Uri("/AlbumDetails.xaml?albumChoice=" + b.Tag.ToString(), UriKind.Relative));
        }

        /// <summary>
        /// Navigate to the settings page.
        /// </summary>
        private void openSettings(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }
    }
}