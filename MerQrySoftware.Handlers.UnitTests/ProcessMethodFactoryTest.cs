using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MerQrySoftware.Handlers
{
    [TestClass]
    public class ProcessMethodFactoryTest
    {
        [TestMethod]
        public void Create_WhenProcessMethodExists_ReturnsCallableLambda()
        {
            var called = false;
            var handler = new TestHandler(execute: () => called = true);
            var handlerCache = new HandlerCache();

            var sut = new ProcessMethodFactory();

            Action<object, HandlerCache> process = sut.Create(typeof(TestHandler));

            process(handler, handlerCache);

            Assert.IsTrue(called, "TestHandler.Process() not executed.");
        }

        [TestMethod]
        public void Create_WhenProcessMethodIsNotAVoidReturn_SetsValueInCache()
        {
            var value = "message";
            var handler = new ValueHandler<string>(value);
            var handlerCache = new HandlerCache();

            var sut = new ProcessMethodFactory();

            Action<object, HandlerCache> process = sut.Create(typeof(ValueHandler<string>));

            process(handler, handlerCache);

            Assert.AreEqual(value, handlerCache.Get(typeof(string)));
        }

        [TestMethod]
        public void Create_WhenProcessMethodDoesNotExist_ThrowsHandlerException()
        {
            TestHelper.Act(
                () =>
                {
                    var sut = new ProcessMethodFactory();

                    sut.Create(typeof(object));
                })
                .ExpectException<ArgumentException>(
                    exception =>
                    {
                        exception.AssertExceptionMessage("Process method does not exist on System.Object.");
                        exception.AssertArgumentExceptionParamName("type");
                    });
        }

        [TestMethod]
        public void Create_WhenTypeIsNull_ThrowsArgumentNullException()
        {
            TestHelper.Act(
                () =>
                {
                    var sut = new ProcessMethodFactory();

                    sut.Create(null);
                })
                .ExpectArgumentNullException("type");
        }

        [TestMethod]
        public void Create_WhenClassHasMultipleProcessMethods_ReturnsCallableLambdaForFirstProcessMethod()
        {
            var calledFirst = false;
            var calledSecond = true;

            var sut = new ProcessMethodFactory();

            Action<object, HandlerCache> process = sut.Create(typeof(MultiProcessHandler));

            process(new MultiProcessHandler(executeFirst: value => calledFirst = true, executeSecond: value => calledSecond = true), new HandlerCache());

            Assert.IsTrue(calledFirst, "First Process method not called.");
            Assert.IsTrue(calledSecond, "Unexpected call to second Process method.");
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

        private class TestHandler
        {
            private readonly Action execute;

            public TestHandler(Action execute)
            {
                this.execute = execute;
            }

            public void Process()
            {
                execute();
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
