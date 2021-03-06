﻿using System;
using System.Linq;
using Akka.TestKit.Xunit;
using Xunit;

// ReSharper disable once CheckNamespace
namespace Akka.TestKit
{
    public static class XAssert
    {
        private static readonly XunitAssertions _assertions = new XunitAssertions();
        /// <summary>
        /// Fails the test with the specified reason.
        /// </summary>
        public static void Fail(string reason)
        {
            Assert.True(false, reason);
        }

        /// <summary>
        /// Asserts that both arguments are the same reference.
        /// </summary>
        public static void Same<T>(T expected, T actual, string message)
        {
            Assert.True(ReferenceEquals(expected,actual),message);
        }

        public static void Equal<T>(T expected, T actual, string format, params object[] args)
        {
            _assertions.AssertEqual(expected,actual,format,args);
        }

        public static void Throws<T>(Action action) where T : Exception
        {
            Exception exception = null;
            try
            {
                action();
            }
            catch(AggregateException ex) //need to flatten AggregateExceptions
            {
                if(ex.Flatten().InnerExceptions.Any(x => x is T)) return;
                exception = ex;
            }
            catch(Exception ex)
            {
                if(ex is T)
                {
                    return;
                }
                exception = ex;
            }
            if(exception != null)
                Fail("Expected exception of type " + typeof(T).FullName + ". Received " + exception);
            else
                Fail("Expected exception of type " + typeof(T).Name + " but no exceptions was thrown.");
        }
    }
}