using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using HTML_Parser.DAL.Data;
using NLog;

namespace HTML_Parser.Business.Commands
{
    public class CommandsManager : ICommandsManager
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Queue<ICommand> _commands = new Queue<ICommand>();
        private readonly ICommandsInterpreter _commandsInterpreter;
        IParsingCommandsRepository _parsingCommandsRepository;
        private readonly Thread _t;

        public CommandsManager(ICommandsInterpreter commandsInterpreter,
            IParsingCommandsRepository parsingCommandsRepository)
        {
            this._commandsInterpreter = commandsInterpreter;
            this._parsingCommandsRepository = parsingCommandsRepository;
            parsingCommandsRepository.CommandAdded += WaitCommands;
            _t = new Thread(ExecuteNextCommand);
        }

        public void Start()
        {
            _t.Start();
            _logger.Info("Start");
        }

        private void ExecuteNextCommand()
        {
            while (true)
            {
                if (_commands.Count > 0)
                {
                    if (_commands.Peek() != null) _commands.Dequeue().Execute();
                    else _commands.Dequeue();
                    Console.WriteLine("Done");
                }
                else Thread.Sleep(600);
            }
        }

        public void Stop()
        {
            _t.Abort();
            _logger.Info("Stop");
        }

        public void WaitCommands(object sender, ParsingCommandsEventArgs e)
        {
            _commands.Enqueue(_commandsInterpreter.Interpret(e.CommandsString));
            _logger.Info("Command!!!: " + e.CommandsString);
        }
    }
}