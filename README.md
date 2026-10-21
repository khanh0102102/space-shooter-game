# Space Explorer — Unity 2D Lab Assignment

A 2D Unity game built around three real scenes: **MainMenu**, **Gameplay**, and **EndGame**. The project keeps the original Space Shooter prototype in the legacy \`Main.unity\` scene, while the assignment flow starts from \`MainMenu.unity\`.

## Assignment requirements implemented

| Requirement | Implementation |
|---|---|
| Main Menu with Play button | Play loads the Gameplay scene and starts a fresh run |
| Instructions button | Opens an instructions panel in the Main Menu |
| Spaceship moves with arrow keys | Up, Down, Left and Right move the player ship |
| Shooting lasers | Hold Space to fire laser projectiles |
| Asteroids move randomly | Asteroids drift and periodically change direction; they bounce at the camera bounds |
| Stars scattered in space | Collectible stars are spawned across the play area and replenished during play |
| Collecting stars adds points | Each star adds 10 points |
| Asteroid collision deducts points | An asteroid hit deducts 10 points (score never drops below zero) and immediately ends the run, matching the assignment's game-over rule |
| Score HUD | The current score appears during gameplay |
| End Game scene | Displays the final score, with Main Menu and Quit buttons |
| Scene transitions | MainMenu → Gameplay → EndGame, with a fade transition |
| Physics and colliders | Player, asteroids, stars and lasers use 2D colliders and Rigidbody2D components |
| Documentation | Comments explain scene setup, movement, collision handling, collectibles and scoring |

The generated assignment scenes are saved under \`Assets/Scenes\`. The editor setup script adds them to Build Settings in the correct order, with MainMenu first.

## Controls

- **Arrow keys**: move the spaceship.
- **Hold Space**: shoot lasers at asteroids.
- **Esc**: pause or resume gameplay.

## Kenney Space Shooter Remastered assets (optional)

The project can run with generated fallback sprites. To use the Kenney artwork and sound effects, copy the folders from the extracted \`kenney_space-shooter-remastered\` pack into:

- \`Backgrounds\` → \`Assets/Resources/Kenney/Backgrounds\`
- the entire \`PNG\` folder → \`Assets/Resources/Kenney/PNG\`
- \`Bonus\` → \`Assets/Resources/Kenney/Bonus\`
- \`license.txt\` → \`Assets/Resources/Kenney/license.txt\`

Preserve the original file names and directory structure. For PNGs, select the files in Unity and set **Texture Type** to **Sprite (2D and UI)**.

## Open and run

1. Clone the repository or download and extract its ZIP.
2. In Unity Hub, add the project folder (the folder containing \`Assets\`, \`Packages\`, and \`ProjectSettings\`).
3. Open the project with a supported Unity LTS Editor and wait for script compilation and asset import.
4. The editor setup creates \`Assets/Scenes/MainMenu.unity\`, \`Assets/Scenes/Gameplay.unity\` and \`Assets/Scenes/EndGame.unity\`, then registers them in Build Settings. You can also run **Space Explorer → Create or Repair Assignment Scenes** from the Unity menu.
5. Double-click \`Assets/Scenes/MainMenu.unity\` and press Play.

If the scenes do not appear after compilation, use the menu command in step 4 and wait for Unity to finish saving the scene files.

## Project structure

- \`Assets/Scripts/SpaceExplorerSceneController.cs\`: scene flow, backdrop, spawning, score, HUD, pause menu and transitions.
- \`Assets/Scripts/SpaceExplorerPlayerController.cs\`: arrow-key movement, laser input and collision detection.
- \`Assets/Scripts/SpaceExplorerAsteroidController.cs\`: random drifting, turning and boundary bouncing.
- \`Assets/Scripts/SpaceExplorerStarController.cs\`: collectible stars and score reward.
- \`Assets/Scripts/SpaceExplorerLaserController.cs\`: laser movement and asteroid hits.
- \`Assets/Editor/SpaceExplorerSceneSetup.cs\`: creates the three assignment scenes and configures Build Settings.
- \`Assets/Scripts/SpriteFactory.cs\`: loads Kenney sprites when available and provides procedural fallbacks.

## Legacy prototype

The original \`Assets/Scenes/Main.unity\` scene and the \`SpaceShooterGame\` scripts remain available as the previous Space Shooter prototype. They are not used by the new assignment scenes. To play the assignment as intended, open **MainMenu.unity** or build with the provided Build Settings order.

## Technical notes

- Unity 2022.3 LTS or newer.
- Built-in 2D physics; no third-party packages required.
- The scene setup is editor-driven so all three scene files are created in the project and included in Build Settings.
