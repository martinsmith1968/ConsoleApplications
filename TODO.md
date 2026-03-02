# To Do

## General

### Library Analysis and Selection

- [ ] Determine which Argument parsing library to use
  - SpectreConsole doesn't quite seem to fit the bill
  - Consider:
    - Discussion: https://softwarerecs.stackexchange.com/questions/76953/which-c-command-line-parser-library-should-i-use
    - https://github.com/natemcmaster/CommandLineUtils
    - https://github.com/Tyrrrz/CliFx
    - https://github.com/bilal-fazlani/commanddotnet
    - https://github.com/commandlineparser/commandline
    - Discussion: https://www.reddit.com/r/csharp/comments/shifxh/whats_your_favorite_command_line_arg_parser/

### Fixes

- [ ] Spectre Console option ordering
  - --ignore-default / local need to sort to the bottom of user options
  - `-?`, `-@`, `--help` need to sort to the bottom of all options
    - NOTE: Can override `GetOptions`, but needs reimplementing as it does a Fetch and then a render, all using internal / private classes
- [ ] Implement --ignore-default / local
- [ ] Help grid is always padded to 100% width

### Enhancements

