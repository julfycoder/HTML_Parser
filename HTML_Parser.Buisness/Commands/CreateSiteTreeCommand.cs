using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.Business.Parsing;
using NLog;

namespace HTML_Parser.Business.Commands
{
    public class CreateSiteTreeCommand : ICommand
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        ISiteTreeBuilder builder;
        string url;
        public CreateSiteTreeCommand(ISiteTreeBuilder builder)
        {
            this.builder = builder;
        }
        public void Execute()
        {
            try
            {
                logger.Info("Start to build site tree of '{0}'", url);
                builder.CreateSiteTree(url);
                logger.Info("End of site tree building");
            }
            catch (Exception e) { logger.Error(e.Message); }
        }

        public void Initialize(CommandInfoBase commandInfo)
        {
            CreateSiteTreeCommandInfo info = (CreateSiteTreeCommandInfo)commandInfo;
            url = info.Url;
        }
    }
}
