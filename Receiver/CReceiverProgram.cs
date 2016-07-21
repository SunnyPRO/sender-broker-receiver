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


namespace CReceiver
{
    class CReceiverProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-> RECEIVER ENABLED.");

            IOperation receiver = new CTransportService(new IPEndPoint(
                                   IPAddress.Parse("127.0.0.1"), 7777));

            Task t = Task.Factory.StartNew(async () =>
            {
                string message = ReceiverInfo();
                await receiver.AsyncWrite(message);

                string m;
                while ((m = await receiver.AsyncRead()) != "quit r")
                {
                    Console.WriteLine(Environment.NewLine+m);                    
                }
            });
            t.Wait();
            Console.ReadLine();
        }

        private static string ReceiverInfo()
        {            
            CUserData user = new CUserData();
            Console.Write("-> NAME: ");
            user.username = Console.ReadLine();

            user.message = "subscribe";

            return CUserData.SerializeUserData(user);
        }
               
    }
}
