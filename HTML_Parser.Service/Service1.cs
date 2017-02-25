using System.ServiceProcess;
using HTML_Parser.Business.Commands;
using HTML_Parser.Business.Commands.Handlers;
using StructureMap;
using NLog;

namespace HTML_Parser.Service
{
    public partial class Service1 : ServiceBase
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        ICommandsManager manager;
        public Service1()
        {
            Container c = new Container(x =>
            {
                x.Scan(s =>
                {
                    s.WithDefaultConventions();
                    s.AssembliesFromApplicationBaseDirectory();
                });
                x.For<IHandlersChainFactory>().Use<HTML_ParserHandlersChainFactory>();
                x.For<CommandHandler>().Use<ParseCommandHandler>();
                x.For<CommandHandler>().Use<CreateSiteTreeCommandHandler>();
            });
            manager = c.GetInstance<ICommandsManager>();

            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            manager.Start();
        }

        protected override void OnStop()
        {
            manager.Stop();
        }
    }
}
