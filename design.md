# Design Notes

This document contains the detailed implementation notes and tuning values for the Unity space shooter. The README stays focused on the project showcase, while this file records how the game systems are built.

## Engine and Packages

- Unity version: `6000.3.9f1`
- Render pipeline: Universal Render Pipeline `17.3.0`
- Input: Unity Input System `1.18.0`
- UI: Unity UI / uGUI and TextMesh Pro

Enabled build scenes:

1. `Assets/Scenes/Main Menu.unity`
2. `Assets/Scenes/Game.unity`

## Controls

Gameplay controls used by scripts:

| Action | Keyboard / Mouse | Gamepad |
| --- | --- | --- |
| Move | WASD or arrow keys | Left stick |
| Fire | Left mouse button or Enter | West face button |
| Pause | Escape | UI cancel bindings |

The input asset also includes extra bindings for look, interact, crouch, jump, previous, next, sprint, and UI navigation. Those actions are available in the generated input class, but not all of them are currently used by gameplay code.

## Main Menu

Implemented features:

- Title screen with Play, Settings, and Quit buttons.
- Settings overlay with Music and SFX sliders.
- Persistent music and SFX volume controls.
- Automatic click SFX registration for scene buttons.
- Animated transitions between menu and settings panels.
- Layered parallax starfield background.

Scripts:

- `MenuController` loads the game scene, quits the application, and toggles settings animation state.
- `ParallaxBackground` duplicates a stretched UI image and scrolls both copies to create a looping background.
- `AudioManager` persists between scenes, selects scene music, and owns the shared music and SFX audio sources.

## Gameplay Scene

The gameplay scene contains:

- Player prefab.
- Spawn manager.
- Game manager.
- Score keeper.
- Main camera and lighting.
- Side walls.
- UI canvas for score, pause, game over, and win screens.

Player movement bounds:

| Axis | Range |
| --- | --- |
| X | `-15` to `15` |
| Z | `0` to `15` |

Enemies spawn ahead of the player and move backward through the playfield. Projectiles and enemies clean themselves up when they leave the configured Z range.

## Player

The player uses focused components rather than one controller containing every responsibility:

| Script | Responsibility |
| --- | --- |
| `PlayerController` | Input and high-level orchestration |
| `PlayerMovement` | Movement, bounds, speed multiplier, and physics drift |
| `PlayerWeapon` | Projectile spawning, cooldown, multifire, and firing events |
| `PlayerHealth` | Health, collision handling, shield consumption, and death events |
| `PlayerPowerUpManager` | Active powerup duration and cancellation |
| `PlayerAudio` | Subscribes to player events and requests SFX playback |

Implemented behavior:

- Reads movement, fire, and pause input.
- Moves with transform translation.
- Fires projectiles with a cooldown.
- Plays firing animation states.
- Tracks health and updates a world-space health bar.
- Supports shield, speed boost, and multifire states.
- Stops physics drift after collisions.
- Triggers game over when health reaches zero.

Current player prefab tuning:

| Setting | Value |
| --- | --- |
| Max health | `10` |
| Move speed | `15` |
| Projectile damage | `1` |
| Fire cooldown | `0.1` seconds |
| Multifire bullet count | `3` |
| Multifire spread angle | `15` degrees |

Collision behavior:

- Normal enemy collision damages the player and routes the enemy through its standard death pipeline.
- Boss collision damages both the player and boss.
- Shield blocks one damage event and then turns off.
- Consuming a shield raises an event that cancels its remaining duration coroutine.

## Projectiles

Script: `ProjectileController`

Implemented behavior:

- Moves along local `Vector3.up`.
- Destroys itself outside the Z range.
- Stores an owner so player and boss projectiles damage different targets.
- Player projectiles damage enemies and bosses.
- Boss projectiles damage the player.
- Raises a `Hit` event after a valid impact.
- `ProjectileAudio` subscribes to `Hit` and plays the impact clip through `AudioManager`.

Current projectile prefab tuning:

| Setting | Value |
| --- | --- |
| Speed | `15` |
| Z range | `30` |
| Base damage | `1` |

## Enemy Waves

Script: `SpawnManager`

Spawn systems:

- Enemy waves start after `3` seconds and repeat every `3` seconds.
- Powerups start after `5` seconds and repeat every `10` seconds.
- Boss spawns after the difficulty ramp duration.

Wave scaling:

| Setting | Value |
| --- | --- |
| Starting enemies per wave | `1` |
| Max enemies per wave | `10` |
| Time to max enemies | `180` seconds |
| Enemy weight stage duration | `15` seconds |

Enemy weight stages:

| Stage | Enemy 1 | Enemy 2 | Enemy 3 |
| --- | ---: | ---: | ---: |
| 1 | 100% | 0% | 0% |
| 2 | 75% | 25% | 0% |
| 3 | 25% | 50% | 25% |
| 4 | 10% | 40% | 50% |
| 5 | 10% | 30% | 60% |

Enemy prefab tuning:

| Enemy | Speed | Health | Score |
| --- | ---: | ---: | ---: |
| `Enemy_1` | `5` | `3` | `10` |
| `Enemy_2` | `4` | `5` | `25` |
| `Enemy_3` | `3` | `7` | `50` |

Each enemy has a health bar, a score value, forward movement, and out-of-bounds cleanup. Enemy death is routed through `EnemyController.Die()`, which guards against duplicate processing, awards score, raises `Died`, and destroys the object. `EnemyAudio` subscribes to `Died`.

## Boss Encounter

Boss behavior is split by responsibility:

| Script | Responsibility |
| --- | --- |
| `BossController` | Initialization, score reward, and win orchestration |
| `BossMovement` | Entry movement and side-to-side targeting |
| `BossWeapon` | Volley sequencing, projectile spawning, and firing events |
| `BossHealth` | Health state and death event |
| `BossAudio` | Boss firing and death SFX |

Implemented behavior:

- Moves from spawn position to battle position.
- Moves side to side after reaching battle position.
- Stops to fire spread volleys.
- Awards score on defeat.
- Triggers the win screen when defeated.

Current boss prefab tuning:

| Setting | Value |
| --- | --- |
| Health | `100` |
| Speed | `15` |
| Battle Z position | `15` |
| Horizontal range | `15` |
| Delay between movement targets | `3` seconds |
| Bullets per volley | `3` |
| Volleys per firing sequence | `5` |
| Spread angle | `15` degrees |
| Fire cooldown | `0.1` seconds |
| Projectile damage | `1` |
| Score value | `500` |

Current limitation:

- Planned red-zone attack is not implemented yet.

## Powerups

Powerups are implemented with ScriptableObjects.

Core files:

| Script | Purpose |
| --- | --- |
| `PowerupEffect` | Base ScriptableObject type for powerup data and apply/remove behavior |
| `PowerupItem` | Trigger pickup behavior |
| `PlayerPowerUpManager` | Tracks active effects and refreshes duration |
| `SpeedBoost` | Applies movement speed multiplier |
| `Shield` | Enables one-hit shield protection |
| `Multifire` | Enables spread firing |

`PlayerPowerUpManager` owns effect duration coroutines. When a shield is consumed early, `PlayerHealth` raises `ShieldConsumed`; the manager subscribes to that event and stops/removes the active shield coroutine.

Powerup tuning:

| Powerup | Duration | Effect |
| --- | ---: | --- |
| Speed Boost | `10` seconds | Player speed multiplier becomes `2` |
| Shield | `5` seconds | Blocks one hit |
| Multifire | `10` seconds | Enables three-shot spread |

Powerup spawn settings:

- Spawn Z range: `2` to `14`
- Initial delay: `5` seconds
- Repeat interval: `10` seconds

## UI and Game State

Script: `GameManager`

Game states:

- Playing
- Paused
- Game over
- Win

Implemented behavior:

- Tracks whether the game is active.
- Pauses and resumes with `Time.timeScale`.
- Uses animator states for pause/resume transitions.
- Stops spawning on game over or win.
- Displays final score.
- Restarts current scene.
- Returns to main menu.

UI helper scripts:

| Script | Purpose |
| --- | --- |
| `ScoreKeeper` | Tracks score and updates TextMesh Pro text |
| `HealthBar` | Wraps a Unity UI Slider for health display |
| `FaceCamera` | Keeps world-space health bars facing the camera |

## Art, Audio, and Presentation

Imported asset groups currently used or kept in the project:

- Starfield background assets.
- Layered starfield background assets for the menu.
- Background music tracks.
- Sci-fi sound effect files.
- TextMesh Pro fonts, shaders, and settings.

### Audio Architecture

Audio uses a hybrid centralized/event-driven design.

`AudioManager` is the persistent playback service:

- Uses separate `AudioSource` components for music and SFX.
- Loops title and gameplay BGM and switches tracks based on the loaded scene.
- Plays ending music on win or game over.
- Applies Music and SFX slider values immediately.
- Registers one shared click listener for all scene buttons, including initially inactive buttons.
- Exposes `PlaySFX(AudioClip)` so gameplay audio components do not manage their own AudioSources.

Gameplay systems publish events without depending directly on audio clips:

| Publisher | Event | Subscriber |
| --- | --- | --- |
| `PlayerHealth` | `Died`, `Shielded` | `PlayerAudio` |
| `PlayerMovement` | `SpedUp` | `PlayerAudio` |
| `PlayerWeapon` | `Fired`, `Multifire` | `PlayerAudio` |
| `EnemyController` | `Died` | `EnemyAudio` |
| `BossHealth` | `Died` | `BossAudio` |
| `BossWeapon` | `Fired` | `BossAudio` |
| `ProjectileController` | `Hit` | `ProjectileAudio` |

Subscribers register in `OnEnable` and unregister in `OnDisable`. The publisher decides when a gameplay event occurred; the audio component decides which clip represents it. Because clips play through the persistent `AudioManager`, sounds can finish after the originating projectile, enemy, boss, or player object is destroyed.

Both build scenes contain an AudioManager so either scene can be tested directly. During normal scene transitions, the persistent instance keeps running and duplicate scene instances destroy themselves.

Custom materials:

- Boss material.
- Wall material.
- Shield material.
- Red, red dark, and red light materials.
- Blue and blue light materials.
- Green material.

Animation folders:

- `Animations/Player` for idle and firing states.
- `Animations/GameUI` for playing and paused UI states.
- `Animations/MenuUI` for menu and settings panel transitions.

## Script Map

| Script | Purpose |
| --- | --- |
| `GameManager` | Overall game state, pause, restart, quit, win, game over |
| `AudioManager` | Persistent BGM/SFX playback, volume controls, and shared button sounds |
| `PlayerController` | Player input and high-level orchestration |
| `PlayerMovement` | Movement, bounds, and speed state |
| `PlayerWeapon` | Shooting, multifire, and firing events |
| `PlayerHealth` | Health, shields, collisions, and death events |
| `PlayerAudio` | Player event-to-SFX adapter |
| `SpawnManager` | Enemy waves, boss spawning, powerup spawning, difficulty ramp |
| `EnemyController` | Enemy movement, health, scoring, death event, cleanup |
| `EnemyAudio` | Enemy death SFX |
| `BossController` | Boss initialization, scoring, and win condition |
| `BossMovement` | Boss movement state |
| `BossWeapon` | Boss projectile volleys and firing event |
| `BossHealth` | Boss health and death event |
| `BossAudio` | Boss firing and death SFX |
| `ProjectileController` | Projectile movement and damage routing |
| `ProjectileAudio` | Projectile impact SFX |
| `PlayerPowerUpManager` | Active powerup duration tracking |
| `PowerupEffect` | Base ScriptableObject type for powerups |
| `PowerupItem` | Pickup trigger behavior |
| `SpeedBoost` | Speed boost implementation |
| `Shield` | Shield implementation |
| `Multifire` | Multifire implementation |
| `ScoreKeeper` | Score state and score text updates |
| `HealthBar` | Slider-based health display |
| `FaceCamera` | Camera-facing world-space UI |
| `MenuController` | Main menu button behavior |
| `ParallaxBackground` | Infinite scrolling UI background |

Generated or placeholder scripts:

- `InputSystem_Actions` is generated by Unity's Input System.
- `ObjectPool` currently exists as a placeholder and does not implement pooling yet.

## Current Status

Completed or mostly complete:

- Player movement and shooting.
- Player, enemy, and boss health.
- Health bar UI.
- Score tracking.
- Main menu UI.
- Pause UI.
- Game-over UI.
- Win UI.
- Enemy wave spawning.
- Difficulty scaling by wave size and enemy weights.
- Three enemy tiers.
- Boss encounter.
- ScriptableObject powerup system.
- Speed, shield, and multifire powerups.
- Menu and player/game UI animations.
- Scene-based looping BGM.
- Music and SFX volume controls.
- Automatic UI button click sounds.
- Event-driven player, enemy, boss, and projectile SFX.

In progress or unfinished:

- Object pooling is planned but not implemented.
- Boss red-zone attack is planned but not implemented.
