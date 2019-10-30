using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Amqp;
using Amqp.Serialization;
using Common;
using Newtonsoft.Json;

namespace ActiveMQ.Host
{
    class State
    {
        public int i { get; set; }
    }
    class Program
    {
        public static Producer producer;
        public static async void Produce(object obj)
        {
            ((State) obj).i++;
            int i = ((State) obj).i;
            Console.WriteLine($"Produce - {i}");
            
            await producer.SendMessage(new Point() { X = i, Y = i * 2 });

        }

        static async Task Main(string[] args)
        {
            var timerCallback = new TimerCallback(Produce);
            producer = new Producer();
            //var timer = new Timer(timerCallback, new State(){i=1}, 0, 1000);
            
            Console.WriteLine("Start Produce!");
            
            //var producer = new Producer();
            var sw1 = new Stopwatch();
            sw1.Start();
            for (int i = 0; i < 100000; i++)
            {
                await producer.SendMessage(new Point(){X=i, Y=i*2});
                Console.WriteLine(i);
                //Thread.Sleep(100);
            }

            Console.WriteLine(sw1.Elapsed);
            

            Console.ReadLine();
        }
    }
}
