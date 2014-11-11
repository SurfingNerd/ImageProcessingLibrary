using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Metadata
{
    public class Metadata
    {
        public System.Collections.ObjectModel.ObservableCollection<ActivityMetadata> Activities { get; private set; }

        public Metadata()
        {
            Activities = new System.Collections.ObjectModel.ObservableCollection<ActivityMetadata>();
        }

    }

    public class ActivityMetadata
    {
        public Type PrimaryImageType { get; set; }
        public Type ActivityType { get; set; }
        public List<string> Hashtags { get; private set; }
        public bool CreateActivity { get; set; }
        public bool ReadsSingleImage { get; set; }

        /// <summary>
        /// Only Activities with an Standard Constructor can be used for some Operations, 
        /// By design, every Activity SHOULD have an Standard Constructor.
        /// </summary>
        public bool HasStandardConstructor { get; set; }

        public ActivityMetadata()
        {
            Hashtags = new List<string>();
        }

        public override string ToString()
        {
            return ActivityType.Name + (Hashtags.Count() > 0 ? " [" + string.Join(" ", Hashtags) + "]" : "");
        }

        public IActivity CreateNewActivity()
        {
            return (IActivity)Activator.CreateInstance(ActivityType);
        }
    }
}
