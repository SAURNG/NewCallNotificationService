using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace NewCallNotificationService
{
    class Program
    {
        static void Main(string[] args)
        {


            var exitCode = HostFactory.Run(x =>
            {
                x.Service<HeartBeat>(s =>
                {
                    s.ConstructUsing(heartbeat => new HeartBeat());
                    s.WhenStarted(heartbeat => heartbeat.Start());
                    s.WhenStopped(heartbeat => heartbeat.Stop());
                });
                x.RunAsLocalSystem();
                x.SetServiceName("CallNotificationService2.1");
                x.SetDisplayName("CallNotificationService2.1");
                x.SetDescription("SNG Twoje Wodociągi - Nowy Serwis do komunikacji z firebase by Wojciech J.");
              
            });
            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }

    }
}
