using System.Collections.Concurrent;

namespace Cidade_Dorme.Models
{
    public class GameRoom
    {
        private readonly object _sync = new();
        private RoomSettings _settings = new();
        private List<Player> _players { get; } = new();
        private int CurrentRound { get; set; }
        private int CurrentRoundStage { get; set; }
        private ConcurrentDictionary<string, string> Votes { get; } = new();

        public bool TryAddPlayer(Player player)
        {
            lock (_sync)
            {
                if (_players.Any(x => x.ConnectionId == player.ConnectionId))
                {
                    return false;
                }

                _players.Add(player);
                return true;
            }
        }

        //Return player count
        public int RemovePlayer(string connectionId, out string? playerName)
        {
            lock (_sync)
            {
                var player = _players.FirstOrDefault(x => x.ConnectionId == connectionId);
                playerName = player?.Name;

                if (player is not null)
                {
                    _players.Remove(player);
                }

                return _players.Count;
            }
        }

        public void ResetRoom()
        {
            lock (_sync)
            {
                CurrentRound = 0;
                CurrentRoundStage = 0;
                Votes.Clear();

                foreach (var player in _players.Where(p => p.Role != PlayerRole.Host))
                {
                    player.Role = PlayerRole.Spectator;
                }
            }
        }

        public void AssignRoles()
        {
            lock (_sync)
            {
                if (_settings.Assasins + _settings.Detectives + _settings.Medics < _players.Count - 1)
                    throw new Exception("Não há jogadores suficientes");

                Random rng = new Random();
                Dictionary<int, PlayerRole> roles = new Dictionary<int, PlayerRole>();

                for (int i = 0; i < _settings.Assasins; i++)
                {
                    bool assigned = false;
                    do
                    {
                        assigned = roles.TryAdd(rng.Next(1, _players.Count), PlayerRole.Assasin); //Começando de 1 porque 0 é o host
                    } while (!assigned);
                }

                for (int i = 0; i < _settings.Detectives; i++)
                {
                    bool assigned = false;
                    do
                    {
                        assigned = roles.TryAdd(rng.Next(1, _players.Count), PlayerRole.Detective);
                    } while (!assigned);
                }

                for (int i = 0; i < _settings.Medics; i++)
                {
                    bool assigned = false;
                    do
                    {
                        assigned = roles.TryAdd(rng.Next(1, _players.Count), PlayerRole.Medic);
                    } while (!assigned);
                }

                foreach (var role in roles)
                {
                    _players[role.Key].Role = role.Value;
                }
            }
        }

        public List<Player> GetPlayers()
        {
            return _players;
        }
    }
}
