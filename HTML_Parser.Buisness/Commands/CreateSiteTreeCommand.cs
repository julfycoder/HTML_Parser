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
        readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISiteTreeBuilder _builder;
        private CreateSiteTreeCommandInfo _commandInfo;
        public CreateSiteTreeCommand(ISiteTreeBuilder builder,CreateSiteTreeCommandInfo commandInfo)
        {
            _builder = builder;
            _commandInfo = commandInfo;
        }
        public void Execute()
        {
            try
            {
                _logger.Info("Start to build site tree of '{0}'", _commandInfo.Url);
                _builder.CreateSiteTree(_commandInfo.Url);
                _logger.Info("End of site tree building");
            }
            catch (Exception e) { _logger.Error(e.Message); }
        }
    }
}
