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
            SlideShow.nextPicture = 0;
            slideShowThread.DoWork += loadNextPicture;
            slideShowThread.RunWorkerCompleted += updateUI;
            slideShowThread.WorkerSupportsCancellation = true;
            InitializeComponent();

            img = new Image()
            {
                Name="Image",
            };
            bmp = new BitmapImage()
            {
                CreateOptions = BitmapCreateOptions.IgnoreImageCache,
            };
            img.Source = bmp;
            img.LayoutUpdated += startImageLoading;
            ContentPanel.Children.Add(img);
        }

        /// <summary>
        /// Start the image loading BackgroundWorker.
        /// This event is raised on Image.LayoutUpdated.
        /// </summary>
        /// <param name="sender">The image that was updated</param>
        /// <param name="e">The event arguments</param>
        public void startImageLoading(object sender, EventArgs e)
        {
            if (!slideShowThread.IsBusy)
            {
                switch ((int)App.userSettings["blendMode"])
                {
                    case 0:
                        img.Opacity = 1;
                        slideShowThread.RunWorkerAsync(albumPics);
                        break;
                    case 1:
                        FadeIn.Begin();
                        slideShowThread.RunWorkerAsync(albumPics);
                        break;
                }
            }
        }

        /// <summary>
        /// Select all pictures for this album and sort them by selected sort order.
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
                    switch ((int)App.userSettings["sortOrder"])
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
            
            if (SlideShow.nextPicture == 0)
            {
                // page was currently opened - skip the sleeping period
                nextPicture = albumPics.ElementAt(SlideShow.nextPicture).GetImage();
                SlideShow.nextPicture++;
            }
            else
            {
                if (SlideShow.pictureCount > SlideShow.nextPicture)
                {
                    // Increment the next picture
                    nextPicture = albumPics.ElementAt(SlideShow.nextPicture).GetImage();
                    SlideShow.nextPicture++;
                }
                else
                {
                    // load the picture with index 0 and set next picture to 1
                    nextPicture = albumPics.ElementAt(0).GetImage();
                    SlideShow.nextPicture = 1;
                }
                var end = System.DateTime.Now;

                // wait until the desired slide show duration is reached
                while ((end - start).Seconds < (int)App.userSettings["slideDuration"])
                {
                    if (!slideShowThread.CancellationPending)
                    {
                        System.Threading.Thread.Sleep(100);
                        end = end.AddMilliseconds(100);
                    }
                    else
                    {
                        e.Cancel = true;
                        end = end.AddSeconds((int)App.userSettings["slideDuration"]);
                    }
                }
            }
            e.Result = nextPicture;
        }

        /// <summary>
        /// Display the image on the page.
        /// </summary>
        private void updateUI(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                var nextPicture = (System.IO.Stream)e.Result;
                switch ((int)App.userSettings["blendMode"]) 
                {
                    case 0:
                        img.Opacity = 0;
                        bmp.SetSource(nextPicture);
                        break;
                    case 1:
                        FadeOut.Completed += (x, y) =>
                        {
                            bmp.SetSource(nextPicture);
                        };
                        FadeOut.Begin();
                        break;
                }
            }
            
        }
        
        /// <summary>
        /// Cancel the slide show thread and navigate back.
        /// </summary>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            img.LayoutUpdated -= startImageLoading;
            try
            {
                slideShowThread.CancelAsync();
            }
            catch
            {

            }
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
            base.OnBackKeyPress(e);
        }
    }
}