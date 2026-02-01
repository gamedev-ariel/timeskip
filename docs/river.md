River — Cross by jumping on rocks

Overview
- Purpose: navigate across a flowing river by jumping between rocks while avoiding hazards. This area measures movement, jump timing, hazards, approaches, and outcome.
- Not a separate minigame: it uses the shared scene controller with its own UI and countdown.

How it starts
- In the main area, when the player approaches the rock portal, an instruction appears: "Click enter to go to the river." Press Enter to load the river scene.
- In the river scene, gameplay does not begin immediately. The InstructionManager must complete any instructions first; once completed, a countdown appears and then the game starts.
  - Default countdown before start: 5 seconds.
  - Default game duration: 60 seconds (the controller ends the run at this time if not already ended by win/lose).

Controls
- Left/Right arrows: move horizontally on rocks.
- Space: jump. Each jump creates a jump_start log from the current rock; landing on a rock logs jump_land and chains to the next jump automatically.
- Down Arrow: increases gravity temporarily to fall faster.

Objectives and rules
- Reach the far river bank (an object tagged RiverBankEnd) by hopping across rocks (objects tagged Mushroom).
- Hazards to avoid:
  - Fish
  - Moving out of camera bounds

How to win
- Collide with the far bank (RiverBankEnd). This plays a victory sound, logs a river_finish "win" and a session outcome with reason "reached_end", shows "Well Done!", and ends the run.

How to lose
- Any of the following ends the run with a loss and shows "Try Again!":
  - Collision with a fish
  - Moving out of the camera bounds

UI behavior
- Countdown: displayed after instructions complete, then hidden when the game starts.
- In‑game timer: shows time remaining.
- Outcome panels:
  - Well Done! with buttons: Restart, Main Menu, Start
  - Try Again! with the same buttons

What data is collected here
- Jumps
  - jump_start with the rock id jumped from and a generated jump id; jump_land with the rock landed on and the same jump id.
- Hazards and outcomes
  - fish collision events and a river_finish event with result win/lose.
  - Out‑of‑bounds losses are logged as river out‑of‑bounds and a session outcome.
- Proximity approaches
  - Approaches toward fish are periodically detected and logged.
- Input timing
  - Total keypress count and inter‑keypress gap statistics (mean and CV) while in the river area.

Contribution to the 23‑feature vector
- river_fish_collisions, river_out_of_bounds, river_win, river_approaches
- river_keys, river_key_gap_mean, river_key_gap_cv

Related scripts (for reference)
- Assets/Scripts/River/GameController.cs (countdown, duration, start/end)
- Assets/Scripts/River/UIManager.cs (countdown, timer, outcome panels, screw UI when used by forest)
- Assets/Scripts/River/CollisionHandler.cs (win/lose, fish collisions, landing, screw pickup when used by forest)
- Assets/Scripts/River/PlayerMovementRiver.cs (controls, jump logging)
- Assets/Scripts/Game1/InstructionManager.cs (Enter to load the river, instruction gating)
- Assets/Scripts/ExperimentLogger/ExperimentLogger.River.cs
