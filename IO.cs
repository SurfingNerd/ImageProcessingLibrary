using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary
{
    public class LoadImageFromSingletonFileStreamCache : IActivity
    {
        public LoadImageFromSingletonFileStreamCache()
            : base()
        {

        }

        public string ImageName { get; set; }
        public string FileName { get; set; }

        private static ActivityContext s_globalContext;

        public void Execute(ActivityContext context)
        {
            if (s_globalContext == null)
            {
                OperationExecutor exe = new OperationExecutor();
                s_globalContext = new ActivityContext(exe);
            }

            if (!s_globalContext.IsImageAvailable(ImageName))
            {
                LoadImageActivity activity = new LoadImageActivity(new FileInfo(FileName), this.ImageName);
                OperationExecutor exe = new OperationExecutor();
                exe.Transform(activity, s_globalContext);
            }
            var image = s_globalContext.Get(ImageName);
            context.Set(ImageName, image);
        }
    }

    public class FileStreamCachingActivity : IActivity
    {
        public string FileName { get; set; }

        public string CreatedImageName { get; private set; }
        public IActivity CreateActivity { get; set; }
        public Stream CreatedStream { get; private set; }

        public FileStreamCachingActivity(string fileName, string createdImageName, IActivity createActivity)
        {
            FileName = fileName;
            CreateActivity = createActivity;
            CreatedImageName = createdImageName;
        }


        public virtual void Execute(ActivityContext context)
        {
            if (System.IO.File.Exists(FileName))
            {
                // = gecacht.
                // von direktory lesen und zurückgeben.

                OperationExecutor transformer = new OperationExecutor();
                LoadImageActivity loadActivity = new LoadImageActivity(new FileInfo(FileName), CreatedImageName);
                ProvideStreamActivity provideStream = new ProvideStreamActivity();
                provideStream.ImageName = CreatedImageName;

                CompoundActivity bag = new CompoundActivity();

                bag.Children.Add(loadActivity);
                bag.Children.Add(provideStream);
                // provideStream.
                transformer.Transform(bag);

                CreatedStream = provideStream.Stream;
            }
            else
            {
                OperationExecutor transformer = new OperationExecutor();

                // Wir müssen das Base Image Laden
                //LoadImageActivity loadActivity = new LoadImageActivity(new FileInfo(this.FileName));
                //this.



                ProvideStreamActivity provideStream = new ProvideStreamActivity();
                provideStream.ImageName = this.CreatedImageName;

                CompoundActivity bag = new CompoundActivity();

                bag.Children.Add(this.CreateActivity);
                bag.Children.Add(provideStream);

                SaveImageActivity saveActivity = new SaveImageActivity(CreatedImageName, this.FileName);
                bag.Children.Add(saveActivity);
                // provideStream.
                var activityContext = transformer.Transform(bag);


                //System.IO.File.WriteAllBytes(, 

     

                Stream stream = activityContext.Get(CreatedImageName).GetStream();
                this.CreatedStream = stream;

            }
        }

    }
}
