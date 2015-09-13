using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MerQrySoftware.Handlers
{
    [TestClass]
    public class MethodCacheTest
    {
        [TestMethod]
        public void Constructor_WhenCreateProcessMethodIsNull_ThrowsArgumentNullException()
        {
            TestHelper.Act(() => new MethodCache(createProcessMethod: null))
                .ExpectArgumentNullException("createProcessMethod");
        }

        [TestMethod]
        public void Get_WhenTypeIsNull_ThrowsArgumentNullException()
        {
            TestHelper.Act(
                () =>
                {
                    var sut = new MethodCache(createProcessMethod: type => (handler, handlerCache) => { });

                    sut.Get(null);
                })
                .ExpectArgumentNullException("type");
        }

        [TestMethod]
        public void Get_WhenMethodIsNotCached_ReturnsMethod()
        {
            var called = false;
            var processMethodFactory = new ProcessMethodFactory();
            var handler = new CommandHandler(action: () => called = true);

            var sut = new MethodCache(createProcessMethod: type => processMethodFactory.Create(type));

            Action<object, HandlerCache> process = sut.Get(typeof(CommandHandler));

            process(handler, new HandlerCache());

            Assert.IsTrue(called, "Process not called.");
        }

        [TestMethod]
        public void Get_WhenMethodIsCached_CreateProcessMethodOnlyCalledOnce()
        {
            var callCount = 0;
            var processMethodFactory = new ProcessMethodFactory();

            var sut = new MethodCache(createProcessMethod: type => { callCount++; return processMethodFactory.Create(type); });

            Action<object, HandlerCache> first = sut.Get(typeof(CommandHandler));
            Action<object, HandlerCache> second = sut.Get(typeof(CommandHandler));

            Assert.AreEqual(first, second);
            Assert.AreEqual(1, callCount);
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
    }
}
