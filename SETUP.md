# Setup

This repo is a Unity project skeleton for a native VR web-swinging game,
targeting Meta Quest (standalone, OpenXR). It was assembled outside the
Unity Editor, so a few one-time steps need the Editor GUI itself — that's
normal for VR projects (the XR rig is always a vendor prefab you drag in,
not something you hand-write).

## 1. Install Unity

- Unity Hub, then Editor version **2022.3.50f1** (LTS) — or update
  `ProjectSettings/ProjectVersion.txt` if you use a different LTS.
- In the Hub's module installer, include **Android Build Support**
  (SDK & NDK Tools, OpenJDK).

## 2. Open the project

- Unity Hub → Add → select this repository folder.
- Let the Editor import; Package Manager resolves `Packages/manifest.json`
  (XR Interaction Toolkit, OpenXR, XR Plug-in Management, Input System, URP).
- If a package version fails to resolve, open **Window > Package Manager**
  and update that package to the latest compatible version — the pinned
  versions here were current at time of writing but the registry moves fast.

## 3. Switch to Android + enable XR

- **File > Build Settings > Android > Switch Platform**.
- **Edit > Project Settings > XR Plug-in Management** → install, then under
  the **Android** tab enable **OpenXR**.
- Under **OpenXR** settings, add the **Meta Quest Support** interaction
  profile / feature group.
- **Player Settings** (Android): Minimum API Level 29+, Target Architecture
  ARM64, Graphics API Vulkan, Color Space Linear, Scripting Backend IL2CPP.

## 4. Assign URP

- **Project Settings > Graphics** → if no pipeline asset is set, create one
  via **Assets > Create > Rendering > URP Asset (with Universal Renderer)**
  and assign it.

## 5. Build the demo scene

- Menu **Spider Dud > Build Demo City Scene**.
- This generates `Assets/Scenes/CityDemo.unity` with:
  - A procedurally generated placeholder city (`CityGenerator`, grid of box
    buildings + ground, all on a `Swingable` layer created automatically).
  - A `GameManager`.
  - A `Player` object (CharacterController + `WebSwingController` +
    `PlayerRespawn`) with placeholder Head/LeftHand/RightHand anchor
    transforms and web `LineRenderer`s already wired up.

## 6. Wire in the real XR rig

- **Window > Package Manager > XR Interaction Toolkit > Samples** → import
  **Starter Assets**. This gives you the **XR Origin (XR Rig)** prefab.
- In `CityDemo`, drag that prefab into the scene as a child of `Player`.
- On the `Player`'s `WebSwingController` component, reassign:
  - **Head Anchor** → the XR Origin's `Camera` transform.
  - **Left Hand Anchor** → the XR Origin's left controller transform.
  - **Right Hand Anchor** → the XR Origin's right controller transform.
- Move each `LineRenderer` (currently on the placeholder hand objects) onto
  the corresponding real controller transform, or just reassign the
  `Left Web Line` / `Right Web Line` fields to new `LineRenderer`s added to
  the real controllers.
- Delete the placeholder `HeadAnchor` / `LeftHandAnchor` / `RightHandAnchor`
  objects once the real ones are wired in.

## 7. Play

- Grip button on either controller, aiming the controller at a building,
  fires a web line and starts a pendulum-style swing from that anchor.
  Release to let go (keeps your momentum, with a small boost).
- Playmode in the Editor without a headset won't show much useful motion —
  deploy to a Quest (**Build & Run** over USB with Developer Mode enabled)
  or test over Meta Quest Link for a PCVR-style pass.

## Trademark note

This scaffold implements a generic web-swinging *mechanic* and a
procedurally generated placeholder city — no Marvel/Spider-Man characters,
likenesses, logos, or licensed assets are included. Bring your own
original (or properly licensed) character model, animations, and city art
before shipping this publicly.
