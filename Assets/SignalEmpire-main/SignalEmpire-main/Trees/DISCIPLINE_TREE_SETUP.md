# Discipline Tree System Setup

## Overview
The late-game discipline tree system provides four distinct upgrade paths with 9 nodes each. Players earn Power Points (PP) through signal processing and spend them combined with secondary materials to unlock nodes.

## File Structure

### Core Classes

**DisciplineNode.cs**
- ScriptableObject representing individual tech nodes
- Fields: name, discipline path, type, description, costs, prerequisites, unlock state
- Methods: `ArePrerequisitesMet()` - validates prerequisite chain

**DisciplineTreeManager.cs**
- Handles node unlock logic and cost validation
- Method: `UnlockNode(node)` - validates costs and applies mechanical effects
- Method: `ApplyNodeMechanicalEffect(node)` - maps node names to engine modifiers

**DisciplineTreeInitializer.cs** *(NEW)*
- Programmatically creates all 36 nodes (9 per path × 4 paths)
- Automatically sets up prerequisite relationships
- Method: `InitializeAllTrees()` - creates and links all nodes

**DisciplineTreeUI.cs** *(NEW)*
- UI layer for displaying and interacting with trees
- Method: `GetNodesByPath(path)` - returns nodes organized by type
- Method: `TryUnlockNode(node)` - validates and processes unlocks
- Method: `GetNodeDisplayInfo(node)` - formats node data for UI display
- Method: `DebugPrintTree(path)` - console debug output

## Tree Structure

### Path of Power (Thermal/Mining Focus)
```
Entry: Induction Coil (+10% Data)
├─ Branch A: Heavy Drills (+15% Mineral Yield)
│  └─ Deep Vein Mining (Pure Mineral Drops)
├─ Branch B: Thermal Pipes (+15% Heat Capacity)
│  └─ Heat Recycling (Heat → Flux Conversion)
├─ Utility: Automated Routing (-10% Signal Travel Time)
├─ Merge A+Utility: Industrial Overdrive (2x Output with Flux)
├─ Merge B+Utility: Stable Amps (3x Amplifier Boost)
└─ Mastery: The Forge Mastery (Infinite 100% Heat)
```

### Path of Clarity (Noise Reduction Focus)
```
Entry: Static Grounding (-10% Noise Floor)
├─ Branch A: Precision Lens (+10% High-SNR Chance)
│  └─ Signal Isolation (Prevent Noise Spikes)
├─ Branch B: Harmonic Tuning (Frequency Match Multiplier)
│  └─ Pattern Filtration (Auto Remove Noise Loops)
├─ Utility: Atmospheric Buffering (-50% Decay)
├─ Merge A+Utility: Sub-Zero Processing (+2% Data per Heat Reduction)
├─ Merge B+Utility: Vacuum Synthesis (SNR Multiplier)
└─ Mastery: Zero-Floor Protocol (Data Squared if Noise = 0)
```

### Path of Logic (Information Density Focus)
```
Entry: Recursive Indexing (+20% Info Value)
├─ Branch A: Fragment Analysis (+15% Fragment Drop Rate)
│  └─ Heuristic Learning (Stacking Value Bonuses)
├─ Branch B: Prime Sequence Detection (5x Math Signal Multiplier)
│  └─ Dictionary Encoding (+10% Compression Efficiency)
├─ Utility: Logic-Gate Optimization (Reduce VC Pipeline Slot Cost)
├─ Merge A+Utility: Fractal Mapping (Non-Linear Multipliers)
├─ Merge B+Utility: Lossless Mastery (No Compression Cap)
└─ Mastery: Singularity Compression (Compress to Single Infinite Bit)
```

### Path of Discovery (Rare Signal Focus)
```
Entry: Wide-Band Sweep (+20% Rare Source Discovery Speed)
├─ Branch A: Xeno-Archaeology (+15% Ancient Schematics Value)
│  └─ Deep Void Scanning (Unlock Tier 4 Signals)
├─ Branch B: Fragment Synthesis (Craft Missing Fragments)
│  └─ Blueprint Stabilization (Reduce Fragment Requirements)
├─ Utility: Void Credit Siphoning (10% Data → VC Conversion)
├─ Merge A+Utility: Chrono-Tuning (Re-Read Missed Patterns)
├─ Merge B+Utility: Interstellar Networking (+5% Planetary Data)
└─ Mastery: Galactic Beacon (Rare Signals Seek This Planet)
```

## Integration Steps

### 1. Scene Setup
Add these components to your main game manager GameObject:
- `DisciplineTreeInitializer` 
- `DisciplineTreeUI`
- `DisciplineTreeManager`

Wire up references in the Inspector:
- `DisciplineTreeUI` → TreeInitializer, TreeManager, ResourceStorage, SignalEngine
- `DisciplineTreeManager` → SignalEngine, ResourceStorage

### 2. Initialize on Game Start
Call from your main menu or game boot sequence:
```csharp
disciplineTreeUI.InitializeTreeUI();
```

### 3. Display Trees in UI
Get nodes organized by path:
```csharp
var nodesForPower = disciplineTreeUI.GetNodesByPath(DisciplinePath.Power);
// nodesForPower[NodeType.Entry], nodesForPower[NodeType.BranchA], etc.
```

### 4. Handle Node Unlocks
When player clicks a node button:
```csharp
bool success = disciplineTreeUI.TryUnlockNode(node);
if (success) {
    // Update UI to show unlocked state
    UpdateNodeDisplay(node);
}
```

### 5. Display Node Info
Format node info for tooltips:
```csharp
string info = disciplineTreeUI.GetNodeDisplayInfo(node);
// Display in UI tooltip
```

## Cost Structure

### Power Points (Universal Cost)
| Path | Entry | Branch | Utility | Merge | Mastery |
|------|-------|--------|---------|-------|---------|
| All | 50 PP | 75 PP | 80 PP | 150 PP | 250 PP |

### Secondary Materials
Prices vary by path and node rarity. Materials required:
- **Path of Power**: CryoQuartz, ObsidianFlux, Isotope9, VoidMatter, GravSalt, PrismDust, AetherGlass
- **Path of Clarity**: PrismDust, Neuralite, AetherGlass, CryoQuartz, Isotope9, ObsidianFlux, VoidMatter
- **Path of Logic**: Neuralite, GravSalt, Neuralite, PrismDust, VoidMatter, AetherGlass, Isotope9
- **Path of Discovery**: GravSalt, ObsidianFlux, PrismDust, VoidMatter, AetherGlass, CryoQuartz, Neuralite

## Debugging

### Print Tree Structure
```csharp
disciplineTreeUI.DebugPrintTree(DisciplinePath.Power);
```

### Check Node Status
```csharp
Debug.Log($"Unlocked: {node.isUnlocked}");
Debug.Log($"Prerequisites Met: {node.ArePrerequisitesMet()}");
Debug.Log($"Can Afford: {storage.CanAffordSpecial(node.requiredMaterial, node.materialAmount)}");
```

## Data Persistence

Trees are saved automatically via `SaveSystem`:
- Unlocked node states persist in PlayerPrefs
- Material inventory tied to secondary material storage
- Power points saved per cycle

## Extension Points

Add new nodes or modify existing ones:
1. Edit `DisciplineTreeInitializer.cs` `InitializePathOfX()` methods
2. Add corresponding case in `DisciplineTreeManager.cs` `ApplyNodeMechanicalEffect()`
3. Add new `SignalEngine` modifiers as needed
