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
using System.ComponentModel;
using System.Collections.ObjectModel;
using pShow.ViewModel;
using pShow.Model;
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
            this.Loaded += pageLoaded;
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// On page load, creates a new MainViewModel and sets it as the DataContext of the current page.
        /// </summary>
        /// <param name="sender">The sender of the loaded event</param>
        /// <param name="e">The RoutedEventArgs</param>
        void pageLoaded(object sender, RoutedEventArgs e)
        {
            MainViewModel data = new MainViewModel();
            DataContext = data;
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
        /// Navigate to the album details page for the chosen album.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Additional arguments from the event</param>
        private void openAlbum(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            NavigationService.Navigate(new Uri("/AlbumDetails.xaml?albumChoice=" + HttpUtility.UrlEncode(b.Tag.ToString()), UriKind.Relative));
        }

        /// <summary>
        /// Navigate to the settings page.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Additional arguments from the event</param>
        private void openSettings(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }
    }
}