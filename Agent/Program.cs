using Agent;
using Agent.Interfaces;
using Agent.Realizations;
using Microsoft.AspNetCore.SignalR.Client;
using Shcheduler.Core;

IConfiguration config = new ConfigurationBuilder()
		   .SetBasePath(Directory.GetCurrentDirectory())
		   .AddJsonFile("appsettings.json", optional: false)
		   .Build();


IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices((services) =>
	{
		services.AddSingleton(config);
		var agentSettings = config.Get<AgentAppSettings>();
		services.AddSingleton(agentSettings);
		services.AddHostedService<Worker>();
		services.AddHttpClient<ISchRunner, SchRunner>();
		services.AddTransient<IJobService, JobService>();
		services.AddTransient<ISchRunner, SchRunner>();
		services.AddTransient<IScheduleService, ScheduleService>();
		services.AddTransient<ISchStatusService, SchStatusService>();
		services.AddTransient<IResultService, SchResultService>();
		services.AddTransient<ISignalRAgent, SignalRAgent>();
		services.AddSingleton<HubConnection>(sp =>
		{
			var config = sp.GetRequiredService<IConfiguration>();
			var hubConnection = new HubConnectionBuilder()
				.WithUrl(config.GetSection("SignalRHubUrl").Get<string>())
				.WithAutomaticReconnect()
				.Build();

			return hubConnection;
		});
		services.AddSingleton<SignalRAgent>();
		services.AddLogging(logging =>
		{
			logging.ClearProviders();
			logging.ClearProviders();
			logging.AddConsole();
		});
	})
	.Build();


await host.RunAsync();
