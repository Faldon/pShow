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
    class AlbumViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BitmapImage> _albumPictures;
        public ObservableCollection<BitmapImage> albumPictures
        {
            get { return _albumPictures; }
            set
            {
                _albumPictures = value;
                NotifyPropertyChanged("albumPictures");
            }
        }
        private String _albumName;
        public String albumName
        {
            get { return _albumName; }
            set 
            {
                _albumName = value;
                NotifyPropertyChanged("albumName");
            }
        }

        private Boolean _isLoading;
        public Boolean isLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading=value;
                NotifyPropertyChanged("isLoading");
            }
        }

        private BackgroundWorker workTask;

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

        private void loadData(object sender, DoWorkEventArgs e)
        {
            var album = (string)e.Argument;
            var pictureStreams = new List<System.IO.Stream>();
            using (MediaLibrary mediaLib = new MediaLibrary())
            {
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
            e.Result = pictureStreams;
        }

        private void dataLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            var pictureStreams = (List<System.IO.Stream>)e.Result;
            foreach (System.IO.Stream pictureStream in pictureStreams)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.CreateOptions = BitmapCreateOptions.DelayCreation;
                bmp.SetSource(pictureStream);
                albumPictures.Add(bmp);
            }
            isLoading = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

