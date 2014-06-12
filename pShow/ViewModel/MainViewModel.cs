using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using pShow.Model;

namespace pShow.ViewModel
{
    /// <summary>
    /// The view model for the album overview page.
    /// </summary>
    class MainViewModel : INotifyPropertyChanged
    {
        // private part of public albumPreviews
        private ObservableCollection<AlbumPreview> _albumPreviews;
        /// <summary>
        /// The preview information for all displayable picture albums.
        /// Raises a PropertyChanged event on being set.
        /// </summary>
        public ObservableCollection<AlbumPreview> albumPreviews
        {
            get { return _albumPreviews; }
            set
            {
                _albumPreviews = value;
                NotifyPropertyChanged("albumPreviews");
            }
        }

        // private part of public isLoading
        private Boolean _isLoading;
        /// <summary>
        /// A flag if the view model is currently retrieving data.
        /// Raises a PropertyChanged event on being set.
        /// </summary>
        public Boolean isLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged("isLoading");
            }
        }

        // BackgroundWorker for loading the album data
        private BackgroundWorker workTask;

        /// <summary>
        /// The event for notifying a property change.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Constructs a new MainViewModel.
        /// Assigns the work and work completed methods to the BackgroundWorker.
        /// </summary>
        public MainViewModel()
        {
            albumPreviews = new ObservableCollection<AlbumPreview>();
            workTask = new BackgroundWorker();
            workTask.DoWork += loadData;
            workTask.RunWorkerCompleted += dataLoaded;
            isLoading = true;
            workTask.RunWorkerAsync();
        }

        // The background work method
        private void loadData(object sender, DoWorkEventArgs e)
        {
            var allAlbums = new List<PictureAlbum>();
            // create a List of AlbumPreview for the work completed method
            var albumList = new List<AlbumPreview>();
            using (MediaLibrary mediaLib = new MediaLibrary())
            {
                // add all albums from the RootPictureAlbum to a list of PictureAlbum
                foreach (PictureAlbum album in mediaLib.RootPictureAlbum.Albums)
                {
                    allAlbums.Add(album);
                }
                // iterate over the list of PictureAlbum
                for (var i = 0; i < allAlbums.Count; i++)
                {
                    // add all children PictureAlbum of current album to the list of PictureAlbum
                    foreach (PictureAlbum album in allAlbums[i].Albums)
                    {
                        allAlbums.Add(album);
                    }
                }
            }
            allAlbums.OrderBy(a => a.Name);
            foreach (PictureAlbum album in allAlbums)
            {
                // if the album in the list of PictureAlbum contains at least one Picture, create a new AlbumPreview,
                // set the title of the album and copy the picture stream of the first picture to the AlbumPreview.
                if (album.Pictures.Count > 0)
                {
                    var ms = new MemoryStream();
                    var pre = new AlbumPreview();

                    album.Pictures[0].GetThumbnail().CopyTo(ms);
                    pre.pictureStream = ms;
                    pre.title = album.Name;
                    // add the AlbumPreview to the list of AlbumPreview
                    albumList.Add(pre);
                }
            }
            // hand over the list of AlbumPreview as result of background task
            e.Result = albumList;
        }

        // The background work completed method
        private void dataLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            // get the list of AlbumPreview
            var albumList = (List<AlbumPreview>)e.Result;
            foreach (AlbumPreview pre in albumList)
            {
                // create a new BitmapImage and add the current AlbumPreview's picture stream as source
                BitmapImage bmp = new BitmapImage();
                bmp.CreateOptions = BitmapCreateOptions.DelayCreation;
                bmp.SetSource(pre.pictureStream);
                // add the BitmapImage as preview image of current AlbumPreview
                pre.preview = bmp;
                // add the current AlbumPreview to the list of AlbumPreview albumPreviews in the ui thread
                albumPreviews.Add(pre);
            }
            // set flag isLoading to indicate that the task completed
            isLoading = false;
        }

        /// <summary>
        /// Notify the system that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
