using System;
using System.Collections.Generic;

namespace MerQrySoftware.Handlers
{
    /// <summary>
    /// Caches values for handlers.
    /// </summary>
    internal class HandlerCache
    {
        private readonly Dictionary<Type, object> cache;
        private readonly Func<Type, object> getMissing;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerCache" /> class.
        /// </summary>
        public HandlerCache() : this(GetMissing) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerCache"/> class.
        /// </summary>
        /// <param name="getMissing">The getMissing function.</param>
        public HandlerCache(Func<Type, object> getMissing)
        {
            if (getMissing == null) { throw new ArgumentNullException("getMissing"); }

            this.getMissing = getMissing;

            cache = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Gets the value of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The value of the specified type.
        /// </returns>
        public object Get(Type type)
        {
            object value;
            if (cache.TryGetValue(type, out value)) { return value; }

            return getMissing(type);
        }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        public void Set<T>(T value)
        {
            if (!typeof(T).IsValueType && value == null) { return; }

            cache[typeof(T)] = value;
        }

        private static object GetMissing(Type mssingType)
        {
            if (mssingType.IsValueType)
            {
                return Activator.CreateInstance(mssingType);
            }

            return null;
        }
    }
}
