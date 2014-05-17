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

namespace pShow
{
    /// <summary>
    /// The page responsible for displaying the album content.
    /// </summary>
    public partial class AlbumDetails : PhoneApplicationPage
    {
        private ObservableCollection<BitmapImage> pictureList;
        private BackgroundWorker workerThread = new BackgroundWorker();
        private bool isWorking;

        /// <summary>
        /// The page constructor.
        /// </summary>
        public AlbumDetails()
        {
            pictureList = new ObservableCollection<BitmapImage>();
            isWorking = true;
            
            workerThread.DoWork += loadPictures;
            workerThread.RunWorkerCompleted += generateBitmaps;
            workerThread.WorkerSupportsCancellation = true;

            InitializeComponent();
            BuildLocalizedApplicationBar();
            AlbumDetailsView.ItemsSource = pictureList;
            
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
        /// Starts loading the pictures of the selected album if navigated to the page.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            isWorking = true;
            string albumChoice = "";

            if (NavigationContext.QueryString.TryGetValue("albumChoice", out albumChoice))
            {
                pictureList.Clear();
                albumName.Text = albumChoice;
                workerThread.RunWorkerAsync(albumChoice);
            }
        }

        /// <summary>
        /// Cancel the slide show thread and navigate back.
        /// </summary>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            workerThread.CancelAsync();
            isWorking = false;
            base.OnBackKeyPress(e);
        }

        /// <summary>
        /// Navigate to the slide show page for the chosen album.
        /// </summary>
        private void startSlideShow(object sender, EventArgs e)
        {
            isWorking = false;
            NavigationService.Navigate(new Uri("/SlideShow.xaml?album=" + albumName.Text, UriKind.Relative));
        }

        /// <summary>
        /// Loads the pictures of the given picture album in the background.
        /// </summary>
        /// <param name="sender">The sender starting the async process</param>
        /// <param name="e">The name of the album the pictures are loaded from</param>
        private void loadPictures(object sender, DoWorkEventArgs e)
        {       
            var albumChoice = (string)e.Argument;
            var pictureStreams = new List<System.IO.Stream>();
            using (MediaLibrary mediaLib = new MediaLibrary())
            {
                var albumPics = from p in mediaLib.Pictures where p.Album.Name.Equals(albumChoice) select p;
                foreach (Picture p in albumPics)
                {
                    if (workerThread.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    pictureStreams.Add(p.GetThumbnail());
                }
            }
            e.Result = pictureStreams;
        }

        /// <summary>
        /// Updates the UI thread with the currently loaded picture.
        /// </summary>
        /// <param name="sender">The background worker raising the event</param>
        /// <param name="e">A Picture from the MediaLibrary</param>
        private void generateBitmaps(object sender, RunWorkerCompletedEventArgs e)
        {
            var pictureStreams = (List<System.IO.Stream>)e.Result;
            foreach (System.IO.Stream pictureStream in pictureStreams)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.CreateOptions = BitmapCreateOptions.DelayCreation;
                bmp.SetSource(pictureStream);
                pictureList.Add(bmp);
            }
            isWorking = false;
        }
    }
}