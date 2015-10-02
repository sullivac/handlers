using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace MerQrySoftware.Handlers
{
    /// <summary>
    /// A <see cref="ControllerActionInvoker" /> that returns a
    /// <see cref="ControllerDescriptor" /> from a configured list.
    /// </summary>
    internal class HandlerControllerActionInvoker : AsyncControllerActionInvoker
    {
        private readonly IDictionary<string, HandlerControllerDescriptor> controllerDescriptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerControllerActionInvoker" /> class.
        /// </summary>
        /// <param name="controllerDescriptors">The controller descriptors.</param>
        public HandlerControllerActionInvoker(IDictionary<string, HandlerControllerDescriptor> controllerDescriptors)
        {
            this.controllerDescriptors = controllerDescriptors;
        }

        #region AsyncControllerActionInvoker Override Members

        /// <summary>
        /// Gets the controller descriptor.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <returns>
        /// The controller descriptor.
        /// </returns>
        protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext)
        {
            string controllerName = controllerContext.RouteData.GetRequiredString("controller");

            return controllerDescriptors[controllerName];
        }

        #endregion
    }
}
