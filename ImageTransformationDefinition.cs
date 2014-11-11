using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary
{

    public class ResizeImageTransformationDefinition : InputToOutputActivity 
    {
        public System.Drawing.Size NewSize { get; set; }


        public static Bitmap GetScaled(Bitmap input, Size newSize)
        {
            //todo: Scaling implementieren
            //Image image = input.ToManagedImage().GetThumbnailImage(newSize.Width, newSize.Height);

            //System.Drawing.Image image = input.GetThumbnailImage(newSize.Width, newSize.Height, new System.Drawing.Image.GetThumbnailImageAbort(AbortThumbnailGeneration), System.IntPtr.Zero);

            System.Drawing.Bitmap image = ResizeImage(newSize.Width, newSize.Height, input);
            return image;
               
        }

        private static System.Drawing.Bitmap ResizeImage(int newWidth, int newHeight,System.Drawing.Image imgPhoto)
        {
            int destWidth = newWidth;
            int destHeight = newHeight;

            Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    

                bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                imgPhoto.VerticalResolution);

                using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
                {
                    grPhoto.Clear(Color.Transparent);
                    grPhoto.InterpolationMode =
                        System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    grPhoto.DrawImage(imgPhoto,
                        new Rectangle(0, 0, destWidth, destHeight),
                        new Rectangle(0, 0, imgPhoto.Width, imgPhoto.Height),
                        GraphicsUnit.Pixel);
                }
                //NOTE: Disposing the original image here may be not wise,
                //because actually you dont know,
                //if someone else needs that image. (it comes via parameter)
                //imgPhoto.Dispose();
                return bmPhoto;
        }

        static bool AbortThumbnailGeneration()
        {
            return false;
        }

        protected override Bitmap GetOutputBitmap(ActivityContext context)
        {
            RuntimeImage runtimeImage = context.Get(InputImageName);
            return ResizeImageTransformationDefinition.GetScaled(runtimeImage.GetBitmap(), NewSize);   
        }
    }


    public abstract class OutputImageActivity : IActivity, ICreateImageActivity
    {
        public OutputImageActivity()
        {
            OutputImageName = "output";
        }

        public string OutputImageName { get; set; }

        public void Execute(ActivityContext context)
        {
            context.Set(OutputImageName, new InMemoryImage(OutputImageName, GetOutputBitmap(context)));
        }

        //protected abstract global::AForge.Imaging.UnmanagedImage GetOutput(AForge.Imaging.UnmanagedImage input);
        protected abstract Bitmap GetOutputBitmap(ActivityContext context);
    }

    public abstract class InputToOutputActivity : OutputImageActivity
    {
        public string InputImageName { get; set; }
    }

    public class ScaleImageActivity : InputToOutputActivity 
    {
        public double ScaleFactor { get; set; }
        public Size? LastUsedSize { get; set; }
        

        protected override Bitmap GetOutputBitmap(ActivityContext context)
        {
            RuntimeImage runtimeImage = context.Get(InputImageName);
            Bitmap input = runtimeImage.GetBitmap();
            double newWidth = input.Width * ScaleFactor;
            double newHeight = input.Height * ScaleFactor;
            LastUsedSize = new Size(Convert.ToInt32(Math.Round(newWidth)), Convert.ToInt32(Math.Round(newHeight)));
            return ResizeImageTransformationDefinition.GetScaled(input, LastUsedSize.Value);
        }
    }


    public class ActivityContext
    {
        private Dictionary<string, RuntimeImage> m_namedObjects = new Dictionary<string, RuntimeImage>();
        private OperationExecutor operationExecutor;

        public ActivityContext(OperationExecutor operationExecutor)
        {
            // TODO: Complete member initialization
            this.operationExecutor = operationExecutor;
        }

        public void NotifyActifityStarted(IActivity activity)
        {
            operationExecutor.NotifyActifityStarted(activity, this);
        }

        public void NotifyActifityFinished(IActivity activity)
        { 
            operationExecutor.NotifyActifityFinished(activity, this);
        }

        public RuntimeImage Get(string name)
        {
            return m_namedObjects[name];
        }

        public void Set(string name, RuntimeImage o)
        {
            m_namedObjects.Add(name, o);
        }

        public void Set(string name, object imageObject)
        {
            m_namedObjects.Add(name, new InMemoryImage(name, imageObject));
        }

        public bool IsImageAvailable(string imageName)
        {
            return m_namedObjects.ContainsKey(imageName);
        }

        public string[] GetAvailableImageIDs()
        {
            return m_namedObjects.Keys.ToArray();
        }

        public T Get<T>(string imageName)
        {
            RuntimeImage image = Get(imageName);
            
            throw new NotImplementedException();
        }
    }

    public interface IActivity
    {
        void Execute(ActivityContext context);
        //public SourceImageDetail Detail { get; set; }
    }

    public class CompoundActivity : IActivity
    {
        public CompoundActivity()
        {
            Children = new ObservableCollection<IActivity>();
        }
        public ObservableCollection<IActivity> Children { get; private set; }

        public void Execute(ActivityContext context)
        {
            foreach (var item in Children)
            {
                item.Execute(context);
            }
        }
    }


    public class CombineImagesActivity : OutputImageActivity
    {
        public CombineImagesActivity()
        {
            InputImageNames = new List<string>();
        }

        public CombineImagesActivity(string outputImageName, IEnumerable<string> inputLayers)
        {
            OutputImageName = outputImageName;
            InputImageNames = new List<string>();
            InputImageNames.AddRange(inputLayers);
        }

        public List<string> InputImageNames { get; private set; }

        protected override Bitmap GetOutputBitmap(ActivityContext context)
        {
            List<System.Drawing.Bitmap> bitmaps = new List<Bitmap>();
            Bitmap drawingBitmap = null;
            Graphics drawingGraphics = null;

            foreach (var imageName in InputImageNames)
            {
                Bitmap layerBitmap = context.Get(imageName).GetBitmap();

                if (drawingBitmap == null)
                {
                    drawingBitmap = new Bitmap(layerBitmap.Width, layerBitmap.Height);
                    drawingBitmap.MakeTransparent();
                    drawingGraphics = Graphics.FromImage(drawingBitmap);
                }

                //drawingGraphics.DrawImageUnscaled(layerBitmap, new Point(0, 0));
                drawingGraphics.DrawImage(layerBitmap, new Rectangle(0,0,layerBitmap.Width,layerBitmap.Height));
            }

            drawingGraphics.Flush();
            return drawingBitmap;
        }
    }
    //public class AForgeFilterActivity : IActivity
    //{
 
    //}
        
        

    //public class HueShiftImageLayerTransformationDefinition : IActivity
    //{
    //    public int HueShift { get; set; }
    //    public string TargetLayer { get; set; }

    //    public void DoSomething(ActivityContext context)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    
}
