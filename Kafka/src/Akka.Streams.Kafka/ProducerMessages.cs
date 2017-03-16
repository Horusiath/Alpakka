using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Akka.Streams.Kafka
{
    #region messages

    /// <summary>
    /// Input element of <see cref="KafkaSink.Committable{TKey,TValue}"/> and <see cref="KafkaSink.Flow"/>.
    /// </summary>
    public sealed class Message<TKey, TValue, TPassed>
    {
        /// <summary>
        /// Contains a topic name to which the record is being sent, an optional
        /// partition number, and an optional key and valsue.
        /// </summary>
        public readonly Message<TKey, TValue> Record;

        /// <summary>
        /// Holds any element that is passed through the <see cref="KafkaProducer.Flow"/>
        /// and included in the <see cref="Result{TKey,TValue,TPassed}"/>. That is useful when some context 
        /// is needed to be passed on downstream operations. That could be done with unzip/zip, but this is more convenient.
        /// It can for example be a <see cref="ICommittableOffset"/> or <see cref="ICommittableOffsetBatch"/>
        /// that can be committed later in the flow.
        /// </summary>
        public readonly TPassed Passed;

        public Message(Message<TKey, TValue> record, TPassed passed = default(TPassed))
        {
            Record = record;
            Passed = passed;
        }
    }

    /// <summary>
    /// Output element of <see cref="KafkaProducer.Flow"/>. Emitted when the message has been
    /// successfully published. Includes the original message, metadata returned from KafkaProducer and the
    /// <see cref="Offset"/> of the produced message.
    /// </summary>
    public sealed class Result<TKey, TValue, TPassed>
    {
        public readonly object Metadata;
        public readonly Message<TKey, TValue, TPassed> Message;

        public Result(Metadata metadata, Message<TKey, TValue, TPassed> message)
        {
            Metadata = metadata;
            Message = message;
        }

        public long Offset => Message.Record.Offset;
    }

    #endregion

}