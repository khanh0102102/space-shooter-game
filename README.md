# Space Shooter Game (Unity 2D)

A small, playable Unity 2D space-shooter prototype built with C# and procedural sprites. It has no external art or asset dependencies: the ships, bullets, asteroids, pickups, and starfield are generated at runtime.

## Features

- WASD or arrow-key movement.
- Automatic shooting aimed toward the mouse cursor.
- 1, 3, and 5-shot weapon levels.
- Enemies that move and fire, drifting asteroids, and collision damage.
- Health, triple-shot, rapid-fire, damage, and movement pickups; slow/weak-shot debuffs.
- Five levels. **Every level has its own Boss**; reaching that level's score goal summons the Boss. Defeating it unlocks the next level.
- Level score resets when a new level starts; total score continues through the run.
- Pause/resume, restart-level, main menu, level selection, Game Over, and final Victory screens.
- Unlocked levels and best score are saved locally with PlayerPrefs.

## Requirements

- Unity 2022.3 LTS or newer.
- The built-in 2D physics module. No third-party packages are required.

## Open and run

1. Download the repository as a ZIP or clone it:
   - Git: `git clone https://github.com/khanh0102102/space-shooter-game.git`
   - Or open the repository page and choose **Code → Download ZIP**, then extract it.
2. Open **Unity Hub**.
3. Choose **Add → Add project from disk** and select the extracted `space-shooter-game` folder (the folder containing `Assets`, `Packages`, and `ProjectSettings`).
4. Open the project with Unity 2022.3 LTS or newer. Wait for the first import and script compilation to finish.
5. The editor setup script creates `Assets/Scenes/Main.unity` and adds it to Build Settings when the project is imported. If it has not appeared yet, use **Space Shooter → Create/Repair Main Scene** from the Unity menu.
6. Open `Assets/Scenes/Main.unity` and press **Play**. Click **NEW GAME** in the game window.

## Controls

- **WASD / Arrow keys**: move the ship.
- **Mouse position**: aim. The ship auto-fires; no mouse button is required to shoot.
- **Esc**: pause/resume.
- The in-game **Pause** button opens Resume, Restart Level, and Main Menu options.

## Level and score rules

| Level | Score goal to summon Boss | Example Boss HP |
|---|---:|---:|
| 1 | 1,000 | 500 |
| 2 | 2,000 | 1,250 |
| 3 | 3,500 | 2,000 |
| 4 | 5,000 | 2,750 |
| 5 | 7,000 | 3,500 |

These are starting balance values and can be changed in `SpaceShooterGame.cs`. Defeating the Boss completes the current level. The final Boss (Level 5) ends the game with Victory.

## Difficulty settings (Easy Mode)

The defaults are tuned to be more forgiving while you learn the controls. You can change these values directly in the C# scripts, then return to Unity and let it recompile.

| What to change | File and setting | Current easy default |
|---|---|---|
| Score needed to summon each Boss | `Assets/Scripts/SpaceShooterGame.cs` → `ScoreGoals` | `600, 1100, 1700, 2300, 3000` |
| How often normal enemies appear | `SpaceShooterGame.cs` → `EnemySpawnIntervalBase`, `EnemySpawnIntervalPerLevel`, `EnemySpawnIntervalMinimum` | About 2 seconds early on |
| How often asteroids appear | `SpaceShooterGame.cs` → `AsteroidSpawnIntervalMinimum`, `AsteroidSpawnIntervalMaximum` | Every 4–5.5 seconds |
| How often pickups appear | `SpaceShooterGame.cs` → `PickupSpawnIntervalMinimum`, `PickupSpawnIntervalMaximum`, `PickupSpawnChance` | Every 5.5–8 seconds, 90% spawn roll |
| Chance an enemy drops a positive pickup | `SpaceShooterGame.cs` → `EnemyPickupDropChance` | 40% |
| Player firing speed | `Assets/Scripts/PlayerController.cs` → `BaseFireInterval` | 0.30 seconds; lower = faster |
| Player bullet damage | `PlayerController.cs` → `BaseBulletDamage` | 15; higher = stronger |
| Player projectile speed | `PlayerController.cs` → `BulletController.Create(..., 9f)` in `PlayerController.cs` | 9; higher = faster bullets |
| Time protected after taking a hit | `PlayerController.cs` → `invulnerableUntil` | 1.0 seconds |
| Enemy firing interval and bullet speed | `Assets/Scripts/EnemyController.cs` → `fireTimer` and `BulletController.Create` | Slower volleys and bullets |
| Boss HP and attack pattern | `Assets/Scripts/BossController.cs` → `MaxHealth`, `fireTimer`, and Boss bullet speed | Boss HP: 300, 650, 1000, 1350, 1700 |

**Rule of thumb:** To make the game easier, increase the enemy/asteroid spawn intervals, increase the player's damage, decrease the Boss HP values, or lower the score goals. To make it harder, do the opposite. Keep the score goals in ascending order so each new level still requires more progress.

### How to apply your changes

1. Open the relevant script under `Assets/Scripts` in Visual Studio or VS Code.
2. Edit the number after the setting named in the table. Keep the `f` suffix on decimal values (for example, `0.30f`).
3. Save the file and return to Unity. Wait for script compilation to finish, then press Play again.

## Pickups

- Health: +10 HP (maximum 100).
- Triple Shot: upgrades the weapon from 1 to 3 to 5 projectiles.
- Rapid Fire: temporarily reduces the firing interval.
- Damage Boost: temporarily increases bullet damage.
- Speed Boost: temporarily increases movement speed.
- Slow, Slow Fire, and Weak Shot: temporary negative effects. Picking up the opposing effect cancels the active one.

Temporary effects pause while the game is paused. At the beginning of a new level, weapon level and remaining HP carry over; the player receives +30 HP (maximum 100) and temporary effects are cleared.

## Project structure

- `Assets/Scripts/SpaceShooterGame.cs`: game state, spawning, scores, levels, UI, pause/resume.
- `Assets/Scripts/PlayerController.cs`: movement, auto-aim shooting, HP, weapon upgrades, timed effects.
- `Assets/Scripts/EnemyController.cs`: normal enemies.
- `Assets/Scripts/BossController.cs`: a distinct Boss fight for every level.
- `Assets/Scripts/BulletController.cs`: player and enemy projectiles.
- `Assets/Scripts/AsteroidController.cs`: moving hazards.
- `Assets/Scripts/PickupController.cs`: positive and negative pickup effects.
- `Assets/Scripts/SpriteFactory.cs`: runtime-generated sprites.
- `Assets/Scripts/StarfieldController.cs`: scrolling background stars.
- `Assets/Editor/SpaceShooterSetup.cs`: creates and registers the demo scene in the Unity Editor.

## Notes

The art is intentionally procedural so the project works without importing external sprites. You can replace the generated sprites with your own artwork later while keeping the gameplay scripts. All score thresholds and timings are prototype defaults intended to be play-tested and balanced.
