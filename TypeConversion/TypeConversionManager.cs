using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
