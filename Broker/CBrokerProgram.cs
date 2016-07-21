using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using CTransport;

namespace CBroker
{
    class CBrokerProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-> BROKER ENABLED.");

            IOperation broker = new CBrokerService();

            Task t = Task.Factory.StartNew(async () =>
            {

                string m;
                while ((m = await broker.AsyncRead()) != "quit b")
                {
                    await broker.AsyncWrite(m);                   
                }
            });
            t.Wait();
            Console.ReadLine();
        }
    }
}
