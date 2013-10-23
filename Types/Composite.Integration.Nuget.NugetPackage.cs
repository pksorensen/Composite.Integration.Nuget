using Composite.Core.WebClient.Renderings.Data;
using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace Composite.Integration.Nuget.Types
{
    #region Composite C1 data type attributes
    [AutoUpdateble()]
    [DataScope("public")]
    [RelevantToUserType("Developer")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("D2B3D639-0FEF-426E-A26F-893896FED54C")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("NugetPackage")]
    [LabelPropertyName("Id")]
    #endregion
    public interface NugetPackage : IData
    {
        
        #region Data field attributes
        [ImmutableFieldId("92bcf76b-1668-490a-a039-987e1e4c6aca")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator()]
        [FieldPosition(0)]
        [StringSizeValidator(0, 256)]
        [DefaultFieldStringValue("")]
        #endregion
        string Id { get; set; }
        
        #region Data field attributes
        [ImmutableFieldId("51daafa1-bf53-4de4-9a5d-4a2147c9e784")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [NotNullValidator()]
        [FieldPosition(1)]
        [StringSizeValidator(0, 32)]
        [DefaultFieldStringValue("")]
        #endregion
        string Version { get; set; }
    }
}