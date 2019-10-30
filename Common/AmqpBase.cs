using System;
using Amqp;

namespace Common
{
    public class AmqpBase
    {
        public const string URI = "netty://localhost:61617";//amqp://127.0.0.1:5672";//"
        protected Connection _connection;
        protected Session _session;

        public AmqpBase()
        {
            _connection = new Connection(new Address(URI));
            if (_connection != null)
            {
                _session = new Session(_connection);
            }
        }
    }
}
