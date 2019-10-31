using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Newtonsoft.Json;


namespace Common
{
    public class Producer : AmqpBase
    {
        public const string DESTINATION = "OrdersQueue";
        public SenderLink _producer;
        private static int messagesSent;
        private static int totalSent;
        private int cntr = 0;

        public Producer(string address = DESTINATION) : base()
        {
            _producer = new SenderLink(_session, "sender", address);
        }

        OutcomeCallback MessageSentHandler = (l, msg, o, s) => {
            Interlocked.Increment(ref messagesSent);
            Interlocked.Increment(ref totalSent);
        };


        public async Task SendMessage<T>(T body)
        {
            try
            {
                var settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.None;
                settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
                settings.DateParseHandling = DateParseHandling.None;
                Message newMessage = new Message(JsonConvert.SerializeObject(body, settings));
                newMessage.Header = new Header();
                newMessage.Properties = new Properties();
                newMessage.Header.Durable = true;
                newMessage.Properties.MessageId = Guid.NewGuid().ToString();
                newMessage.Properties.CorrelationId = newMessage.Properties.MessageId;
                newMessage.Properties.GroupId = "group1";
                if (cntr%2 == 0)
                {
                    //newMessage.Properties.GroupId = "group1";
                }
                else
                {
                    //newMessage.Properties.GroupId = "group2";
                }
                await _producer.SendAsync(newMessage);
                cntr = cntr+1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}
