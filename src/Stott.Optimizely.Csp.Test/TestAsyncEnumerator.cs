using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stott.Optimizely.Csp.Test
{
    /// <summary>
    /// A Test Class to support mocking of DbSets
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            // Multiple calls on mocked DbSets need the inner collection not to be reset and not disposed
            _inner.Reset();

            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }
    }
}
