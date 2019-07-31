using Newtonsoft.Json;
using System;
using System.IO;

namespace DailyKanjiLogic.Helper
{
    /// <summary>
    /// Helper class to easier handle with JSON files
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Return a new de-serialized object from the given JSON file
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="filename">The name of the JSON file</param>
        /// <returns>The de-serialized object</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static T ReadJson<T>(in string filename) where T : class, new()
        {
            if(filename.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                throw new FileNotFoundException("Path contains illegal characters", filename);
            }

            using var fileStream   = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var streamReader = new StreamReader(fileStream);

            TryConvertFromString<T>(streamReader.ReadToEnd(), out var newObject, out var exception);

            if(!(exception is null))
            {
                throw exception;
            }

            return newObject;
        }

        /// <summary>
        /// Write a <see cref="object"/> into a file in JSON format
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the <see cref="object"/></typeparam>
        /// <param name="filename">The name of the file</param>
        /// <param name="objectToTransform">The object to write</param>
        public static void WriteJson<T>(in string filename, in T objectToTransform) where T : class, new()
        {
            if(filename.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return;
            }

            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };

            using(var fileStream     = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using(var streamWriter   = new StreamWriter(fileStream))
            using(var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(jsonTextWriter, objectToTransform);
            }
        }

        /// <summary>
        /// Convert a <see cref="string"/>, that contains a JSON object, into to a <see cref="object"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the new <see cref="object"/></typeparam>
        /// <param name="jsonString">The <see cref="string"/> that contains a JSON object</param>
        /// <param name="newObject">The new <see cref="object"/> from the string</param>
        /// <param name="exception">The thrown <see cref="Exception"/> until the converting</param>
        /// <exception cref="JsonException"></exception>
        public static void TryConvertFromString<T>(in string jsonString, out T newObject, out Exception? exception)
                where T : class, new()
        {
            try
            {
                exception = null;
                newObject = JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch(Exception jsonException)
            {
                throw new JsonException($"Exception: {Environment.NewLine}{jsonException}{Environment.NewLine}JSON string: {Environment.NewLine}{jsonString}{Environment.NewLine}");
            }
        }
    }
}
