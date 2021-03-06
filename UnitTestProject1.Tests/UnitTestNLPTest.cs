// <copyright file="UnitTestNLPTest.cs">Copyright ©  2016</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject1;

namespace UnitTestProject1.Tests
{
    /// <summary>This class contains parameterized unit tests for UnitTestNLP</summary>
    [PexClass(typeof(UnitTestNLP))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class UnitTestNLPTest
    {
        /// <summary>Test stub for NLPTest()</summary>
        [PexMethod]
        public void NLPTestTest([PexAssumeUnderTest]UnitTestNLP target)
        {
            target.NLPTest();
            // TODO: add assertions to method UnitTestNLPTest.NLPTestTest(UnitTestNLP)
        }
    }
}
