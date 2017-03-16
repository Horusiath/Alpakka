using System;
using System.Threading;
using System.Threading.Tasks;

namespace Akka.Streams.Kafka
{
    /// <summary>
    /// Interface used to dispose resources in asynchronous manner.
    /// </summary>
    public interface IAsyncDisposable : IDisposable
    {
        /// <summary>
        /// Task which is completed once a current object has been successfully disposed.
        /// </summary>
        Task Completed { get; }

        /// <summary>
        /// Disposes current object asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token, that may be used to prematurelly cancel object disposal.</param>
        /// <returns></returns>
        Task DisposeAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
