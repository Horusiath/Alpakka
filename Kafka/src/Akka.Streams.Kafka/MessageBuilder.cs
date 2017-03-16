using Akka.Streams.Kafka.Internals;
using Confluent.Kafka;

namespace Akka.Streams.Kafka
{
    public interface IMessageBuilder<TKey, TVal, out TMsg>
    {
        TMsg CreateMessage(Message<TKey, TVal> record);
    }

    public sealed class DefaultMessageBuilder<TKey, TVal> : IMessageBuilder<TKey, TVal, Message<TKey, TVal>>
    {
        public Message<TKey, TVal> CreateMessage(Message<TKey, TVal> record) => record;
    }

    internal abstract class CommittableMessageBuilder<TKey, TVal> :
        IMessageBuilder<TKey, TVal, CommittableMessage<TKey, TVal>>
    {
        public abstract string GroupId { get; }
        public abstract ICommitter Committer { get; }

        public CommittableMessage<TKey, TVal> CreateMessage(Message<TKey, TVal> record)
        {
            var offset = new PartitionOffset(new PartitionId(
                groupId: GroupId,
                topic: record.Topic,
                partition: record.Partition), record.Offset);
            return new CommittableMessage<TKey, TVal>(new CommittableOffset(offset, Committer), record);
        }
    }
}