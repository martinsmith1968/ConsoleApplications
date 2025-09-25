using ConsoleApplications.Common.Services.GUIDGeneration;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit.Abstractions;

namespace ConsoleApplications.Common.Tests.Services.GUIDGeneration;

public class SequentialAtEndGUIDGeneratorTests(ITestOutputHelper outputHelper)
{
    private static SequentialAtEndGUIDGenerator Sut => new();

    [Fact]
    public void Can_generate_a_value()
    {
        //Act
        var result = Sut.Generate();
        outputHelper.WriteLine($"{nameof(result)}: {result}");

        // Assert
        result.ShouldNotBe(Guid.Empty);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Can_generate_incrementing_values(int repetitions)
    {
        repetitions.ShouldBeGreaterThan(1);

        var values = new List<Guid>();

        for (var repetition = 0; repetition < repetitions; repetition++)
        {
            //Act
            var result = Sut.Generate();
            outputHelper.WriteLine($"{nameof(result)}: {result}");

            // Assert
            result.ShouldNotBe(Guid.Empty);

            values.Add(result);
        }

        // Verify
        values.Any().ShouldBeTrue();
        values.Distinct().Count().ShouldBe(values.Count);
    }
}
