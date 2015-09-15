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
            TestHelper.Act(() => new HandlerProcessor(methodCache: new MethodCache(createProcessMethod: type => null), handlerCache: null, handlers: new object[0]))
                .ExpectArgumentNullException("handlerCache");
        }

        [TestMethod]
        public void Constructor_WhenHandlersIsNull_ThrowsArgumentNullException()
        {
            TestHelper.Act(() => new HandlerProcessor(methodCache: new MethodCache(createProcessMethod: type => null), handlerCache: new HandlerCache(), handlers: null))
                .ExpectArgumentNullException("handlers");
        }

        [TestMethod]
        public void Process_WhenMultipleHandlersAreCalled_CallsEachHandler()
        {
            var processMethodFactory = new ProcessMethodFactory();
            var handlerCache = new HandlerCache();
            var message = "message";

            var sut =
                new HandlerProcessor(
                    new MethodCache(createProcessMethod: type => processMethodFactory.Create(type)),
                    handlerCache,
                    new object[] { new MessageHandler(message), new ConvertToUpperCaseHandler() });

            sut.Process();

            Assert.AreEqual("MESSAGE", ((Result)handlerCache.Get(typeof(Result))).Value);
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
