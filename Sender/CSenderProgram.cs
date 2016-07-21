using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using CTransport;
using System.Xml.Serialization;
using System.IO;


namespace CSender
{
    class CSenderProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-> SENDER ENABLED.");
            IOperation sender = new CTransportService(new IPEndPoint(
                                    IPAddress.Parse("127.0.0.1"),7777));

            CUserData user = new CUserData();
            Auth(user); 
              
            Task t = Task.Factory.StartNew(()=>{
                string message;
                BuildMessage(user, out message);
                while (user.message != "quit")
                {                    
                    sender.AsyncWrite(message);
                    BuildMessage(user, out message);
                }
            });
            t.Wait();
        }

        private static void Auth(CUserData user)
        {
            Console.Write("-> NAME: ");
            user.username = Console.ReadLine();
        }

        private static void BuildMessage(CUserData user, out string message)
        {            
            Console.WriteLine("-> MESSAGE: ");
            user.message = Console.ReadLine();

            message = CUserData.SerializeUserData(user);
        }                
    }
}
