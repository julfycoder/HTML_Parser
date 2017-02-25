using HTML_Parser.Business.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Commands.Handlers
{
    public class CreateSiteTreeCommandHandler : CommandHandler
    {
        readonly ISiteTreeBuilder _builder;

        public CreateSiteTreeCommandHandler(ISiteTreeBuilder builder)
        {
            _builder = builder;
            _commandKey = "CST";
        }

        protected override ICommand HandleConcreteCommand(string[] commandSegments)
        {
            return new CreateSiteTreeCommand(_builder,
                new CreateSiteTreeCommandInfo
                {
                    Url = commandSegments[1]
                });
        }
    }
}
