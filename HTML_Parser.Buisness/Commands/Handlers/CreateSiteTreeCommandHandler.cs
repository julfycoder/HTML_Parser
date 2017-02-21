using HTML_Parser.Business.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace HTML_Parser.Business.Commands.Handlers
{
    public class CreateSiteTreeCommandHandler : CommandHandler
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        ISiteTreeBuilder builder;
        public CreateSiteTreeCommandHandler(ISiteTreeBuilder builder)
        {
            this.builder = builder;
        }
        public override ICommand HandleCommand(string[] commandSegments)
        {
            if (commandSegments[0] == "CST")
            {
                CreateSiteTreeCommand command = new CreateSiteTreeCommand(builder);
                command.Initialize(new CreateSiteTreeCommandInfo
                {
                    Url = commandSegments[1]
                });
                return command;
            }
            if (successor != null) return successor.HandleCommand(commandSegments);
            logger.Info("Unknown command");
            return null;
        }
    }
}
