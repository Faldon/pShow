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
        private IEnumerable<Picture> albumPics;
        Image img;
        BitmapImage bmp;

        /// <summary>
        /// The page constructor
        /// </summary>
        public SlideShow()
        {
            InitializeComponent();
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            slideShowThread.DoWork += loadNextPicture;
            slideShowThread.RunWorkerCompleted += updateUI;
            slideShowThread.WorkerSupportsCancellation = true;
            img = new Image();
            bmp = new BitmapImage();
            bmp.CreateOptions = BitmapCreateOptions.BackgroundCreation;
            ContentPanel.Children.Add(img);
        }

        /// <summary>
        /// Select all pictures for this album and start the slide show thread.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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
                    slideShowThread.RunWorkerAsync(albumPics);
                }
            }
        }

        /// <summary>
        /// Background thread to load the next picture of the album.
        /// </summary>
        private void loadNextPicture(object sender, DoWorkEventArgs e)
        {
            var albumPics = (IEnumerable<Picture>) e.Argument;
            System.IO.Stream nextPicture;
            if (albumPics.Count() > SlideShow.nextPicture)
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
                System.Threading.Thread.Sleep(App.slideDuration * 1000);
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
                slideShowThread.RunWorkerAsync(albumPics);
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