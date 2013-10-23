using Composite.Core;
using Composite.Core.IO;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using NuGet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Composite.Integration.Nuget.PackageManager
{
    public class CompositePackageManager : NuGet.PackageManager
    {
        private static readonly string mapping_rules = Path.Combine(PathUtil.BaseDirectory, "App_Data/nuget/nuget.mapping.rules.xml");
        private Lazy<XElement> rules = new Lazy<XElement>(() => XDocument.Load(mapping_rules).Root);

        public XElement Rules { get { return rules.Value; } }

        private sealed class FileToCopy
        {
            public string Source { get; set; }
            public string TargetFilePath { get; set; }
        }

        public  CompositePackageManager()
            : base(PackageRepositoryFactory.Default.CreateRepository("https://www.nuget.org/api/v2/"), Path.Combine( PathUtil.BaseDirectory , "App_Data/nuget/packages"))
        {
            PackageInstalled += CompositePackageManager_PackageInstalled;
            PackageInstalling += CompositePackageManager_PackageInstalling;
            PackageUninstalling += CompositePackageManager_PackageUninstalling;
            PackageUninstalled += CompositePackageManager_PackageUninstalled;
        }

        void CompositePackageManager_PackageUninstalled(object sender, PackageOperationEventArgs e)
        {
           
        }

        void CompositePackageManager_PackageUninstalling(object sender, PackageOperationEventArgs e)
        {
           
        }


        void CompositePackageManager_PackageInstalling(object sender, PackageOperationEventArgs e)
        {
            Log.LogInformation("Nuget Installer", "Nuget Installing: {0}", e.Package);
        }

        void CompositePackageManager_PackageInstalled(object sender, PackageOperationEventArgs e)
        {
            Log.LogInformation("Nuget Installer", "Nuget Installed: {0}", e.Package);

            var _filesToCopy = new List<FileToCopy>();
            var libs = new DirectoryInfo(Path.Combine(e.InstallPath, "lib"));
            if (libs.Exists)
            {
                //Take the dlls in the order net45,net45-client, net40, net40-client
                string bindir = Path.Combine(libs.FullName, "net45");
                if (!Directory.Exists(bindir))
                    bindir = Path.Combine(libs.FullName, "net45-Client");
                if (!Directory.Exists(bindir))
                    bindir = Path.Combine(libs.FullName, "net40");                                
                if (!Directory.Exists(bindir))
                    bindir = Path.Combine(libs.FullName, "net40-Client");
                if (!Directory.Exists(bindir))
                    bindir = string.Empty;
                if (!string.IsNullOrEmpty(bindir))
                    _filesToCopy.AddRange(Directory.GetFiles(bindir)
                        .Select(f => new FileToCopy { Source = f, TargetFilePath = PathUtil.Resolve(@"~\bin\" + Path.GetFileName(f)) }));
            }

            var content = Path.Combine(e.InstallPath, "content");
            if (Directory.Exists(content))
            {
                foreach (var dir in Directory.GetDirectories(content))
                {
                    String[] allfiles = System.IO.Directory.GetFiles(dir, "*.*", System.IO.SearchOption.AllDirectories);
                    var dirname = Path.GetFileName(dir).ToLower();

                    _filesToCopy.AddRange(allfiles.Select(f =>
                            new FileToCopy { Source = f, TargetFilePath = GetTargetPath(f.Substring(dir.Length + 1), dirname, e.Package) }));
                              //  PathUtil.Resolve((new[]{"scripts", "content"}.Contains(dirname) ? @"~\Frontend\" :@"~\" )+ dirname + "\\" +f.Substring(dir.Length + 1)) }));
                
                }
                

                _filesToCopy.AddRange(Directory.GetFiles(content).Select(f =>
                        new FileToCopy { Source = f, TargetFilePath = PathUtil.Resolve(@"~\Frontend\" + f.Substring(content.Length + 1)) }));


            }

            var DataTypeTypesManager = typeof(BasePackageFragmentInstaller).Assembly.GetType("Composite.Data.DataTypeTypesManager");

            foreach (FileToCopy fileToCopy in _filesToCopy)
            {
                var directory = Path.GetDirectoryName(fileToCopy.TargetFilePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                File.Copy(fileToCopy.Source, fileToCopy.TargetFilePath, true);


                if (fileToCopy.TargetFilePath.StartsWith(Path.Combine(PathUtil.BaseDirectory, "Bin"), StringComparison.InvariantCultureIgnoreCase)
                    && fileToCopy.TargetFilePath.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    Assembly assembly = Assembly.LoadFrom(fileToCopy.TargetFilePath);
                    DataTypeTypesManager.InvokeMember("AddNewAssembly", BindingFlags.InvokeMethod, null, null, new[] { assembly });
                    // DataTypeTypesManager.AddNewAssembly(assembly);
                }

            }

        }


        private string GetTargetPath(string f, string dirname, IPackage package)
        {
            var xpackage = Rules.Elements().SingleOrDefault(e => string.Compare((string)e.Attribute("id"), package.Id, true) == 0);
            if(xpackage != null)
            {
                var mapping = xpackage.Elements("path").SingleOrDefault(x => string.Compare((string)x.Attribute("from"), dirname, true) == 0);
                if (mapping != null)
                    return PathUtil.Resolve((string)mapping.Attribute("to") + "\\" + f);
            }

            return PathUtil.Resolve((new[]{"scripts", "content"}.Contains(dirname) ? @"~\Frontend\" :@"~\" )+ dirname + "\\" +f );


        }



        [ImportMany]
        public IEnumerable<ICommand> Commands { get; set; }
    }
}
