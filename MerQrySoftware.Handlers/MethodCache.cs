using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MerQrySoftware.Handlers
{
    /// <summary>
    /// Caches process methods.
    /// </summary>
    public class MethodCache
    {
        private readonly Dictionary<Type, Action<object, HandlerCache>> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCache" /> class.
        /// </summary>
        public MethodCache()
        {
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

            return cache[type] = Create(type);
        }

        private Action<object, HandlerCache> Create(Type type)
        {
            MethodInfo processMethod = type.GetMethods().FirstOrDefault(method => method.Name == "Process");
            if (processMethod == null) { throw new ArgumentException(string.Format("Process method does not exist on {0}.", type), "type"); }

            MethodInfo getMethod = typeof(HandlerCache).GetMethod("Get");

            // object handler
            ParameterExpression handlerParameter = Expression.Parameter(typeof(object), "handler");

            // HandlerCache handlerCache
            ParameterExpression handlerCacheParameter = Expression.Parameter(typeof(HandlerCache), "handlerCache");

            // (type)handler
            UnaryExpression convert = Expression.Convert(handlerParameter, type);

            // (parameterType)handlerCache.Get(parameterType), ...
            IEnumerable<UnaryExpression> getCalls =
                processMethod.GetParameters()
                    .Select(
                        processParameter =>
                            Expression.Convert(
                                Expression.Call(handlerCacheParameter, getMethod, Expression.Constant(processParameter.ParameterType, typeof(Type))),
                                processParameter.ParameterType));

            MethodCallExpression call = null;

            // ((type)handler).Process(handlerCache.Get(parameterType), ...)
            MethodCallExpression processCall = Expression.Call(convert, processMethod, getCalls);
            if (processMethod.ReturnType == typeof(void))
            {
                call = processCall;
            }
            else
            {
                MethodInfo setMethod = typeof(HandlerCache).GetMethod("Set").MakeGenericMethod(processMethod.ReturnType);

                // handlerCache.Set(convertedHandler.Process(handlerCache.Get(parameterType), ...))
                call = Expression.Call(handlerCacheParameter, setMethod, processCall);
            }

            return Expression.Lambda<Action<object, HandlerCache>>(call, handlerParameter, handlerCacheParameter).Compile();
        }
    }
}
