using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class ExtensionUtils
    {
        public static bool RemoveByValue<T1,T2>(this Dictionary<T1,T2> dictionary, T2 value) where T2 : Component 
        {
            var result = dictionary.FirstOrDefault(pair => pair.Value == value);
            return dictionary.Remove(result.Key);
        }
    }
}