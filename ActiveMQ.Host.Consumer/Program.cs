using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Amqp;
using Amqp.Types;
using Common;
using Newtonsoft.Json;

namespace ActiveMQ.Host.Consumer
{
    class Program
    {
        static long ReceivedMessages = 0;
        static Stopwatch sw1;
        static void TheCallback(IReceiverLink link, Message message)
        {
            Interlocked.Increment(ref ReceivedMessages);
            try
            {
                var settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.Auto;
                settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
                settings.DateParseHandling = DateParseHandling.None;
                var point = JsonConvert.DeserializeObject<Point>(message.Body.ToString(), settings);
                if (point.X > 99900)
                {
                    sw1.Stop();
                    Console.WriteLine(sw1.Elapsed);
                }
            }
            catch(Exception err)
            {
                Console.WriteLine(err.Message);
            }

            Console.WriteLine(message.Body.ToString());
            
            link.Accept(message);

        }
        static async Task Main(string[] args)
        {
            sw1 = new Stopwatch();
            sw1.Start();
            var consumer = new Common.Consumer(TheCallback);
            consumer.start();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
              
            
        }
    }
}
