// unset

namespace Tests
{
    using Server.Core;
    using Xunit;

    public class IdGeneratorTests
    {
        [Fact]
        public void should_be_generate_new_id()
        {
            var generator = new IdGuidGenerator();
            var value = generator.New();
            
            Assert.NotEmpty(value);
        }
    }
}