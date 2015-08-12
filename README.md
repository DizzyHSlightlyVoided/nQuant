A fork of [nQuant](http://nquant.codeplex.com/), specifically [Phillipecp's nQuant Leaner And Faster](https://nquant.codeplex.com/SourceControl/network/forks/Philippecp/nQuantLeanerAndFaster) fork. This fork may be used in exactly the same way as the original version, with two additional `WuQuantizerBase.QuantizeImage()` overloads:

* `public Image QuantizeImage(Bitmap image, int alphaThreshold, int alphaFader, int colorCount)` - the `colorCount` parameter indicates the maximum number of colors allowed. Must be between 2 and 256 inclusive.
* `public Image QuantizeImage(Bitmap image, int alphaThreshold, int alphaFader, ref int colorCount)` - the same, except that when this method returns, `colorCount` will be changed to the actual computed maximum number of colors in the quantized image.

It also includes a second library, nQuantWpf.Core with a corresponding nQuantWpf command-line program, which performs the same operations using the Windows Presentation Foundation.
