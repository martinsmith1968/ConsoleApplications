using ConsoleApplications.Common.Configuration;

namespace ConsoleApplications.Common.Services;

public class TextGUIDFormatter
{
    public static string FormatGUID(Guid guid, TextGUIDFormatOptions options)
    {
        var value = string.IsNullOrEmpty(options.GUIDFormat)
            ? guid.ToString()
            : guid.ToString(options.GUIDFormat);

        value = PostProcess(value, options.CaseConversionType);

        return string.Format($"{options.Prefix}{value}{options.Suffix}", guid);
    }

    private static string PostProcess(string value, GUIDCaseConversionType postProcessing)
    {
        return postProcessing switch
        {
            GUIDCaseConversionType.Lower => value.ToLower(),
            GUIDCaseConversionType.Upper => value.ToUpper(),
            _ => value
        };
    }
}
