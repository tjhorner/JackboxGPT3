namespace JackboxGPT3.Clients.Models
{
    public struct GameState<RoomType, PlayerType>
    {
        public int PlayerId;
        public RoomType Room;
        public PlayerType Self;
    }
}
