using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Integration.Nuget.PackageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Composite.Integration.Nuget.C1Console
{
    public sealed class NugetActionExecutor : IActionExecutor
    {
        public FlowToken Execute(
           EntityToken entityToken,
           ActionToken actionToken,
           FlowControllerServicesContainer flowControllerServicesContainer)
        {
            string currentConsoleId = flowControllerServicesContainer.
               GetService<IManagementConsoleMessageService>().CurrentConsoleId;
            if (entityToken is NugetRootEntityToken)
            {
                var dataEntityToken = entityToken as NugetRootEntityToken;

                string url = string.Format("/Composite/InstalledPackages/Composite.Integration.Nuget/views/NugetPackageManager.cshtml");
                ConsoleMessageQueueFacade.Enqueue(new OpenViewMessageQueueItem
                {
                    Url = url,
                    ViewId = Guid.NewGuid().ToString(),
                    ViewType = ViewType.Main,
                    Label = "loading..."
                }, currentConsoleId);
              
            }
            else if (entityToken is NugetPackageEntityToken)
            {
                var pm = new CompositePackageManager();
                var token = entityToken as NugetPackageEntityToken;
                IManagementConsoleMessageService managementConsoleMessageService = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();

                if(token.IsLocalPackage())
                {
                    try
                    {
                        pm.UninstallPackage(token.PackageId, new NuGet.SemanticVersion(token.Version),false,true);
                       
                    }catch(InvalidOperationException ex)
                    {
                       
   
                      //  ConsoleMessageQueueFacade.Enqueue(new MessageBoxMessageQueueItem { DialogType = DialogType.Question, Message = ex.Message, Title = "Package Manager" }, managementConsoleMessageService.CurrentConsoleId);
                        ConsoleMessageQueueFacade.Enqueue(new MessageBoxMessageQueueItem { DialogType = DialogType.Error, Message = ex.Message, Title = "Package Manager" }, managementConsoleMessageService.CurrentConsoleId);
                   //     ConsoleMessageQueueFacade.Enqueue(new MessageBoxMessageQueueItem { DialogType = DialogType.Message, Message = ex.Message, Title = "Package Manager" }, managementConsoleMessageService.CurrentConsoleId);
                   //     ConsoleMessageQueueFacade.Enqueue(new MessageBoxMessageQueueItem { DialogType = DialogType.Warning, Message = ex.Message, Title = "Package Manager" }, managementConsoleMessageService.CurrentConsoleId);
                 //       throw;
                    }
                }
                else if(token.IsRemotePackage())
                {
                    try
                    {


                        pm.InstallPackage(token.PackageId, new NuGet.SemanticVersion(token.Version));
                    }
                    catch(Exception ex)
                    {
                        ConsoleMessageQueueFacade.Enqueue(new MessageBoxMessageQueueItem { DialogType = DialogType.Error, Message = ex.Message, Title = "Package Manager" }, managementConsoleMessageService.CurrentConsoleId);
                    

                    }
                    finally
                    {
                        ConsoleMessageQueueFacade.Enqueue(new RefreshTreeMessageQueueItem { EntityToken = NugetEntityToken.InstalledEntityToken }, currentConsoleId);
                   
                    }
                
                }else if(token.IsUpdatePackage())
                {
                    pm.UpdatePackage(token.PackageId, true, false);

                    ConsoleMessageQueueFacade.Enqueue(new RefreshTreeMessageQueueItem { EntityToken = NugetEntityToken.InstalledEntityToken }, currentConsoleId);
                
                }
            }
            ConsoleMessageQueueFacade.Enqueue(new RefreshTreeMessageQueueItem { EntityToken = new NugetRootEntityToken()}, currentConsoleId);
            return null;
        }
    }
}
