namespace Devmonster.Core.Extensions.Dictionary
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Adds an item to the collection if the Key is not found. Updates the item otherwise
        /// </summary>
        /// <typeparam name="T1">Key</typeparam>
        /// <typeparam name="T2">Value</typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate<T1,T2>(this Dictionary<T1,T2> collection, T1 key, T2 value) where T1 : notnull
        {
            if (collection.ContainsKey(key))
            {
                collection[key] = value;
            }
            else
            {
                collection.Add(key, value);
            }
        }
    }
}