using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.Streams.Dsl;
using Akka.Streams.Stage;
using Confluent.Kafka;

namespace Akka.Streams.Kafka.Internals
{
    public abstract class SubSourceLogic<TKey, TValue, TMessage> : GraphStageLogic
    {
        protected readonly ConsumerSettings<TKey, TValue> Settings;
        protected readonly IAutoSubscription Subscription;

        protected IActorRef Consumer;
        protected StageActorRef Self;
        protected ImmutableQueue<TopicPartition> Buffer = ImmutableQueue<TopicPartition>.Empty;
        protected Dictionary<TopicPartition, IDisposable> SubSources = new Dictionary<TopicPartition, IDisposable>();

        protected SubSourceLogic(
            SourceShape<KeyValuePair<TopicPartition, Source<TMessage, NotUsed>>> shape,
            ConsumerSettings<TKey, TValue> settings,
            IAutoSubscription subscription)
            : base(shape)
        {
            this.Settings = settings;
            this.Subscription = subscription;
        }

        public override void PreStart()
        {
            base.PreStart();
        }

        public override void PostStop()
        {
            base.PostStop();
        }


    }
}