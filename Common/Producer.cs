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

        public Producer() : base()
        {
            _producer = new SenderLink(_session, "sender", DESTINATION);
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
                newMessage.Header.Durable = true;
                
                
                await _producer.SendAsync(newMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}
