namespace Weezlabs.Storgage.UtilService.Helpers
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    using ImageResizer;

    /// <summary>
    /// Helper for Stream
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// Orientation property id from exif information
        /// </summary>
        private const Int32 orientationExifId = 0x112;

        /// <summary>
        /// Resize image from Stream
        /// </summary>
        /// <param name="inputStream">Input Stream.</param>
        /// <param name="maxSide">Max side of image in pixels.</param>
        /// <param name="disposeSource">Flag: true if should dispose input stream after resize.</param>
        /// <returns>Stream contains resized image.</returns>
        public static Stream ResizeImage(this Stream inputStream, Int32 maxSide, Boolean disposeSource = true)
        {
            RotateFlipType flip = OrientationToFlipType(1); // default flip

            using (var image = System.Drawing.Image.FromStream(inputStream,
                /* useEmbeddedColorManagement = */ true,
                /* validateImageData = */ false))
            {
                PropertyItem[] properties = image.PropertyItems; // get exif from image
                PropertyItem orientation = properties.FirstOrDefault(x => x.Id == orientationExifId);

                if (orientation != null)
                {
                    flip = OrientationToFlipType(BitConverter.ToInt16(orientation.Value, 0));
                }
            }

            inputStream.Seek(0, SeekOrigin.Begin);

            Stream outStream = new MemoryStream();
            var settings = new ResizeSettings
            {
                MaxWidth = maxSide,
                MaxHeight = maxSide,
                Flip = flip,
                Mode = FitMode.Max
            };
            ImageBuilder.Current.Build(inputStream, outStream, settings, disposeSource);

            return outStream;
        }

        /// <summary>
        /// Get RotateFlipType by id 
        /// note: http://www.impulseadventure.com/photo/exif-orientation.html
        /// </summary>
        /// <param name="orientation">Orientation</param>
        /// <returns>RotateFlipType</returns>
        private static RotateFlipType OrientationToFlipType(Int16 orientation)
        {
            switch (orientation)
            {
                case 1:
                    return RotateFlipType.RotateNoneFlipNone;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
                default:
                    return RotateFlipType.RotateNoneFlipNone;
            }
        }
    }
}
