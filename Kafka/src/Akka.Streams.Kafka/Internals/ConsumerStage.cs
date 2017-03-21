using System;
using Akka.Streams.Stage;

namespace Akka.Streams.Kafka.Internals
{
    internal class KafkaSourceStage<TKey, TVal, TMsg> : GraphStageWithMaterializedValue<SourceShape<TMsg>, IDisposable>
    {
        protected readonly Outlet<TMsg> Out = new Outlet<TMsg>("out");
        public override SourceShape<TMsg> Shape { get; }

        public KafkaSourceStage(Func<SourceShape<TMsg>, GraphStageLogic> logic)
        {
            Logic = logic;
            Shape = new SourceShape<TMsg>(Out);
        }

        protected Func<SourceShape<TMsg>, GraphStageLogic> Logic { get; }

        public override ILogicAndMaterializedValue<IDisposable> CreateLogicAndMaterializedValue(Attributes inheritedAttributes)
        {
            var result = Logic(Shape);
            return new LogicAndMaterializedValue<IDisposable>(result, (IDisposable)result);
        }
    }

    internal class ConsumerStage
    {

    }
}