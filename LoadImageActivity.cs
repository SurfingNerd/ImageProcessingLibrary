using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary
{
    /// <summary>
    /// Loads all images from directory
    /// </summary>
    public class LoadImagesActivity : IActivity
    {
        public LoadImagesActivity()
        {

        }

        public LoadImagesActivity(DirectoryInfo dir)
        {
            Dir = dir.FullName;
        }

        public string Dir { get; set; }

        public void Execute(ActivityContext context)
        {
            DirectoryInfo dir = new DirectoryInfo(Dir);
            foreach (var filter in new string[] { "*.png","*.jpg","*.bmp"})
            {
                foreach (var item in dir.EnumerateFiles(filter))
                {
                    string name = item.Name;
                    InMemoryImage fsi = new InMemoryImage(name, Bitmap.FromFile(item.FullName));
                    //FileSystemImage fsi = new FileSystemImage(item);
                    context.Set(fsi.Name, fsi);
                }
            }
        }
    }

    public class LoadImageActivity : IActivity, ICreateImageActivity
    {
        public string FileLocation { get; set; }
        public string OutputImageName { get; set; }
        public RuntimeImage LoadedImage { get; private set; }


        public LoadImageActivity()
        {

        }

        public LoadImageActivity(FileInfo fileInfo)
        {
            FileLocation = fileInfo.FullName;
        }

        public LoadImageActivity(FileInfo fileInfo, string imageName)
        {
            FileLocation = fileInfo.FullName;
            OutputImageName = imageName;
        }

        public LoadImageActivity(string fileLocation)
        {
            
            this.FileLocation = fileLocation;
        }

        public void Execute(ActivityContext context)
        {
            //TODO: Support web downloads ?!
 
            //FileSystemImage fsi = RuntimeImage;
            //context.Set(OutputImageName ?? fsi.Name, fsi);
            FileInfo fileInfo = new FileInfo(FileLocation);
            string imageName = OutputImageName ?? fileInfo.Name;
            Bitmap bitmap = (Bitmap)(Bitmap.FromFile(fileInfo.FullName));
            InMemoryImage image = new InMemoryImage(imageName, bitmap);
            LoadedImage = image;
            context.Set(imageName, image);
        }
    }

    public class ProvideStreamActivity : IActivity, ISingleImageActivity
    {
        public string ImageName { get; set; }

        public Stream Stream { get; private set; }

        public void Execute(ActivityContext context)
        {
            RuntimeImage image = context.Get(ImageName);
            Stream = image.GetStream();
        }
    }

    public class SaveImageActivity : IActivity
    {
        public SaveImageActivity()
        {

        }

        public SaveImageActivity(string sourceImageName, string destinationFile)
        {
            SourceImageName = sourceImageName;
            DestinationFile = destinationFile;
        }

        public string SourceImageName { get; set; }
        public string DestinationFile { get; set; }

        public void Execute(ActivityContext context)
        {
            FileInfo fi = new FileInfo(DestinationFile);
            DirectoryInfo dir = fi.Directory;
            if (!dir.Exists)
            {
                dir.Create();
            }

            RuntimeImage image = context.Get(SourceImageName);
            image.GetBitmap().Save(DestinationFile);
        }
    }
}
