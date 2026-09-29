using System;

namespace BatteryDischarger.Miscellaneous
{
    // Keeps a typed value alongside a resource-derived display name for selector controls.
    public abstract class NamedContainerBase<T>
    {
        // Preserves the original typed value when the container is selected by the UI.
        public T EmbeddedEnum { get; protected set; }

        // Stores the value that subclasses use to resolve their localized display name.
        public NamedContainerBase(T e)
        {
            this.EmbeddedEnum = e;
        }

        // Uses the resource name when available and falls back to the enum name for missing labels.
        public override string ToString()
        {
            var name = GetNameFromResources();
            return (string.IsNullOrEmpty(name)) ? GetDefaultName() : name;
        }

        // Supplies the localized display name for this typed value.
        protected abstract string GetNameFromResources();

        // Returns the enum member name when no resource label is available.
        protected string GetDefaultName() => Enum.GetName(typeof(T), EmbeddedEnum);
    }
}
