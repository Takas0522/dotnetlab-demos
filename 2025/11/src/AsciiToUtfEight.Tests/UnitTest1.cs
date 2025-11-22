using System.Text;

namespace AsciiToUtfEight.Tests;

public class ProgramTests : IDisposable
{
    [Fact]
    public void HelloWorld_ShouldPass()
    {
        // Arrange
        var expected = "Hello World";

        // Act
        var actual = "Hello World";

        // Assert
        Assert.Equal(expected, actual);
    }

    public void Dispose()
    {
    }
}
