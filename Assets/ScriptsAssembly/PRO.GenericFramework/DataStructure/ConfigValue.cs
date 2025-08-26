using Sirenix.OdinInspector;

namespace PRO
{
    [HideReferenceObjectPicker]
    public abstract class ConfigValue
    { }

    public class ConfigValue<T> : ConfigValue
    {
        public T Value;
    }
}
