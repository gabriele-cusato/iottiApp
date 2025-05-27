namespace Utils.Classes
{
    public abstract class ConfigObjBase
    {
        public T GetValueByName<T>(string name)
        {
            var prop = GetType().GetProperty(name);
            if (prop != null)
            {
                object value = prop.GetValue(this)!;
                return (T)Convert.ChangeType(value, typeof(T));
            }
            throw new Exception($"Parametro '{name}' non trovato.");
        }
    }
}
