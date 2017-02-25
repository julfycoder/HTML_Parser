using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HTML_Parser.Business.Parsing;
using NLog;

namespace HTML_Parser.Business.Commands
{
    public class ParseCommand : ICommand
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IParser _parser;
        private readonly ParseCommandInfo _commandInfo;

        public ParseCommand(IParser parser, ParseCommandInfo commandInfo)
        {
            _parser = parser;
            _commandInfo = commandInfo;
        }

        public void Execute()
        {
            try
            {
                _logger.Info("Start to parse '{0}', with {1} threads, with depth = {2}, foreign links = {3}",
                    _commandInfo.Url, _commandInfo.WorkerThreadsCount, _commandInfo.Depth, _commandInfo.UseForeignLinks);
                _parser.Start(_commandInfo.Url, _commandInfo.WorkerThreadsCount, _commandInfo.Depth,
                    _commandInfo.UseForeignLinks);
                _logger.Info("End of parsing");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }
}
