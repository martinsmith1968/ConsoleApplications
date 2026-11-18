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
        result.ShouldNotBeNull();
        result.ReturnCode.ShouldBe(returnCode);
        result.Message.ShouldBe(message);
        result.InnerException.ShouldBeNull();
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
        result.ShouldNotBeNull();
        result.ReturnCode.ShouldBe(returnCode);
        result.Message.ShouldBe(message);
        result.InnerException.ShouldNotBeNull();
        result.InnerException.ShouldBe(innerException);
        result.InnerException!.Message.ShouldBe(innerMessage);
    }
}
