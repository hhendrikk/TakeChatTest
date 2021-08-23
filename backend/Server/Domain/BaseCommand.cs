namespace Server.Domain
{
    using Contracts;
    using Enum;
    using Infrastructure.Extensions;

    public class BaseCommand : ICommand
    {
        public BaseCommand() { }
        
        public CommandType Type { get; set; }
        
        public User Sender { get; set; }

        public string Message { get; set; }
        
        public override string ToString() => this.ToJsonSerialize();
    }
}