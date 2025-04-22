using System.Reflection;

namespace DistributedBanking.TransactionalClock.Host.Extensions;

public static class ObjectExtensions
{
    public static Dictionary<string, object> ToDictionary<T>(this T obj)
    {
        var dict = new Dictionary<string, object>();
        if (obj == null) return dict;

        foreach (PropertyInfo prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var value = prop.GetValue(obj, null);
            dict[prop.Name] = value;
        }

        return dict;
    }
}