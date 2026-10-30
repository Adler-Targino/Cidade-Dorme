namespace Cidade_Dorme.Models
{
    public class Player
    {
        public required string ConnectionId { get; set; }
        public required string Name { get; set; }
        public PlayerRole Role { get; set; }
    }
}
