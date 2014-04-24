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
        /// Loads the pictures of this album if navigated to the page.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ContentPanel.Children.Clear();
            string albumChoice = "";

            if (NavigationContext.QueryString.TryGetValue("albumChoice", out albumChoice))
            {
                albumName.Text = albumChoice;
                using (MediaLibrary mediaLib = new MediaLibrary())
                {
                    var albumPics = from p in mediaLib.Pictures where p.Album.Name.Equals(albumChoice) select p;
                    foreach (Picture p in albumPics)
                    {
                        App.insertInGrid(ContentPanel, getThumbImage(p));
                    }
                }
            }
        }

        /// <summary>
        /// Get the thumbnail image stream from a media lib picture.
        /// </summary>
        /// <param name="pic">
        /// A picture from the media lib.
        /// </param>
        private Image getThumbImage(Picture pic)
        {
            Image i = new Image();
            BitmapImage bi = new BitmapImage();
            bi.SetSource(pic.GetThumbnail());
            i.Source = bi;
            i.Stretch = System.Windows.Media.Stretch.UniformToFill;
            i.Margin = new System.Windows.Thickness(10);

            return i;
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