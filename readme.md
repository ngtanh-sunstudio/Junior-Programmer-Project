# Junior Programmer Project

A 3D top-down shooter built in Unity. The player controls a tank-inspired fighter jet, survives enemy waves, collects powerups, builds score, and defeats a boss to win.

## What I Learned

### [Lab 2 - New Project with Primitives](https://learn.unity.com/pathway/junior-programmer/unit/basic-gameplay/tutorial/lab-2-new-project-with-primitives?version=6.3)

**New progress**

- **New project for my Personal Project:** created a new Unity project with a Space Shooter theme.
- **Camera positioned and rotated based on project type:** placed the camera above the play area for a top-down space shooter.
- **All key objects in the scene with unique materials:** created the player (blue), three enemy types (3 shades of red), walls (white), projectiles (light blue), and powerups (green) materials.

**New concepts & skills**

- **Primitives:** built the gameplay objects and reusable prefabs from Unity's built-in 3D shapes.
- **Create new materials:** created and applied materials to make each type of gameplay object recognizable.
- **Export Unity packages:** learned to export the Assets folder as a project backup.

### [Lab 3 - Player Control](https://learn.unity.com/pathway/junior-programmer/unit/sound-and-effects/tutorial/lab-3-player-control?version=6.3)

**New progress**

- **Player can move based on user input:** read movement through the [Input System](Assets/InputSystem_Actions.inputactions) in [PlayerController](Assets/Scripts/GameScene/Player/PlayerController.cs) and applied it in [PlayerMovement](Assets/Scripts/GameScene/Player/PlayerMovement.cs).
- **Player movement is constrained to suit the game:** clamped the player's X and Z positions in [PlayerMovement](Assets/Scripts/GameScene/Player/PlayerMovement.cs).

**New concepts & skills**

- **Program in C# independently:** planned and wrote the [player movement](Assets/Scripts/GameScene/Player/PlayerMovement.cs) by researching and adapting previously learned input and movement code.
- **Troubleshoot issues independently:** tested the controls and fixed movement problems by clamping the play area and clearing unwanted Rigidbody drift in [PlayerMovement](Assets/Scripts/GameScene/Player/PlayerMovement.cs).

### [Lab 4 - Basic Gameplay](https://learn.unity.com/pathway/junior-programmer/unit/gameplay-mechanics/tutorial/lab-4-basic-gameplay?version=6.3)

**New functionality**

- **Non-player object prefabs have basic movement:** through [EnemyMovement](Assets/Scripts/GameScene/Enemies/NormalEnemy/EnemyMovement.cs) and [ProjectileController](Assets/Scripts/GameScene/Combat/ProjectileController.cs).
- **Objects are recycled when they leave the screen:** returned off-screen enemies and projectiles to their pools through [EnemyMovement](Assets/Scripts/GameScene/Enemies/NormalEnemy/EnemyMovement.cs) and [ProjectileController](Assets/Scripts/GameScene/Combat/ProjectileController.cs).
- **Collisions between objects are handled appropriately**
- **Objects are spawned at appropriate locations on timed intervals:** used [SpawnManager](Assets/Scripts/GameScene/Managers/SpawnManager.cs) to spawn enemy waves and powerups at randomized positions.

**New concepts & skills**

- **Create basic gameplay independently:** planned how non-player objects should move, collide, and spawn, then implemented those rules in [EnemyMovement](Assets/Scripts/GameScene/Enemies/NormalEnemy/EnemyMovement.cs), [ProjectileController](Assets/Scripts/GameScene/Combat/ProjectileController.cs), and [SpawnManager](Assets/Scripts/GameScene/Managers/SpawnManager.cs).

### [Lab 5 - Swap Out Your Assets](https://learn.unity.com/pathway/junior-programmer/unit/user-interface/tutorial/lab-5-swap-out-your-assets?version=6.3)

**New functionality**

- **Primitive visuals replaced without rewriting gameplay:** the player, enemies, boss, and projectile now use models from Unity's Tanks tutorial assets while retaining the existing movement, health, combat, collision, and pooling components.
- **Moon environment added:** the original primitive arena was replaced with reusable Moon environment prefabs from the Tanks asset set.
- **Projectile presentation updated:** projectiles use the imported shell model and an adapted shell-explosion particle effect while continuing to use the existing projectile and audio scripts.
- **UI typography updated:** menu and gameplay text now use the Exo 2 font included with the Tanks assets.

**New concepts & skills**

- **Art workflow:** import, browse, select, and apply external assets to an existing project.
- **High vs. Low Poly:** model complexity vs. visual quality and performance.
- **Asset Store:** imported starfield backgrounds and Unity's Tanks tutorial assets.
- **Nested Prefabs:** placed model assets under existing gameplay prefab roots so visual changes do not replace scripts, colliders, or serialized references.
- **Material properties:** editable shader values stored in a material asset, such as its color, texture, or surface settings. (Source: [Unity Manual](https://docs.unity3d.com/6000.0/Documentation/Manual/writing-shader-material-properties.html))

### Extras

- **Difficulty progression:** learned to combine timed method calls, weighted random selection, and interpolation to grow enemy waves in [SpawnManager](Assets/Scripts/GameScene/Managers/SpawnManager.cs).
- **Boss encounter:** split a complex enemy into orchestration, health, movement, projectile volleys, warning-zone overlap checks, and event-driven audio using [BossController](Assets/Scripts/GameScene/Enemies/Boss/BossController.cs), [BossHealth](Assets/Scripts/GameScene/Enemies/Boss/BossHealth.cs), [BossMovement](Assets/Scripts/GameScene/Enemies/Boss/BossMovement.cs), [BossWeapon](Assets/Scripts/GameScene/Enemies/Boss/BossWeapon.cs), [BossZoneAttack](Assets/Scripts/GameScene/Enemies/Boss/BossZoneAttack.cs), and [BossAudio](Assets/Scripts/GameScene/Enemies/Boss/BossAudio.cs).
- **Reusable powerups:** learned to store effect data and behavior in [ScriptableObjects](Assets/Scripts/GameScene/Powerups/PowerupEffect.cs), collect them through [PowerupItem](Assets/Scripts/GameScene/Powerups/PowerupItem.cs), and implement [speed](Assets/Scripts/GameScene/Powerups/SpeedBoost.cs), [shield](Assets/Scripts/GameScene/Powerups/Shield.cs), and [multifire](Assets/Scripts/GameScene/Powerups/Multifire.cs) effects.
- **Object pooling:** learned to pre-create and reuse typed projectile and enemy pools with [PoolManager](Assets/Scripts/GameScene/Infrastructure/PoolManager.cs) and [PoolType](Assets/Scripts/GameScene/Infrastructure/PoolType.cs), shared by [SpawnManager](Assets/Scripts/GameScene/Managers/SpawnManager.cs), [PlayerWeapon](Assets/Scripts/GameScene/Player/PlayerWeapon.cs), [BossWeapon](Assets/Scripts/GameScene/Enemies/Boss/BossWeapon.cs), and [ProjectileController](Assets/Scripts/GameScene/Combat/ProjectileController.cs).
- **Game state and UI:** learned to coordinate pause, restart, game-over, and win states in [GameManager](Assets/Scripts/GameScene/Managers/GameManager.cs), with focused [score](Assets/Scripts/GameScene/UI/ScoreKeeper.cs), [world-space health bar](Assets/Scripts/GameScene/UI/WorldHealthBar.cs), and [camera-facing UI](Assets/Scripts/GameScene/UI/FaceCamera.cs) components.
- **Audio and effects:** learned to keep playback centralized in [AudioManager](Assets/Scripts/GameScene/Managers/AudioManager.cs) while gameplay events trigger sounds through [PlayerAudio](Assets/Scripts/GameScene/Player/PlayerAudio.cs), [EnemyAudio](Assets/Scripts/GameScene/Enemies/NormalEnemy/EnemyAudio.cs), [BossAudio](Assets/Scripts/GameScene/Enemies/Boss/BossAudio.cs), and [ProjectileAudio](Assets/Scripts/GameScene/Combat/ProjectileAudio.cs); hit and death particles are spawned by the related health and projectile components.
- **Scene-owned UI buttons:** replaced the shared UI button prefab with independently editable scene objects. [UIButtonSoundEmitter](Assets/Scripts/UI/UIButtonSoundEmitter.cs) gives each button a `Default`, `Confirm`, or `Back` sound while [AudioManager](Assets/Scripts/GameScene/Managers/AudioManager.cs) remains responsible for playback.
- **Menus and presentation:** connected scene loading and animated settings panels in [MenuController](Assets/Scripts/MainMenuScene/MenuController.cs), then added the looping background through [ParallaxBackground](Assets/Scripts/MainMenuScene/ParallaxBackground.cs).

For the main development decisions, refactors, and lessons learned, see [design.md](design.md).

## Project Structure

```text
Junior Programmer Project/
|-- Assets/
|   |-- Animations/
|   |   |-- GameUI/
|   |   |-- MenuUI/
|   |   `-- Player/
|   |-- Asset Packs/
|   |   |-- Background Music/
|   |   |-- Sci-fi Sounds/
|   |   |-- Starfield Background/
|   |   |-- Starfield Layered Background/
|   |   `-- _Tanks/
|   |-- Materials/
|   |-- Particles/
|   |-- Powerups/
|   |-- Prefabs/
|   |-- Scenes/
|   |   |-- Main Menu.unity
|   |   `-- Game.unity
|   `-- Scripts/
|       |-- GameScene/
|       |   |-- Combat/
|       |   |-- Enemies/
|       |   |   |-- Boss/
|       |   |   `-- NormalEnemy/
|       |   |-- Infrastructure/
|       |   |-- Input/
|       |   |-- Managers/
|       |   |-- Player/
|       |   |-- Powerups/
|       |   `-- UI/
|       |-- MainMenuScene/
|       |-- UI/
|       `-- Utilities/
|-- Packages/
|-- ProjectSettings/
|-- Screenshots/
|-- design.md
`-- readme.md
```

## Gameplay Flow

![Gameplay flowchart](Screenshots/Flowchart.drawio.png)

Editable source: [Screenshots/Flowchart.drawio](Screenshots/Flowchart.drawio)

## Third-Party Assets

- **Tanks! — Make a Battle Game for Web and Mobile:** Unity Technologies' tutorial assets provide the tank and shell models, Moon environment prefabs and materials, and shell-explosion visual effect used by this project. See the local [asset documentation](<Assets/Asset Packs/_Tanks/Documentation.txt>) and [third-party notice](<Assets/Asset Packs/_Tanks/Tanks!_Third-PartyNotice.txt>). The asset package is governed by the Unity Asset Store EULA except where its notice specifies another license.

## Screenshots

### Main Menu

![Main menu](Screenshots/main-menu.png)

### Gameplay Start

![Gameplay start](Screenshots/gameplay-start.png)

### Boss Fight

![Boss fight](Screenshots/gameplay-boss_fight.png)

### Paused

![Paused](Screenshots/paused.png)

### Win Screen

![Win screen](Screenshots/win.png)

## Gameplay Video

[Watch the gameplay demo](https://www.dropbox.com/scl/fi/r9yol8jy907zp8nfgmzeu/Demo_02.mp4?rlkey=jwlvd8j8lk2cw0p8or7sxurfn&st=lkjcvlit&dl=0)

## Play Online

[Unity Play](https://play.unity.com/en/games/672f0ea6-bed0-45af-8355-194055eea3ad/junior-programmer-project)

## How to Run Locally

1. Open the project in Unity `6000.3.9f1` or a compatible Unity 6 editor.
2. Open `Assets/Scenes/Main Menu.unity`.
3. Press Play.
4. Use the main menu Play button to start the game.
