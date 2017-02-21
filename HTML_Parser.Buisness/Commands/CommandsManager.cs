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
        Logger logger = LogManager.GetCurrentClassLogger();
        Queue<ICommand> commands = new Queue<ICommand>();
        ICommandsInterpreter commandsInterpreter;
        IParsingCommandsRepository parsingCommandsRepository;
        Thread t;
        public CommandsManager(ICommandsInterpreter commandsInterpreter, IParsingCommandsRepository parsingCommandsRepository)
        {
            this.commandsInterpreter = commandsInterpreter;
            this.parsingCommandsRepository = parsingCommandsRepository;
            parsingCommandsRepository.CommandAdded += WaitCommands;
            t = new Thread(ExecuteNextCommand);
        }
        public void Start()
        {
            t.Start();
            logger.Info("Start");
        }
        public void ExecuteNextCommand()
        {
            while (true)
            {
                if (commands.Count > 0)
                {
                    if (commands.Peek() != null) commands.Dequeue().Execute();
                    else commands.Dequeue();
                    Console.WriteLine("Done");
                }
                else Thread.Sleep(600);
            }
        }
        public void Stop()
        {
            t.Suspend();
            logger.Info("Stop");
        }
        public void WaitCommands(object sender, ParsingCommandsEventArgs e)
        {
            commands.Enqueue(commandsInterpreter.Interpret(e.CommandsString));
            logger.Info("Command!!!: " + e.CommandsString);
        }
    }
}
