#if WPF
using Bitmap = System.Windows.Media.Imaging.BitmapSource;
using Image = System.Windows.Media.Imaging.BitmapSource;

namespace nQuantWpf
#else
using System.Drawing;

namespace nQuant
#endif
{
    public interface IWuQuantizer
    {
        Image QuantizeImage(Bitmap image, int alphaThreshold, int alphaFader);
    }
}