using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary
{
    public class RuntimeImageCollection : System.Collections.ObjectModel.KeyedCollection<string, RuntimeImage>
    {
        protected override string GetKeyForItem(RuntimeImage item)
        {
            return item.Name;
        }
    }

    public abstract class RuntimeImage
    {
        public RuntimeImage(string name)
        {
           Name = name;
        }

        public string Name { get; private set; }
        public abstract Bitmap GetBitmap();
        public abstract T GetFrameworkImage<T>();
        
        public virtual Stream GetStream()
        {
            Bitmap bitmap = GetBitmap();
            MemoryStream stream = new MemoryStream();
            //todo: what file format is best ? Png = loseless + compressed, bitmap = losess, uncompressed, but fast
            bitmap.Save(stream, ImageFormat.Png);
            return stream;
        }
    }

    [Serializable]
    public class InMemoryImage : RuntimeImage
    {
        private Bitmap m_bitmap = null;
        private object m_frameworkImage = null;
        //private object m_currentAlternative

        public InMemoryImage(string imageName, Bitmap inputBitmap)
            : base(imageName)
        {
            m_bitmap = inputBitmap;
        }

        public InMemoryImage(string name, object imageObject)
            : base(name)
        {
            // TODO: Complete member initialization
            
            if (imageObject is Bitmap)
            {
                m_bitmap = (Bitmap)(imageObject);
            }
            else
            {
                this.m_frameworkImage = imageObject;
            }
        }

        public override Bitmap GetBitmap()
        {
            if (m_bitmap != null)
            {
                return m_bitmap;
            }
            else if (m_frameworkImage != null)
            {
                m_bitmap = GetFrameworkImage<Bitmap>();
                m_frameworkImage = null;
                return m_bitmap;

            }
            else
            {
                return null;
            }
        }

        public override T GetFrameworkImage<T>()
        {
            if (m_frameworkImage is T)
                return (T)m_frameworkImage;

            if (m_bitmap != null)
            {
                T result = TypeConversion.TypeConversionManager.Instance.GetImage<T>(m_bitmap);
                //we clear the bitmap slot, because maybe the framework image gets changed.
                //once a bitmap is requested again, it gets converted back again.
                m_bitmap = null;
                m_frameworkImage = result;
                return result;
            }

            if (m_frameworkImage != null)
            {
                T result = TypeConversion.TypeConversionManager.Instance.GetImage<T>(m_bitmap);
                m_frameworkImage = result;
                return result;
            }

            return default(T);
        }
    }

    //[Serializable]
    //public class FileSystemImage : RuntimeImage //: ImageDefinition
    //{

    //    public FileInfo File { get; private set; }

    //    public FileSystemImage(FileInfo file)
    //        : base(System.IO.Path.GetFileNameWithoutExtension(file.FullName))
    //    {
    //        File = file;
    //    }

    //    public FileSystemImage(string name, FileInfo file)
    //        : base(name)
    //    {
    //        File = file;
    //    }

    //    public static RuntimeImageCollection FromDirectory(string directory)
    //    {
    //        DirectoryInfo dirInfo = new DirectoryInfo(directory);
    //        return FromDirectory(dirInfo);
    //    }

    //    private static RuntimeImageCollection FromDirectory(DirectoryInfo dirInfo)
    //    {
    //        RuntimeImageCollection result = new RuntimeImageCollection();

    //        foreach (FileInfo file in dirInfo.GetFiles())
    //        {
    //            result.Add(new FileSystemImage(file));
    //        }

    //        return result;
    //    }

    //    private Bitmap m_bitmap;
    //    private global::AForge.Imaging.UnmanagedImage m_unmanagedImage;

    //    public override Bitmap GetBitmap()
    //    {
    //        if (m_bitmap == null)
    //        {
    //            //Manchmal wird ein bil geladen, und der zugriff auf ein property führt zu einem 
    //            //"Invalid Parameter" Exception.
    //            //m_bitmap =(Bitmap) Bitmap.FromFile(this.File.FullName); 


    //            using (FileStream stream = this.File.OpenRead())
    //            {
    //                m_bitmap = Bitmap.FromStream(stream) as Bitmap;
    //            }
    //        }
    //        return m_bitmap;
    //    }
    //}
}
