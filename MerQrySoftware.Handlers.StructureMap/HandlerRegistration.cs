using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace MerQrySoftware.Handlers
{
    /// <summary>
    /// Defines methods for registering handlers.
    /// </summary>
    public static class HandlerRegistration
    {
        public static IRegisterHandlers Register(this Registry registry, string name)
        {
            var result = new HandlerRegistrationImplementation();

            registry.For<HandlerProcessor>()
                .Add<HandlerProcessor>()
                .Named(name)
                .Ctor<IList<object>>("handlers").Is(new EnumerableInstance(result));

            return result;
        }

        /// <summary>
        /// Defines a method that registers a handler.
        /// </summary>
        public interface IRegisterHandlers
        {
            /// <summary>
            /// Registers a handler.
            /// </summary>
            /// <typeparam name="THandler">The type of the handler.</typeparam>
            /// <returns>
            /// An instance of <see cref="HandlerRegistration.IRegisterHandlers" />.
            /// </returns>
            IRegisterHandlers Processes<THandler>();
        }

        private class HandlerRegistrationImplementation : IRegisterHandlers, IEnumerable<Instance>
        {
            private readonly List<Instance> instances;

            /// <summary>
            /// Initializes a new instance of the <see cref="HandlerRegistrationImplementation"/> class.
            /// </summary>
            public HandlerRegistrationImplementation()
            {
                instances = new List<Instance>();
            }

            #region IEnumerable<Instance> Members

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// An enumerator that can be used to iterate through the collection.
            /// </returns>
            IEnumerator<Instance> IEnumerable<Instance>.GetEnumerator()
            {
                return instances.GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return instances.GetEnumerator();
            }

            #endregion

            #region IRegisterHandlers Members

            /// <summary>
            /// Registers a handler.
            /// </summary>
            /// <typeparam name="THandler">The type of the handler.</typeparam>
            /// <returns>
            /// An instance of <see cref="HandlerRegistration.IRegisterHandlers" />.
            /// </returns>
            /// <exception cref="System.ArgumentException"><typeparamref name="THandler" /> does not have a public method named Process.</exception>
            public IRegisterHandlers Processes<THandler>()
            {
                if (!typeof(THandler).GetMethods().Where(method => method.Name == "Process").Any())
                {
                    throw new ArgumentException(string.Format("{0} does not have a public, instance method named Process.", typeof(THandler)));
                }

                instances.Add(new SmartInstance<THandler, object>());
                
                return this;
            }

            #endregion
        }
    }
}
