using System;

namespace SharpSettingsStore
{
    /// <summary>
    /// This attribute specifies the setting name used to identify the class in the database.
    /// Format is "ClassName, PythonModule" where ClassName is the name of the Class and PythonModule
    /// is the legacy python module that used it. This is to facilitate easy migration.
    /// </summary>
    public class SettingNameAttribute : Attribute
    {
        public SettingNameAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}