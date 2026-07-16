# Building Reviver — Cities: Skylines II mod

Automatically revives abandoned buildings: clears the status, resets condition, and puts the property back on the market. Toggleable in Options → Mods → Building Reviver, with a master switch, adjustable sweep frequency, and a session counter (with a reset button).

The philosophical opposite of Auto Bulldozer: instead of tearing derelict buildings down, it gives them a second chance. Don't run both on the same city unless you want them fighting over the same buildings.

## Prerequisites (one-time)

1. Windows with Cities: Skylines II installed.
2. Visual Studio 2022 (free Community edition works) with the ".NET desktop development" workload.
3. Install the CS2 modding toolchain: launch the game, go to **Options → Modding** (or via the Paradox launcher's game settings) and download/install all modding toolsets. This sets the `CSII_TOOLPATH` environment variable that this project's `.csproj` relies on. Restart Visual Studio afterwards so it picks up the variable.

## Build

Either run `.\build.ps1` from the project folder (needs only the .NET SDK — no Visual Studio; the script sets `DOTNET_ROLL_FORWARD=LatestMajor` because the toolchain's post-processor targets .NET 6), or:

1. Open `BuildingReviver.sln` in Visual Studio.
2. Set configuration to **Release** and build (Ctrl+Shift+B).

Either way, the toolchain automatically copies the built mod to `%USERPROFILE%\AppData\LocalLow\Colossal Order\Cities Skylines II\Mods\BuildingReviver\`.

If the build fails with missing references or import errors, `CSII_TOOLPATH` isn't set — redo step 3 of the prerequisites.

## Test in-game

1. Launch CS2 and load a city (or start one).
2. Check **Options → Mods → Building Reviver** — you should see the toggles.
3. Let the simulation run. Abandoned buildings are revived a few times per in-game day; watch the "Buildings revived this session" counter climb. The log is at `%USERPROFILE%\AppData\LocalLow\Colossal Order\Cities Skylines II\Logs\BuildingReviver.log`.

## Publish to Paradox Mods (PDX Mods)

Same flow as AutoBulldozer (see its `SHIPPING.md` for the detailed guide):

1. Add a thumbnail: place a square PNG (at least 256×256) at `Properties/Thumbnail.png`. Optionally add screenshots and reference them in `Properties/PublishConfiguration.xml`.
2. Review `Properties/PublishConfiguration.xml` — display name, descriptions, tags, version.
3. In Visual Studio, right-click the **BuildingReviver** project → **Publish New Mod**. Log in with your Paradox account the first time.
4. After the first publish, the toolchain writes your `ModId` into `PublishConfiguration.xml`. Keep it — future updates use **Publish New Version** (bump `ModVersion` and update `ChangeLog` first).

## How it works

`BuildingReviverSystem` is an ECS system that runs during the simulation phase (1–64 sweeps per in-game day, configurable, default 16; no cost while paused or in menus). It queries buildings tagged `Abandoned` (excluding destroyed buildings, temporary tool previews, and already-deleted entities) and revives them: removes the `Abandoned` component and resets a negative `BuildingCondition` to zero, so the vanilla condition system doesn't immediately re-abandon it. An `Updated` tag nudges the game to refresh the building's visuals and state.

Re-listing the property is left to the game. `Game.Simulation.PropertyRenterSystem` markets any building that is neither abandoned nor destroyed and has free capacity, so clearing `Abandoned` is enough — and it applies guards (signature buildings, spare capacity) that this mod would otherwise have to duplicate.

Because it only removes/adds vanilla components, it's save-safe and can be added or removed from a save at any time. Buildings whose root problems remain (low land value, missing services, high rent) can relapse and will simply be revived again on a later sweep.

## Why condemned buildings aren't handled

An earlier version tried to rescue condemned buildings by removing the `Condemned` component. Decompiling the game shows this can't work. `Game.Buildings.ZoneCheckSystem` validates every building against the zone blocks under it; if validation fails it adds `Condemned`, and if validation passes **it removes `Condemned` itself**. So when the zoning is valid the game already un-condemns the building, and when it's invalid the game re-condemns it on the next pass — a loop the mod can't win. Condemnation is a zoning problem, and the fix is to correct the zoning.

## Files

- `Mod.cs` — entry point (`IMod`): loads settings, registers options UI and locale, schedules the system
- `Setting.cs` — options UI definition and persisted settings
- `LocaleEN.cs` — English strings for the options UI
- `BuildingReviverSystem.cs` — the actual revival logic
- `Properties/PublishConfiguration.xml` — PDX Mods listing metadata
