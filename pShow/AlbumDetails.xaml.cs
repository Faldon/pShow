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
using pShow.Resources;
using pShow.ViewModel;

namespace pShow
{
    /// <summary>
    /// The page responsible for displaying the album content.
    /// </summary>
    public partial class AlbumDetails : PhoneApplicationPage
    {
        /// <summary>
        /// The page constructor.
        /// </summary>
        public AlbumDetails()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Build a localized application bar.
        /// </summary>
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.5;

            ApplicationBarIconButton startButton = new ApplicationBarIconButton(new Uri("/Resources/transport.play.png", UriKind.Relative));
            startButton.Text = AppResources.StartSlideShow;
            startButton.Click += startSlideShow;
            ApplicationBar.Buttons.Add(startButton);
        }

        /// <summary>
        /// Gets the selected album and create a new AlbumViewModel from the chosen album.
        /// Sets the AlbumViewModel as DataContext for the cuurent page.
        /// </summary>
        /// <param name="e">The NavigationEventArgs</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string albumChoice = "";

            if (NavigationContext.QueryString.TryGetValue("albumChoice", out albumChoice))
            {
                AlbumViewModel data = new AlbumViewModel(albumChoice);
                DataContext = data;
            }
        }

        /// <summary>
        /// Navigate to the slide show page for the chosen album.
        /// </summary>
        private void startSlideShow(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SlideShow.xaml?album=" + albumName.Text, UriKind.Relative));
        }
    }
}