using System;
using System.Collections.Generic;

namespace MerQrySoftware.Handlers
{
    /// <summary>
    /// Caches process methods.
    /// </summary>
    public class MethodCache
    {
        private readonly Dictionary<Type, Action<object, HandlerCache>> cache;
        private readonly Func<Type, Action<object, HandlerCache>> createProcessMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCache"/> class.
        /// </summary>
        /// <param name="createProcessMethod">The create process method.</param>
        /// <exception cref="System.ArgumentNullException">createProcessMethod</exception>
        public MethodCache(Func<Type, Action<object, HandlerCache>> createProcessMethod)
        {
            if (createProcessMethod == null) { throw new ArgumentNullException("createProcessMethod"); }

            this.createProcessMethod = createProcessMethod;

            cache = new Dictionary<Type, Action<object, HandlerCache>>();
        }

        /// <summary>
        /// Gets the process method for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An instance of <see cref="Action{object, HandlerCache}"/></returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public Action<object, HandlerCache> Get(Type type)
        {
            if (type == null) { throw new ArgumentNullException("type"); }

            Action<object, HandlerCache> result;
            if (cache.TryGetValue(type, out result)) { return result; }

            return cache[type] = createProcessMethod(type);
        }
    }
}
