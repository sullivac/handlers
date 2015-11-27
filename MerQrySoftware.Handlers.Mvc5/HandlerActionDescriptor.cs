using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MerQrySoftware.Handlers
{
    internal class HandlerActionDescriptor : ActionDescriptor
    {
        private readonly string actionName;
        private readonly Dictionary<Type, List<Attribute>> attributes;
        private readonly ControllerDescriptor controllerDescriptor;
        private readonly Func<string, HandlerCache, HandlerProcessor> createHandlerProcessor;
        private readonly List<ParameterDescriptor> parameterDescriptors;

        public HandlerActionDescriptor(
            string actionName,
            ControllerDescriptor controllerDescriptor,
            Func<string, HandlerCache, HandlerProcessor> createhandlerProcessor)
        {
            this.actionName = actionName;
            this.controllerDescriptor = controllerDescriptor;
            this.createHandlerProcessor = createhandlerProcessor;

            this.attributes = new Dictionary<Type, List<Attribute>>();
            this.parameterDescriptors = new List<ParameterDescriptor>();
        }

        public void Add(ParameterDescriptor parameterDescriptor)
        {
            if (parameterDescriptor == null) { throw new ArgumentNullException("parameterDescriptor"); }

            parameterDescriptors.Add(parameterDescriptor);
        }

        public void AddAttribute(Attribute attribute)
        {
            if (attribute == null) { throw new ArgumentNullException("attribute"); }

        }

        private string HandlerProcessorKey
        {
            get { return string.Format("{0}/{1}", controllerDescriptor.ControllerName.ToLowerInvariant(), ActionName.ToLowerInvariant()); }
        }

        #region ActionDescriptor Override Members

        public override string ActionName
        {
            get { return actionName; }
        }

        public override ControllerDescriptor ControllerDescriptor
        {
            get { return controllerDescriptor; }
        }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            var handlerCache = new HandlerCache();

            foreach (var value in parameters.Values.Where(value => value != null))
            {
                handlerCache.SetValue(value.GetType(), value);
            }

            HandlerProcessor handlerProcessor = createHandlerProcessor(HandlerProcessorKey, handlerCache);

            handlerProcessor.Process();

            return (ActionResult)handlerCache.Get(typeof(ActionResult));
        }

        public override ParameterDescriptor[] GetParameters()
        {
            return parameterDescriptors.ToArray();
        }

        #endregion
    }
}