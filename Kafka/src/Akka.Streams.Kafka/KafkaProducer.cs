using System;
using System.Threading.Tasks;
using Akka.Streams.Dsl;
using Confluent.Kafka;

namespace Akka.Streams.Kafka
{
    public static class Producer
    {
        public static Sink<Message<TKey, TValue>, Task> KafkaSink<TKey, TValue>(ProducerSettings<TKey, TValue> settings)
        {
            throw new NotImplementedException();
        }

        public static Sink<Message<TKey, TValue>, Task> KafkaSink<TKey, TValue>(ProducerSettings<TKey, TValue> settings, Producer<TKey, TValue> producer)
        {
            throw new NotImplementedException();
        }

        public static Sink<Message<TKey, TValue, ICommittable>, Task> KafkaCommittableSink<TKey, TValue>(ProducerSettings<TKey, TValue> settings)
        {
            throw new NotImplementedException();
        }

        public static Sink<Message<TKey, TValue, ICommittable>, Task> KafkaCommittableSink<TKey, TValue>(ProducerSettings<TKey, TValue> settings, Producer<TKey, TValue> producer)
        {
            throw new NotImplementedException();
        }

        public static Flow<Message<TKey, TValue, TPass>, Result<TKey, TValue, TPass>, NotUsed> Flow<TKey, TValue, TPass>(ProducerSettings<TKey, TValue> settings)
        {
            throw new NotImplementedException();
        }

        public static Flow<Message<TKey, TValue, TPass>, Result<TKey, TValue, TPass>, NotUsed> Flow<TKey, TValue, TPass>(ProducerSettings<TKey, TValue> settings, Producer<TKey, TValue> producer)
        {
            throw new NotImplementedException();
        }
    }
}