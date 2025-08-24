# Heuristic Agent Project Folder Structure

This document outlines the canonical folder structure for the heuristic agent uploads.

## Directory Structure

```
Heuristic-Agent-Project/
├── src/                 # All source code scripts
│   ├── WaypointKartAI.cs
│   ├── ArcadeKart.cs
│   └── core logic, controller/AI code
├── config/              # Parameter/configuration files
│   ├── agent parameters (YAML/JSON)
│   ├── track configurations
│   └── simulation test parameters
├── logs/                # Output logs and debug information
│   └── performance run logs
├── results/             # Analysis and metrics exports
│   ├── lap_times.csv
│   ├── collision_rates.json
│   ├── per_lap_metrics.csv
│   └── experiment_summaries.json
├── assets/              # Scene-specific assets
│   ├── sample Unity prefabs
│   ├── demo waypoints
│   └── small reference files (no large binaries)
├── tests/               # Unit tests and integration tests
│   └── heuristic logic test scripts
└── docs/                # Additional documentation
    ├── setup_notes.md
    ├── diagrams/
    └── agent-specific documentation
```

