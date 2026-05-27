using UnityEngine;
using System.Collections.Generic;

public class DisciplineTreeInitializer : MonoBehaviour
{
    [SerializeField] private List<DisciplineNode> disciplineNodes = new List<DisciplineNode>();

    public List<DisciplineNode> GetAllNodes() => disciplineNodes;

    public void InitializeAllTrees()
    {
        disciplineNodes.Clear();

        // Initialize Path of Power
        InitializePathOfPower();

        // Initialize Path of Clarity
        InitializePathOfClarity();

        // Initialize Path of Logic
        InitializePathOfLogic();

        // Initialize Path of Discovery
        InitializePathOfDiscovery();

        Debug.Log($"<color=cyan>Discipline Tree Initialized:</color> {disciplineNodes.Count} nodes loaded.");
    }

    private void InitializePathOfPower()
    {
        DisciplineNode inductionCoil = CreateNode("Induction Coil", DisciplinePath.Power, NodeType.Entry,
            "Lowers signal strength floor by 10%.", 50, Material.CryoQuartz, 5, null);
        disciplineNodes.Add(inductionCoil);

        DisciplineNode heavyDrills = CreateNode("Heavy Drills", DisciplinePath.Power, NodeType.BranchA,
            "Mineral yield increased by 15%.", 75, Material.ObsidianFlux, 8, new[] { inductionCoil });
        disciplineNodes.Add(heavyDrills);

        DisciplineNode thermalPipes = CreateNode("Thermal Pipes", DisciplinePath.Power, NodeType.BranchB,
            "Heat capacity multiplied by 1.15x.", 75, Material.Isotope9, 8, new[] { inductionCoil });
        disciplineNodes.Add(thermalPipes);

        DisciplineNode deepVeinMining = CreateNode("Deep Vein Mining", DisciplinePath.Power, NodeType.BranchA,
            "Unlocks Pure Mineral rare drops.", 100, Material.ObsidianFlux, 12, new[] { heavyDrills });
        disciplineNodes.Add(deepVeinMining);

        DisciplineNode heatRecycling = CreateNode("Heat Recycling", DisciplinePath.Power, NodeType.BranchB,
            "Every 10% Heat generates +1% Flux.", 100, Material.Isotope9, 12, new[] { thermalPipes });
        disciplineNodes.Add(heatRecycling);

        DisciplineNode automatedRouting = CreateNode("Automated Routing", DisciplinePath.Power, NodeType.Utility,
            "Signal Travel Time reduced by 10%.", 80, Material.VoidMatter, 10, new[] { inductionCoil });
        disciplineNodes.Add(automatedRouting);

        DisciplineNode industrialOverdrive = CreateNode("Industrial Overdrive", DisciplinePath.Power, NodeType.Merge,
            "Spend Flux to instantly double Mineral and Data output.", 150, Material.GravSalt, 15, 
            new[] { deepVeinMining, automatedRouting });
        disciplineNodes.Add(industrialOverdrive);

        DisciplineNode stableAmps = CreateNode("Stable Amps", DisciplinePath.Power, NodeType.Merge,
            "Amplifiers provide 3x boost with 0 Noise penalty.", 150, Material.PrismDust, 15, 
            new[] { heatRecycling, automatedRouting });
        disciplineNodes.Add(stableAmps);

        DisciplineNode theForgeMastery = CreateNode("The Forge Mastery", DisciplinePath.Power, NodeType.Mastery,
            "Pipeline can run at 100% Heat indefinitely without corruption.", 250, Material.AetherGlass, 25, 
            new[] { industrialOverdrive, stableAmps });
        disciplineNodes.Add(theForgeMastery);
    }

    private void InitializePathOfClarity()
    {
        DisciplineNode staticGrounding = CreateNode("Static Grounding", DisciplinePath.Clarity, NodeType.Entry,
            "Noise floor lowered by 10%.", 50, Material.PrismDust, 5, null);
        disciplineNodes.Add(staticGrounding);

        DisciplineNode precisionLens = CreateNode("Precision Lens", DisciplinePath.Clarity, NodeType.BranchA,
            "High-SNR signal discovery chance +10%.", 75, Material.Neuralite, 8, new[] { staticGrounding });
        disciplineNodes.Add(precisionLens);

        DisciplineNode harmonicTuning = CreateNode("Harmonic Tuning", DisciplinePath.Clarity, NodeType.BranchB,
            "Signal strength multiplied if it matches planet frequency.", 75, Material.AetherGlass, 8, 
            new[] { staticGrounding });
        disciplineNodes.Add(harmonicTuning);

        DisciplineNode signalIsolation = CreateNode("Signal Isolation", DisciplinePath.Clarity, NodeType.BranchA,
            "Prevents The Static from causing noise spikes.", 100, Material.Neuralite, 12, 
            new[] { precisionLens });
        disciplineNodes.Add(signalIsolation);

        DisciplineNode patternFiltration = CreateNode("Pattern Filtration", DisciplinePath.Clarity, NodeType.BranchB,
            "Automatically removes repeating noise loops.", 100, Material.AetherGlass, 12, 
            new[] { harmonicTuning });
        disciplineNodes.Add(patternFiltration);

        DisciplineNode atmosphericBuffering = CreateNode("Atmospheric Buffering", DisciplinePath.Clarity, NodeType.Utility,
            "Reduces Information Decay in the buffer by 50%.", 80, Material.CryoQuartz, 10, 
            new[] { staticGrounding });
        disciplineNodes.Add(atmosphericBuffering);

        DisciplineNode subZeroProcessing = CreateNode("Sub-Zero Processing", DisciplinePath.Clarity, NodeType.Merge,
            "Every 1% Heat reduction grants +2% Data bonus.", 150, Material.Isotope9, 15, 
            new[] { signalIsolation, atmosphericBuffering });
        disciplineNodes.Add(subZeroProcessing);

        DisciplineNode vacuumSynthesis = CreateNode("Vacuum Synthesis", DisciplinePath.Clarity, NodeType.Merge,
            "Increases SNR multiplier for all Filter modules.", 150, Material.ObsidianFlux, 15, 
            new[] { patternFiltration, atmosphericBuffering });
        disciplineNodes.Add(vacuumSynthesis);

        DisciplineNode zeroFloorProtocol = CreateNode("Zero-Floor Protocol", DisciplinePath.Clarity, NodeType.Mastery,
            "If Noise is 0, Data output is squared.", 250, Material.VoidMatter, 25, 
            new[] { subZeroProcessing, vacuumSynthesis });
        disciplineNodes.Add(zeroFloorProtocol);
    }

    private void InitializePathOfLogic()
    {
        DisciplineNode recursiveIndexing = CreateNode("Recursive Indexing", DisciplinePath.Logic, NodeType.Entry,
            "Base Information Value increased by 20%.", 50, Material.Neuralite, 5, null);
        disciplineNodes.Add(recursiveIndexing);

        DisciplineNode fragmentAnalysis = CreateNode("Fragment Analysis", DisciplinePath.Logic, NodeType.BranchA,
            "Blueprint Fragment drop rate +15%.", 75, Material.GravSalt, 8, new[] { recursiveIndexing });
        disciplineNodes.Add(fragmentAnalysis);

        DisciplineNode primeSequenceDetection = CreateNode("Prime Sequence Detection", DisciplinePath.Logic, NodeType.BranchB,
            "Automatically applies 5x multiplier to math signals.", 75, Material.Neuralite, 8, 
            new[] { recursiveIndexing });
        disciplineNodes.Add(primeSequenceDetection);

        DisciplineNode heuristicLearning = CreateNode("Heuristic Learning", DisciplinePath.Logic, NodeType.BranchA,
            "Signals of same type gain stacking value bonuses.", 100, Material.GravSalt, 12, 
            new[] { fragmentAnalysis });
        disciplineNodes.Add(heuristicLearning);

        DisciplineNode dictionaryEncoding = CreateNode("Dictionary Encoding", DisciplinePath.Logic, NodeType.BranchB,
            "Increases efficiency of all Compression modules.", 100, Material.Neuralite, 12, 
            new[] { primeSequenceDetection });
        disciplineNodes.Add(dictionaryEncoding);

        DisciplineNode logicGateOptimization = CreateNode("Logic-Gate Optimization", DisciplinePath.Logic, NodeType.Utility,
            "Reduces VC cost of unlocking new Pipeline slots.", 80, Material.PrismDust, 10, 
            new[] { recursiveIndexing });
        disciplineNodes.Add(logicGateOptimization);

        DisciplineNode fractalMapping = CreateNode("Fractal Mapping", DisciplinePath.Logic, NodeType.Merge,
            "Unlocks non-linear multipliers for complex alien signals.", 150, Material.VoidMatter, 15, 
            new[] { heuristicLearning, logicGateOptimization });
        disciplineNodes.Add(fractalMapping);

        DisciplineNode losslessMastery = CreateNode("Lossless Mastery", DisciplinePath.Logic, NodeType.Merge,
            "Compression modules no longer have max multiplier cap.", 150, Material.AetherGlass, 15, 
            new[] { dictionaryEncoding, logicGateOptimization });
        disciplineNodes.Add(losslessMastery);

        DisciplineNode singularityCompression = CreateNode("Singularity Compression", DisciplinePath.Logic, NodeType.Mastery,
            "Compress any signal into a single bit of infinite value.", 250, Material.Isotope9, 25, 
            new[] { fractalMapping, losslessMastery });
        disciplineNodes.Add(singularityCompression);
    }

    private void InitializePathOfDiscovery()
    {
        DisciplineNode wideBandSweep = CreateNode("Wide-Band Sweep", DisciplinePath.Discovery, NodeType.Entry,
            "Speed for finding new rare signal sources +20%.", 50, Material.GravSalt, 5, null);
        disciplineNodes.Add(wideBandSweep);

        DisciplineNode xenoArchaeology = CreateNode("Xeno-Archaeology", DisciplinePath.Discovery, NodeType.BranchA,
            "Increases value of Ancient Schematics drops.", 75, Material.ObsidianFlux, 8, 
            new[] { wideBandSweep });
        disciplineNodes.Add(xenoArchaeology);

        DisciplineNode fragmentSynthesis = CreateNode("Fragment Synthesis", DisciplinePath.Discovery, NodeType.BranchB,
            "Use Flux and Minerals to create missing Fragments.", 75, Material.PrismDust, 8, 
            new[] { wideBandSweep });
        disciplineNodes.Add(fragmentSynthesis);

        DisciplineNode deepVoidScanning = CreateNode("Deep Void Scanning", DisciplinePath.Discovery, NodeType.BranchA,
            "Unlocks Tier 4 signal types (inter-dimensional).", 100, Material.ObsidianFlux, 12, 
            new[] { xenoArchaeology });
        disciplineNodes.Add(deepVoidScanning);

        DisciplineNode blueprintStabilization = CreateNode("Blueprint Stabilization", DisciplinePath.Discovery, NodeType.BranchB,
            "Reduces total Fragments needed to unlock local modules.", 100, Material.PrismDust, 12, 
            new[] { fragmentSynthesis });
        disciplineNodes.Add(blueprintStabilization);

        DisciplineNode voidCreditSiphoning = CreateNode("Void Credit Siphoning", DisciplinePath.Discovery, NodeType.Utility,
            "Converts 10% of extracted Data directly into VC.", 80, Material.VoidMatter, 10, 
            new[] { wideBandSweep });
        disciplineNodes.Add(voidCreditSiphoning);

        DisciplineNode chronoTuning = CreateNode("Chrono-Tuning", DisciplinePath.Discovery, NodeType.Merge,
            "Allows a signal to be re-read if a pattern was missed.", 150, Material.AetherGlass, 15, 
            new[] { deepVoidScanning, voidCreditSiphoning });
        disciplineNodes.Add(chronoTuning);

        DisciplineNode interstellarNetworking = CreateNode("Interstellar Networking", DisciplinePath.Discovery, NodeType.Merge,
            "Boosts Data output of all other owned planets by 5%.", 150, Material.CryoQuartz, 15, 
            new[] { blueprintStabilization, voidCreditSiphoning });
        disciplineNodes.Add(interstellarNetworking);

        DisciplineNode galacticBeacon = CreateNode("Galactic Beacon", DisciplinePath.Discovery, NodeType.Mastery,
            "The rarest signals in the universe actively seek this planet.", 250, Material.Neuralite, 25, 
            new[] { chronoTuning, interstellarNetworking });
        disciplineNodes.Add(galacticBeacon);
    }

    private DisciplineNode CreateNode(string name, DisciplinePath path, NodeType type, 
        string description, int ppCost, Material material, int materialAmount, DisciplineNode[] prerequisites)
    {
        DisciplineNode node = ScriptableObject.CreateInstance<DisciplineNode>();
        node.nodeName = name;
        node.discipline = path;
        node.type = type;
        node.description = description;
        node.ppCost = ppCost;
        node.requiredMaterial = material;
        node.materialAmount = materialAmount;
        node.prerequisites = prerequisites ?? new DisciplineNode[0];
        node.isUnlocked = false;
        return node;
    }
}
