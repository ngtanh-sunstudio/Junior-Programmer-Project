# Junior Programmer Project

A 3D space shooter built in Unity. The player survives enemy waves, collects powerups, builds score, and defeats a boss to win.

## What I Learned

### [Lab 2 - New Project with Primitives](https://learn.unity.com/pathway/junior-programmer/unit/basic-gameplay/tutorial/lab-2-new-project-with-primitives?version=6.3)

- Set up and organize the menu and gameplay scenes.
- Block out the player, enemies, walls, boss, and projectiles as reusable prefabs.
- Use simple materials to distinguish objects before adding visual polish.
- Position the camera and gameplay boundaries for a top-down space shooter.

### [Lab 3 - Player Control](https://learn.unity.com/pathway/junior-programmer/unit/sound-and-effects/tutorial/lab-3-player-control?version=6.3)

- Read move, fire, and pause actions through the [Input System](Assets/InputSystem_Actions.inputactions) in [PlayerController](Assets/Scripts/GameScene/Player/PlayerController.cs).
- Apply frame-rate-independent movement and clamp the player inside the arena in [PlayerMovement](Assets/Scripts/GameScene/Player/PlayerMovement.cs).
- Separate firing, cooldowns, animation, and projectile creation into [PlayerWeapon](Assets/Scripts/GameScene/Player/PlayerWeapon.cs).
- Keep player responsibilities modular through [PlayerHealth](Assets/Scripts/GameScene/Player/PlayerHealth.cs), [PlayerPowerUpManager](Assets/Scripts/GameScene/Player/PlayerPowerUpManager.cs), and [PlayerAudio](Assets/Scripts/GameScene/Player/PlayerAudio.cs).

### [Lab 4 - Basic Gameplay](https://learn.unity.com/pathway/junior-programmer/unit/gameplay-mechanics/tutorial/lab-4-basic-gameplay?version=6.3)

- Implement movement, health, scoring, collisions, and off-screen cleanup in [EnemyController](Assets/Scripts/GameScene/Enemies/EnemyController.cs).
- Route projectile movement and damage by its owner in [ProjectileController](Assets/Scripts/GameScene/Combat/ProjectileController.cs).
- Spawn reusable enemy and powerup prefabs at timed, randomized positions through [SpawnManager](Assets/Scripts/GameScene/Core/SpawnManager.cs).
- Connect combat to score and game outcomes through [ScoreKeeper](Assets/Scripts/GameScene/UI/ScoreKeeper.cs) and [GameManager](Assets/Scripts/GameScene/Core/GameManager.cs).

### [Lab 5 - Swap Out Your Assets](https://learn.unity.com/pathway/junior-programmer/unit/user-interface/tutorial/lab-5-swap-out-your-assets?version=6.3)

- Import and select starfield assets for the main menu and gameplay background.
- Fit the background images to their canvases while keeping UI scaling responsive in both scenes.
- Create a continuous scrolling menu background by duplicating and wrapping UI images in [ParallaxBackground](Assets/Scripts/MainMenuScene/ParallaxBackground.cs).
- Keep the primitive gameplay prefabs; this project only applies the lab's asset-replacement workflow to its backgrounds.

### Extras

- **Difficulty progression:** learned to combine timed method calls, weighted random selection, and interpolation to grow enemy waves in [SpawnManager](Assets/Scripts/GameScene/Core/SpawnManager.cs).
- **Boss encounter:** split a complex enemy into orchestration, health, movement, projectile volleys, warning-zone overlap checks, and event-driven audio using [BossController](Assets/Scripts/GameScene/Enemies/BossController.cs), [BossHealth](Assets/Scripts/GameScene/Enemies/BossHealth.cs), [BossMovement](Assets/Scripts/GameScene/Enemies/BossMovement.cs), [BossWeapon](Assets/Scripts/GameScene/Enemies/BossWeapon.cs), [BossZoneAttack](Assets/Scripts/GameScene/Enemies/BossZoneAttack.cs), and [BossAudio](Assets/Scripts/GameScene/Enemies/BossAudio.cs).
- **Reusable powerups:** learned to store effect data and behavior in [ScriptableObjects](Assets/Scripts/GameScene/Powerups/PowerupEffect.cs), collect them through [PowerupItem](Assets/Scripts/GameScene/Powerups/PowerupItem.cs), and implement [speed](Assets/Scripts/GameScene/Powerups/SpeedBoost.cs), [shield](Assets/Scripts/GameScene/Powerups/Shield.cs), and [multifire](Assets/Scripts/GameScene/Powerups/Multifire.cs) effects.
- **Object pooling:** learned to pre-create and reuse projectiles with [ObjectPool](Assets/Scripts/GameScene/Infrastructure/ObjectPool.cs), shared by [PlayerWeapon](Assets/Scripts/GameScene/Player/PlayerWeapon.cs), [BossWeapon](Assets/Scripts/GameScene/Enemies/BossWeapon.cs), and [ProjectileController](Assets/Scripts/GameScene/Combat/ProjectileController.cs).
- **Game state and UI:** learned to coordinate pause, restart, game-over, and win states in [GameManager](Assets/Scripts/GameScene/Core/GameManager.cs), with focused [score](Assets/Scripts/GameScene/UI/ScoreKeeper.cs), [health bar](Assets/Scripts/GameScene/UI/HealthBar.cs), and [camera-facing UI](Assets/Scripts/GameScene/UI/FaceCamera.cs) components.
- **Audio and effects:** learned to keep playback centralized in [AudioManager](Assets/Scripts/GameScene/Core/AudioManager.cs) while gameplay events trigger sounds through [PlayerAudio](Assets/Scripts/GameScene/Player/PlayerAudio.cs), [EnemyAudio](Assets/Scripts/GameScene/Enemies/EnemyAudio.cs), [BossAudio](Assets/Scripts/GameScene/Enemies/BossAudio.cs), and [ProjectileAudio](Assets/Scripts/GameScene/Combat/ProjectileAudio.cs); hit and death particles are spawned by the related health and projectile components.
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
|   |   `-- Starfield Layered Background/
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
|       |   |-- Core/
|       |   |-- Enemies/
|       |   |-- Infrastructure/
|       |   |-- Input/
|       |   |-- Player/
|       |   |-- Powerups/
|       |   `-- UI/
|       `-- MainMenuScene/
|-- Packages/
|-- ProjectSettings/
|-- Screenshots/
|-- design.md
`-- readme.md
```

## Gameplay Flow

![Gameplay flowchart](Screenshots/Flowchart.drawio.png)

Editable source: [Screenshots/Flowchart.drawio](Screenshots/Flowchart.drawio)

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
