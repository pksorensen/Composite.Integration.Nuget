using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementAttachingProvider;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem.Icons;
using Composite.Integration.Nuget.PackageManager;
using Composite.Plugins.Elements.ElementProviders.PackageElementProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NuGet;
using System.Threading.Tasks;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;

namespace Composite.Integration.Nuget.C1Console
{

    //public class PackageRepository
    //{
    //    private static Lazy<PackageRepository> _instance = new Lazy<PackageRepository>(() => new PackageRepository());
    //    private readonly CompositePackageManager _packagemanager;


    //    private 
       
    //    PackageRepository()
    //    {
    //        _packagemanager = new CompositePackageManager();
    //    }
    //    public static PackageRepository Instance { get { return _instance.Value; } }
    //}

    public class NugetElementActionProvider : IElementActionProvider
    {
        private static readonly ActionGroup AppendedActionGroup = new ActionGroup("Nuget", ActionGroupPriority.GeneralAppendHigh);
        public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
        {
            if (entityToken is NugetPackageEntityToken)
            {
                var isRemote = ((NugetPackageEntityToken)entityToken).IsRemotePackage();
                var isUpdate = ((NugetPackageEntityToken)entityToken).IsUpdatePackage();
                ElementAction elementAction = new ElementAction(new ActionHandle(new NugetActionToken()))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label =  isRemote ? "Install Package" : isUpdate? "Update Package" : "Uninstall Package",
                        ToolTip = string.Empty,
                        Icon = isRemote ? CommonCommandIcons.AddNew : isUpdate? CommonCommandIcons.Refresh : CommonCommandIcons.Delete,
                        ActionLocation = new ActionLocation
                        {
                            ActionType = ActionType.Edit,
                            IsInToolbar = true,
                            ActionGroup = AppendedActionGroup
                        }
                    }
                };
                yield return elementAction;

                

                

            } 
        }
    }

    public class NugetElementAttachingProvider : IElementAttachingProvider
    {
        private static readonly ActionGroup PrimaryActionGroup = new ActionGroup("Nuget", ActionGroupPriority.PrimaryMedium);

        public ElementProviderContext Context
        {
            get;
            set;
        }
        public bool HaveCustomChildElements(EntityToken parentEntityToken,
                           Dictionary<string, string> piggybag)
        {
            if (ElementAttachingPointFacade.IsAttachingPoint(parentEntityToken,
              AttachingPoint.ContentPerspective) == false) return false;
            return true;
        }
        public ElementAttachingProviderResult GetAlternateElementList(
           EntityToken parentEntityToken,
           Dictionary<string, string> piggybag)
        {
            if (!(parentEntityToken is PackageElementProviderRootEntityToken))
                return null;

            
            ElementAttachingProviderResult result = new ElementAttachingProviderResult()
            {
                Elements = GetRootElements(piggybag),
                Position = ElementAttachingProviderPosition.Top,
                PositionPriority = 0
            };


            return result;
        }
        private IEnumerable<Element> GetRootElements(Dictionary<string, string> piggybag)
        {
            
            var element = new Element(this.Context.CreateElementHandle(new NugetRootEntityToken()))
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Nuget Packages",
                    ToolTip = "Nuget Packages overview",
                    HasChildren = true,
                    Icon = CommonElementIcons.Folder
                }
            };             


            element.AddAction(new ElementAction(new ActionHandle(new NugetActionToken()))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Open Package Manager",
                    ToolTip = "Open Package Manager",
                    Icon =CommonElementIcons.Advanced,
                    ActionLocation = new ActionLocation
                    {
                        ActionType = ActionType.Add,
                        IsInFolder = false,
                        IsInToolbar = true,
                        ActionGroup = PrimaryActionGroup
                    }
                }
            });

            yield return element;
        }
        public IEnumerable<Element> GetChildren(EntityToken parentEntityToken,
                            Dictionary<string, string> piggybag)
        {
            if (parentEntityToken is NugetEntityToken && parentEntityToken.Id == "AvaliblePackages")
            {

                var pm = new CompositePackageManager();
                var locals = pm.LocalRepository.GetPackages().ToList().GroupBy(p=>p.Id)
                    .Select(key => key.OrderByDescending(p=>p.Version).First());
               
                
                var Updates = pm.SourceRepository.GetUpdates(locals, false, false).ToList();
                
                var CompositeNugetPackages = pm.SourceRepository.GetPackages().Where(p => p.Tags.IndexOf("composite-c1") != -1)
                    .AsEnumerable().Where(PackageExtensions.IsListed);
                var NotInstalledCompositePackages = CompositeNugetPackages
                    .Where(p => !locals.Any(l => l.Id == p.Id));


                foreach (var p in Updates)
                {
                    yield return new Element(this.Context.CreateElementHandle(
                    NugetPackageEntityToken.UpdateablePackageEntityToken(p.Id,p.Version.ToString())))
                    {
                        VisualData = new ElementVisualizedData
                        {
                            Label = string.Format("{0} {1}", p.Id, p.Version),
                            ToolTip = string.Format("Update from version {0}",locals.First(lp=>p.Id==lp.Id).Version),
                            HasChildren = false,
                            Icon = CommonCommandIcons.Refresh
                        }
                    };
                }
                foreach (var p in NotInstalledCompositePackages)
                {
                    yield return new Element(this.Context.CreateElementHandle(
                        NugetPackageEntityToken.RemotePackageEntityToken(p.Id,p.Version.ToString())))
                    {
                        VisualData = new ElementVisualizedData
                        {
                            Label = string.Format("{0} {1}",p.Id ,p.Version),
                            ToolTip = "Nuget Sources",
                            HasChildren = false,
                            Icon = CommonCommandIcons.AddNew
                        }
                    };
                }
            }
            else if (parentEntityToken is NugetEntityToken && parentEntityToken.Id == "InstalledPackages")
            {
                var pm = new CompositePackageManager();

             


                foreach (var p in pm.LocalRepository.GetPackages())
                {                      
                   
                    yield return new Element(this.Context.CreateElementHandle(
                   NugetPackageEntityToken.LocalPackageEntityToken(p.Id,p.Version.ToString())))
                    {
                        VisualData = new ElementVisualizedData
                        {
                            Label = string.Format("{0} {1}", p.Id, p.Version),
                            ToolTip = "Nuget Sources",
                            HasChildren = false,
                            Icon = CommonElementIcons.Advanced
                        }
                    };

                }
            }
            else if (parentEntityToken is NugetRootEntityToken)
            {
             //   var pm = new CompositePackageManager();



                yield return new Element(this.Context.CreateElementHandle(
                  new NugetEntityToken("InstalledPackages")))
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = "Installed Packages",
                        ToolTip = "Already Installed Packages",
                        HasChildren = true,
                        Icon = CommonElementIcons.Folder
                    },
                };


                yield return new Element(this.Context.CreateElementHandle(
                  new NugetEntityToken("AvaliblePackages")))
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = "Avalible Packages",
                        ToolTip = "Avalible Packages",
                        HasChildren = true,
                        Icon = CommonElementIcons.Earth
                    },
                };

                yield return new Element(this.Context.CreateElementHandle(
                  new NugetEntityToken("Respositories")))
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = "Nuget Sources",
                        ToolTip = "Nuget Sources",
                        HasChildren = false,
                        Icon = CommonElementIcons.Advanced
                    }
                };


            }
        }
    }
}
