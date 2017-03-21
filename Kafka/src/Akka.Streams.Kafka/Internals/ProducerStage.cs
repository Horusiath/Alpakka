using System;
using System.Threading.Tasks;
using Akka.Event;
using Akka.Streams.Stage;
using Akka.Util.Internal;
using Confluent.Kafka;

namespace Akka.Streams.Kafka.Internals
{

    internal sealed class ProducerStage<TKey, TValue, TPass> : GraphStage<FlowShape<Message<TKey, TValue, TPass>, Task<Result<TKey, TValue, TPass>>>>
    {
        public readonly Inlet<Message<TKey, TValue, TPass>> In = new Inlet<Message<TKey, TValue, TPass>>("kafka-in");
        public readonly Outlet<Task<Result<TKey, TValue, TPass>>> Out = new Outlet<Task<Result<TKey, TValue, TPass>>>("kafka-out");

        private readonly TimeSpan closeTimeout;
        private readonly ILoggingAdapter log;
        private readonly bool closeProducerOnStop;
        private readonly Func<Producer<TKey, TValue>> producerProvider;

        public override FlowShape<Message<TKey, TValue, TPass>, Task<Result<TKey, TValue, TPass>>> Shape { get; }

        public ProducerStage(TimeSpan closeTimeout, bool closeProducerOnStop, Func<Producer<TKey, TValue>> producerProvider, ILoggingAdapter log)
        {
            this.closeTimeout = closeTimeout;
            this.closeProducerOnStop = closeProducerOnStop;
            this.producerProvider = producerProvider;
            this.log = log;
            Shape = new FlowShape<Message<TKey, TValue, TPass>, Task<Result<TKey, TValue, TPass>>>(In, Out);
        }

        protected override GraphStageLogic CreateLogic(Attributes inheritedAttributes) => new Logic(this);

        #region logic 

        sealed class Logic : GraphStageLogic
        {
            private readonly ProducerStage<TKey, TValue, TPass> self;
            private readonly Producer<TKey, TValue> producer;
            private readonly AtomicCounter awaitingConfirmation = new AtomicCounter(0);
            private readonly TaskCompletionSource<NotUsed> completionState = new TaskCompletionSource<NotUsed>();

            private volatile bool inputIsClosed = false;

            public Logic(ProducerStage<TKey, TValue, TPass> self) : base(self.Shape)
            {
                this.self = self;
                this.producer = self.producerProvider();

                SetHandler(self.Out, onPull: () => TryPull(self.In));
                SetHandler(self.In, onPush: () =>
                {
                    var msg = Grab(self.In);
                    var result = SendToProducer(msg);
                    awaitingConfirmation.IncrementAndGet();
                    Push(self.Out, result);
                },
                onUpstreamFinish: () =>
                {
                    inputIsClosed = true;
                    completionState.SetResult(NotUsed.Instance);
                    CheckForCompletion();
                },
                onUpstreamFailure: cause =>
                {
                    inputIsClosed = true;
                    completionState.SetException(cause);
                    CheckForCompletion();
                });
            }

            public override void PostStop()
            {
                self.log.Debug("Stage completed");
                if (self.closeProducerOnStop)
                {
                    try
                    {
                        producer.Flush();
                        producer.Dispose();
                        self.log.Debug("Producer closed");
                    }
                    catch (Exception cause)
                    {
                        self.log.Error(cause, "Problem occurred during producer close");
                    }
                }

                base.PostStop();
            }

            private async Task<Result<TKey, TValue, TPass>> SendToProducer(Message<TKey, TValue, TPass> msg)
            {
                var record = msg.Record;
                var res = await producer.ProduceAsync(record.Topic, record.Key, record.Value, record.Partition);
                return new Result<TKey, TValue, TPass>(res, msg);
            }

            private void CheckForCompletion()
            {
                if (IsClosed(self.In) && awaitingConfirmation.Current == 0)
                {
                    var completionTask = completionState.Task;
                    if (completionTask.IsFaulted) FailStage(completionTask.Exception);
                    else if (completionTask.IsCompleted) CompleteStage();
                }
            }
        }

        #endregion
    }
}