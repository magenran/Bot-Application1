// <copyright file="ControlerTest.cs">Copyright ©  2016</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using wikipedia;

namespace wikipedia.Tests
{
    /// <summary>This class contains parameterized unit tests for Controler</summary>
    [PexClass(typeof(Controler))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ControlerTest
    {
        /// <summary>Test stub for getPage(String, Int32)</summary>
        [PexMethod]
        internal void getPageTest(
            [PexAssumeUnderTest]Controler target,
            string pageName = "סוס",
            int pageId =1
        )
        {
            target.getPage(pageName, pageId);

            // TODO: add assertions to method ControlerTest.getPageTest(Controler, String, Int32)
        }
    }
}
