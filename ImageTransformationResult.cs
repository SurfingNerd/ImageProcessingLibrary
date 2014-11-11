using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary
{
    public class ImageTransformationResult
    {
        public List<IActivity> TransformationDetails { get; set; }
        public Image OutputBitmap{ get; private set; }

        public ImageTransformationResult(Image outputBitmap)
        {
            TransformationDetails = new List<IActivity>();
            OutputBitmap = outputBitmap;
        }        
    }
}
