using AutoFixture;
using TomlFileHandler.Exceptions;

namespace TomlFileHandler.Tests.Exceptions;

public class ReturnCodeExceptionTests
{
    private static readonly Fixture AutoFixture = new();

    [Fact]
    public void Construct_with_minimal_parameters_can_create_successfully()
    {
        var message = AutoFixture.Create<string>();
        var returnCode  = AutoFixture.Create<int>();

        // Act
        var result = new ReturnCodeException(returnCode, message);

        // Assert
        result.Should().NotBeNull();
        result.ReturnCode.Should().Be(returnCode);
        result.Message.Should().Be(message);
        result.InnerException.Should().BeNull();
    }

    [Fact]
    public void Construct_with_minimal_parameters_and_inner_exception_can_create_successfully()
    {
        var message = AutoFixture.Create<string>();
        var returnCode = AutoFixture.Create<int>();

        var innerMessage = AutoFixture.Create<string>();
        var innerException = new Exception(innerMessage);

        // Act
        var result = new ReturnCodeException(returnCode, message, innerException);

        // Assert
        result.Should().NotBeNull();
        result.ReturnCode.Should().Be(returnCode);
        result.Message.Should().Be(message);
        result.InnerException.Should().NotBeNull();
        result.InnerException.Should().Be(innerException);
        result.InnerException!.Message.Should().Be(innerMessage);
    }
}
