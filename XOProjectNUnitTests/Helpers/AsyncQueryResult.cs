using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace XOProject.UnitTests.Helpers
{
    public class AsyncQueryResult<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncQueryResult(IEnumerable<T> enumerable) : base(enumerable) { }

        public AsyncQueryResult(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);
    }
}
