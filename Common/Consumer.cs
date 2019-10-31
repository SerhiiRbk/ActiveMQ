using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amqp;

namespace Common
{
    public class Consumer : AmqpBase
    {
        public delegate void MessageReceived(ReceiverLink link, Message msg);

        //public void MessageCallback (ReceiverLink link, Message msg);

        public const string DESTINATION = "OrdersQueue";
        public int MessagesReceived;

        MessageCallback _callback;
        
        private Object receiverLock = new Object();
        private Boolean running = true;

        private ReceiverLink _receiever;
      

        public Consumer(MessageCallback callback, string address = DESTINATION) : base()
        {
            _receiever = new ReceiverLink(_session, "receiver ", address);
            _callback = callback;
            if (_callback == null)
            {

            }
        }

        static void TheCallback(IReceiverLink link, Message message)
        {
            //Interlocked.Increment(ref ReceivedMessages);
            //Console.WriteLine((Point)message.Body);

            link.Accept(message);

        }

        public void stop()
        {
            running = false;
            
            _receiever.Close();
            _session.Close();
        }


        public void start()
        {
            //Task.Factory.StartNew(WorkerRun, TaskCreationOptions.LongRunning);
            Start();
        }

        void Start()
        {
            try
            {
                _receiever.Start(100, _callback);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void WorkerRun()
        {
            try
            {
                while (running)
                {
                    Message theMessage = _receiever.Receive(TimeSpan.FromSeconds(1));

                    if (theMessage != null)
                    {
                        _callback(_receiever, theMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
