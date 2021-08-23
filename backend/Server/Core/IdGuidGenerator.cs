namespace Server.Core
{
    using Contracts;
    using System;

    public class IdGuidGenerator : IIdGenerator
    {
        public string New() => Guid.NewGuid().ToString();
    }
}