using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace lonefire.Extensions
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            tempData.TryGetValue(key, out object o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        public static T Peek<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o = tempData.Peek(key);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        //Create a List of Value with same key
        public static void AddToList<T>(this ITempDataDictionary tempData, string key, T value) where T : class
         {
            tempData.TryGetValue(key, out object o);
            if(o == null)
            {
                //Create
                List<T> list = new List<T>();
                list.Add(value);
                tempData[key] = JsonConvert.SerializeObject(list);
            }
            else
            {
                //Append
                List<T> list = JsonConvert.DeserializeObject<List<T>>((string)o);
                list.Add(value);
                tempData[key] = JsonConvert.SerializeObject(list);
            }
        }

        //Get the List of Value with key
        public static List<T> GetList<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            tempData.TryGetValue(key, out object o);
            return o == null ? null : JsonConvert.DeserializeObject<List<T>>((string)o);
        }
    }
}
