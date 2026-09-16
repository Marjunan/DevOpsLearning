using Xunit;

namespace ProductApi.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void PassingTest()
        {
            // This test should pass.
            Assert.Equal(2, 1 + 1);
        }

        [Fact]
        public void FailingTest()
        {
            // This test is intended to fail.
            Assert.Equal(3, 1 + 1);
        }
    }
}
