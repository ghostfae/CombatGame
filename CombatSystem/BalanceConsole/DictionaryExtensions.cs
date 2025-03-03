using System;
using System.Collections.Generic;

namespace BalanceConsole;

public static class DictionaryExtensions
{
   public static TValue GetOrCreate<TKey, TValue>(
      this Dictionary<TKey, TValue> dictionary,
      TKey key,
      Func<TValue> valueFactory) where TKey : notnull
   {
      if (dictionary.TryGetValue(key, out var value)) 
         return value;

      value = valueFactory();
      dictionary.Add(key, value);
      return value;
   }
}