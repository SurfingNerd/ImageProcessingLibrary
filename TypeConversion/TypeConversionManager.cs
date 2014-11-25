using ImageProcessingLibrary.Metadata;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.TypeConversion
{
    public class TypeConversionManager
    {
        private static TypeConversionManager s_instance;

        public static TypeConversionManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new TypeConversionManager();
                    s_instance.Initialize();
                }
                return s_instance;
            }
        }

        private List<IImageTypeConverter> ImageTypeConverters
        {
            get;
            set;
        }

        public void Initialize()
        {
            ImageTypeConverters = new List<IImageTypeConverter>();
             //AppConfigFascade.Instance.Dlls

            //List<Assembly> assemblies = new List<Assembly>();
            //assemblies.Add( Assembly.GetExecutingAssembly());


            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (ImageTypeConverterRegistration registeredConverter in assembly.GetCustomAttributes<ImageTypeConverterRegistration>())
                {
                    foreach (Type type in registeredConverter.Types)
                    {
                        ImageTypeConverters.Add((IImageTypeConverter)Activator.CreateInstance(type));
                    }
                }
            }
        }

        private TypeConversionManager()
        {

        }

        /// <summary>
        /// Gets the delivered Image as the wanted T Representation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="imageObject"></param>
        /// <returns></returns>
        internal T GetImage<T>(object imageObject)
        {
            //1. check if imageObject is allready T
            //2. try to find a direct conversion
            //3. try to convert to Bitmap and than try to convert to the correct type.
            if (imageObject is T)
            {
                return (T)imageObject;
            }
            else
            {
                foreach (var converter in ImageTypeConverters)
                {
                    if (converter.CanConvert(imageObject, typeof(T)))
                    {
                        return (T)converter.Convert(imageObject, typeof(T));
                    }
                }
            }

            Bitmap bmp = GetImage<Bitmap>(imageObject);
            return GetImage<T>(bmp);
        }
    }
}
