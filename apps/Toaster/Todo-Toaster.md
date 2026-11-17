# Toaster – remaining work and potential issues

Status: functional. Text / title / timeout / location / monitor / background + foreground colour / text +
title alignment all work and were smoke-tested (stacking across instances, 4 corners, 3 monitors, colour
parsing, invalid-argument errors, host exit after expiry). The solution builds with 0 errors. Nothing here has
been tested by eye beyond pixel/rect checks.

Arguments (current): `Text` (positional, `-t`), `-Title` (`-ti`), `-TimeoutSeconds` (`-s`),
`-TextAlignment` (`-ta`), `-TitleAlignment` (`-tia`), `-Location` (`-l`), `-Monitor` (`-m`),
`-BackgroundColour` (`-bg`), `-ForegroundColour` (`-fg`, default `Auto`).

**Uncommitted:** foreground colour, alignment options, `TextAlignmentType.cs` and this
file are not yet committed (last commit: `f491501`).

## Not yet verified

- [ ] **Visual check of the animations.** Slide-in, slide-out and the "drop down into the gap" reflow were
      only checked via window rectangles, never watched. Confirm the easing/duration (300ms) feels right.
- [ ] **Text contrast on custom backgrounds (`-fg Auto`).** Text switches to dark when the background is "light"
      (luminance > 160, `ColourParser.IsLight`). The threshold is a guess and ignores alpha. An explicit `-fg`
      colour is used as-is (for both title and text) with no contrast check.
- [ ] **Alignment.** `-ta` / `-tia` (Left / Right / Centre) were run once but the result was not looked at. Check
      wrapped multi-line text, and a title aligned differently from the body.
- [ ] **Mixed-DPI multi-monitor setups.** The app is system-DPI aware (WPF default); monitor work areas are
      divided by the system scale. Tested on 3 monitors, but not with differing scale factors – toasts may be
      mis-sized or mis-placed on a secondary monitor with a different DPI. Fix is probably a per-monitor-v2
      manifest plus per-monitor DPI conversion in `Monitors.cs`.
- [x] **CI.** `ci-build.yml` builds the solution with `-p:Platform=x64` in Release on `windows-latest`. Verified
      locally with the same `dotnet build`/`dotnet test` flags (`-c Release -p:Platform=x64`); Toaster maps to
      `Release|Any CPU` for that platform per the `.sln`, same as it would in CI. Not yet confirmed on an
      actual CI run.
- [ ] **Concurrency stress.** The host hand-off/shutdown race (client connects while the host is closing) is
      handled by an ack ("OK"/"NO") plus client retry, but was only exercised with a handful of launches.
      Try 20+ rapid launches, and launches timed to land at the end of the idle grace period.

## Known gaps / limitations

- [ ] **No cap on stack height.** Enough simultaneous toasts will run off the far edge of the screen. Options:
      cap per stack and queue the rest, or collapse to "+N more".
- [ ] **No maximum toast height.** Very long messages make a very tall card (text wraps, never truncates).
- [ ] **`-s 0` toasts never expire** and keep the host process alive until clicked. There is no "dismiss all"
      or "close existing" command.
- [x] **Sliding toasts overlap the taskbar.** Windows start/finish just outside the work area, so while moving
      they briefly draw over the taskbar. The fade mostly hides it; clipping would fix it properly.
- [ ] **Stack work area is captured once.** A stack remembers the work area from when it was created. If
      monitors/taskbar change while the host is alive (it normally lives only seconds), placement goes stale.
      Stacks are also keyed by monitor *index*, which can change if monitors are added/removed.
- [ ] **Click is the only interaction.** No close button, no hover-to-pause the timeout, no action
      buttons, icon/image, sound, or screen-reader (UI Automation) support.
- [ ] **Focus Assist / Do Not Disturb is ignored.** Toasts always appear.
- [ ] **Multi-placement toasts share one fate.** A toast shown in several corners/monitors expires together and
      clicking any copy dismisses all copies. Probably what you want, but it is a decision.

## Command line / argument handling

- [ ] **Console output from a `WinExe`.** Errors and usage are written via `AttachConsole(-1)`. When run from
      cmd/PowerShell the process returns immediately (GUI app), so output can interleave with the next prompt,
      and the exit code isn't seen unless the caller waits (`start /wait`, `Start-Process -Wait`). `-?` /
      `--help` output has not been tested.
- [ ] **`Arguments.cs` tidy-ups.** `Description` typos: "Monitor for
      where the Toast popup show"; the foreground description says "Message Text" but it also colours the
      title. Ookii warns OCL0040 (use `[GeneratedParser]`).
- [ ] **`Centre` only.** `TextAlignmentType` has `Centre`; `Center` is rejected. Consider accepting both.
- [x] **Unused `using System.Drawing;`** in `Arguments.cs` (harmless).
- [ ] **Colour names.** `KnownColor` also contains *system* colours (e.g. `Control`, `Window`) which resolve to
      the current theme's colour, and `Transparent` gives a fully transparent card. Decide whether to restrict
      to non-system colours.
- [x] **Stale examples (will fail).** Fixed: `-m` is a valid alias again (now `Monitor`, not the old Text
      alias), and `launchSettings.json` / `Toaster.args.json` now exercise every flag, including `-s` and
      `-fg` which neither one touched before. Added a `Toaster` entry to the root `README.md`.

## Protocol / design notes

- Instance coordination: `Local\Toaster.<sessionId>.Host` mutex elects the host; requests go over the
  `Toaster.<sessionId>.Pipe` named pipe (current user only) as one line of JSON, answered with `OK`/`NO`.
- [ ] **No protocol versioning.** If an older host is still running when a newer build is launched, the newer
      fields are ignored (JSON), which is harmless today but would not be for a breaking change. Enums are
      sent as numbers, so reordering `ToastLocationType` / `ToastMonitorType` would break a mixed-version pair.
- [ ] **Per-session only.** The host is per Windows session, so separate RDP/console sessions don't share a stack
      (intended, but worth knowing).

## Tests

- [x] `Toaster.Tests` covers `ColourParser` (names, `#RGB`, `#ARGB`, `#RRGGBB`, `#AARRGGBB`, garbage, gradient
      pairs), `Arguments.Validate` / `ToToastRequest` (including dynamic text, gradient fill and window width),
      `DynamicText` and `Constants.IsAuto` (86 tests). Needed `InternalsVisibleTo` since the types under test
      are `internal`.
- [ ] The stack layout maths (`ToastHost.Reflow`) is still untested; it would need pulling out of the WPF
      types (`ToastWindow` is a real `Window`, needs a UI thread) to be unit-testable.

## Housekeeping noticed elsewhere in the repo

- [ ] `TomlFileHandler.Tests` is still on older test packages (`Microsoft.NET.Test.Sdk` 17.12.0, `xunit` 2.9.2,
      `xunit.runner.visualstudio` 2.8.2) than the other test projects.
- [x] Apps still reference `Microsoft.Extensions.DependencyInjection` 9.0.0 while `ConsoleApplications.Common`
      resolves 10.0.x; Toaster dropped its direct references for this reason (NU1605). The same tidy-up applies to
      the other apps. All in-solution apps now reference `10.0.15`, matching `ConsoleApplications.Common`;
      only `BannerText` and `VarList` (outside the solution, see below) are still on `9.0.0`.
- [ ] `BannerText` and `VarList` were backfilled with settings from the deleted `Directory.Build.props`, and
      `BannerText` needed a `TriState` fix for Ookii 5; both were only built standalone, and they are outside
      the solution.
