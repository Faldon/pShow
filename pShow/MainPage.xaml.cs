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
using pShow.Model;
using pShow.Resources;

namespace pShow
{
    /// <summary>
    /// The page responsible for displaying the available albums with preview.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        private ObservableCollection<AlbumPreview> albumList;
        private BackgroundWorker workerThread;

        /// <summary>
        /// The page constructor.
        /// </summary>
        public MainPage()
        {
            albumList = new ObservableCollection<AlbumPreview>();

            workerThread = new BackgroundWorker();
            workerThread.DoWork += getAllAlbums;
            workerThread.RunWorkerCompleted += updateUI;
            workerThread.WorkerSupportsCancellation = true;  
            progress = new ProgressIndicator();
            
            InitializeComponent();
            BuildLocalizedApplicationBar();

            AlbumsView.ItemsSource = albumList;
            progress.IsIndeterminate = true;
            workerThread.RunWorkerAsync();
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
        /// Get a list of PictureAlbum containing all albums available from the media library.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Events for the background thread</param>
        private void getAllAlbums(object sender, DoWorkEventArgs e)
        {
            var allAlbums = new List<PictureAlbum>();
            using (MediaLibrary mediaLib = new MediaLibrary())
            {
                foreach (PictureAlbum album in mediaLib.RootPictureAlbum.Albums)
                {
                    allAlbums.Add(album);
                }
                for (var i = 0; i < allAlbums.Count; i++)
                {
                    if (workerThread.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    foreach (PictureAlbum album in allAlbums[i].Albums)
                    {
                        allAlbums.Add(album);
                    }
                }
            }
            e.Result = allAlbums.OrderBy(i => i.Name).ToList<PictureAlbum>();
        }

        /// <summary>
        /// Update the UI with the preview pictures and album titles for each album.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Events for the completion of the background thread, with a list of PictureAlbum as Result property</param>
        private void updateUI(object sender, RunWorkerCompletedEventArgs e)
        {
            var allAlbums = (List<PictureAlbum>)e.Result;
            foreach (PictureAlbum album in allAlbums)
            {
                if (album.Pictures.Count > 0)
                {
                    AlbumPreview aPreview = new AlbumPreview();
                    aPreview.title = album.Name;

                    BitmapImage albumPic = new BitmapImage();
                    albumPic.CreateOptions = BitmapCreateOptions.DelayCreation;
                    albumPic.DecodePixelHeight = 180;
                    albumPic.SetSource(album.Pictures[0].GetThumbnail());
                    aPreview.preview = albumPic;
                    
                    albumList.Add(aPreview);
                }
            }
            progress.IsIndeterminate = false;
        }

        /// <summary>
        /// Navigate to the album details page for the chosen album.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Additional arguments from the event</param>
        private void openAlbum(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            workerThread.CancelAsync();
            NavigationService.Navigate(new Uri("/AlbumDetails.xaml?albumChoice=" + b.Tag.ToString(), UriKind.Relative));
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

        /// <summary>
        /// Cancel BackgroundWorker and perform default action.
        /// </summary>
        /// <param name="e">Additional arguments from the event</param>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            workerThread.CancelAsync();
            base.OnBackKeyPress(e);
        }
    }
}