namespace Cidade_Dorme.Models
{
    public class GameRoom
    {
        private readonly object _sync = new();
        private List<Player> _players { get; } = new();
        public int CurrentRound { get; set; }
        public int CurrentRoundStage { get; set; }
        public Dictionary<string, string> Votes { get; } = new();

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

        public List<Player> GetPlayers() 
        { 
            return _players;
        }
    }
}
