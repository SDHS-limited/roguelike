# Project Overview
- Game Title: Rogue Experiment (Working Title)
- High-Level Concept: "The stronger you get, the more control you lose." A roguelike FPS where character growth is tied to increasing instability and loss of player control.
- Players: Single player
- Inspiration / Reference Games: Risk of Rain 2 (progression), Hotline Miami (intensity), Slay the Spire (trade-offs)
- Tone / Art Direction: Dark, scientific, glitchy, chaotic.
- Target Platform: PC (Standalone Windows)
- Screen Orientation / Resolution: Landscape 1920x1080
- Render Pipeline: Built-in

# Game Mechanics
## Core Gameplay Loop
1.  **전투 (Combat)**: Fight enemies in a room-based dungeon.
2.  **실험 선택 (Experiment Selection)**: After clearing a room or at specific points, choose from 3 experiments.
3.  **능력 강화 (Power-up)**: Experiments grant powerful buffs (Damage, Speed, Fire Rate, etc.).
4.  **후유증 발생 (Aftereffects)**: Experiments also increase the "Fever" gauge. Higher Fever leads to more frequent and severe "Loss of Control" events.
5.  **폭주 위험 증가 (Berserk Risk)**: If Fever reaches 100%, the player enters a Berserk state.
6.  **다음 전투 (Next Combat)**: Enter the next room with new powers and new instabilities.

## Controls and Input Methods
- **WASD**: Movement (subject to "Drift" or "Stagger" aftereffects)
- **Mouse**: Aiming (subject to "Recoil", "Jitter", or "Sway" aftereffects)
- **Left Click**: Shoot (subject to "Jamming" or "Auto-fire" aftereffects)
- **R**: Reload
- **Space**: Jump
- **Left Shift**: Run
- **Right Click**: Dash

# UI
- **HUD**:
    - HP Slider (current health)
    - Fever Slider (current instability gauge)
    - Ammo Counter
    - Crosshair (may jitter or change size based on instability)
- **Experiment Screen**: Card-based selection for choosing buffs.

# Key Asset & Context
- `ExperimentManager.cs`: Manages selection and application of buffs/debuffs.
- `AftereffectManager.cs`: Handles the logic for "Loss of Control" events.
- `Fever_Slider.cs`: Visual representation and data for the instability gauge.
- `Player.cs` / `Move.cs`: Modified to handle instability factors (drift, stagger, etc.).
- `Gun.cs` / `Recoil.cs`: Modified to handle weapon instability.

# Implementation Steps
## Phase 1: Instability Framework (Fever System)
1.  **Update `Fever_Slider`**: Add events for when Fever reaches specific thresholds (e.g., 25%, 50%, 75%, 100%).
2.  **Refactor `ExperimentManager`**: Ensure every experiment properly increments the Fever gauge.

## Phase 2: Loss of Control (Aftereffects)
1.  **Implement `AftereffectManager`**: 
    - Create a list of potential aftereffects (Visual, Movement, Combat).
    - Logic to trigger random aftereffects based on current Fever level.
    - Examples:
        - *Twitch*: Random sudden camera rotation.
        - *Stumble*: Brief reduction in movement speed or random lateral force.
        - *Spasm*: Forced firing of the weapon.
        - *Glitch*: Visual distortion (using existing Post-processing or simple UI overlays).

## Phase 3: Integration with Player & Weapon
1.  **Modify `Move.cs`**: Add a `driftForce` or `inputMultiplier` that can be manipulated by `AftereffectManager`.
2.  **Modify `Gun.cs` / `Recoil.cs`**: Scale recoil and spread based on Fever level. Add a chance for "Jamming" (briefly unable to fire).

## Phase 4: Berserk Mode (폭주)
1.  **Implement Berserk State**: 
    - When Fever == 100%:
        - Massive damage increase.
        - Unlimited ammo or extremely fast reload.
        - Automatic firing.
        - Rapidly decaying health or forced movement.
        - Intense visual "Glitch" effects.
    - End condition: After a duration, Fever resets to 50% but max health might be reduced.

# Verification & Testing
1.  **Test Fever Progression**: Verify that picking multiple experiments increases Fever correctly.
2.  **Test Aftereffect Frequency**: Ensure that at low Fever, effects are rare/minor, and at high Fever, they are frequent/severe.
3.  **Combat Feel**: Verify that the "Loss of Control" makes the game harder but still "engagingly" frustrating rather than just broken.
4.  **Berserk Transition**: Test the 100% Fever trigger and the return to normal state.
