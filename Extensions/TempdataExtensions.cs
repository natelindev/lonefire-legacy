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

        //Toast Message List
        public static void PutString(this ITempDataDictionary tempData, string key, string value)
        {
            tempData.TryGetValue(key, out object o);
            if(o == null)
            {
                //Create
                List<string> s_list = new List<string>();
                s_list.Add(value);
                tempData[key] = JsonConvert.SerializeObject(s_list);
            }
            else
            {
                //Append
                List<string> s_list = JsonConvert.DeserializeObject<List<string>>((string)o);
                s_list.Add(value);
                tempData[key] = JsonConvert.SerializeObject(s_list);
            }
        }

        public static List<string> GetStringList(this ITempDataDictionary tempData, string key)
        {
            //Get Toast Message List
            tempData.TryGetValue(key, out object o);
            return o == null ? null : JsonConvert.DeserializeObject<List<string>>((string)o);
        }
    }
}
