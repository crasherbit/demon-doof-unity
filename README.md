# Demon Doof Unity

Un gioco tattico a turni sviluppato in Unity 2022.3.55f1, dove i giocatori controllano unità in un ambiente a griglia per combattere contro nemici controllati dall'IA.

## Panoramica del Gioco

Demon Doof è un gioco tattico a turni ispirato ai classici del genere come XCOM e Into the Breach. I giocatori controllano una squadra di unità, ciascuna con abilità e caratteristiche uniche, in un ambiente basato su griglia. Il gameplay si alterna tra il turno del giocatore e quello dei nemici, con un focus sulla pianificazione strategica e sull'uso tattico delle abilità disponibili.

## Struttura del Progetto

### Architettura del Codice

La codebase è organizzata secondo principi di programmazione orientata agli oggetti con una chiara separazione delle responsabilità:

- **Assets/Scripts/Actions/** - Sistema di azioni delle unità
- **Assets/Scripts/Grid/** - Sistema di griglia e pathfinding
- **Assets/Scripts/UI/** - Interfaccia utente
- **Assets/Prefabs/** - Prefabbricati per unità, effetti, ecc.
- **Assets/Scenes/** - Scene di gioco
- **Assets/Models/** - Modelli 3D
- **Assets/Animations/** - Animazioni

### Componenti Principali

#### Sistema a Turni
- `TurnSystem.cs` - Gestisce l'alternanza dei turni tra giocatore e nemici
- Eventi che notificano i cambiamenti di turno per sincronizzare le varie componenti

#### Sistema di Griglia
- `Grid/GridSystem.cs` - Sistema di coordinate basato su celle
- `Grid/GridPosition.cs` - Rappresentazione delle coordinate logiche
- `PathNode.cs` e `Pathfinding.cs` - Algoritmo A* per la navigazione

#### Sistema di Azioni
- `Actions/BaseAction.cs` - Classe base per tutte le azioni
- Azioni specifiche:
  - `Actions/MoveAction.cs` - Movimento delle unità
  - `Actions/ShootAction.cs` - Attacco a distanza
  - `Actions/SwordAction.cs` - Attacco corpo a corpo
  - `Actions/GrenadeAction.cs` - Lancio di granate con effetti ad area
  - `Actions/InteractAction.cs` - Interazione con oggetti dell'ambiente
  - `Actions/SpinAction.cs` - Rotazione dell'unità

#### Gestione delle Unità
- `Unit.cs` - Rappresentazione delle unità con statistiche e collezione di azioni disponibili
- `UnitManager.cs` - Gestione di tutte le unità nel campo di battaglia
- `UnitActionSystem.cs` - Sistema di selezione e esecuzione delle azioni
- `UnitAnimator.cs` - Gestione delle animazioni delle unità

#### AI dei Nemici
- `EnemyAI.cs` - Logica di comportamento per i nemici controllati dal computer
- `EnemyAIAction.cs` - Struttura per definire le azioni dell'IA

#### Input e Camera
- `InputManager.cs` - Gestione input da tastiera e mouse
- `TouchInputManager.cs` - Supporto per input touch (mobile)
- `CameraController.cs` e `CameraManager.cs` - Gestione della camera di gioco

#### Interazioni con l'Ambiente
- `IInteractable.cs` - Interfaccia per oggetti interattivi
- `Door.cs`, `BarrelInteract.cs`, `DestructibleCrate.cs` - Oggetti interattivi

## Pattern di Progettazione Utilizzati

1. **Singleton** - Utilizzato per manager globali come `UnitActionSystem`, `TurnSystem`
2. **Observer** - Sistema di eventi per notificare cambiamenti di stato (turni, selezione unità)
3. **Command** - Implementazione delle azioni delle unità
4. **Factory** - Creazione di componenti di gioco
5. **State Machine** - Gestione degli stati delle unità e dell'IA

## Requisiti di Sistema

- Unity 2022.3.55f1 o superiore
- .NET Framework 4.x

## Setup del Progetto

1. Clona la repository : git clone https://github.com/crasherbit/demon-doof-unity.git
2. Apri il progetto in Unity 2022.3.55f1
3. Apri la scena principale in `Assets/Scenes/GameScene.unity`

## Comandi di Gioco

- **Clic sinistro** - Seleziona unità/esegui azione
- **Clic destro** - Annulla selezione
- **WASD/Frecce** - Movimento camera
- **Q/E** - Rotazione camera
- **Rotellina mouse** - Zoom camera
- **Spazio** - Fine turno

## Piano di Implementazione Multiplayer con FishNet

### Fase 1: Implementazione Base (2-3 settimane)
- Installazione e configurazione di FishNet
- Refactoring di TurnSystem per supportare più giocatori
- Conversione delle unità in NetworkObjects
- Adattamento del sistema di azioni per il multiplayer
- Creazione di un sistema di lobby base

### Fase 2: Backend e Sistemi di Supporto (3-4 settimane)
- Sistema di autenticazione
- Persistenza dei dati e salvataggio partite
- Matchmaking e configurazione server dedicati

### Fase 3: Integrazione e Rifinitura (2-3 settimane)
- Integrazione completa frontend-backend
- Ottimizzazione delle prestazioni di rete
- Sicurezza e funzionalità sociali
- Polishing e preparazione al lancio

## Considerazioni Tecniche per il Multiplayer

1. **Sincronizzazione Deterministica**
   - Assicurare che la logica di gioco produca risultati identici su tutti i client
   - Utilizzare numeri random con seed condiviso
   - Minimizzare effetti physics imprevedibili

2. **Gestione Connessioni**
   - Implementare reconnect automatico
   - Gestire gracefully la perdita di connessione
   - Prevedere meccaniche di "stand-in" per giocatori disconnessi

3. **Scalabilità**
   - Progettare per supportare la crescita degli utenti
   - Implementare logging e analytics
   - Considerare soluzioni serverless per alcuni componenti

## Prossimi Passi Specifici per l'Implementazione

1. **Setup Iniziale di FishNet**
   - Installazione del package FishNet da Unity Asset Store
   - Configurazione di NetworkManager nella scena principale
   - Creazione di scena di connessione/lobby

2. **Adattamento dei Core Systems**
   - Modifica del `TurnSystem.cs` per supportare turni multiplayer
   - Aggiunta di NetworkBehaviour ai principali manager
   - Sincronizzazione del GridSystem tra client

3. **Networking delle Unità e Azioni**
   - Conversione delle unit in NetworkObjects
   - Implementazione di RPCs per le azioni
   - Gestione dell'autorità client/server per le azioni

4. **Sistema di Lobby e Matchmaking**
   - UI per creazione/join delle partite
   - Implementazione di room e matchmaking
   - Gestione delle disconnessioni

5. **Testing e Ottimizzazione**
   - Test con varie condizioni di rete
   - Ottimizzazione della bandwidth usage
   - Minimizzazione della latenza percepita

## Contribuire

Le pull request sono benvenute. Per modifiche importanti, apri prima un issue per discutere cosa vorresti cambiare.

## Licenza

[MIT](LICENSE)
