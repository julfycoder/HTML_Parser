using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using HTML_Parser.DAL.Data;

namespace HTML_Parser.Business.Commands
{
    public class CommandsManager : ICommandsManager
    {
        Queue<ICommand> commands = new Queue<ICommand>();
        ICommandsInterpreter commandsInterpreter;
        IParsingCommandsRepository parsingCommandsRepository;
        public CommandsManager(ICommandsInterpreter commandsInterpreter, IParsingCommandsRepository parsingCommandsRepository)
        {
            this.commandsInterpreter = commandsInterpreter;
            this.parsingCommandsRepository = parsingCommandsRepository;
            parsingCommandsRepository.CommandAdded += WaitCommands;
        }
        public void ExecuteNextCommand()
        {
            while (true)
            {
                if (commands.Count > 0)
                {
                    commands.Dequeue().Execute();
                    Console.WriteLine("Done!");
                }
            }
        }

        public void WaitCommands(object sender, ParsingCommandsEventArgs e)
        {
            commands.Enqueue(commandsInterpreter.Interpret(e.CommandsString));
        }
    }
}
