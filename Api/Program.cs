using Minimal_Api;

 IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults( webBuider =>
    {
        webBuider.UseStartup<Startup>();
    });
  }

CreateHostBuilder(args).Build().Run();