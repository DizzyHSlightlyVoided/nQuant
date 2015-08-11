using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WPF
namespace nQuantWpf
#else
namespace nQuant
#endif
{
    [Serializable]
    public class QuantizationException : ApplicationException
    {
        public QuantizationException(string message) : base(message)
        {

        }
    }
}
