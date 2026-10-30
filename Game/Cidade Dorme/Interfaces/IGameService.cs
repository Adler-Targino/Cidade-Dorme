using Cidade_Dorme.Models;

namespace Cidade_Dorme.Interfaces
{
    public interface IGameService
    {
        //Room Management
        string CreateRoom(string connectionId, string name);
        void JoinRoom(string connectionId, string name, string code);
        string? Disconnect(string connectionId, out string? playerName);
        List<PlayerDTO> GetPlayerList(string code);


        //Game management
        void StartGame(string connectionId);
        GameResult NextStage(string connectionId);
        GameResult NextRound(string connectionId);
        Player Vote(string connectionId, string targetConnectionId);
    }
}
