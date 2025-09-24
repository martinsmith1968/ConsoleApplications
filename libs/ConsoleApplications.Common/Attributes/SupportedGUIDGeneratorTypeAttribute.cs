using ConsoleApplications.Common.Types;

namespace ConsoleApplications.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SupportedGUIDGeneratorTypeAttribute : Attribute
    {
        public GUIDGeneratorType GUIDGeneratorType { get; private set; }

        public SupportedGUIDGeneratorTypeAttribute(GUIDGeneratorType guidGeneratorType)
        {
            GUIDGeneratorType = guidGeneratorType;
        }
    }
}
