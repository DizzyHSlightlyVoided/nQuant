using System.Collections.Generic;
#if WPF
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Media.Imaging.BitmapSource;

namespace nQuantWpf
#else
using System.Drawing;
using System.Drawing.Imaging;

namespace nQuant
#endif
{
    public class WuQuantizer : WuQuantizerBase, IWuQuantizer
    {
        private IEnumerable<byte[]> indexedPixels(ImageBuffer image, Pixel[] lookups, int alphaThreshold, PaletteColorHistory[] paletteHistogram)
        {
#if WPF
            int pixelsCount = image.Image.PixelWidth * image.Image.PixelHeight;
#else
            int pixelsCount = image.Image.Width * image.Image.Height;
            var lineIndexes = new byte[image.Image.Width];
#endif
            PaletteLookup lookup = new PaletteLookup(lookups);
            foreach (var pixelLine in image.PixelLines)
            {
#if WPF
                var lineIndexes = new byte[image.Image.PixelWidth];
#endif
                for (int pixelIndex = 0; pixelIndex < pixelLine.Length; pixelIndex++)
                {
                    Pixel pixel = pixelLine[pixelIndex];
                    byte bestMatch = AlphaColor;
                    if (pixel.Alpha > alphaThreshold)
                    {
                        bestMatch = lookup.GetPaletteIndex(pixel);
                        paletteHistogram[bestMatch].AddPixel(pixel);
                    }
                    lineIndexes[pixelIndex] = bestMatch;
                }
                yield return lineIndexes;
            }
        }

        internal override Image GetQuantizedImage(ImageBuffer image, int colorCount, Pixel[] lookups, int alphaThreshold)
        {
#if WPF
            var paletteHistogram = new PaletteColorHistory[colorCount + 1];
            List<byte[]> indices = new List<byte[]>(indexedPixels(image, lookups, alphaThreshold, paletteHistogram));
            var result = new WriteableBitmap(image.Image.PixelWidth, image.Image.PixelHeight, image.Image.DpiX, image.Image.DpiY, PixelFormats.Indexed8,
                BuildPalette(paletteHistogram));
#else
            var result = new Bitmap(image.Image.Width, image.Image.Height, PixelFormat.Format8bppIndexed);
            var paletteHistogram = new PaletteColorHistory[colorCount + 1];
#endif
            var resultBuffer = new ImageBuffer(result);
            resultBuffer.UpdatePixelIndexes(indexedPixels(image, lookups, alphaThreshold, paletteHistogram));
#if !WPF
            result.Palette = BuildPalette(result.Palette, paletteHistogram);
#endif
            return result;
        }

#if WPF
        private BitmapPalette BuildPalette(PaletteColorHistory[] paletteHistogram)
        {
            Color[] palette = new Color[paletteHistogram.Length];
            for (int paletteColorIndex = 0; paletteColorIndex < paletteHistogram.Length; paletteColorIndex++)
            {
                palette[paletteColorIndex] = paletteHistogram[paletteColorIndex].ToNormalizedColor();
            }
            return new BitmapPalette(palette);
        }
#else
        private ColorPalette BuildPalette(ColorPalette palette, PaletteColorHistory[] paletteHistogram)
        {
            for (int paletteColorIndex = 0; paletteColorIndex < paletteHistogram.Length; paletteColorIndex++)
            {
                palette.Entries[paletteColorIndex] = paletteHistogram[paletteColorIndex].ToNormalizedColor();
            }
            return palette;
        }
#endif
    }
}
