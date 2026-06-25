# Stopwatch Application (C#)

A Windows Forms stopwatch built in C#. It displays elapsed time in
`hh:mm:ss` format and supports Start, Pause, Resume, Reset, and Stop.

## How it works

The app is split into two parts:

- **StopwatchEngine** (`stopwatchcode.cs`) — the logic. It keeps a running
  count of elapsed seconds using a 1-second timer, tracks the current state
  (Idle, Running, Paused, Stopped), and formats the time as `hh:mm:ss`. It
  raises a `TimerTick` event every second and a `StateChanged` event whenever
  the state changes.
- **StopwatchForm** (`StopwatchForm.cs`) — the interface. It shows the time and
  the five buttons, subscribes to the engine's events to update the display,
  and forwards each button click to the engine.

### Buttons

- **Start** — begins timing from `00:00:00`.
- **Pause** — pauses timing and shows the current time.
- **Resume** — continues from the paused time.
- **Reset** — returns the display to `00:00:00`.
- **Stop** — stops timing and shows the last recorded time.

Buttons are enabled or disabled based on the current state, so only valid
actions are available (for example, Resume is only clickable while paused).

## Requirements

- Windows 10 or 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

> If you have an older SDK, open `StopwatchApp.Core.csproj` and change
> `net10.0-windows` to the version you have (e.g. `net8.0-windows`). The code
> does not change.

## How to run

1. Open a terminal in the `stopwatchcode` folder (the folder containing
   `StopwatchApp.Core.csproj`).
2. Run:

   ```bash
   dotnet run
   ```

The stopwatch window will open.

## Team

- Divine Ikirezi
- Emmanuella Ikirezi
- Darcy Mbanza
