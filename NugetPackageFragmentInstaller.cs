using Composite.Core;
using Composite.Core.IO;
using Composite.Core.PackageSystem;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Composite.Integration.Nuget.PackageManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using NuGet;

namespace Composite.Integration.Nuget
{
    public class NugetPackageFragmentInstaller : BasePackageFragmentInstaller
    {
        private sealed class FileToCopy
        {
            public string Source { get; set; }
            public string TargetFilePath { get; set; }
        }
        private sealed class PackagesInstalled
        {
            public string Name { get; set; }
            public string Version { get; set; }
        }
      //  private static readonly string nugetexe = Path.Combine(PathUtil.BaseDirectory, "App_Data/nuget/nuget.exe");
      //  private static readonly string packagesfolder = Path.Combine(PathUtil.BaseDirectory, "App_Data/nuget/packages");

        private List<FileToCopy> _filesToCopy = null;
        private List<IPackage> _packagesToInstall = null;
        private CompositePackageManager packageManager = new CompositePackageManager();

        public override IEnumerable<XElement> Install()
        {
            Verify.IsNotNull(_packagesToInstall, "NugetPackageFragmentInstaller has not been validated");

           
            packageManager.PackageInstalled += packageManager_PackageInstalled;
            packageManager.PackageInstalling += packageManager_PackageInstalling;
          
            foreach(var package in _packagesToInstall)
            {
                packageManager.InstallPackage(package, false, true);

            }
            

            return Configuration;
        }

        void packageManager_PackageInstalling(object sender, PackageOperationEventArgs e)
        {
            Log.LogInformation("Nuget Installer", "Nuget Installing: {0}", e.Package);
        }

        void packageManager_PackageInstalled(object sender, PackageOperationEventArgs e)
        {
            Log.LogInformation("Nuget Installer", "Nuget Installed: {0}", e.Package); 
        }


        public override IEnumerable<PackageFragmentValidationResult> Validate()
        {
            var result = new List<PackageFragmentValidationResult>();
            Log.LogInformation("NugetPackageFragmentInstaller validation", string.Join("\n", Configuration.Select(e=>e.ToString())));

            if(Configuration.Any(n=> n.Name != "nuget"))
                result.Add(new PackageFragmentValidationResult(PackageFragmentValidationResultType.Fatal, new ArgumentOutOfRangeException("element","each elements need to be a nuget")));

            _filesToCopy = new List<FileToCopy>();
            _packagesToInstall = new List<IPackage>();

           


            foreach (var package in Configuration.Select(n =>
                new
                {
                    PackageName = (string)n.Attribute("id"),
                    Version = (string)n.Attribute("version"),
                    Pre = (bool)n.Attribute("pre")
                }))
            {

                if (string.IsNullOrWhiteSpace(package.Version) || string.IsNullOrWhiteSpace(package.PackageName))
                {
                    result.Add(new PackageFragmentValidationResult(PackageFragmentValidationResultType.Fatal,
                        new KeyNotFoundException("Package Name and Version must be specified")));
                    continue;
                }


                var nugetPackage = packageManager.SourceRepository.FindPackage(package.PackageName, SemanticVersion.Parse(package.Version), package.Pre, true);



                if (nugetPackage == null)
                {
                    result.Add(new PackageFragmentValidationResult(PackageFragmentValidationResultType.Fatal, new KeyNotFoundException(package.PackageName +
                        " is not supported.\n Ensure that the package exist on nuget")));
                    continue;
                }
                _packagesToInstall.Add(nugetPackage);
            }
                //var proc = new Process {
                //    StartInfo = new ProcessStartInfo {
                //        FileName =  nugetexe,
                //        Arguments = string.Format("list {1} -ConfigFile nuget.xml {0}", package.PackageName, package.Pre ? "-pre" : ""),
                //        UseShellExecute = false,
                //        WorkingDirectory = Path.GetDirectoryName(nugetexe),
                //        RedirectStandardError = true,
                //        RedirectStandardOutput = true,
                //        CreateNoWindow = true
                //    }
                //};

                //proc.Start(); bool found = false; 
                //var search = string.Format("{0} {1}", package.PackageName, package.Version).ToLower();
                //while (!proc.StandardOutput.EndOfStream)
                //{
                //    string line = proc.StandardOutput.ReadLine();
                //    if (line.ToLower() == search)
                //    {
                //        found = true;
                //        proc.StandardOutput.ReadToEnd();
                //    }// do something with line
                //}
                //if (proc.ExitCode != 0)
                //{
                //   result.Add(new PackageFragmentValidationResult(PackageFragmentValidationResultType.Fatal,
                //       new InvalidOperationException("Nuget didnt exit with code 0 " + proc.ExitCode)));
                //}
        

 








                //var installproc = new Process
                //{
                //    StartInfo = new ProcessStartInfo
                //    {
                //        FileName = nugetexe,
                //        Arguments = string.Format("install {0} {1} -version {2} -outputdirectory packages -ConfigFile nuget.xml -nocache", package.PackageName, package.Pre ? "-pre" : "", package.Version),
                //        UseShellExecute = false,
                //        WorkingDirectory = Path.GetDirectoryName(nugetexe),
                //        RedirectStandardError = true,
                //        RedirectStandardOutput = true,
                //        CreateNoWindow = true
                //    }
                //};


                //installproc.Start();
                //var installedpackages = new List<PackagesInstalled>();
                //while (!installproc.StandardOutput.EndOfStream)
                //{
                //    string line = installproc.StandardOutput.ReadLine();
                //    Log.LogInformation("NugetPackageFragmentInstaller", line);
                //    var match = Regex.Match(line, "Successfully installed '(.*) (.*)'.");
                //    if(match.Success)
                //    {
                //        var name = match.Groups[1].Value;
                //        var version = match.Groups[2].Value;
                //        installedpackages.Add(new PackagesInstalled { Name = name, Version = version });
                //    }

                //}


            //    foreach (var pack in installedpackages)
            //    {
            //        var filename = Path.Combine(PathUtil.BaseDirectory, "App_Data/nuget/" + pack.Name + ".xml");
            //        if (File.Exists(filename))
            //        {

            //            //We controll the install process.

            //            var installmapping = XElement.Load(filename);

            //            var mapping = installmapping.Descendants("package").SingleOrDefault(n => (string)n.Attribute("version") == pack.Version);
            //            if (mapping == null)
            //            {
            //                result.Add(new PackageFragmentValidationResult(PackageFragmentValidationResultType.Fatal,
            //                    new KeyNotFoundException(package.PackageName + " is not supported for version " + package.Version)));
            //                continue;
            //            }

            //            _filesToCopy.AddRange(mapping.Elements("copy").Select(n => new FileToCopy
            //            {
            //                TargetFilePath = (string)n.Attribute("Destination"),
            //                Source = PathUtil.Resolve((string)n.Attribute("Source"))
            //            }));
            //        }
            //        else
            //        {
            //            var libs = new DirectoryInfo(Path.Combine(packagesfolder, pack.Name + "." + pack.Version,"lib"));
            //            if(libs.Exists)
            //            {
            //                string bindir = Path.Combine(libs.FullName, "net45");
            //                if(!Directory.Exists(bindir))
            //                    bindir = Path.Combine(libs.FullName, "net40");
            //               if(!Directory.Exists(bindir))
            //                   bindir = string.Empty;
            //                if(!string.IsNullOrEmpty(bindir))
            //                    _filesToCopy.AddRange(Directory.GetFiles(bindir)
            //                        .Select(f => new FileToCopy{ Source = f, TargetFilePath=PathUtil.Resolve(@"~\bin\"+ Path.GetFileName(f))} ));
            //            }

            //            var content = Path.Combine(packagesfolder, pack.Name + "." + pack.Version,"content");
            //            if(Directory.Exists(content))
            //            {
            //                foreach (var dir in Directory.GetDirectories(content))
            //                {
            //                    String[] allfiles = System.IO.Directory.GetFiles(dir, "*.*", System.IO.SearchOption.AllDirectories);
            //                    var dirname = Path.GetFileName(dir);
            //                    if(dirname.ToLower() == "scripts")
            //                    {
            //                        _filesToCopy.AddRange(allfiles.Select(f => 
            //                            new FileToCopy { Source = f, TargetFilePath =PathUtil.Resolve(@"~\Frontend\Scripts\" + f.Substring(dir.Length+1)) }));
            //                    }
            //                }
            //            //   

            //                    _filesToCopy.AddRange(Directory.GetFiles(content).Select(f =>
            //                            new FileToCopy { Source = f, TargetFilePath = PathUtil.Resolve(@"~\Frontend\" + f.Substring(content.Length + 1)) }));
                            

            //            }

            //        }

            //    }

            //}
            return result;
        }
    }

    public class NugetPackageFragmentUninstaller : BasePackageFragmentUninstaller
    {


        public override void Uninstall()
        {
           // throw new NotImplementedException();
        }

        public override IEnumerable<PackageFragmentValidationResult> Validate()
        {
            //throw new NotImplementedException();
            return new PackageFragmentValidationResult[] { };
        }
    }
}
