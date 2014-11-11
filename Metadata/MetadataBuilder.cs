using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessingLibrary.Metadata;
using System.Reflection;

namespace ImageProcessingLibrary.Metadata
{
    public class MetadataBuilder
    {
        public Metadata BuildFromReferencedAssemblies()
        {
            var dict = System.Reflection.Assembly.GetExecutingAssembly().MyGetReferencedAssembliesRecursive();
            
            return BuildFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());               
        }

        public Metadata BuildFromAppConfig()
        {
            AppConfigFascade appConfig = AppConfigFascade.Instance;
            List<Assembly> assembliesToBuild = new List<Assembly>();
            assembliesToBuild.Add( Assembly.GetExecutingAssembly());

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromConfiguredAssemblyLocation);
            foreach (string assemblyName in appConfig.Dlls)
            {
                Assembly assembly = Assembly.Load(assemblyName);
                assembliesToBuild.Add(assembly);
            }

            return BuildFromAssemblies(assembliesToBuild);
        }

        private Assembly LoadFromConfiguredAssemblyLocation(object sender, ResolveEventArgs args)
        {
            AppConfigFascade fascade = AppConfigFascade.Instance;

            foreach (var directory in fascade.DllDirectories)
            {
                string fileName = System.IO.Path.Combine(directory, args.Name + ".dll");
                if (System.IO.File.Exists(fileName))
                {
                    return Assembly.LoadFile(fileName);
                }
            }
            return null;
        }

        public Metadata BuildFromAssemblies(IEnumerable<System.Reflection.Assembly> assemblies)
        {
            Metadata result = new Metadata();
 	        foreach (var assembly in assemblies)
	        {
                foreach( Type type in assembly.GetExportedTypes())
                {
                    if (!type.IsAbstract && type.GetInterface("ImageProcessingLibrary.IActivity") != null)
                    {
                        ActivityMetadata metadata  =new ActivityMetadata();                       
                        metadata.HasStandardConstructor = type.GetConstructor(new Type[0]) != null;
                        metadata.ActivityType = type;
                        metadata.CreateActivity = type.GetInterface(typeof(ICreateImageActivity).FullName) != null;
                        metadata.ReadsSingleImage = type.GetInterface(typeof(ISingleImageActivity).FullName) != null;
                        result.Activities.Add(metadata);
                    }
                }
	        }
            return result;
        }
        
    }
}
