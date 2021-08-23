namespace Server.Domain
{
    using System.Text.Json.Serialization;

    public record User
    {
        public User(string nickname, string id)
        {
            Nickname = nickname;
            Id = id;
        }
        
        public string Id { get; set; }
        public string Nickname { get; set; }
    };
}