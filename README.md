# spider-dud

A native Unity VR game: web-swinging traversal through a city, built for
Meta Quest (OpenXR).

- **Engine:** Unity 2022.3 LTS
- **XR stack:** OpenXR + XR Interaction Toolkit + XR Plug-in Management
- **Render pipeline:** URP

## What's here

- `Assets/Scripts/Player/WebSwingController.cs` — dual-hand, rope-constraint
  web swinging (raycast to fire, pendulum physics while held, momentum kept
  on release).
- `Assets/Scripts/Player/PlayerRespawn.cs` — teleports the player back to
  spawn if they fall out of the city.
- `Assets/Scripts/World/CityGenerator.cs` — procedurally generates a
  placeholder box-building city to swing through.
- `Assets/Scripts/Core/GameManager.cs` — scene-lifetime singleton hook for
  future game state.
- `Assets/Editor/SpiderDudSceneSetup.cs` — **Spider Dud > Build Demo City
  Scene** menu command that assembles the demo scene via the Editor API.

## Setup

See [SETUP.md](SETUP.md) for the full one-time Editor setup (Android/XR
config, importing the XR Interaction Toolkit rig, building the demo scene).

## Note

This is a generic web-swinging mechanic prototype with placeholder
procedural art — no Marvel/Spider-Man IP is included.
