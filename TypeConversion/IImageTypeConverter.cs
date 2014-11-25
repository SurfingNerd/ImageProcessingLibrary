using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary
{
    public interface IImageTypeConverter
    {
        bool CanConvert(object instance, Type targetType);
        object Convert(object instance, Type targetType);
    }

    /// <summary>
    /// abstract baseclass that simplifies creating converters to convert from T to Bitmap and Bitmap to T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingleBitmapImageTypeConverter<T> : IImageTypeConverter
    {
        public bool CanConvert(object instance, Type targetType)
        {
            return IsForwardConversion(instance, targetType) || IsBackwardConversion(instance, targetType);
        }

        private bool IsForwardConversion(object instance, Type targetType)
        {
            return instance is Bitmap && (targetType == typeof(T));
        }

        private bool IsBackwardConversion(object instance, Type targetType)
        {
            return instance is T && (targetType == typeof(Bitmap));
        }

        public object Convert(object instance, Type targetType)
        {
            if (IsForwardConversion(instance, targetType))
            {
                return ConvertToSpecial((Bitmap)instance);
            }
            else if (IsBackwardConversion(instance, targetType))
            {
                return ConverToBitmap((T)instance);
            }

            throw new InvalidOperationException();
        }

        protected abstract Bitmap ConverToBitmap(T instance);

        protected abstract T ConvertToSpecial(Bitmap bitmap);
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
