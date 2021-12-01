using System.Collections.Generic;
using System.IO;

namespace Domo
{
    public static class DomoExtensions
    {
        public static IRepository<T> GetRepository<T>(this IDataStore store)
            => (IRepository<T>)store.GetRepository(typeof(T));

        public static void DeleteAllRepositories(this IDataStore store)
        {
            foreach (var r in store.GetRepositories())
                store.DeleteRepository(r);
        }
    }
}