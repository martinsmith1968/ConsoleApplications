using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ookii.CommandLine.Spike.SingleCommand.Configuration;

public class MyUsageWriter : UsageWriter
{
    protected override void WriteParserUsageCore(UsageHelpRequest request)
    {
        base.WriteParserUsageCore(request);
    }

    protected override void WriteApplicationDescription(string description)
    {
        base.WriteApplicationDescription(description);
    }
}
