using Cidade_Dorme.Hubs;
using Cidade_Dorme.Interfaces;
using Cidade_Dorme.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddRazorPages();
builder.Services.AddSignalR(); 

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapRazorPages();
app.MapHub<GameHub>("/gameHub");

app.Run();
