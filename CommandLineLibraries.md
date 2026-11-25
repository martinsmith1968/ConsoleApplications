# Command-Line Parsing Library Usage

Tracks which command-line parsing library each app uses, as part of the effort to standardize all apps on Spectre.Console.Cli.

## Spectre.Console.Cli

- BannerText
- Calendar
- FigLetText
- GUIDGenerator
- IniFileHandler
- PauseN
- PrintFormat
- SpecialFolders
- Toaster
- VarList

## Ookii.CommandLine (to be converted)

- TomlFileHandler (has two inconsistent entry points — see below; conversion paused pending a decision)

## No CLI parsing (not yet implemented)

- BackgroundTimer — stub app, only prints "Hello, World!"

## TomlFileHandler: two inconsistent entry points

- `Program.cs` + `Configuration/Arguments.cs`: currently wired up, flat Read/Write via Tomlyn.
- `Commands/` folder (`GetCommand`/`SetCommand`/`DeleteCommand`/`ExportCommand`, built on `TommyTOMLFileService`):
  richer (array indices, typed values), matches the saved args.json and the test suite, but never
  actually wired into `Program.cs` — currently dead code.
