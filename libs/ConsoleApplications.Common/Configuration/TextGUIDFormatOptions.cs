namespace ConsoleApplications.Common.Configuration;

public class TextGUIDFormatOptions
{
    public string Prefix { get; set; } = string.Empty;

    public string Suffix { get; set; } = string.Empty;

    public string GUIDFormat { get; set; } = string.Empty;

    public  GUIDCaseConversionType CaseConversionType{ get; set; }
}
