# Demon Doof Unity

**Demon Doof** is a turn-based tactical game developed in Unity 2022.3.55f1. Players control a squad of unique units on a grid-based battlefield, engaging in strategic combat against AI-controlled enemies.

---

## 🕹️ Game Overview

Inspired by genre classics like *XCOM* and *Into the Breach*, Demon Doof emphasizes deep tactical decision-making and strategic use of each unit's abilities. Gameplay alternates between the player’s turn and the enemy’s, requiring careful planning to outmaneuver the opposition.

---

## 📁 Project Structure

### Code Architecture

The codebase follows object-oriented principles with a clear separation of concerns:

- `Assets/Scripts/Actions/` – Unit action system  
- `Assets/Scripts/Grid/` – Grid management and pathfinding  
- `Assets/Scripts/UI/` – User interface logic  
- `Assets/Prefabs/` – Prefabs for units, effects, objects  
- `Assets/Scenes/` – Game scenes  
- `Assets/Models/` – 3D models  
- `Assets/Animations/` – Animations  

### Core Components

#### Turn System
- `TurnSystem.cs` – Manages the turn order between player and enemies  
- Uses events to synchronize game systems on turn change

#### Grid System
- `Grid/GridSystem.cs` – Manages a coordinate-based cell grid  
- `Grid/GridPosition.cs` – Logical coordinate representation  
- `PathNode.cs`, `Pathfinding.cs` – A* algorithm implementation for movement

#### Action System
- `BaseAction.cs` – Abstract class for all unit actions  
- Implemented actions:  
  - `MoveAction.cs`  
  - `ShootAction.cs`  
  - `SwordAction.cs`  
  - `GrenadeAction.cs`  
  - `InteractAction.cs`  
  - `SpinAction.cs`

#### Unit Management
- `Unit.cs` – Represents unit stats and available actions  
- `UnitManager.cs` – Handles all active units in battle  
- `UnitActionSystem.cs` – Handles input and action execution  
- `UnitAnimator.cs` – Manages unit animations

#### Enemy AI
- `EnemyAI.cs` – Controls enemy behavior logic  
- `EnemyAIAction.cs` – Encapsulates AI action evaluation

#### Input and Camera
- `InputManager.cs` – Keyboard and mouse input  
- `TouchInputManager.cs` – Mobile touch support  
- `CameraController.cs`, `CameraManager.cs` – In-game camera movement and control

#### Environment Interactions
- `IInteractable.cs` – Interface for interactable objects  
- Examples: `Door.cs`, `BarrelInteract.cs`, `DestructibleCrate.cs`

---

## 🧠 Design Patterns Used

- **Singleton** – Global managers like `UnitActionSystem`, `TurnSystem`  
- **Observer** – Event-driven updates for turn changes, selections, etc.  
- **Command** – Encapsulation of unit actions  
- **Factory** – Object creation logic for gameplay elements  
- **State Machine** – Unit state and enemy AI logic

---

## ⚙️ System Requirements

- Unity Editor 2022.3.55f1 or newer  
- .NET Framework 4.x

---

## 🚀 Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/crasherbit/demon-doof-unity.git
   ```
2. Open the project in Unity 2022.3.55f1  
3. Load the main scene located at `Assets/Scenes/GameScene.unity`

---

## 🎮 Gameplay Controls

- **Left Click** – Select units / Confirm actions  
- **Right Click** – Cancel selection  
- **WASD / Arrow Keys** – Move camera  
- **Q / E** – Rotate camera  
- **Mouse Scroll** – Zoom in/out  
- **Spacebar** – End turn  

---

## 🌐 Multiplayer Roadmap (via FishNet)

### Phase 1: Core Implementation (2–3 weeks)
- Install and configure FishNet  
- Refactor `TurnSystem` for multi-player compatibility  
- Convert units into `NetworkObjects`  
- Adapt action system for networking  
- Create a basic multiplayer lobby system

### Phase 2: Backend & Support Systems (3–4 weeks)
- User authentication  
- Match persistence and save/load  
- Matchmaking and dedicated server configuration

### Phase 3: Integration & Polishing (2–3 weeks)
- Frontend-backend integration  
- Network performance optimization  
- Security, social features  
- Final polish and launch prep

---

## 📌 Multiplayer Technical Considerations

1. **Deterministic Synchronization**  
   - Ensure identical outcomes across clients  
   - Use shared RNG seeds  
   - Avoid unpredictable physics where possible

2. **Connection Handling**  
   - Automatic reconnect logic  
   - Graceful handling of disconnections  
   - Stand-in system for dropped players

3. **Scalability**  
   - Design for increasing user base  
   - Integrate logging and analytics  
   - Evaluate serverless solutions for some systems

---

## ✅ Immediate Next Steps for Multiplayer

1. **FishNet Setup**  
   - Install FishNet from the Unity Asset Store  
   - Add `NetworkManager` to main scene  
   - Create lobby and connection scenes

2. **Core Systems Refactor**  
   - Modify `TurnSystem.cs` for multiplayer turns  
   - Add `NetworkBehaviour` to managers  
   - Sync `GridSystem` state across clients

3. **Networked Units & Actions**  
   - Make units `NetworkObjects`  
   - Use RPCs to handle action requests  
   - Implement authority checks per client/server

4. **Lobby & Matchmaking**  
   - Create UI for joining/creating matches  
   - Handle room management  
   - Implement disconnect handling

5. **Testing & Optimization**  
   - Simulate various network conditions  
   - Reduce bandwidth usage  
   - Minimize perceived input lag

---

## 🤝 Contributing

Pull requests are welcome! For major changes, please open an issue first to discuss what you'd like to propose.

---

## 📄 License  and Usage

This project is shared for **educational and portfolio purposes only**.  
Commercial use, redistribution, or integration into other commercial projects is **not permitted without explicit permission** from the author.

Some parts of the code or assets may be inspired by or derived from community tutorials, open source projects, or other learning materials.  
Proper credit is given where due, and no ownership is claimed over third-party content.

If you'd like to use this project in any way beyond personal learning or review, feel free to reach out and ask.

© 2025 crasherbit. All rights reserved.
