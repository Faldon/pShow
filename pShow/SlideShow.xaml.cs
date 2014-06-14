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
using pShow.Resources;

namespace pShow
{
    /// <summary>
    /// The page responsible for displaying the pictures during the sideshow.
    /// </summary>
    public partial class SlideShow : PhoneApplicationPage
    {
        private readonly BackgroundWorker slideShowThread = new BackgroundWorker();
        private static int nextPicture;
        private static int pictureCount;
        private IEnumerable<Picture> albumPics;
        Image img;
        BitmapImage bmp;

        /// <summary>
        /// The page constructor
        /// </summary>
        public SlideShow()
        {
            InitializeComponent();
            slideShowThread.DoWork += loadNextPicture;
            slideShowThread.RunWorkerCompleted += updateUI;
            slideShowThread.WorkerSupportsCancellation = true;
            img = new Image();
            img.LayoutUpdated += startImageLoading;
            bmp = new BitmapImage()
            {
                CreateOptions = BitmapCreateOptions.BackgroundCreation
            };
            ContentPanel.Children.Add(img);
        }

        void startImageLoading(object sender, EventArgs e)
        {
            slideShowThread.RunWorkerAsync(albumPics);
        }

        /// <summary>
        /// Select all pictures for this album and start the slide show thread.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            string album = "";

            if (NavigationContext.QueryString.TryGetValue("album", out album))
            {
                using (MediaLibrary mediaLib = new MediaLibrary())
                {
                    albumPics = from p in mediaLib.Pictures where p.Album.Name.Equals(album) select p;
                    switch (App.sortOrder)
                    {
                        case 0:
                            albumPics = albumPics.OrderBy(pic => pic.Date);
                            break;
                        case 1:
                            albumPics = albumPics.OrderByDescending(pic => pic.Date);
                            break;
                        case 2:
                            albumPics = albumPics.OrderBy(pic => pic.Name);
                            break;
                        case 3:
                            albumPics = albumPics.OrderByDescending(pic => pic.Name);
                            break;
                        case 4:
                            albumPics = albumPics.OrderBy(pic => Guid.NewGuid());
                            break;
                    }
                    bmp.SetSource(albumPics.ElementAt(0).GetImage());
                    img.Source = bmp;
                    SlideShow.nextPicture = 1;
                    SlideShow.pictureCount = albumPics.Count();
                }
            }
        }

        /// <summary>
        /// Background thread to load the next picture of the album.
        /// </summary>
        private void loadNextPicture(object sender, DoWorkEventArgs e)
        {
            var start = System.DateTime.Now;
            var albumPics = (IEnumerable<Picture>) e.Argument;
            System.IO.Stream nextPicture;
            if (SlideShow.pictureCount > SlideShow.nextPicture)
            {
                nextPicture = albumPics.ElementAt(SlideShow.nextPicture).GetImage();
                SlideShow.nextPicture++;
            }
            else
            {
                nextPicture = albumPics.ElementAt(0).GetImage();
                SlideShow.nextPicture = 1;
            }
            if (!slideShowThread.CancellationPending)
            {
                var end = System.DateTime.Now;
                var duration = end - start;
                System.Threading.Thread.Sleep((App.slideDuration - duration.Seconds) * 1000);
                e.Result = nextPicture;
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Display the image on the page.
        /// </summary>
        private void updateUI(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                var nextPicture = (System.IO.Stream)e.Result;
                bmp.SetSource(nextPicture);
            }
            
        }

        /// <summary>
        /// Cancel the slide show thread and navigate back.
        /// </summary>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            slideShowThread.CancelAsync();
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
            base.OnBackKeyPress(e);
        }
    }
}