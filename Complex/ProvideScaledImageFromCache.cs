using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Complex
{
    public class ProvideScaledImageFromCache : IActivity
    {
        public Image ProvidedImage { get; private set; }

        public void Execute(ActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
