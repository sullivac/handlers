using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MerQrySoftware.Handlers
{
    internal class HandlerControllerDescriptor : ControllerDescriptor
    {
        private readonly Dictionary<string, ActionDescriptor> actionDescriptors;
        private readonly string controllerName;

        public HandlerControllerDescriptor(string controllerName)
        {
            this.controllerName = controllerName;
            this.actionDescriptors = new Dictionary<string, ActionDescriptor>();
        }

        public void Add(string actionName, ActionDescriptor actionDescriptor)
        {
            if (actionDescriptors == null) { throw new ArgumentNullException("actionDescriptors"); }

            actionDescriptors[actionName] = actionDescriptor;
        }

        #region ControllerDescriptor Override Members

        public override string ControllerName
        {
            get { return controllerName; }
        }

        public override Type ControllerType
        {
            get { return typeof(HandlerController); }
        }

        public override ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            return actionDescriptors[actionName];
        }

        public override ActionDescriptor[] GetCanonicalActions()
        {
            return actionDescriptors.Values.ToArray();
        }

        #endregion
    }
}
