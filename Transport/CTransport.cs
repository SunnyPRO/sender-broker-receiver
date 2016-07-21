using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using Newtonsoft.Json;

namespace CTransport
{
    public interface IOperation
    {
        Task<String> AsyncRead();
        Task AsyncWrite(String message);
    }
    public class CTransportService: IOperation
    {
        UdpClient trasport = new UdpClient();

        public CTransportService(IPEndPoint broker)
        {
            trasport.Connect(broker);
        }
        public async Task<string> AsyncRead()
        {
            var rec = await trasport.ReceiveAsync();
            return ASCIIEncoding.ASCII.GetString(rec.Buffer, 0, rec.Buffer.Length);
        }

        public async Task AsyncWrite(string message)
        {
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(message);
            trasport.SendAsync(bytes, bytes.Length);
        }
    }

    public class CBrokerService : IOperation
    {
        UdpClient trasport = new UdpClient();
        HashSet<CReceiverInfo> receivers = new HashSet<CReceiverInfo>();

        public CBrokerService()
        {
            trasport = new UdpClient(7777);
        }
        public async Task<string> AsyncRead()
        {
            var rec = await trasport.ReceiveAsync();
            string s = ASCIIEncoding.ASCII.GetString(rec.Buffer, 0, rec.Buffer.Length);
            CUserData user;
            if (s.Contains(@"<?xml") == true)
            {
                DeserializeMessage(s, out user);
                if (user.message.Equals("subscribe"))
                {
                    AddReceiver(rec, user);
                    Console.WriteLine("-> SUBSCRIBED \n{0}\n", s);
                    return  "";
                }
            }   
            return s;
        }

        private void AddReceiver(UdpReceiveResult rec, CUserData user)
        {
            CReceiverInfo ri = new CReceiverInfo();
            ri.Name = user.username;
            ri.IpEndPoint = rec.RemoteEndPoint;
            receivers.Add(ri);
        }

        public async Task AsyncWrite(string message)
        {
            CUserData user;
            string JsonString = "";

            if (message.Length > 0)
            {
                Console.WriteLine("-> INCOMMING XML {0}\n", message);
                DeserializeMessage(message, out user);
                Console.WriteLine("-> TRANSFORMED IN CUSER_DATA: \nUsername: {0}\nMessage: {1}\n", 
                                  user.username,user.message);


                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(user.message);
                    JsonString = JsonConvert.SerializeXmlNode(doc);
                Console.WriteLine("-> TRANSFORMED IN JSON: \n{0}\n",JsonString);

                byte[] bytes = ASCIIEncoding.ASCII.GetBytes(user.username + ": " + user.message);

                receivers.ToList().ForEach(r =>
                {
                        Console.WriteLine("-> SENDING TO: {0}",
                                           r.Name);
                        trasport.SendAsync(bytes, bytes.Length, r.IpEndPoint);
                });
            }
        }

        private static void DeserializeMessage(string message, out CUserData user)
        {
            XmlSerializer serializedxml = new XmlSerializer(typeof(CUserData));
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(message); 
                writer.Flush();
                stream.Position = 0;

                user = (CUserData)serializedxml.Deserialize(stream);
            }
        }
    }
}
