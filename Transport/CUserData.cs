using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CTransport
{
    [Serializable()]    //Set this attribute to all the classes that want to serialize
    public class CUserData : ISerializable //derive your class from ISerializable
    {
        public string username { get; set; }
        public string message { get; set; }

        public CUserData()
        {
            username = "";
            message = "";
        }

        //Deserialization constructor.
        public CUserData(SerializationInfo info, StreamingContext ctxt)
        {
            username = (string)info.GetValue("NAME", typeof(string));
            message = (string)info.GetValue("MESSAGE", typeof(string));
        }

        //Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("NAME", username);
            info.AddValue("MESSAGE", message);
        }
        public static string SerializeUserData(CUserData user)
        {
            string message;
            XmlSerializer serializer = new XmlSerializer(typeof(CUserData)); //format to xml

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, user);
                stream.Position = 0;
                StreamReader sr = new StreamReader(stream);
                message = sr.ReadToEnd();
            }
            return message;
        }
    }
}
