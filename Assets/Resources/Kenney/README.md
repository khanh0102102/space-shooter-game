# Kenney assets for Space Shooter

Copy the asset folders from the extracted kenney_space-shooter-remastered pack into this project without renaming the source files:

- Source Backgrounds → Assets/Resources/Kenney/Backgrounds
- Source PNG → Assets/Resources/Kenney/PNG
- Source Bonus → Assets/Resources/Kenney/Bonus
- Source license.txt → Assets/Resources/Kenney/license.txt

Copy each whole folder, not just the files in its root. The PNG subfolders must keep their original names, including Power-ups with a hyphen.

The code loads sprites from these Resources paths at runtime. Unity does not automatically copy files from Downloads; this folder must be populated on your computer. Once copied, wait for Unity to finish importing assets. If sprites are not recognized, select them in Unity and choose Texture Type = Sprite (2D and UI), then Apply.

The code has procedural fallbacks; a missing asset will use its placeholder rather than stopping the game.
