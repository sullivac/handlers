using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MerQrySoftware.Handlers
{
    [TestClass]
    public class MethodCacheTest
    {
        [TestMethod]
        public void Get_WhenTypeIsNull_ThrowsArgumentNullException()
        {
            TestHelper.Act(
                () =>
                {
                    var sut = new MethodCache();

                    sut.Get(null);
                })
                .ExpectArgumentNullException("type");
        }

        [TestMethod]
        public void Create_WhenTypeDoesNotHaveAProcessMethod_ThrowsArgumentException()
        {
            TestHelper.Act(
                () =>
                {
                    var sut = new MethodCache();

                    sut.Get(typeof(object));
                })
                .ExpectException<ArgumentException>(
                    exception =>
                    {
                        exception.AssertExceptionMessage("Process method does not exist on System.Object.");
                        exception.AssertArgumentExceptionParamName("type");
                    });
        }

        [TestMethod]
        public void Get_WhenTypeIsNotCached_ReturnsWrappedProcessMethod()
        {
            var called = false;
            var handler = new CommandHandler(action: () => called = true);

            var sut = new MethodCache();

            Action<object, HandlerCache> process = sut.Get(typeof(CommandHandler));

            process(handler, new HandlerCache());

            Assert.IsTrue(called, "Process not called.");
        }

        [TestMethod]
        public void Get_WhenTypeIsCached_ReturnsSameReference()
        {
            var type = typeof(CommandHandler);

            var sut = new MethodCache();

            Action<object, HandlerCache> first = sut.Get(type);
            Action<object, HandlerCache> second = sut.Get(type);

            Assert.AreEqual(first, second);
        }
        
        [TestMethod]
        public void Get_WhenProcessMethodIsNotAVoidReturn_ReturnsWrappedProcessMethodAndHandlerCacheContainsReturnValue()
        {
            var value = "message";
            var handler = new ValueHandler<string>(value);
            var handlerCache = new HandlerCache();

            var sut = new MethodCache();

            Action<object, HandlerCache> process = sut.Get(typeof(ValueHandler<string>));

            process(handler, handlerCache);

            Assert.AreEqual(value, handlerCache.Get(typeof(string)));
        }

        [TestMethod]
        public void Get_WhenTypeHasMultipleProcessMethods_ReturnsWrappedFirstProcessMethod()
        {
            var calledFirst = false;
            var calledSecond = true;

            var sut = new MethodCache();

            Action<object, HandlerCache> process = sut.Get(typeof(MultiProcessHandler));

            process(new MultiProcessHandler(executeFirst: value => calledFirst = true, executeSecond: value => calledSecond = true), new HandlerCache());

            Assert.IsTrue(calledFirst, "First Process method not called.");
            Assert.IsTrue(calledSecond, "Unexpected call to second Process method.");
        }

        private class CommandHandler
        {
            private readonly Action execute;

            public CommandHandler(Action action)
            {
                this.execute = action;
            }

            public void Process() { execute(); }
        }

        private class MultiProcessHandler
        {
            private readonly Action<string> executeFirst;
            private readonly Action<int> executeSecond;

            public MultiProcessHandler(Action<string> executeFirst, Action<int> executeSecond)
            {
                this.executeFirst = executeFirst;
                this.executeSecond = executeSecond;
            }

            public void Process(string value)
            {
                executeFirst(value);
            }

            public void Process(int value)
            {
                executeSecond(value);
            }
        }
        
        private class ValueHandler<T>
        {
            private readonly T value;

            public ValueHandler(T value)
            {
                this.value = value;
            }

            public T Process()
            {
                return value;
            }
        }
    }
}
