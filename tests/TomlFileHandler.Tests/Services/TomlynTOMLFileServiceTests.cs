using System.Reflection;
using FluentAssertions;
using TomlFileHandler.Services;
using TomlFileHandler.Tests.Data;
using TomlFileHandler.Tests.Extensions;
using Xunit;

#pragma warning disable xUnit1026   // Disable unused method parameter warning

// ReSharper disable InconsistentNaming

namespace TomlFileHandler.Tests.Services;

public class TomlynTOMLFileServiceTests
{
    private const string TestData1_ResourceName = "Data.TestData1.toml";

    [Fact]
    public void ReadText_can_read_and_parse_valid_toml()
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        // Act
        var table = TomlynTOMLFileService.ReadText(text);

        // Assert
        table.Should().NotBeNull();
        table.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void ReadFile_can_read_and_parse_valid_toml()
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var fileName = Path.GetTempFileName();
        File.WriteAllText(fileName, text);

        // Act
        var table = TomlynTOMLFileService.ReadFile(fileName);

        // Assert
        table.Should().NotBeNull();
        table.Count.Should().BeGreaterThan(0);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownStringData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlNode_can_read_and_return_nodes_as_expected(string sectionName, string keyName, string expectedNodeText)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var result = TomlynTOMLFileService.GetTomlNode(table, sectionName: sectionName, keyName: keyName)
            ?.ToString();

        // Assert
        result.Should().Be(expectedNodeText);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownStringArrayData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlNode_can_read_and_return_array_nodes_as_expected(string sectionName, string keyName, int index, string expectedNodeText)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var result = TomlynTOMLFileService.GetTomlNode(table, sectionName: sectionName, keyName: keyName, valueIndex: index)
            ?.ToString();

        // Assert
        result.Should().Be(expectedNodeText);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.InvalidStringArrayData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlNode_fails_to_read_invalid_array_nodes(string sectionName, string keyName, int index)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => TomlynTOMLFileService.GetTomlValueAsInt(table, sectionName: sectionName, keyName: keyName, valueIndex: index)
        );

        // Assert
        ex.Should().NotBeNull();
        ex.ParamName.Should().Be(keyName);
        ex.ActualValue.Should().Be(index);
        ex.Message.Should().Contain($"'{keyName}'");
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownStringData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValue_can_read_and_return_values_as_strings(string sectionName, string keyName, string expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var value = TomlynTOMLFileService.GetTomlValue(table, sectionName: sectionName, keyName: keyName);

        // Assert
        value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownStringArrayData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValue_can_read_and_return_array_values_as_strings(string sectionName, string keyName, int index, string expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var value = TomlynTOMLFileService.GetTomlValue(table, sectionName: sectionName, keyName: keyName, valueIndex: index);

        // Assert
        value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownIntData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsInt_can_read_and_return_values_as_int(string sectionName, string keyName, int expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var value = TomlynTOMLFileService.GetTomlValueAsInt(table, sectionName: sectionName, keyName: keyName);

        // Assert
        value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownIntArrayData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsInt_can_read_and_return_array_values_as_int(string sectionName, string keyName, int index, int expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var value = TomlynTOMLFileService.GetTomlValueAsInt(table, sectionName: sectionName, keyName: keyName, valueIndex: index);

        // Assert
        value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownFloatData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsInt_fails_to_read_incorrect_data_type(string sectionName, string keyName, double expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var ex = Assert.Throws<InvalidCastException>(
            () => TomlynTOMLFileService.GetTomlValueAsInt(table, sectionName: sectionName, keyName: keyName)
        );

        // Assert
        ex.Should().NotBeNull();
        ex.Message.Should().Contain($"'{keyName}'");
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownLongData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsLong_can_read_and_return_values_as_long(string sectionName, string keyName, long expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var value = TomlynTOMLFileService.GetTomlValueAsLong(table, sectionName: sectionName, keyName: keyName);

        // Assert
        value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownFloatData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsLong_fails_to_read_incorrect_data_type(string sectionName, string keyName, double expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var ex = Assert.Throws<InvalidCastException>(
            () => TomlynTOMLFileService.GetTomlValueAsLong(table, sectionName: sectionName, keyName: keyName)
        );

        // Assert
        ex.Should().NotBeNull();
        ex.Message.Should().Contain($"'{keyName}'");
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownLongArrayData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsLong_can_read_and_return_array_values_as_long(string sectionName, string keyName, int index, long expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var value = TomlynTOMLFileService.GetTomlValueAsLong(table, sectionName: sectionName, keyName: keyName, valueIndex: index);

        // Assert
        value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownBoolData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsBool_can_read_and_return_array_values_as_long(string sectionName, string keyName, bool expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var value = TomlynTOMLFileService.GetTomlValueAsBool(table, sectionName: sectionName, keyName: keyName);

        // Assert
        value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownFloatData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsBool_fails_to_read_incorrect_data_type(string sectionName, string keyName, double expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var ex = Assert.Throws<InvalidCastException>(
            () => TomlynTOMLFileService.GetTomlValueAsBool(table, sectionName: sectionName, keyName: keyName)
        );

        // Assert
        ex.Should().NotBeNull();
        ex.Message.Should().Contain($"'{keyName}'");
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownFloatData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsFloat_can_read_and_return_array_values_as_long(string sectionName, string keyName, double expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var value = TomlynTOMLFileService.GetTomlValueAsFloat(table, sectionName: sectionName, keyName: keyName);

        // Assert
        value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownBoolData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsFloat_fails_to_read_incorrect_data_type(string sectionName, string keyName, bool expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var ex = Assert.Throws<InvalidCastException>(
            () => TomlynTOMLFileService.GetTomlValueAsFloat(table, sectionName: sectionName, keyName: keyName)
        );

        // Assert
        ex.Should().NotBeNull();
        ex.Message.Should().Contain($"'{keyName}'");
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownDateTimeData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsDateTime_can_read_and_return_values_as_long(string sectionName, string keyName, DateTime expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var value = TomlynTOMLFileService.GetTomlValueAsDateTime(table, sectionName: sectionName, keyName: keyName);

        // Assert
        value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownFloatData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsDateTime_fails_to_read_incorrect_data_type(string sectionName, string keyName, double expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var ex = Assert.Throws<InvalidCastException>(
            () => TomlynTOMLFileService.GetTomlValueAsDateTime(table, sectionName: sectionName, keyName: keyName)
        );

        // Assert
        ex.Should().NotBeNull();
        ex.Message.Should().Contain($"'{keyName}'");
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownDateTimeOffsetData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsDateTimeOffset_can_read_and_return_values_as_long(string sectionName, string keyName, DateTimeOffset expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var value = TomlynTOMLFileService.GetTomlValueAsDateTimeOffset(table, sectionName: sectionName, keyName: keyName);

        // Assert
        value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(TOMLFileServiceData.KnownFloatData), MemberType = typeof(TOMLFileServiceData))]
    public void GetTomlValueAsDateTimeOffset_fails_to_read_incorrect_data_type(string sectionName, string keyName, double expectedValue)
    {
        var text = Assembly.GetExecutingAssembly().GetEmbeddedResourceText(TestData1_ResourceName);
        text.Should().NotBeNullOrWhiteSpace();

        var table = TomlynTOMLFileService.ReadText(text);

        // Act
        var ex = Assert.Throws<InvalidCastException>(
            () => TomlynTOMLFileService.GetTomlValueAsDateTimeOffset(table, sectionName: sectionName, keyName: keyName)
        );

        // Assert
        ex.Should().NotBeNull();
        ex.Message.Should().Contain($"'{keyName}'");
    }
}
