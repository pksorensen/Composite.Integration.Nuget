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
    [ImmutableTypeId("7E5A2795-E8ED-484E-A47F-AEABD450576F")]
    [KeyTemplatedXhtmlRenderer(XhtmlRenderingType.Embedable, "<span>{label}</span>")]
    [KeyPropertyName("Id")]
    [Title("PackageSource")]
    [LabelPropertyName("Name")]
    #endregion
    public interface PackageSource : IData
    {
        #region Data field attributes
        [ImmutableFieldId("a80b1724-113d-4c27-ab3c-58c0318dcbaf")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net" +
            "/ns/function/1.0\" />")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [NotNullValidator()]
        [FieldPosition(-1)]
        #endregion
        Guid Id { get; set; }
        
        #region Data field attributes
        [ImmutableFieldId("92bcf76b-1668-490a-a039-987e1e4c6aca")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [NotNullValidator()]
        [FieldPosition(0)]
        [StringSizeValidator(0, 64)]
        [DefaultFieldStringValue("")]
        #endregion
        string Name { get; set; }
        
        #region Data field attributes
        [ImmutableFieldId("51daafa1-bf53-4de4-9a5d-4a2147c9e784")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [LazyFunctionProviedProperty("<f:function xmlns:f=\"http://www.composite.net/ns/function/1.0\" name=\"Composite.Ut" +
            "ils.Validation.RegularExpressionValidation\">\r\n  <f:param name=\"pattern\" value=\"(" +
            "http|https)://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&amp;=]*)?\" />\r\n</f:function>")]
        [NotNullValidator()]
        [FieldPosition(1)]
        [StringSizeValidator(0, 128)]
        [DefaultFieldStringValue("")]
        #endregion
        string Source { get; set; }
    }
}