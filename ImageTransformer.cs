
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary
{
    public class OperationExecutor
    {
        public ActivityContext Transform(IActivity transformDefinition)
        {
            ActivityContext context = new ActivityContext(this);
            transformDefinition.Execute(context);
            return context;
        }

       


        public ActivityContext Transform(IActivity activity, ActivityContext context)
        {
            activity.Execute(context);
            return context;
        }

        internal void NotifyActifityStarted(IActivity activity, ActivityContext activityContext)
        {
            //TODO: events werfen
        }

        internal void NotifyActifityFinished(IActivity activity, ActivityContext activityContext)
        {
            //todo events werfen.
            
        }
    }
}
