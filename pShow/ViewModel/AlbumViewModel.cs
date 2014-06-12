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

namespace pShow.ViewModel
{
    /// <summary>
    /// The view model for a single album page.
    /// </summary>
    class AlbumViewModel : INotifyPropertyChanged
    {
        // private part of public albumPictures
        private ObservableCollection<BitmapImage> _albumPictures;
        /// <summary>
        /// The pictures contained in the album.
        /// Raises a PropertyChanged event on being set.
        /// </summary>
        public ObservableCollection<BitmapImage> albumPictures
        {
            get { return _albumPictures; }
            set
            {
                _albumPictures = value;
                NotifyPropertyChanged("albumPictures");
            }
        }

        // private part of public albumName
        private String _albumName;
        /// <summary>
        /// The album title.
        /// Raises a PropertyChanged event on being set.
        /// </summary>
        public String albumName
        {
            get { return _albumName; }
            set 
            {
                _albumName = value;
                NotifyPropertyChanged("albumName");
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
                _isLoading=value;
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
        /// Constructs a new AlbumViewModel.
        /// Assigns the work and work completed methods to the BackgroundWorker and sets the album title.
        /// </summary>
        /// <param name="albumname">The album title</param>
        public AlbumViewModel(string albumname)
        {
            albumPictures = new ObservableCollection<BitmapImage>();
            workTask = new BackgroundWorker();
            workTask.DoWork += loadData;
            workTask.RunWorkerCompleted += dataLoaded;
            albumName = albumname;
            isLoading = true;
            workTask.RunWorkerAsync(albumname);
        }

        // The background work method
        private void loadData(object sender, DoWorkEventArgs e)
        {
            // get selected album
            var album = (string)e.Argument;
            var pictureStreams = new List<System.IO.Stream>();
            using (MediaLibrary mediaLib = new MediaLibrary())
            {
                // select all pictures with their album name equals the selected album
                var albumPics = from p in mediaLib.Pictures where p.Album.Name.Equals(album) select p;
                foreach (Picture p in albumPics)
                {
                    var ms = new MemoryStream();
                    {
                        p.GetThumbnail().CopyTo(ms);
                        pictureStreams.Add(ms);
                    }
                }
            }
            // hand over a list of System.IO.Stream as result of background task
            e.Result = pictureStreams;
        }

        // The background work completed method
        private void dataLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            // get the list of System.IO.Stream
            var pictureStreams = (List<System.IO.Stream>)e.Result;
            foreach (System.IO.Stream pictureStream in pictureStreams)
            {
                // create a new BitmapImage and add the current stream as source
                BitmapImage bmp = new BitmapImage();
                bmp.CreateOptions = BitmapCreateOptions.DelayCreation;
                bmp.SetSource(pictureStream);
                // add the image to the list of BitmapImage albumPictures in the ui thread
                albumPictures.Add(bmp);
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

