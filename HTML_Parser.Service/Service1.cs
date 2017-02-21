using System.ServiceProcess;
using HTML_Parser.Business.Commands;
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
            });
            manager = c.GetInstance<ICommandsManager>();

            InitializeComponent();
        }
        //public void OnDebug()
        //{
        //    OnStart(null);
        //}
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
