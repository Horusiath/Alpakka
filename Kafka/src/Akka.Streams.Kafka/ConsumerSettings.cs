﻿using System;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.Configuration;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Akka.Streams.Kafka
{
    public sealed class ConsumerSettings<TKey, TValue>
    {
        public static ConsumerSettings<TKey, TValue> Create(ActorSystem system)
        {
            var config = system.Settings.Config.GetConfig("akka.kafka.consumer");
            var keyDeserializer = system.Serialization.FindSerializerForType(typeof(TKey)).ToKafkaDeserializer<TKey>();
            var valueDeserializer = system.Serialization.FindSerializerForType(typeof(TValue)).ToKafkaDeserializer<TValue>();
            return Create(config, keyDeserializer, valueDeserializer);
        }

        public static ConsumerSettings<TKey, TValue> Create(Config config, IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer)
        {
            if (config == null) throw new ArgumentNullException(nameof(config), "Kafka config for Akka.NET consumer was not provided");

            return new ConsumerSettings<TKey, TValue>(
                keyDeserializer: keyDeserializer,
                valueDeserializer: valueDeserializer,
                pollInterval: config.GetTimeSpan("poll-interval", TimeSpan.FromMilliseconds(50)),
                pollTimeout: config.GetTimeSpan("poll-timeout", TimeSpan.FromMilliseconds(50)),
                stopTimeout: config.GetTimeSpan("stop-timeout", TimeSpan.FromSeconds(30)),
                closeTimeout: config.GetTimeSpan("close-timeout", TimeSpan.FromSeconds(20)),
                commitTimeout: config.GetTimeSpan("commit-timeout", TimeSpan.FromSeconds(15)),
                wakeUpTimeout: config.GetTimeSpan("wakeup-timeout", TimeSpan.FromSeconds(3)),
                maxWakeUps: config.GetInt("max-wakeups", 10),
                dispatcherId: config.GetString("use-dispatcher", "akka.kafka.default-dispatcher"),
                properties: ImmutableDictionary<string, object>.Empty);
        }

        public object this[string propertyKey] => this.Properties.GetValueOrDefault(propertyKey);

        public IDeserializer<TKey> KeyDeserializer { get; }
        public IDeserializer<TValue> ValueDeserializer { get; }
        public TimeSpan PollInterval { get; }
        public TimeSpan PollTimeout { get; }
        public TimeSpan StopTimeout { get; }
        public TimeSpan CloseTimeout { get; }
        public TimeSpan CommitTimeout { get; }
        public TimeSpan WakeUpTimeout { get; }
        public int MaxWakeUps { get; }
        public string DispatcherId { get; }
        public IImmutableDictionary<string, object> Properties { get; }

        public ConsumerSettings(IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer, TimeSpan pollInterval, TimeSpan pollTimeout, TimeSpan stopTimeout, TimeSpan closeTimeout, TimeSpan commitTimeout, TimeSpan wakeUpTimeout, int maxWakeUps, string dispatcherId, IImmutableDictionary<string, object> properties)
        {
            KeyDeserializer = keyDeserializer;
            ValueDeserializer = valueDeserializer;
            PollInterval = pollInterval;
            PollTimeout = pollTimeout;
            StopTimeout = stopTimeout;
            CloseTimeout = closeTimeout;
            CommitTimeout = commitTimeout;
            WakeUpTimeout = wakeUpTimeout;
            MaxWakeUps = maxWakeUps;
            DispatcherId = dispatcherId;
            Properties = properties;
        }

        public ConsumerSettings<TKey, TValue> WithBootstrapServers(string bootstrapServers) =>
            Copy(properties: Properties.SetItem(KafkaConfig.BoostrapServers, bootstrapServers));

        public ConsumerSettings<TKey, TValue> WithClientId(string clientId) =>
            Copy(properties: Properties.SetItem(KafkaConfig.ClientId, clientId));

        public ConsumerSettings<TKey, TValue> WithGroupId(string groupId) =>
                    Copy(properties: Properties.SetItem(KafkaConfig.GroupId, groupId));

        public ConsumerSettings<TKey, TValue> WithProperty(string key, string value) =>
                    Copy(properties: Properties.SetItem(key, value));

        public ConsumerSettings<TKey, TValue> WithPollInterval(TimeSpan pollInterval) => Copy(pollInterval: pollInterval);

        public ConsumerSettings<TKey, TValue> WithPollTimeout(TimeSpan pollTimeout) => Copy(pollTimeout: pollTimeout);

        public ConsumerSettings<TKey, TValue> WithStopTimeout(TimeSpan stopTimeout) => Copy(stopTimeout: stopTimeout);

        public ConsumerSettings<TKey, TValue> WithCloseTimeout(TimeSpan closeTimeout) => Copy(closeTimeout: closeTimeout);

        public ConsumerSettings<TKey, TValue> WithCommitTimeout(TimeSpan commitTimeout) => Copy(commitTimeout: commitTimeout);

        public ConsumerSettings<TKey, TValue> WithWakeUpTimeout(TimeSpan wakeUpTimeout) => Copy(wakeUpTimeout: wakeUpTimeout);

        public ConsumerSettings<TKey, TValue> WithMaxWakeUps(int maxWakeUps) => Copy(maxWakeUps: maxWakeUps);

        public ConsumerSettings<TKey, TValue> WithDispatcher(string dispatcherId) => Copy(dispatcherId: dispatcherId);

        private ConsumerSettings<TKey, TValue> Copy(
            IDeserializer<TKey> keyDeserializer = null,
            IDeserializer<TValue> valueDeserializer = null, 
            TimeSpan? pollInterval = null, 
            TimeSpan? pollTimeout = null, 
            TimeSpan? stopTimeout = null,
            TimeSpan? closeTimeout = null, 
            TimeSpan? commitTimeout = null, 
            TimeSpan? wakeUpTimeout = null, 
            int? maxWakeUps = null, 
            string dispatcherId = null,
            IImmutableDictionary<string, object> properties = null) => 
        new ConsumerSettings<TKey, TValue>(
            keyDeserializer: keyDeserializer ?? this.KeyDeserializer,
            valueDeserializer: valueDeserializer ?? this.ValueDeserializer,
            pollInterval: pollInterval ?? this.PollInterval,
            pollTimeout: pollTimeout ?? this.PollTimeout,
            stopTimeout: stopTimeout ?? this.StopTimeout,
            closeTimeout: closeTimeout ?? this.CloseTimeout,
            commitTimeout: commitTimeout ?? this.CommitTimeout,
            wakeUpTimeout: wakeUpTimeout ?? this.WakeUpTimeout,
            maxWakeUps: maxWakeUps ?? this.MaxWakeUps,
            dispatcherId: dispatcherId ?? this.DispatcherId,
            properties: properties ?? this.Properties);

        internal Confluent.Kafka.Consumer<TKey, TValue> CreateKafkaConsumer() =>
            new Confluent.Kafka.Consumer<TKey, TValue>(this.Properties, this.KeyDeserializer, this.ValueDeserializer);
    }

    public interface ISubscription { }
    public interface IManualSubscription : ISubscription { }
    public interface IAutoSubscription : ISubscription { }
    #region internal classes

    internal sealed class TopicSubscription : IAutoSubscription
    {
        public TopicSubscription(IImmutableSet<string> topics)
        {
            Topics = topics;
        }

        public IImmutableSet<string> Topics { get; }
    }

    internal sealed class TopicSubscriptionPattern : IAutoSubscription
    {
        public TopicSubscriptionPattern(string pattern)
        {
            Pattern = pattern;
        }

        public string Pattern { get; }
    }

    internal sealed class Assignment : IManualSubscription
    {
        public Assignment(IImmutableSet<TopicPartition> topicPartitions)
        {
            TopicPartitions = topicPartitions;
        }

        public IImmutableSet<TopicPartition> TopicPartitions { get; }
    }

    internal sealed class AssignmentWithOffset : IManualSubscription
    {
        public AssignmentWithOffset(IImmutableDictionary<TopicPartition, long> topicPartitions)
        {
            TopicPartitions = topicPartitions;
        }

        public IImmutableDictionary<TopicPartition, long> TopicPartitions { get; }
    }

    #endregion

    public static class Subscriptions
    {
        public static IAutoSubscription Topics(params string[] topics) => new TopicSubscription(topics.ToImmutableHashSet());

        public static IAutoSubscription TopicPattern(string pattern) => new TopicSubscriptionPattern(pattern);

        public static IManualSubscription Assignment(params TopicPartition[] topicPartitions) =>
            new Assignment(topicPartitions.ToImmutableHashSet());

        public static IManualSubscription AssignmentWithOffset(IImmutableDictionary<TopicPartition, long> topicPartitions) =>
            new AssignmentWithOffset(topicPartitions);
    }
}