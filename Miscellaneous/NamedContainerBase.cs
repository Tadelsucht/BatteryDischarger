using System;

namespace BatteryDischarger.Miscellaneous
{
    public abstract class NamedContainerBase<T>
    {
        public T EmbeddedEnum { get; protected set; }

        public NamedContainerBase(T e)
        {
            this.EmbeddedEnum = e;
        }

        public override string ToString()
        {
            var name = GetNameFromResources();
            return (string.IsNullOrEmpty(name)) ? GetDefaultName() : name;
        }

        protected abstract string GetNameFromResources();

        protected string GetDefaultName() => Enum.GetName(typeof(T), EmbeddedEnum);
    }
}