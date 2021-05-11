/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>May 11, 2021</date>
/////////////////////////////////////////////////////////

using JeremySnyder.Common;
using JeremySnyder.Common.Web;
using Microsoft.AspNetCore.Hosting;

namespace JeremySnyder.Example.Web
{
    /// <inheritdoc />
    public class Program : CommonWebProgram
    {
        public static void Main(string[] args)
        {
            Logging.Info("Client API site has initiated");

            CreateWebHostBuilder(args).Build().Run();
        }
        
        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return CommonWebHostBuilder(args).UseStartup<Startup>();
        }

    }
}
