Dwarf’s House — Memory game

Overview
- Purpose: a working‑memory task. The player observes a scene snapshot, then answers a question about it. This minigame contributes reaction‑time and accuracy metrics.
- The minigame runs in its own scene and has its own UI.

How it starts
- After entering the house from the main area, the MemoryGame scene loads and the minigame begins automatically:
  - The scene snapshot is shown first.
  - After 5 seconds, the snapshot hides and a question with multiple options is shown.

Controls
- Mouse: click an answer option on the question panel.

Rules and flow
- Lives: the player starts with 3 lives.
- Level loop:
  1) Show snapshot for 5 seconds.
  2) Hide snapshot, show question and options.
  3) Player selects an answer.
  4) If correct: progress to the next level.
     - If the level index reaches 4, the game records the end reason "reached_level_4" and loads the kitchen scene.
     - If all configured questions are cleared, end reason is "all_levels_cleared" and the next scene is loaded.
  5) If wrong: lose one life and update the lives UI.
     - If lives drop to 0, the minigame ends with "game_over" and shows the Game Over panel.

How to win
- Clear all configured levels/questions for the session, or specifically reach level index 4 (code path loads the next scene), both counted as successful completion paths.

How to lose
- Select incorrect answers until lives reach 0; the Game Over panel appears.

UI behavior
- Scene snapshot panel: shows the level’s image for 5 seconds.
- Question panel: appears after the snapshot and contains clickable answer options.
- Lives text: displays remaining lives.
- Game Over panel: shown when lives reach 0.

What data is collected here
- Minigame lifecycle
  - dwarf_minigame_start at the beginning; dwarf_minigame_end at completion or game over with an end reason.
- Per‑level events
  - memory_level_start when a level begins.
  - dwarf_trial_start when the question appears, including the list of shown answer options and the correct one.
  - dwarf_trial_response when the player responds, including selected item, correctness, and reaction time (ms) measured from question onset.
  - memory_level_result at each level outcome (correct, game_over) with remaining lives.
- Accuracy metrics aggregated for the 23‑feature vector
  - dwarf_enters, dwarf_wm_rt_median, dwarf_wm_rt_cv, dwarf_correct, dwarf_incorrect.

Related scripts (for reference)
- Assets/Scripts/MemoryGame/GameManager.cs (core flow, logs, lives and levels)
- Assets/Scripts/MemoryGame/UIManagerMG.cs (UI panels and lives)
- Assets/Scripts/MemoryGame/QuestionManager.cs (question text and answer options)
- Assets/Scripts/ExperimentLogger/ExperimentLogger.Persistence.cs (feature export)
