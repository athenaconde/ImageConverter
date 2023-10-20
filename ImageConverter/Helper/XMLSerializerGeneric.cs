using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ImageConverter.Helper
{
        /// <summary>
        /// Generic class XML Serializer and Deserializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
       public class XMLSerializerGeneric<T>
        {
            private string _fileName = string.Empty;
            internal XMLSerializerGeneric(string fileName)
            {
                _fileName = fileName;
            }

            internal void Serialize(T objectToSerialize)
            {
                if (objectToSerialize == null)
                    throw new ArgumentNullException("objectToSerialize");
                using (var fs = new FileStream(_fileName, FileMode.OpenOrCreate))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    var writer = new StreamWriter(fs);
                    serializer.Serialize(writer, objectToSerialize);
                }

            }

            internal T Deserialize()
            {
                using (var fs = new FileStream(_fileName, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    var reader = new XmlTextReader(fs);
                    return (T)serializer.Deserialize(reader);
                }

            }

        }
}
