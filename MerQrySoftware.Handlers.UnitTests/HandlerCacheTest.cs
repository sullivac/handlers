using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MerQrySoftware.Handlers
{
    [TestClass]
    public class HandlerCacheTest
    {
        [TestMethod]
        public void Get_WhenTypeIsAReferenceTypeAndIsMissing_ReturnsNull()
        {
            var sut = new HandlerCache();

            var result = sut.Get(typeof(string));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Get_WhenTypeIsAValueTypeAndIsMissing_ReturnsDefault()
        {
            var sut = new HandlerCache();

            var result = sut.Get(typeof(int));

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Get_WhenTypeIsAReferenceTypeAndIsCached_ReturnsValueMatchingType()
        {
            var value = "hello, world!";

            var sut = new HandlerCache();
            sut.Set(value);

            var result = sut.Get(typeof(string));

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Get_WhenTypeIsAReferenceTypeAndIsCachedUsingNonGenericSet_ReturnsValueMatchingType()
        {
            var value = "hello, world!";

            var sut = new HandlerCache();
            sut.SetValue(typeof(string), value);

            var result = sut.Get(typeof(string));

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Get_WhenTypeIsAValueTypeAndIsCached_ReturnsValueMatchingType()
        {
            var value = 1;

            var sut = new HandlerCache();
            sut.Set(value);

            var result = sut.Get(typeof(int));

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Get_WhenTypeIsAValueTypeAndIsCachedWhenUsingNonGenericSet_ReturnsValueMatchingType()
        {
            var value = 1;

            var sut = new HandlerCache();
            sut.SetValue(typeof(int), value);

            var result = sut.Get(typeof(int));

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Get_WhenTypeIsMissingAndGetMissingIsSpecified_ReturnsValueFromGetMissing()
        {
            TestHelper.Theory("type", new[] { typeof(int), typeof(string) })
                .Act(
                    type =>
                    {
                        var value = new object();

                        var sut = new HandlerCache();
                        sut.GetMissing = missingType => value;

                        var result = sut.Get(type);

                        Assert.AreEqual(value, result);
                    })
                .Verify();
        }

        [TestMethod]
        public void Get_WhenTypeIsReferenceTypeAndValueIsNull_ReturnsValueOfGetMissingFunction()
        {
            var value = new object();

            var sut = new HandlerCache();
            sut.GetMissing = missingType => value;
            sut.Set<string>(null);

            var result = sut.Get(typeof(string));

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Get_WhenTypeIsReferenceTypeAndValueIsNullUsingNonGenericSet_ReturnsValueOfGetMissingFunction()
        {
            var value = new object();

            var sut = new HandlerCache();
            sut.GetMissing = missingType => value;
            sut.SetValue(typeof(string), null);

            var result = sut.Get(typeof(string));

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void GetMissing_WhenValueIsNull_ThrowsArgumentNullException()
        {
            TestHelper.Act(
                () =>
                {
                    var sut = new HandlerCache();

                    sut.GetMissing = null;
                })
                .ExpectArgumentNullException("value");
        }

        [TestMethod]
        public void SetValue_WhenTypeAndValueDoNotMatch_ThrowsArgumentException()
        {
            TestHelper.Act(
                () =>
                {
                    var sut = new HandlerCache();

                    sut.SetValue(typeof(string), new object());
                })
                .ExpectException<ArgumentException>(
                    exception =>
                    {
                        exception.AssertArgumentExceptionParamName("type");
                        exception.AssertExceptionMessage("type does not match value's type.");
                    });
        }
    }
}
