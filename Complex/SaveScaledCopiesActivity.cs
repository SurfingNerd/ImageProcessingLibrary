using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Complex
{
    public class SaveScaledCopiesActivity : IActivity
    {
        public String DirectoryWithInput { get; set; }
        public String OutputDirectory { get; set; }
        public double ScaleFactor { get; set; }

        public void Execute(ActivityContext context)
        {
            CompoundActivity result = new CompoundActivity();
            DirectoryInfo dir = new DirectoryInfo(DirectoryWithInput);

            foreach (var item in dir.EnumerateFiles("*.png"))
            {
                LoadImageActivity loadImage = new LoadImageActivity(item);
                result.Children.Add(loadImage);

                ImageProcessingLibrary.ScaleImageActivity scale = new ScaleImageActivity();
                scale.ScaleFactor = ScaleFactor;
                scale.InputImageName = loadImage.LoadedImage.Name;
                scale.OutputImageName = scale.InputImageName + "_scaled";
                result.Children.Add(scale);
                SaveImageActivity saveImage = new SaveImageActivity();
                saveImage.SourceImageName = scale.OutputImageName;
                saveImage.DestinationFile = Path.Combine(OutputDirectory, saveImage.SourceImageName + ".png");
                result.Children.Add(saveImage);
            }

            result.Execute(context);
        }
    }
}
