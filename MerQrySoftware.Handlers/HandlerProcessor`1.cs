using System;
using System.Collections.Generic;
using System.Linq;

namespace MerQrySoftware.Handlers
{
    /// <summary>
    /// Processes handlers. Executes handlers until a handler returns
    /// <typeparam name="TResult" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class HandlerProcessor<TResult>
    {
        private readonly HandlerCache handlerCache;
        private readonly IList<object> handlers;
        private readonly MethodCache methodCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerProcessor"/> class.
        /// </summary>
        /// <param name="methodCache">The method cache.</param>
        /// <param name="handlerCache">The handler cache.</param>
        /// <param name="handlers">The handlers.</param>
        public HandlerProcessor(MethodCache methodCache, HandlerCache handlerCache, IList<object> handlers)
        {
            if (methodCache == null) { throw new ArgumentNullException("methodCache"); }
            if (handlerCache == null) { throw new ArgumentNullException("handlerCache"); }
            if (handlers == null) { throw new ArgumentNullException("handlers"); }

            this.methodCache = methodCache;
            this.handlerCache = handlerCache;
            this.handlers = handlers;
        }

        /// <summary>
        /// Processes this instance.
        /// </summary>
        public void Process()
        {
            foreach (object handler in handlers.Where(handler => handler != null))
            {
                methodCache.Get(handler.GetType())(handler, handlerCache);

                object result = handlerCache.Get(typeof(TResult));
                if (result != null) { break; }
            }
        }
    }
}
