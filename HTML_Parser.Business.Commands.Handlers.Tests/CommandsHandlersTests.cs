using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HTML_Parser.Business.Parsing;
using HTML_Parser.Business.Commands;

namespace HTML_Parser.Business.Commands.Handlers.Tests
{
    [TestClass]
    public class CommandsHandlersTests
    {
        [TestMethod]
        public void CommandsHandlersInteractionTest_On_P_ReturnsParseCommand()
        {
            ICommand command = HandleCommandCall(new string[] { "P", "10", "1", "false", "https://www.wikipedia.org" });

            Assert.IsInstanceOfType(command, typeof(ParseCommand));
        }
        [TestMethod]
        public void CommandsHandlersInteractionTest_On_CST_ReturnsCreateSiteTreeCommand()
        {
            ICommand command = HandleCommandCall(new string[] { "CST", "https://www.wikipedia.org" });

            Assert.IsInstanceOfType(command, typeof(CreateSiteTreeCommand));
        }

        ICommand HandleCommandCall(string[] commandSegments)
        {
            var parserMock = new Mock<IParser>();
            var siteTreeBuilderMock = new Mock<ISiteTreeBuilder>();

            ParseCommandHandler p = new ParseCommandHandler(parserMock.Object);
            CreateSiteTreeCommandHandler cst = new CreateSiteTreeCommandHandler(siteTreeBuilderMock.Object);

            p.SetSuccessor(cst);

            return p.HandleCommand(commandSegments);
        }
    }
}
