using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.Application;
using Composite.Integration.Nuget.C1Console;
using Composite.Plugins.Elements.ElementProviders.PackageElementProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Composite.Integration.Nuget
{

    public sealed class NugetRootEntityTokenAuxiliarySecurityAncestorProvider :
  IAuxiliarySecurityAncestorProvider
    {
        public Dictionary<EntityToken, IEnumerable<EntityToken>>
          GetParents(IEnumerable<EntityToken> entityTokens)
        {
            Dictionary<EntityToken, IEnumerable<EntityToken>> result =
              new Dictionary<EntityToken, IEnumerable<EntityToken>>();

            foreach (EntityToken entityToken in entityTokens)
            {
                if (entityToken.GetType() == typeof(NugetRootEntityToken))
                {
                    // Here we specify that the Content perspective element is the parent of our root element
                    result.Add(entityToken, new EntityToken[] 
                    {
                      new PackageElementProviderRootEntityToken()
                    });
                }
            }
            return result;
        }
    }

    [ApplicationStartup]
    public static class NugetBootstrap
    {
        public static void OnBeforeInitialize()
        {
        }
        public static void OnInitialized()
        {
            AuxiliarySecurityAncestorFacade.AddAuxiliaryAncestorProvider<NugetRootEntityToken>(new NugetRootEntityTokenAuxiliarySecurityAncestorProvider());
        }
    }
}
