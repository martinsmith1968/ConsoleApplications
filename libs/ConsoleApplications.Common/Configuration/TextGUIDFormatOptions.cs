namespace ConsoleApplications.Common.Configuration;

public class TextGUIDFormatOptions
{
    public string Prefix { get; set; }

    public string Suffix { get; set; }

    public string GUIDFormat { get; set; }

    public  GUIDCaseConversionType CaseConversionType{ get; set; }
}
