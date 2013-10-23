using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Composite.Integration.Nuget.C1Console
{

        [ActionExecutor(typeof(NugetActionExecutor))]
        public sealed class NugetActionToken : ActionToken
        {
            private static PermissionType[] _permissionTypes = new PermissionType[] { PermissionType.Administrate };
            public override IEnumerable<PermissionType> PermissionTypes
            {
                get { return _permissionTypes; }
            }
            public override string Serialize()
            {
                return "MyUrlAction";
            }
            public static ActionToken Deserialize(string serializedData)
            {
                return new NugetActionToken();
            }
        }
   
}
