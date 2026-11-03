# CommandLineParser

## The Good

1. Help show the correct desired header

```
CommandLineParser.Spike.SingleCommand 1.0.0+a44c2905871263394b74184779a523f35a7ea8f6
Copyright (C) 2026 CommandLineParser.Spike.SingleCommand
```

## The Bad

1. Can't parse positional arrays
2. Doesn't support "arrays" - needs to be IEnumerable
3. Throws exceptions for any issue
4. Usage doesn't appear by default (Needs HelpWriter = Console.Out)
5. Really difficult to get going with
6. Not configured for usability out of the box

## The Ugly

1. Output is revolting

```
CommandLineParser.Spike.SingleCommand 1.0.0+a44c2905871263394b74184779a523f35a7ea8f6
Copyright (C) 2026 CommandLineParser.Spike.SingleCommand

ERROR(S):
  Required option 'n' is missing.

  -n           Required.

  -d

  -o           (Default: false)

  -f           (Default: true)

  --help       Display this help screen.

  --version    Display version information.

CommandLine.MissingRequiredOptionError
```
