using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MerQrySoftware
{
    internal static class TestHelper
    {
        public static IVerify Act(Action execute)
        {
            return new Verification(execute);
        }

        public static void AssertArgumentExceptionParamName(this Exception exception, string paramName)
        {
            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
            Assert.AreEqual(paramName, ((ArgumentException)exception).ParamName);
        }

        public static void AssertExceptionMessage(this Exception exception, string message)
        {
            Assert.IsTrue(
                exception.Message.IndexOf(message, StringComparison.Ordinal) > -1,
                string.Format("Message does not contain \"{0}\".", message));
        }

        public static void ExpectArgumentNullException(this IVerify verification, string paramName)
        {
            verification.ExpectException<ArgumentNullException>(exception => exception.AssertArgumentExceptionParamName(paramName));
        }

        public static IVerify<T> Theory<T>(string paramName, T[] values)
        {
            return new TheoryVerification<T>(paramName, values);
        }

        public interface IVerify
        {
            void ExpectException<TException>(Action<TException> assert) where TException : Exception;

            void Verify();
        }

        public interface IVerify<T> : IVerify
        {
            IVerify Act(Action<T> execute);
        }

        private class Verification : IVerify
        {
            private readonly Action execute;

            public Verification(Action execute)
            {
                this.execute = execute;
            }

            public void ExpectException<TException>(Action<TException> assert) where TException : Exception
            {
                TException expected = null;

                try
                {
                    execute();

                    Assert.Fail("{0} not thrown.", typeof(TException));
                }
                catch (TException exception)
                {
                    expected = exception;
                }
                catch (AssertFailedException exception)
                {
                    throw exception;
                }
                catch (Exception exception)
                {
                    Assert.Fail(
                        new StringBuilder()
                        .Append(typeof(TException)).Append(" not thrown; ").Append(exception.GetType()).Append(" thrown instead.").AppendLine()
                        .Append(exception.ToString())
                        .ToString());
                }

                assert(expected);
            }

            public void Verify()
            {
                execute();
            }
        }

        private class TheoryVerification<T> : IVerify, IVerify<T>
        {
            private Action<T> execute;
            private readonly string paramName;
            private readonly IList<T> values;

            public TheoryVerification(string paramName, IList<T> values)
            {
                this.paramName = paramName;
                this.values = values;

                execute = DoNothing;
            }

            private static void DoNothing(T value) { }

            private void ExecuteTheory(Action<T> execute)
            {
                int failureCount = 0;
                var failures = new StringBuilder();

                foreach (T value in values)
                {
                    try
                    {
                        execute(value);
                    }
                    catch (AssertFailedException exception)
                    {
                        ++failureCount;

                        failures.AppendLine()
                            .Append(
                                new StringBuilder()
                                .Append("Assertion failed for ").Append(paramName).Append(": \"").Append(value).Append("\"").AppendLine()
                                .Append(exception.Message)
                                .ToString());
                    }
                    catch (Exception exception)
                    {
                        ++failureCount;

                        failures.AppendLine()
                            .Append(
                                new StringBuilder()
                                .Append("An unhandled exception occurred for ").Append(paramName).Append(": \"").Append(value).Append("\"").AppendLine()
                                .Append(exception.Message)
                                .ToString());
                    }
                }

                if (failureCount > 0)
                {
                    throw new AssertFailedException(new StringBuilder().Append("At least one test case failed:").Append(failures.ToString()).ToString());
                }
            }

            #region IVerify Members

            void IVerify.ExpectException<TException>(Action<TException> assert)
            {
                if (assert == null) { return; }

                ExecuteTheory(execute: value => new Verification(execute: () => execute(value)).ExpectException<TException>(assert));
            }

            void IVerify.Verify()
            {
                ExecuteTheory(execute: value => execute(value));
            }

            #endregion

            #region IVerify<T> Members

            IVerify IVerify<T>.Act(Action<T> execute)
            {
                if (execute != null) { this.execute = execute; }

                return this;
            }

            #endregion
        }
    }
}
