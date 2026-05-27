using UnityEngine;

public class DisciplineTreeManager : MonoBehaviour
{
    public SignalEngine engine;
    public ResourceStorage storage;

    public void UnlockNode(DisciplineNode node)
    {
        if (node.isUnlocked) return;
        if (!node.ArePrerequisitesMet()) return;

        // Check Costs
        if (engine.totalPowerPoints >= node.ppCost && storage.CanAffordSpecial(node.requiredMaterial, node.materialAmount))
        {
            engine.totalPowerPoints -= node.ppCost;
            storage.SpendSpecial(node.requiredMaterial, node.materialAmount);

            node.isUnlocked = true;
            ApplyNodeMechanicalEffect(node);
            Debug.Log($"<color=gold>{node.discipline} Upgrade:</color> {node.nodeName} Unlocked!");
        }
    }

    private void ApplyNodeMechanicalEffect(DisciplineNode node)
    {
        // Add specific logic for your unique nodes here
        // Path of Power tree structure:
        //  Entry: Induction Coil
        //    Branch A: Heavy Drills -> Deep Vein Mining
        //    Branch B: Thermal Pipes -> Heat Recycling
        //    Utility: Automated Routing
        //      Merge A+Utility: Industrial Overdrive
        //      Merge B+Utility: Stable Amps
        //    Mastery: The Forge Mastery
        // Path of Clarity tree structure:
        //  Entry: Static Grounding
        //    Branch A: Precision Lens -> Signal Isolation
        //    Branch B: Harmonic Tuning -> Pattern Filtration
        //    Utility: Atmospheric Buffering
        //      Merge A+Utility: Sub-Zero Processing
        //      Merge B+Utility: Vacuum Synthesis
        //    Mastery: Zero-Floor Protocol
        // Path of Logic tree structure:
        //  Entry: Recursive Indexing
        //    Branch A: Fragment Analysis -> Heuristic Learning
        //    Branch B: Prime Sequence Detection -> Dictionary Encoding
        //    Utility: Logic-Gate Optimization
        //      Merge A+Utility: Fractal Mapping
        //      Merge B+Utility: Lossless Mastery
        //    Mastery: Singularity Compression
        // Path of Discovery tree structure:
        //  Entry: Wide-Band Sweep
        //    Branch A: Xeno-Archaeology -> Deep Void Scanning
        //    Branch B: Fragment Synthesis -> Blueprint Stabilization
        //    Utility: Void Credit Siphoning
        //      Merge A+Utility: Chrono-Tuning
        //      Merge B+Utility: Interstellar Networking
        //    Mastery: Galactic Beacon
        switch (node.nodeName)
        {
            // Path of Power
            case "Induction Coil": engine.dataMult += 0.10; break;
            case "Heavy Drills": engine.mineralYieldMult += 0.15f; break;
            case "Thermal Pipes": engine.heatCapacityMult += 0.15f; break;
            case "Deep Vein Mining": engine.pureMineralDropsUnlocked = true; break;
            case "Heat Recycling": engine.heatToFluxRate += 0.10f; break;
            case "Automated Routing": engine.travelTimeReduction += 0.10f; break;
            case "Industrial Overdrive": engine.industrialOverdriveUnlocked = true; break;
            case "Stable Amps": engine.stableAmpsEnabled = true; engine.amplifierBoost = 3.0f; break;
            case "The Forge Mastery": engine.canRunAtFullHeat = true; engine.heatCorruptionEnabled = false; break;

            // Path of Clarity
            case "Static Grounding": engine.baseNoiseFloor -= 0.10f; break;
            case "Precision Lens": engine.highSNRChanceBonus += 0.10f; break;
            case "Harmonic Tuning": engine.frequencyMatchMultiplier += 1.0f; break;
            case "Signal Isolation": engine.preventsStaticSpikes = true; break;
            case "Pattern Filtration": engine.autoRemoveNoiseLoops = true; break;
            case "Atmospheric Buffering": engine.bufferDecayReduction += 0.50f; break;
            case "Sub-Zero Processing": engine.subZeroDataBonusPerHeatReduction = 0.02f; break;
            case "Vacuum Synthesis": engine.snrFilterBoost += 0.25f; break;
            case "Zero-Floor Protocol": engine.unlocksSquaredOutput = true; break;

            // Path of Logic
            case "Recursive Indexing": engine.infoValueMult += 0.20; break;
            case "Fragment Analysis": engine.blueprintFragmentDropRate += 0.15f; break;
            case "Prime Sequence Detection": engine.mathSignalMultiplier = 5.0f; break;
            case "Heuristic Learning": engine.stackingValueBonuses = true; break;
            case "Dictionary Encoding": engine.compressionEfficiency += 0.10f; break;
            case "Logic-Gate Optimization": engine.pipelineSlotCostReduction += 0.05f; break;
            case "Fractal Mapping": engine.nonLinearMultiplierUnlocked = true; break;
            case "Lossless Mastery": engine.compressionCapRemoved = true; break;
            case "Singularity Compression": engine.infiniteValueUnlocked = true; break;

            // Path of Discovery
            case "Wide-Band Sweep":
                engine.rareSourceDiscoverySpeed += 0.20f;
                PipelineManager.instance?.UnlockSignalSource("Deep Space");
                break;
            case "Xeno-Archaeology": engine.ancientSchematicsValue += 0.15f; break;
            case "Fragment Synthesis":
                engine.fragmentSynthesisEnabled = true;
                PipelineManager.instance?.UnlockSignalSource("Alien Encoding");
                break;
            case "Deep Void Scanning":
                engine.tier4SignalUnlocked = true;
                PipelineManager.instance?.UnlockSignalSource("Network Optimization");
                break;
            case "Blueprint Stabilization": engine.fragmentCostReduction += 0.10f; break;
            case "Void Credit Siphoning": engine.vcSiphonRate = 0.10f; break;
            case "Chrono-Tuning": engine.chronoTuningEnabled = true; break;
            case "Interstellar Networking": engine.planetaryDataBoost += 0.05f; break;
            case "Galactic Beacon":
                engine.rareSignalChance = 0.50f;
                PipelineManager.instance?.UnlockSignalSource("Universal Constants");
                break;
        }
    }
}