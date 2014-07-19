namespace Zbu.Blocks.PropertyEditors
{
    // this is not actually inheriting from PropertyEditor
    // because it's all done in the manifest already
    // just defining a constant for the converter here

    //[PropertyEditor(StructuresAlias, "Structures Editor", "structures", ValueType = "TEXT", HideLabel = true, IsParameterEditor = false)]
    public class StructuresPropertyEditor //: PropertyEditor
    {
        public const string StructuresAlias = "Zbu.Blocks.PropertyEditors.Structures";
    }
}
