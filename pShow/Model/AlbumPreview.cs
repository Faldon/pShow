using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;

namespace pShow.Model
{
    /// <summary>
    /// The class representing a single album preview.
    /// </summary>
    class AlbumPreview
    {
        /// <summary>
        /// The name of the album.
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// The image servig as album preview.
        /// </summary>
        public BitmapImage preview { get; set; }
        /// <summary>
        /// A picture stream serving as source for the album preview image.
        /// </summary>
        public System.IO.Stream pictureStream { get; set; }
    }
}
