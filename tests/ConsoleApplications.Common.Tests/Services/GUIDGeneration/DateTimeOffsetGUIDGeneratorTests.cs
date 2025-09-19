using ConsoleApplications.Common.Services.GUIDGeneration;
using ConsoleApplications.Common.Tests.Helpers;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit.Abstractions;

namespace ConsoleApplications.Common.Tests.Services.GUIDGeneration;

public class DateTimeOffsetGUIDGeneratorTests(ITestOutputHelper outputHelper)
{
    private static DateTimeOffsetGUIDGenerator Sut => new();

    [Fact]
    public void Can_generate_a_value()
    {
        outputHelper.AttachTraceListeners();

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
        outputHelper.AttachTraceListeners();

        repetitions.ShouldBeGreaterThan(1);

        var values = new List<Guid>();

        var value = Guid.Empty;

        for (var repetition = 0; repetition < repetitions; repetition++)
        {
            //Act
            var result = Sut.Generate();
            outputHelper.WriteLine($"{nameof(result)}: {result}");

            // Assert
            result.ShouldNotBe(Guid.Empty);
            result.ShouldBeGreaterThan(value);

            value = result;

            values.Add(result);
        }

        // Verify
        values.Any().ShouldBeTrue();
        values.Distinct().Count().ShouldBe(values.Count);
    }
}
