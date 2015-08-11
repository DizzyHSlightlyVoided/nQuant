using System.Collections.Generic;
#if WPF
using System.Windows;
using System.Windows.Media.Imaging;
using Bitmap = System.Windows.Media.Imaging.BitmapSource;

namespace nQuantWpf
#else
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
namespace nQuant
#endif
{
    class ImageBuffer
    {
        public ImageBuffer(Bitmap image)
        {
#if WPF
            this.Image = image as WriteableBitmap;
            if (Image == null)
                Image = new WriteableBitmap(image);
#else
            this.Image = image;
#endif
        }

#if WPF
        public WriteableBitmap Image { get; set; }
#else
        public Bitmap Image { get; set; }
#endif

        public IEnumerable<Pixel[]> PixelLines
        {
            get
            {
                var bitDepth =
#if WPF
                    this.Image.Format.BitsPerPixel;
                var palette = this.Image.Palette;
#else
                    System.Drawing.Image.GetPixelFormatSize(Image.PixelFormat);
#endif
                if (bitDepth != 32)
                    throw new QuantizationException(string.Format("The image you are attempting to quantize does not contain a 32 bit ARGB palette. This image has a bit depth of {0} with {1} colors.", bitDepth,
#if WPF
                        palette == null ? 0 : palette.Colors.Count));

                int width = this.Image.PixelWidth;
                int height = this.Image.PixelHeight;
#else
                        Image.Palette.Entries.Length));

                int width = this.Image.Width;
                int height = this.Image.Height;
#endif

                int[] buffer = new int[width];
                Pixel[] pixels = new Pixel[width];
                for (int rowIndex = 0; rowIndex < height; rowIndex++)
                {
#if WPF
                    var rect = new Int32Rect(0, rowIndex, width, 1);
                    Image.CopyPixels(rect, buffer, width * 4, 0);
#else
                    BitmapData data = this.Image.LockBits(new Rectangle(0, rowIndex, width, 1), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    try
                    {
                        Marshal.Copy(data.Scan0, buffer, 0, width);
#endif
                        for (int pixelIndex = 0; pixelIndex < buffer.Length; pixelIndex++)
                        {
                            pixels[pixelIndex] = new Pixel(buffer[pixelIndex]);
                        }
#if !WPF
                    }
                    finally
                    {
                        this.Image.UnlockBits(data);
                    }
#endif
                    yield return pixels;
                }
            }
        }

        public void UpdatePixelIndexes(IEnumerable<byte[]> lineIndexes)
        {
#if WPF
            int width = this.Image.PixelWidth;
            int height = this.Image.PixelHeight;
#else
            int width = this.Image.Width;
            int height = this.Image.Height;
#endif
            var indexesIterator = lineIndexes.GetEnumerator();
            for (int rowIndex = 0; rowIndex < height; rowIndex++)
            {
                indexesIterator.MoveNext();
#if WPF
                this.Image.WritePixels(new Int32Rect(0, rowIndex, width, 1), indexesIterator.Current, width, 0);
#else
                BitmapData data = this.Image.LockBits(Rectangle.FromLTRB(0, rowIndex, width, rowIndex + 1), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                try
                {
                    Marshal.Copy(indexesIterator.Current, 0, data.Scan0, width);
                }
                finally
                {
                    this.Image.UnlockBits(data);
                }
#endif
            }
        }
    }
}

