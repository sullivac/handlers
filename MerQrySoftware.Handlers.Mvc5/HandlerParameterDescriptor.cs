using System;
using System.Web.Mvc;

namespace MerQrySoftware.Handlers
{
    internal class HandlerParameterDescriptor : ParameterDescriptor
    {
        private readonly ActionDescriptor actionDescriptor;
        private readonly string parameterName;
        private readonly Type parameterType;

        public HandlerParameterDescriptor(ActionDescriptor actionDescriptor, Type parameterType, string parameterName)
        {
            this.actionDescriptor = actionDescriptor;
            this.parameterType = parameterType;
            this.parameterName = parameterName;
        }

        #region ParameterDescriptor Override Membera

        public override ActionDescriptor ActionDescriptor
        {
            get { return actionDescriptor; }
        }

        public override string ParameterName
        {
            get { return parameterName; }
        }

        public override Type ParameterType
        {
            get { return parameterType; }
        }

        #endregion
    }
}
