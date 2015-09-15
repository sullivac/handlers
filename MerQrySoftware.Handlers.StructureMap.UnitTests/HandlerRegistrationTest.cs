using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace MerQrySoftware.Handlers
{
    [TestClass]
    public class HandlerRegistrationTest
    {
        [TestMethod]
        public void Processes_WhenHandlerTypeDoesNotHaveProcessMethod_ThrowsArgumentException()
        {
            TestHelper.Act(
                () =>
                {
                    var sut = new Registry();

                    sut.Register("name")
                        .Processes<object>();
                })
                .ExpectMessageContains<ArgumentException>("System.Object does not have a public, instance method named Process.");
        }

        [TestMethod]
        public void Processes_WhenHandlerTypeHasProcessMethod_RegistersHandlerType()
        {
            const string instanceName = "empty";
            var handlerCache = new HandlerCache();

            var sut = new Registry();

            sut.Register(instanceName)
                .Processes<EmptyHandler>();

            var container = new Container(sut);

            HandlerProcessor processor = container.With(handlerCache).GetInstance<HandlerProcessor>(instanceName);

            processor.Process();

            Assert.IsTrue(container.Model.For<HandlerProcessor>().HasImplementations(), "Missing implementations.");
            Assert.IsNotNull(container.Model.Find<HandlerProcessor>(instanceName));
            Assert.AreEqual("message", handlerCache.Get(typeof(string)));
        }

        [TestMethod]
        public void Processes_WhenCalledMultipleTimes_RegistersEachHandlerType()
        {
            const string instanceName = "empty";
            var handlerCache = new HandlerCache();
            handlerCache.Set(0);

            var sut = new Registry();

            sut.Register(instanceName)
                .Processes<CounterHandler>()
                .Processes<CounterHandler>();

            var container = new Container(sut);

            HandlerProcessor processor = container.With(handlerCache).GetInstance<HandlerProcessor>(instanceName);

            processor.Process();

            Assert.AreEqual(2, handlerCache.Get(typeof(int)));
        }

        private class CounterHandler
        {
            public int Process(int value)
            {
                return value + 1;
            }
        }

        private class EmptyHandler
        {
            public string Process() { return "message"; }
        }
    }
}
