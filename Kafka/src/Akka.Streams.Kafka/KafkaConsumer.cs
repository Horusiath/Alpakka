using System;
using Akka.Streams.Dsl;
using Confluent.Kafka;

namespace Akka.Streams.Kafka
{
    public class Consumer
    {
        public static Source<Message<TKey, TValue>, IDisposable> KafkaSource<TKey, TValue>(ConsumerSettings<TKey, TValue> settings,
            ISubscription subscription)
        {
            throw new NotImplementedException();
        }

        public static Source<CommittableMessage<TKey, TValue>, IDisposable> KafkaCommittableSource<TKey, TValue>(ConsumerSettings<TKey, TValue> settings,
            ISubscription subscription)
        {
            throw new NotImplementedException();
        }

        public static Source<Message<TKey, TValue>, IDisposable> KafkaAutoCommittedSource<TKey, TValue>(ConsumerSettings<TKey, TValue> settings,
            ISubscription subscription)
        {
            throw new NotImplementedException();
        }
    }
}