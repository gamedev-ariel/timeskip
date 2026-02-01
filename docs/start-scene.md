Start scene — Controls tutorial

Overview
- Purpose: brief onboarding to ensure the player knows how to jump and move before entering gameplay areas.
- Not a minigame: there is no score or timer here; it is only a quick control tutorial.

How it starts
- When the Start scene loads, an on‑screen instruction appears:
  - "Use space key to jump."
  - After the player presses Space once, the prompt changes to:
    - "Use right arrow to go right or left arrow to go left."
  - After the player presses either Left or Right Arrow, the tutorial completes and the prompt disappears.
- A voice prompt can be played if configured in the scene.

Controls
- Space: jump
- Left/Right arrows: move horizontally

What the player can do
- Practice pressing Space and moving left/right until the tutorial marks instructions as completed. No other objectives are present here.

How to win or lose
- There is no win/lose condition in the Start scene. Once both prompts are satisfied (jump and one arrow pressed), the instructions are considered completed.

Data collected here
- This scene serves as onboarding. It does not contribute to the 23‑feature vector used for the prediction.
- General session/scene visit logs may still be recorded by the logger, but the Start scene has no dedicated gameplay metrics.

Related scripts (for reference)
- Assets/Scripts/Game1/InstructionManager.cs
  - Shows the jump and arrow prompts and marks them as completed after the corresponding key presses.
