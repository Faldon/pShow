using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;

namespace pShow.Model
{
    class AlbumPreview
    {
        public string title { get; set; }
        public BitmapImage preview { get; set; }
        public System.IO.Stream pictureStream { get; set; }
    }
}
