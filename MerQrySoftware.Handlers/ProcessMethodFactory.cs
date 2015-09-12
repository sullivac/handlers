using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MerQrySoftware.Handlers
{
    /// <summary>
    /// Creates a method that generalizes calls to Process on a type.
    /// </summary>
    internal class ProcessMethodFactory
    {
        /// <summary>
        /// Creates a Process method for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An instance of <see cref="Action{System.Object, HandlerCache}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentException">Process method does not exist on <paramref name="type"/>.</exception>
        /// <remarks>
        /// <para><paramref name="type"/> must have at least one public, non-static method called Process. The method can have any return type or any number of parameters.</para>
        /// <para>If the process call has multiple parameters of the same type, then the same value will be passed to the Process call.</para>
        /// <para>If <paramref name="type"/> has more than one Process method, the first declare Process method will be wrapped.</para>
        /// </remarks>
        public Action<object, HandlerCache> Create(Type type)
        {
            if (type == null) { throw new ArgumentNullException("type"); }

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
