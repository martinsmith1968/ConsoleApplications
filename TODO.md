# To Do

## General

### Library Evaluation and Selection

- [ ] Determine which Argument parsing library to use
  - SpectreConsole doesn't quite seem to fit the bill
  - Consider:
    - Discussion: https://softwarerecs.stackexchange.com/questions/76953/which-c-command-line-parser-library-should-i-use
    - https://github.com/natemcmaster/CommandLineUtils
    - https://github.com/Tyrrrz/CliFx
    - https://github.com/bilal-fazlani/commanddotnet
    - https://github.com/commandlineparser/commandline
    - Discussion: https://www.reddit.com/r/csharp/comments/shifxh/whats_your_favorite_command_line_arg_parser/
  - Requirements
    - Simple mechanism for structuring single command arguments
      - Prefer Attributes, Fluent builder API aligns with CPP project
    - Supports multiple commands easily in structure separated way
    - Argument parsing from `main` is straightforward, simple and easy to read
      - Supports exception handling with output control and optional usage output
    - Help output aligns with CPP, or is configurable to be output that way
      - Can be invoked via `-?` or `--help`
    - Supports (or can be made to support) argument / option files
      - Supports `-@` and/or `-$` for options file overriding (as per CPP)
    - Supports version output via argument
      - CPP uses `-!`
    - Boolean options can be negated (when default is `True`)
      - E.g. `-u Uppercase` with a default `True` can be specified as `-u-` to force Lowercase

#### Evaluation

- Single Command structure
  - Accept:
    - string[]
    - Enum
    -

##### Ookii.Commandline

- Rating: **40%**
- [Notes](spikes/Ookii.CommandLine.Spike.SingleCommand/notes.md)

##### SpectreConsole.cli

##### CommandlineParser

- Rating: **10%**
- [Notes](spikes/CommandLineParser.Spike.SingleCommand/notes.md)

##### CommandLineUtils (NateMcMaster)

##### CliFx

##### CommandDotNet (BilalFazlani)

##### System.Commandline

### Fixes

- [ ] Spectre Console option ordering
  - --ignore-default / local need to sort to the bottom of user options
  - `-?`, `-@`, `--help` need to sort to the bottom of all options
    - NOTE: Can override `GetOptions`, but needs reimplementing as it does a Fetch and then a render, all using internal / private classes
- [ ] Implement --ignore-default / local
- [ ] Help grid is always padded to 100% width

### Enhancements

- [ ] Add legacy apps
  - [ ] TaskbarAlert
  - [ ] PEInfo
