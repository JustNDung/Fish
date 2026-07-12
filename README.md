# Triple Tray Match

A small Unity puzzle game in which the player moves items from a board into a five-slot tray. Collecting three identical items removes that set. The board is generated so that every item belongs to a complete set of three, making each round solvable.

## Implemented Features

### Core gameplay

- A configurable grid-based board populated with seven item types.
- Tap or click an item to move it from the board to the tray.
- A tray with a maximum capacity of five items.
- Automatic detection and removal of three identical tray items.
- Smooth movement, rearrangement, appearance, and removal animations powered by DOTween.
- Live HUD information for the number of items remaining on the board and occupied tray slots.
- Separate win and game-over screens.

### Game modes

- **Play** - the standard manual mode. Clear both the board and tray to win. The game is lost when the tray reaches five items without producing a match.
- **Autoplay** - demonstrates a winning solution by automatically selecting complete sets of three.
- **Auto Lose** - demonstrates the losing condition by filling the tray with different item types.
- **Time Attack** - clear the board within 60 seconds. In this mode, clicking an occupied tray slot returns its item to its original empty board cell, allowing moves to be undone. A full tray does not immediately end the round, but no additional item can be selected until a tray item is returned.

### UI and flow

- Main menu with access to all four modes.
- Pause and resume support through the pause button or the `Esc` key.
- DOTween animations are paused and resumed together with the game state.
- A sound preference toggle with a persistent on/off value stored in `PlayerPrefs`.
- Runtime transitions between setup, main menu, gameplay, pause, win, and game-over states.

### Board generation and configuration

- Board dimensions and matching values are stored in a `GameSettings` ScriptableObject.
- The current board is **4 x 6**, containing 24 items arranged as eight shuffled triples.
- The Unity editor includes:
  - `Game Tools > Create Game Settings`
  - `Game Tools > Open Game Settings`
- Item and cell prefabs are loaded from `Assets/Resources/prefabs`.

## How to Play

1. Choose a mode from the main menu.
2. Click or tap items on the board to place them into the tray.
3. Collect three items of the same type to remove them from the tray.
4. Clear every board item and leave the tray empty to win.
5. In standard Play mode, avoid filling all five tray slots with unmatched items.
6. In Time Attack, finish before the timer reaches zero; click a tray slot to undo that item when needed.

## Controls

| Input | Action |
| --- | --- |
| Left mouse button / tap | Select a board item |
| Left mouse button / tap on a tray slot | Return an item to the board in Time Attack |
| Pause button | Pause the game |
| `Esc` | Toggle pause and resume |

## Project Structure

| Path | Responsibility |
| --- | --- |
| `Assets/Scripts/Controllers/GameManager.cs` | Game state, mode selection, level lifecycle, and win/loss flow |
| `Assets/Scripts/Controllers/BoardController.cs` | Input, tray behavior, matching, timer, autoplay, and end conditions |
| `Assets/Scripts/Board/Board.cs` | Grid creation, item generation, and board data |
| `Assets/Scripts/Board/Cell.cs` | Cell coordinates, neighbours, and item ownership |
| `Assets/Scripts/Board/Item.cs` | Item view lifecycle and DOTween animations |
| `Assets/Scripts/UI` | Menu panels, HUD, pause handling, and sound preference UI |
| `Assets/Scripts/Editor/MainToolMenu.cs` | Unity editor shortcuts for the game settings asset |
| `Assets/Resources/gamesettings.asset` | Current board and level configuration |

## Requirements

- Unity **2020.3.38f1**
- DOTween (included in the project under `Assets/Demigiant`)
- Unity UI (`com.unity.ugui`)

## Running the Project

1. Open the project with Unity 2020.3.38f1 or a compatible Unity 2020.3 LTS editor.
2. Open `Assets/Scenes/Game.unity`.
3. Enter Play Mode.
4. Select one of the modes from the main menu.

No additional package installation or scene setup is required because the required assets and DOTween files are included in the repository.

## Notes

The project still contains reusable code from an earlier swap-based Match-3 implementation, including board swapping, potential-match searches, hints, gravity, and bonus-item classes. The active game loop uses the tap-to-tray system described above; those legacy systems are not currently exposed during gameplay.
