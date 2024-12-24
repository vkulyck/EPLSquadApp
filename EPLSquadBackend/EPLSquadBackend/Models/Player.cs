namespace EPLSquadBackend.Models
{
    public class Player
    {
        public required string ProfilePicture { get; set; }
        public required string FirstName { get; set; }
        public required string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string Position { get; set; }
    }
}
