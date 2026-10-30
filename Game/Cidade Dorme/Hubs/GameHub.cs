using Cidade_Dorme.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Cidade_Dorme.Hubs
{
    public class GameHub(IGameService gameService) : Hub
    {

        public async Task<string> CreateRoom(string name)
        {
            var code = gameService.CreateRoom(Context.ConnectionId, name);

            await Groups.AddToGroupAsync(Context.ConnectionId, code);
            await Clients.Group(code).SendAsync("PlayerListUpdated", gameService.GetPlayerList(code));

            return code;
        }

        public async Task<string> JoinRoom(string code, string name)
        {
            gameService.JoinRoom(Context.ConnectionId, name, code);

            await Groups.AddToGroupAsync(Context.ConnectionId, code);

            await Clients.Group(code).SendAsync("PlayerJoined", $"{name} Entrou!");
            await Clients.Group(code).SendAsync("PlayerListUpdated", gameService.GetPlayerList(code));

            return code;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var code = gameService.Disconnect(Context.ConnectionId, out string? playerName);

            if (code is not null)
            {
                await Clients.Group(code).SendAsync("PlayerLeft", $"{playerName} Saiu.");
                await Clients.Group(code).SendAsync("PlayerListUpdated", gameService.GetPlayerList(code));
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
