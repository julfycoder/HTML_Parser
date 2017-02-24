using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HTML_Parser.Business.Parsing;

namespace HTML_Parser.Business.Commands.Handlers.Tests
{
    [TestClass]
    public class CreateSiteTreeCommandHandlerTests
    {
        [TestMethod]
        public void CreateSiteTreeCommandHandler_HandleCommand_On_CST_ReturnsCreateSiteTreeCommand()
        {
            var mock = new Mock<ISiteTreeBuilder>();
            CreateSiteTreeCommandHandler cst = new CreateSiteTreeCommandHandler(mock.Object);

            Assert.IsInstanceOfType(cst.HandleCommand(new string[] { "CST", "https://www.wikipedia.org" }), typeof(CreateSiteTreeCommand));
        }
        [TestMethod]
        public void HandleCommand_On_UnknownCommand_ReturnsNull()
        {
            var mock = new Mock<ISiteTreeBuilder>();
            CreateSiteTreeCommandHandler p = new CreateSiteTreeCommandHandler(mock.Object);

            Assert.IsNull(p.HandleCommand(new string[] { "Unknown" }));
        }
    }
}
