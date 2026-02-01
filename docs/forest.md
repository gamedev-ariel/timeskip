Forest — Collect screws, avoid hazards

Overview
- Purpose: collect screws to fix the time machine while avoiding hazards. This area is used to measure collection performance, proximity approaches, input timing, and safety.
- Not a separate minigame: gameplay happens in the main scene context, but it has clear objectives and UI.

How it starts
- When entering the forest area, an instruction appears once per session:
  - "Collect the screws to fix the time machine! Avoid the berries, they are poisonous! Press any key to start."
- After the player presses any key, the prompt disappears and the forest run begins.

Controls
- Left/Right arrows: move horizontally
- Space: jump

Objectives and rules
- Collect all visible screws. A screw indicator UI shows progress (dark screw icons turn to normal/bright when collected).
- Hazards to avoid:
  - Poisonous berries if present in the shared scene.
  - Going out of bounds of the camera view also counts as a failure.
- The system also observes proximity “approach” behavior around hazards (e.g., moving toward a blueberry) without requiring actual collisions.

How to win
- When the number of collected screws equals the configured total, the forest run ends with a win state and a "Well Done!" message.

How to lose
- Any of the following will end the run with a lose state and show "Try Again!":
  - Collision with a berry
  - Moving out of the camera bounds

UI behavior
- Screw progress bar: updated as each screw is collected.
- On game over: try again button, go to Main Menu button, or go to Start button.

What data is collected here
- Item lifecycle
  - Each screw or blueberry emits an item_spawn event on activation and an item_resolve event when collected, expired, or otherwise resolved.
- Outcomes and hazards
  - Forest collisions are logged (berries), out‑of‑bounds losses, and win on collecting all screws.
- Proximity approaches
  - The logger periodically detects when the player moves toward a nearby blueberry and records an approach event.
- Input timing
  - Total number of key presses and the gaps between them (mean and coefficient of variation) while in the forest area.
- Target reaction‑time metrics
  - Median and coefficient of variation of the reaction time to target actions (e.g., pickups) computed from the session log.

Contribution to the 23‑feature vector
- screws_spawned, screws_collected
- forest_fish_collisions(actually berries), forest_out_of_bounds, forest_5_screws_collected, forest_approaches
- target_rt_median, target_rt_cv
- forest_keys, forest_key_gap_mean, forest_key_gap_cv

Related scripts (for reference)
- Assets/Scripts/Game1/InstructionManager.cs
- Assets/Scripts/Forest/PlayerMovementForest.cs
- Assets/Scripts/Forest/ForestItemTrack.cs
- Assets/Scripts/River/CollisionHandler.cs (shared collision handling including forest win/lose and screw pickup)
- Assets/Scripts/River/UIManager.cs (screw UI and outcome panels)
- Assets/Scripts/ExperimentLogger/ExperimentLogger.Forest.cs
- Assets/Scripts/ExperimentLogger/ExperimentLogger.Approach.cs
- Assets/Scripts/ExperimentLogger/ExperimentLogger.Scene.cs
