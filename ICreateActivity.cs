using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary
{
    /// <summary>
    /// Declares an activity that creates a new image.
    /// Combine this interface with ISingleImageActivity if you create an Image Transformation. 
    /// </summary>
    public interface ICreateImageActivity : IActivity
    {
        string OutputImageName { get; set; }
    }

    /// <summary>
    /// Reads or changes a single Image.
    /// </summary>
    public interface ISingleImageActivity : IActivity
    {
        string ImageName { get; set; }
    }
}
