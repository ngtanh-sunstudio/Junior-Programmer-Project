# Development Review

This document reviews how the project changed while I built and tested it. It focuses on the problems I encountered, the decisions I made, and what I learned from improving the first implementation.

## Refactoring the Player

The first version of `PlayerController` was monolithic and handled almost everything: input, movement, shooting, health, collisions, powerups, animation, and pausing. This worked while the game was small, but the script grew beyond 300 lines. Changes became harder to isolate because one class had too many reasons to change.

I kept [PlayerController](Assets/Scripts/GameScene/Player/PlayerController.cs) as the high-level coordinator and separated the other responsibilities:

- [PlayerMovement](Assets/Scripts/GameScene/Player/PlayerMovement.cs) handles movement, arena boundaries, and speed changes.
- [PlayerWeapon](Assets/Scripts/GameScene/Player/PlayerWeapon.cs) handles firing, cooldowns, multifire, and firing animation.
- [PlayerHealth](Assets/Scripts/GameScene/Player/PlayerHealth.cs) handles damage, shields, collisions, and death.
- [PlayerPowerUpManager](Assets/Scripts/GameScene/Player/PlayerPowerUpManager.cs) manages timed effects.
- [PlayerAudio](Assets/Scripts/GameScene/Player/PlayerAudio.cs) reacts to player events and plays sound effects.

This taught me to split components by responsibility instead of continuing to add features to one controller. It also made later work, such as object pooling and event-driven audio, easier to add without rewriting the whole player.

## Refactoring the Boss

The boss started with the same monolithic structure. Its original controller contained movement, health, projectile volleys, scoring, and the win condition in one script. As the encounter became more complex, that structure made each new attack harder to add safely.

I reduced [BossController](Assets/Scripts/GameScene/Enemies/BossController.cs) to orchestration and split the behavior into [BossMovement](Assets/Scripts/GameScene/Enemies/BossMovement.cs), [BossWeapon](Assets/Scripts/GameScene/Enemies/BossWeapon.cs), [BossHealth](Assets/Scripts/GameScene/Enemies/BossHealth.cs), and [BossAudio](Assets/Scripts/GameScene/Enemies/BossAudio.cs).

Because those responsibilities were already separated, I could later add [BossZoneAttack](Assets/Scripts/GameScene/Enemies/BossZoneAttack.cs) as an independent warning-and-damage system. The projectile attack, movement, and zone attack can run together without one large controller managing every detail.

## Improving the UI Through Playtesting

The UI was initially designed around constant pixel sizes. It looked correct at the resolution used in the editor, but elements became too large, too small, or poorly positioned when I tested other window and fullscreen sizes.

I changed the main canvases to use Unity's **Scale With Screen Size** mode with a `1920 x 1080` reference resolution and balanced width/height scaling. The menu and gameplay UI now resize more consistently with the display instead of keeping a fixed pixel size.

I also kept UI logic separate from game rules. [GameManager](Assets/Scripts/GameScene/Core/GameManager.cs) controls pause, game-over, and win states, while [ScoreKeeper](Assets/Scripts/GameScene/UI/ScoreKeeper.cs), [HealthBar](Assets/Scripts/GameScene/UI/HealthBar.cs), and [FaceCamera](Assets/Scripts/GameScene/UI/FaceCamera.cs) each handle one display responsibility.

This showed me that UI should be tested at multiple resolutions during development rather than only after the layout is complete.

## Optimizing UI Button Wiring

During review, I was told that some UI and audio code was doing too much hidden work at runtime. Buttons were either wired through Inspector `OnClick` events or discovered globally by [AudioManager](Assets/Scripts/GameScene/Managers/AudioManager.cs). That made the source of button behavior harder to trace and meant newly spawned popup buttons would not automatically fit the same sound system.

I changed the overlays so they own their own buttons and register listeners in code. The main menu, settings overlay, pause overlay, win overlay, and game-over overlay now validate their serialized button references once, log a clear error if a reference is missing, and then wire the expected behavior procedurally.

I initially put the reusable click behavior on a shared button prefab. Review showed that this unnecessarily constrained the button's appearance and hierarchy, and made it harder for teammates to understand which settings belonged to a scene and which came from the prefab. I unpacked the existing buttons and removed that prefab so every button is independently editable in its scene.

For buttons that need sound, **GameObject > UI (Canvas) > Button With Audio** creates a standard TextMeshPro Unity button and adds [UIButtonSoundEmitter](Assets/Scripts/UI/UIButtonSoundEmitter.cs). The component retrieves the colocated `Button` once in `Awake`, exposes only a `Default`, `Confirm`, or `Back` sound category, and publishes that category when clicked. [AudioManager](Assets/Scripts/GameScene/Managers/AudioManager.cs) maps the category to its configured clip and remains responsible for playback. An ordinary Unity button can still be used when no click sound is wanted.

This keeps button actions visible in their owning scripts, avoids persistent Inspector `OnClick` wiring, and lets teammates create and style buttons without depending on a project prefab. The editor command itself is implemented by [ButtonWithAudioMenu](Assets/Scripts/UI/Editor/ButtonWithAudioMenu.cs).

## Removing Fallback References

Another issue from review was that some scripts tried to repair missing references at runtime. For example, a null check would call `GetComponent`, `GetComponentInChildren`, or a scene `Find` method and assign the result automatically. This can hide setup mistakes because the script keeps running instead of showing exactly which Inspector reference was missing.

I changed this rule for the project: serialized dependencies should not use fallback lookups. If a required reference is missing, the script should log a `Debug.LogError` that names the missing dependency and the object that reported it. That makes the problem easier to pinpoint during testing and keeps broken scene setup visible.

I also reduced `GetComponent` and `Find` usage to the smallest amount needed. If a lookup is truly necessary, it should happen once during initialization, such as in `Awake`, and the result should be reused. Repeated lookups during gameplay or hidden lookups inside null checks should be avoided.

## Decoupling Gameplay and Audio

At first, sound playback could have been added directly inside every gameplay script. That would tightly couple game rules to audio clips and make duplicated playback code difficult to maintain.

Instead, gameplay components publish events such as firing, taking a shield, hitting a target, or dying. Small audio components subscribe to those events, while [AudioManager](Assets/Scripts/GameScene/Core/AudioManager.cs) provides shared music and sound-effect playback.

This means an enemy or projectile decides **when** an event happened, but its audio component decides **which sound** represents it. It also allows a sound to continue after the object that triggered it has been destroyed.

## Persisting Audio Settings

Keeping [AudioManager](Assets/Scripts/GameScene/Managers/AudioManager.cs) alive with `DontDestroyOnLoad` preserves its state while changing scenes, but that state is lost when the application closes. To retain the player's last configuration, the manager stores the music and SFX volume values in Unity's `PlayerPrefs`.

During initialization, the manager loads both saved values before the settings sliders are initialized. If no saved values exist, it uses the volumes configured in the scene as first-run defaults. Loaded and newly selected volumes are clamped to the valid `0` to `1` range before they are applied to their `AudioSource`.

Slider changes update the audio immediately and update the corresponding preference in memory. Disk writes are delayed briefly and combined, so dragging a slider does not synchronously save every intermediate value. Any pending settings are also flushed when the application pauses, quits, or destroys the active manager. This keeps the latest configuration reliable across normal sessions without adding repeated disk writes during slider movement.

## Reusing Projectiles

The first shooting implementation instantiated a new projectile for every shot and destroyed it after impact or when it left the screen. This was simple, but both the player and boss can create many projectiles in a short time.

I replaced that lifecycle with [ObjectPool](Assets/Scripts/GameScene/Infrastructure/ObjectPool.cs). It creates a fixed collection of projectiles when the scene starts. [PlayerWeapon](Assets/Scripts/GameScene/Player/PlayerWeapon.cs) and [BossWeapon](Assets/Scripts/GameScene/Enemies/BossWeapon.cs) request inactive projectiles, and [ProjectileController](Assets/Scripts/GameScene/Combat/ProjectileController.cs) returns them after use.

This taught me to consider object lifetime and repeated allocation when a gameplay object is created frequently.

## Making Powerups Reusable

I used Google to research a simple ScriptableObject powerup pattern, then adapted it for this project. The shared [PowerupEffect](Assets/Scripts/GameScene/Powerups/PowerupEffect.cs) base defines how an effect is applied and removed, while [PowerupItem](Assets/Scripts/GameScene/Powerups/PowerupItem.cs) and [PlayerPowerUpManager](Assets/Scripts/GameScene/Player/PlayerPowerUpManager.cs) handle collection and duration.

Using the same pattern for [speed](Assets/Scripts/GameScene/Powerups/SpeedBoost.cs), [shield](Assets/Scripts/GameScene/Powerups/Shield.cs), and [multifire](Assets/Scripts/GameScene/Powerups/Multifire.cs) helped me understand how ScriptableObjects can separate reusable effect data from pickup behavior.

## Main Takeaway

The largest improvement was not a single feature. It was changing from large scripts that directly controlled everything to smaller components with clear responsibilities and event-based communication. Playtesting also showed me that technical structure and presentation both need iteration: code must remain maintainable as features grow, and UI must work outside the editor's default resolution.
