using Cidade_Dorme.Interfaces;
using Cidade_Dorme.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Cidade_Dorme.Services
{
    public class GameService : IGameService
    {
        private readonly ConcurrentDictionary<string, GameRoom> _rooms = new();
        private readonly ConcurrentDictionary<string, string> _playerConnectionRooms = new();


        public string CreateRoom(string connectionId, string name)
        {
            if (_playerConnectionRooms.ContainsKey(connectionId))
                throw new InvalidOperationException("Você já está em uma sala.");

            string code;
            GameRoom room = new GameRoom();
            Player player = new Player
            {
                ConnectionId = connectionId,
                Name = name,
                Role = PlayerRole.Host
            };

            do
            {
                code = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            }
            while (!_rooms.TryAdd(code, room));

            if (!room.TryAddPlayer(player)) //Não foi possível adicionar a sala
            {
                _rooms.TryRemove(code, out _);

                throw new InvalidOperationException("Não foi possível adicionar o jogador à sala.");
            }

            if (!_playerConnectionRooms.TryAdd(connectionId, code))
            {
                _rooms.TryRemove(code, out _);

                throw new InvalidOperationException("Você já está em uma sala.");
            }

            return code;
        }

        public void JoinRoom(string connectionId, string name, string code)
        {
            if (_playerConnectionRooms.ContainsKey(connectionId))
                throw new InvalidOperationException("Você já está em uma sala.");

            var player = new Player
            {
                ConnectionId = connectionId,
                Name = name,
                Role = PlayerRole.Spectator
            };

            if (!_rooms.TryGetValue(code, out var room))
                throw new HubException("Sala não encontrada.");

            if (!room.TryAddPlayer(player))
            {
                throw new InvalidOperationException("Não foi possível adicionar o jogador à sala.");
            }

            _playerConnectionRooms[connectionId] = code;
        }

        public string? Disconnect(string connectionId, out string? playerName)
        {
            playerName = null;

            if (!_playerConnectionRooms.TryRemove(connectionId, out var code))
            {
                return null;
            }

            if (!_rooms.TryGetValue(code, out var room))
                return code;

            int remainingPlayers = room.RemovePlayer(connectionId, out playerName);

            if (remainingPlayers == 0)
                _rooms.TryRemove(code, out _);

            return code;
        }

        public List<PlayerDTO> GetPlayerList(string code)
        {
            if(!_rooms.TryGetValue(code, out var room))
            {
                throw new HubException("Sala não encontrada.");
            }

            return room.GetPlayers().Select(p => new PlayerDTO
            {
                ConnectionId = p.ConnectionId,
                Name = p.Name
            }).ToList();
        }

        public GameResult NextRound(string connectionId)
        {
            throw new NotImplementedException();
        }

        public GameResult NextStage(string connectionId)
        {
            throw new NotImplementedException();
        }

        public void StartGame(string connectionId)
        {
            throw new NotImplementedException();
        }

        public Player Vote(string connectionId, string targetConnectionId)
        {
            throw new NotImplementedException();
        }
    }
}
