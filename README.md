# SignalEngine Game Project: Onboarding & Design Document

Welcome to the team! This document serves as a high-level manual for understanding the purpose, progression flow, and core features of **SignalEngine**. It is designed to get a new team member up to speed on the player experience and game direction without getting bogged down in internal development specifics.

---

## 1. Game Vision & Narrative Theme

### The Core Concept

**SignalEngine** is an atmospheric, sci-fi incremental/idle game where players manage a deep-space intelligence grid. The player steps into the shoes of a lone signal analyst stationed at a monitoring terminal, tracking down mysterious transmissions across the cosmos.

### Visual & Experiential Direction

* **Technical Interface:** The UI mimics an authentic, dense telemetry terminal—complete with frequency sweeps, data streams, and hardware diagnostic dashboards.
* **Atmospheric Progression:** The player begins by manually filtering through crackling static noise on a single receiver. Over time, they clean up these frequencies, automate their arrays, and eventually scale up to monitoring entire planetary systems.
* **The Progression Payoff:** The game smoothly transitions from a hands-on, active terminal monitoring loop into a massive, self-sustaining network of automated macro-systems.

---

## 2. The Core Player Loop

The core gameplay centers around an ongoing loop of extraction, conversion, and structural reinvestment.

```
  [ Raw Frequencies ] ──> Intercepted via Terminal Sweeps
              │
              ▼
    [ Signal Data Packets ] ──> Processed & Amplified
              │
              ▼
     [ Power Points (PP) ] ──> Primary Currency Generated
              │
              ▼
   [ Macro-Expansion ] ──> Reinvested into Tech Trees, Planets, & Mines

```

1. **Intercepting:** The player's scanning equipment sweeps radio frequencies to capture raw signal packets.
2. **Refining:** These packets are processed, filtered, and amplified, turning data into our main progression resource: **Power Points (PP)**.
3. **Expanding:** Players spend their accumulated PP to upgrade their hardware, unlock operational specializations, or funding space exploration initiatives to discover entirely new resource layers.

---

## 3. Structural Progression: The Tree Matrices

Progression in the game splits into two parallel, permanent development paths that players interact with through the main interface.

### The Foundation Tree

This tree represents the player's direct **hardware and software grid upgrades**. Investing points here scales up the baseline capabilities of the monitoring station.

* **Signal Yields:** Amplifies the data value and PP output of captured transmissions.
* **Hardware Speed:** Accelerates scanning cycles, letting the terminal sweep through frequency bands much faster.
* **Automation Scripts:** Unlocks background capture assistants that automatically seek out and grab signals so the player doesn't have to click them manually.
* **Exotic Discovery:** Tunes filters to increase the appearance rate of high-value rare and anomalous signals.

### The Discipline Tree

Unlocked later in the game, the Discipline Tree represents specialized **operational frameworks**. Instead of just increasing raw numbers, this tree unlocks entirely new sub-systems and gameplay loops, specifically acting as the bridge that allows the player to command planetary operations and deep-space logistics.

---

## 4. Macro Expansion: Planets & Mining

Once players break past local satellite grid limitations, the scope of the game opens up to the cosmic stage, introducing a multi-tiered management layer.

### Planetary Exploration

Using high-powered deep-space scanning sequences, players spend their data capital to discover new planets.

* Each planet features its own unique atmospheric conditions and composition profile.
* Players can drop permanent **Atmospheric Signal Relays** onto these worlds, utilizing their unique positions in space to permanently multiply specific transmission types back at HQ.

### Deep-Space Mining Operations

Planets aren't just coordinates on a map—they contain deep mineral deposits that open up a parallel economic loop.

* **Automated Drilling Rigs:** Players purchase and place modular drilling rigs onto planetary surfaces to excavate rare ores over time.
* **The Refining Loop:** Raw ores are hauled up and funneled through automated refineries. The resulting refined materials are used directly to fuel and craft the absolute highest-tier upgrades found in the Discipline Tree.

---

## 5. Offline & Idle Design

As an idle game, **SignalEngine** highly values the player's time away from the screen. When the application is closed, the simulation transitions to a passive calculation state.

Upon launching the game again, players are greeted with an onboarding summary of what their systems achieved while they were gone:

* Automated collectors continue scanning and banking PP based on their active efficiency ratings.
* Planetary drilling rigs continue chewing through deep stone strata, accumulating raw ore stockpiles ready for immediate refining.

This ensures that returning to the game always feels rewarding, providing a surge of resources to fund the next wave of space exploration and technological development.
