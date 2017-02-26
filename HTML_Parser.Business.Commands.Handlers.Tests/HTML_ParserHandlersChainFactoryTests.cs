using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTML_Parser.Business.Parsing;
using HTML_Parser.Business.SiteTree;
using Moq;

namespace HTML_Parser.Business.Commands.Handlers.Tests
{
    [TestClass]
    public class HTML_ParserHandlersChainFactoryTests
    {
        [TestMethod]
        public void CreateHandlersChain_On_P_ParserCommandReturns()
        {
            var handler = CreateHandlersChainCall();

            var expected = handler.HandleCommand(new[] { "P", "10", "1", "false", "https://www.wikipedia.org" });

            Assert.IsInstanceOfType(expected, typeof(ParseCommand));
        }

        [TestMethod]
        public void CreateHandlersChain_On_CST_CreateSiteTreeCommandReturns()
        {
            var handler = CreateHandlersChainCall();

            var expected = handler.HandleCommand(new[] { "CST", "https://www.wikipedia.org" });

            Assert.IsInstanceOfType(expected, typeof(CreateSiteTreeCommand));
        }

        [TestMethod]
        public void CreateHandlersChain_On_UnknownCommand_NullReturns()
        {
            var handler = CreateHandlersChainCall();

            var expected = handler.HandleCommand(new[] { "Sun", "https://www.wikipedia.org" });

            Assert.IsNull(expected);
        }

        CommandHandler CreateHandlersChainCall()
        {
            var parserMock = new Mock<IParser>();
            var cstMock = new Mock<ISiteTreeBuilder>();
            HTML_ParserHandlersChainFactory factory =
                new HTML_ParserHandlersChainFactory(new CommandHandler[]
                    {new ParseCommandHandler(parserMock.Object), new CreateSiteTreeCommandHandler(cstMock.Object)});

            return factory.CreateHandlersChain();
        }
    }
}
