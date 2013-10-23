using Composite.C1Console.Security;
using Composite.C1Console.Security.SecurityAncestorProviders;
using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Composite.Integration.Nuget.C1Console
{

    [SecurityAncestorProvider(typeof(NoAncestorSecurityAncestorProvider))]
    public sealed class NugetRootEntityToken : EntityToken
    {
        public override string Type
        {
            get { return ""; }
        }
        public override string Source
        {
            get { return ""; }
        }
        public override string Id
        {
            get { return "NugetRootEntityToken"; }
        }
        public override string Serialize()
        {
            return DoSerialize();
        }
        public static EntityToken Deserialize(string serializedEntityToken)
        {
            return new NugetRootEntityToken();
        }
    }

    public sealed class NugetSecurityAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            if (entityToken is NugetEntityToken)
                yield return new NugetRootEntityToken();
            if (entityToken is NugetPackageEntityToken)
                yield return entityToken.Id.Contains("local") ? 
                    new NugetEntityToken("InstalledPackages") : new NugetEntityToken("AvaliblePackages");

        }
    }




    [SecurityAncestorProvider(typeof(NugetSecurityAncestorProvider))]
    public class NugetPackageEntityToken : EntityToken
    {
        public static NugetPackageEntityToken LocalPackageEntityToken(string id, string version)
        { return new NugetPackageEntityToken(id,version,"local"); }
        public static NugetPackageEntityToken RemotePackageEntityToken(string id, string version)
        { return new NugetPackageEntityToken(id, version, "remote"); }
        public static NugetPackageEntityToken UpdateablePackageEntityToken(string id, string version)
        { return new NugetPackageEntityToken(id, version, "update"); }
       
        public string PackageId { get; set; }
        public string Version { get; set; }

        private string _type;

        public NugetPackageEntityToken(string id, string version,string type)
        {

            PackageId = id;
            Version = version.ToString();
            _type = type;
        }

        public bool IsUpdatePackage()
        {
            return _type == "update";
        }
        public bool IsLocalPackage()
        {
            return _type == "local";
        }
        public bool IsRemotePackage()
        {
            return _type == "remote";
        }

        public override string Type
        {
            get { return _type; }
        }
        public override string Source
        {
            get { return ""; }
        }
        public override string Id
        {
            get { return string.Format("{0} {1} {2}", Type, PackageId, Version); }
        }
        public override string Serialize()
        {
            return DoSerialize();
        }
        public static EntityToken Deserialize(string serializedEntityToken)
        {
            string type, source, id;
            DoDeserialize(serializedEntityToken, out type, out source, out id);
            var parts = id.Split(' ');
            return new NugetPackageEntityToken(parts[1], parts[2], parts[0]);
          
          //  throw new InvalidOperationException("Deserialize " + serializedEntityToken);
        }
    }

    [SecurityAncestorProvider(typeof(NugetSecurityAncestorProvider))]
    public sealed class NugetEntityToken : EntityToken
    {
        
        public static NugetEntityToken AvalibleEntityToken { get { return new NugetEntityToken("AvaliblePackages");} }
        public static NugetEntityToken InstalledEntityToken { get { return new NugetEntityToken("InstalledPackages"); } }

        public NugetEntityToken(string publicationStatus)
        {
            this.PublicationStatus = publicationStatus;
        }
        public string PublicationStatus
        {
            get;
            set;
        }
        public override string Type
        {
            get { return ""; }
        }
        public override string Source
        {
            get { return ""; }
        }
        public override string Id
        {
            get { return this.PublicationStatus; }
        }
        public override string Serialize()
        {
            return DoSerialize();
        }
        public static EntityToken Deserialize(string serializedEntityToken)
        {
            string type, source, id;
            DoDeserialize(serializedEntityToken, out type, out source, out id);
            return new NugetEntityToken(id);
        }
    }

}
