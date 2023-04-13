using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    /// Encapsulates utility classes that helps with reading/writing JSON data on PlayerPrefs
    /// </summary>
    public static class PlayerPrefsUtils
    {
        /// <summary>
        /// Deserializes a class from JSON data stored on PlayerPrefs.
        /// </summary>
        /// <param name="key">PlayerPrefs key name</param>
        /// <typeparam name="T">The class type to deserialize</typeparam>
        /// <returns>An instance of deserialized class. New class if the key is not found</returns>
        public static T Read<T>(string key) where T : new()
        {
            return PlayerPrefs.HasKey(key)
                    ? JsonUtility.FromJson<T>(PlayerPrefs.GetString(key))
                    : new T();
        }

        /// <summary>
        /// Serializes a class to JSON string data and stores it on PlayerPrefs
        /// </summary>
        /// <param name="key">PlayerPrefs key name</param>
        /// <param name="data">Instance of the class to serialize</param>
        /// <typeparam name="T">The class type to serialize</typeparam>
        public static void Write<T>(string key, T data) where T : new()
        {
            PlayerPrefs.SetString(key, JsonUtility.ToJson(data));
        }

        /// <summary>
        /// Deletes a PlayerPrefs entry
        /// </summary>
        /// <param name="key">PlayerPrefs key name</param>
        public static void Clear(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}
