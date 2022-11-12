// See https://aka.ms/new-console-template for more information

using public_apis.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(configure =>
{
	configure.AddCommand<ShipCommand>("ship")
		.WithDescription("Copy and merge contents of PublicAPI.Unshipped.txt to PublicAPI.Shipped.txt.");
});

return await app.RunAsync(args);
