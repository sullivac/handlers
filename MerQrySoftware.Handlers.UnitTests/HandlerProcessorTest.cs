using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MerQrySoftware.Handlers
{
    [TestClass]
    public class HandlerProcessorTest
    {
        [TestMethod]
        public void Constructor_WhenMethodCacheIsNull_ThrowsArgumentNullException()
        {
            TestHelper.Act(() => new HandlerProcessor(methodCache: null, handlerCache: new HandlerCache(), handlers: new object[0]))
                .ExpectArgumentNullException("methodCache");
        }

        [TestMethod]
        public void Constructor_WhenHandlerCacheIsNull_ThrowsArgumentNullException()
        {
            TestHelper.Act(() => new HandlerProcessor(methodCache: new MethodCache(), handlerCache: null, handlers: new object[0]))
                .ExpectArgumentNullException("handlerCache");
        }

        [TestMethod]
        public void Constructor_WhenHandlersIsNull_ThrowsArgumentNullException()
        {
            TestHelper.Act(() => new HandlerProcessor(methodCache: new MethodCache(), handlerCache: new HandlerCache(), handlers: null))
                .ExpectArgumentNullException("handlers");
        }

        [TestMethod]
        public void Process_WhenMultipleHandlersAreCalled_CallsEachHandler()
        {
            var handlerCache = new HandlerCache();
            var message = "message";

            var sut =
                new HandlerProcessor(
                    new MethodCache(),
                    handlerCache,
                    new object[] { new MessageHandler(message), new ConvertToUpperCaseHandler() });

            sut.Process();

            Assert.AreEqual("MESSAGE", ((Result)handlerCache.Get(typeof(Result))).Value);
        }

        [TestMethod]
        public void Process_WhenHandlerReturnsResult_OtherHandlersNotExecuted()
        {
            bool called = false;

            var sut =
                new HandlerProcessor<Result>(
                    new MethodCache(),
                    new HandlerCache(),
                    new object[] { new MessageHandler("hello"), new ActionHandler(() => { called = true; }) });

            sut.Process();

            Assert.IsFalse(called);
        }

        private class ActionHandler
        {
            private readonly Action action;

            public ActionHandler(Action action)
            {
                this.action = action;
            }

            public void Process()
            {
                action();
            }
        }

        private class ConvertToUpperCaseHandler
        {
            public Result Process(Result result)
            {
                return new Result(result.Value.ToUpperInvariant());
            }
        }

        private class MessageHandler
        {
            private readonly string message;

            public MessageHandler(string message)
            {
                this.message = message;
            }

            public Result Process()
            {
                return new Result(message);
            }
        }

        private class Result
        {
            public Result(string value)
            {
                Value = value;
            }

            public string Value { get; private set; }
        }
    }
}
