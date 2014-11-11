using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary
{
    public interface IImageTypeConverter
    {
        bool CanConvert(object instance, Type targetType);
        object Convert(object instance);
    }
    
    /// <summary>
    /// Registers a TypeConverter in order to make integration of different frameworks easier.
    /// Usually an imageprocessing Framework introduces its own image type, in order to make things easier for use.
    /// Declare this Registration at assembly level attribute.
    /// </summary>
    public class ImageTypeConverterRegistration : Attribute
    {
        public ImageTypeConverterRegistration(params Type[] types)
        {
            Types = types;
        }

        public Type[] Types { get; set; }
    }
}
