﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HTML_Parser.Business.Parsing;
using HTML_Parser.Business.Commands;
using HTML_Parser.Business.SiteTree;

namespace HTML_Parser.Business.Commands.Handlers.Tests
{
    [TestClass]
    public class CommandsHandlersTests
    {
        [TestMethod]
        public void CommandsHandlersInteractionTest_On_P_ReturnsParseCommand()
        {
            var command = HandleCommandCall(new string[] {"P", "10", "1", "false", "https://www.wikipedia.org"});

            Assert.IsInstanceOfType(command, typeof(ParseCommand));
        }

        [TestMethod]
        public void CommandsHandlersInteractionTest_On_CST_ReturnsCreateSiteTreeCommand()
        {
            var command = HandleCommandCall(new string[] {"CST", "https://www.wikipedia.org"});

            Assert.IsInstanceOfType(command, typeof(CreateSiteTreeCommand));
        }

        [TestMethod]
        public void HandleCommand_On_UnknownCommand_ReturnsNull()
        {
            var command = HandleCommandCall(new string[] {"Unknown"});

            Assert.IsNull(command);
        }

        ICommand HandleCommandCall(string[] commandSegments)
        {
            var parserMock = new Mock<IParser>();
            var siteTreeBuilderMock = new Mock<ISiteTreeBuilder>();

            var p = new ParseCommandHandler(parserMock.Object);
            var cst = new CreateSiteTreeCommandHandler(siteTreeBuilderMock.Object);

            p.SetSuccessor(cst);

            return p.HandleCommand(commandSegments);
        }

    }
}
