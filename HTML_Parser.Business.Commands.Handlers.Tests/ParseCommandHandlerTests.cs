using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HTML_Parser.Business.Commands.Handlers;
using HTML_Parser.Business.Parsing;


namespace HTML_Parser.Business.Commands.Handlers.Tests
{
    [TestClass]
    public class ParseCommandHandlerTests
    {
        [TestMethod]
        public void HandleCommand_On_P_ReturnsParseCommand()
        {
            var mock = new Mock<IParser>();
            var p = new ParseCommandHandler(mock.Object);

            Assert.IsInstanceOfType(p.HandleCommand(new[] { "P","10","1","false","https://www.wikipedia.org" }), typeof(ParseCommand));
        }
        [TestMethod]
        public void HandleCommand_On_UnknownCommand_ReturnsNull()
        {
            var mock = new Mock<IParser>();
            var p = new ParseCommandHandler(mock.Object);

            Assert.IsNull(p.HandleCommand(new[] { "Unknown" }));
        }
    }
}
