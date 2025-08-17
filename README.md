# SPEED-Intelligent-Racing-Agents

This repository documents the design, implementation, and quantitative benchmarking of AI for autonomous racing in Unity, featuring a **deterministic waypoint-based heuristic controller** and a **deep reinforcement learning (DRL) agent** with Unity ML-Agents. The research systematically benchmarks classic rule-based AI versus modern DRL in a controlled, fully reproducible Unity simulation environment.

---

## 🚗 Project Overview
- **Heuristic Agent**: Classical, fully-deterministic waypoint-following strategy with curvature-adaptive speed and jitter-free steering. Provides a robust rule-based baseline for autonomous racing in Unity (2021.3.45f1).
- **DRL Agent (PPO)**: Neural racing agent trained using Proximal Policy Optimization (PPO) via Unity ML-Agents. Includes reward shaping for both raw speed and human-like driving. Integrates ray-based perception, advanced progress, and smoothness incentives.
- **Human Benchmark**: Framework for human player evaluation in the same Unity simulation for direct apples-to-apples comparison on lap time and behavioral realism.

All code, configs, results, and experimental logs are provided to support full research reproducibility.

---

## 📂 Repository Structure
```
SPEED-Intelligent-Racing-Agents/
├── Heuristic-Agent-Project/     # Classic waypoint controller for Unity
│   ├── Assets/
│   ├── ProjectSettings/
│   └── Packages/
├── DRL-Agent-Project/           # ML-Agents DRL PPO/SAC agent & scenes
│   ├── Assets/
│   ├── ProjectSettings/
│   ├── Packages/
│   └── Trained-Models/
├── Results-and-Analysis/        # Logs, benchmarks, plots
├── Documentation/               # Detailed setup, research paper
│   └── Setup-Instructions.md
└── README.md
```

---

## ⚡ Quick Setup & Reproduction
### Prerequisites
- **Unity 2021.3.45f1 LTS** *(required for project compatibility)*
- **Python 3.8+**  (for retraining/running DRL agents)
- **ML-Agents** `v0.30.0`  (see docs for installation)
- **8GB+ RAM**, CUDA GPU recommended (for DRL)

### 1. Clone Repository
```sh
git clone https://github.com/Sujyeet/SPEED-Intelligent-Racing-Agents.git
cd SPEED-Intelligent-Racing-Agents
```

### 2. Open in Unity
- Open **Unity Hub**, select version **2021.3.45f1**
- Add and open either `Heuristic-Agent-Project/` or `DRL-Agent-Project/`
- Wait for Unity to import dependencies (must have Assets, ProjectSettings, Packages folders!)
- 
### 3. Python & ML-Agents
- To retrain/test the DRL agent:
```sh
python -m venv racing_env
# Windows:
racing_env\Scripts\activate
# Mac/Linux:
source racing_env/bin/activate
pip install -r requirements.txt
```

---

## ▶️ Running & Evaluating Agents
### Heuristic Agent
1. Open `Heuristic-Agent-Project` in Unity
2. Open a test scene (e.g., `Assets/Scenes/OvalTrack.unity`)
3. Press Play in the editor to watch the deterministic AI drive

---

### DRL Agent (PPO)
1. Open `DRL-Agent-Project` in Unity
2. Confirm ML-Agents package is installed (`com.unity.ml-agents@0.30.0`)
3. Load a DRL test scene & assign a `.onnx` model from `Trained-Models/` to the kart agent
4. Set Behavior Type to **Inference Only**
5. Press Play to evaluate

---

### Human Comparison
- Use a test scene that supports player input (see documentation)
- Keyboard/gamepad controls enabled for side-by-side comparison

---

### DRL Agent Retraining (Optional)
- Detailed steps, configs, and command-line training options are documented in `Documentation/Setup-Instructions.md`

---

## 📊 Experimental Results & Benchmarks

| Agent                       | Mean Lap Time (s) | Std Dev | Collision Rate | Human-Likeness |
|-----------------------------|------------------:|--------:|--------------:|:--------------|
| **Heuristic Agent**         |    41.52          |  0.09   | 0%            | Low            |
| **DRL Agent (PPO)**         |    39.8           |  1.4    | <2%           | High           |
| **DRL Agent (Humanlike)**   |    43.0           |  1.1    | <1%           | Very High      |
| **Human (Reference)**       |    44.2           |  2.8    | ~3%           | Baseline       |

- Metrics include: Lap time, completion rate, steering smoothness, lane adherence
- All experiment logs and performance plots are in `Results-and-Analysis/`

---

## 🔬 Technical Details
- **Heuristic AI**: Jitter suppression, predictive curvature-aware steering, parameterized for path smoothness.
- **DRL Architecture**: PPO with multi-input raycasts, reward shaping for speed, safety, and naturalness.
- **Robustness**: Evaluation on randomized seeds and alternate tracks for generalization.
- **Reproducibility**: All versions, configs, and models for repeatable results are included; see Setup-Instructions.md for full pipeline.

---
## 📄 Documentation
- **Paper Draft:** See `Documentation/SPEED-Dissertation.pdf` for full research writeup
- **Setup Guide:** `Documentation/Setup-Instructions.md` (detailed installer & troubleshooting)

---
## 📚 Citation
If you use part of this work or infrastructure in your research, please cite:
```bibtex
@mastersthesis{sujyeet2025speed,
  title={SPEED: Intelligent Racing Agents Using Deep Reinforcement Learning and Unity ML-Agents},
  author={Sujyeet, [Your Full Name]},
  year={2025},
  school={[Your University]},
  type={MSc Thesis}
}
```

---

## 📬 Contact / Issues
- **Author/Student:** Sujyeet ([your.email@domain])
- **Open Issues** for questions and support
- License: MIT

---
Ongoing Masters dissertation; continuous improvements & extensions will be pushed as research progresses.
