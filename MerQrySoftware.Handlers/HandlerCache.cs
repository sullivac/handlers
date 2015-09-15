using System;
using System.Collections.Generic;

namespace MerQrySoftware.Handlers
{
    /// <summary>
    /// Caches values for handlers.
    /// </summary>
    public class HandlerCache
    {
        private readonly Dictionary<Type, object> cache;
        private Func<Type, object> getMissing;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerCache" /> class.
        /// </summary>
        public HandlerCache()
        {
            cache = new Dictionary<Type, object>();
            getMissing =
                missingType =>
                {
                    if (missingType.IsValueType)
                    {
                        return Activator.CreateInstance(missingType);
                    }

                    return null;
                };
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
        /// Sets the getMissing function.
        /// </summary>
        /// <value>
        /// The getMissing function.
        /// </value>
        /// <exception cref="System.ArgumentNullException">value</exception>
        public Func<Type, object> GetMissing
        {
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }

                getMissing = value;
            }
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
    }
}
