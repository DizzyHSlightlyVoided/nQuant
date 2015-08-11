#if WPF
using System.Windows.Media;
namespace nQuantWpf
#else
using System.Drawing;
namespace nQuant
#endif
{
    struct PaletteColorHistory
    {
        public int Alpha;
        public int Red;
        public int Green;
        public int Blue;
        public int Sum;

        public Color ToNormalizedColor()
        {
            return (Sum != 0) ? Color.FromArgb((byte)(Alpha /= Sum), (byte)(Red /= Sum), (byte)(Green /= Sum), (byte)(Blue /= Sum)) :
#if WPF
                new Color();
#else
                Color.Empty;
#endif
        }

        public void AddPixel(Pixel pixel)
        {
            Alpha += pixel.Alpha;
            Red += pixel.Red;
            Green += pixel.Green;
            Blue += pixel.Blue;
            Sum++;
        }
    }
}
