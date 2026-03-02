# Ookii.Commandline

## The Good

1. Version output is ok

```
Ookii.CommandLine.Spike.SingleCommand 1.0.0+a44c2905871263394b74184779a523f35a7ea8f6
```


## The Bad

1. Boolean options can't be negated when default is `True`

2. Custom Help output requires re-implementing `UsageWriter` class

```csharp
protected virtual void WriteParserUsageCore(UsageHelpRequest request)
{
    if (request == UsageHelpRequest.None)
    {
        WriteMoreInfoMessage();
        return;
    }

    if (request == UsageHelpRequest.Full && IncludeApplicationDescription && !string.IsNullOrEmpty(Parser.Description))
    {
        WriteApplicationDescription(Parser.Description);
    }

    WriteParserUsageSyntax();
    if (request == UsageHelpRequest.Full)
    {
        if (IncludeValidatorsInDescription)
        {
            WriteClassValidators();
        }

        WriteArgumentDescriptions();
        Writer.Indent = 0;
        WriteParserUsageFooter();
    }
    else
    {
        Writer.Indent = 0;
        WriteMoreInfoMessage();
    }
}
```

3. Default help output for Boolean is incorrect - implies a True option can be turned off

4. No easy way to implement file based options, or to support `-@` and `-$` for options file overriding (as per CPP)

## The Ugly

### Hideous Help Output by Default

```txt
Usage: Ookii.CommandLine.Spike.SingleCommand [--Name] <String>... [--BirthDate <DateTime>] [--Help]
   [--SwitchOff] [--SwitchOn] [--Version]

        --Name <String>
            A required positional argument.

    -d, --BirthDate <DateTime>
            An argument that can only be supplied by name.

    -?, --Help [<Boolean>] (-h)
            Displays this help message.

    -f, --SwitchOff [<Boolean>]
            A switch argument, which doesn't require a value. Default value: True.

    -o, --SwitchOn [<Boolean>]
            A switch argument, which doesn't require a value. Default value: False.

        --Version [<Boolean>]
            Displays version information.

ERROR: Value cannot be null. (Parameter 'arguments')
```
