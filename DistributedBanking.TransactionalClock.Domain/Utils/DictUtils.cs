namespace DistributedBanking.TransactionalClock.Domain.Utils;

public static class DictUtils
{
    public static void Merge(Dictionary<string, object> target, Dictionary<string, object> source)
    {
        foreach (var kvp in source)
        {
            if (target.ContainsKey(kvp.Key) &&
                target[kvp.Key] is Dictionary<string, object> targetDict &&
                kvp.Value is Dictionary<string, object> sourceDict)
            {
                Merge(targetDict, sourceDict);
            }
            else
            {
                target[kvp.Key] = kvp.Value;
            }
        }
    }
}