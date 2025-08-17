# SPEED: MSc Intelligent Racing Agents (Unity ML-Agents)

**A Masters research project on autonomous racing with Unity 2021.3.45f1, Deep RL (PPO/SAC), and classic heuristics.**

---
### About
This project implements and analyzes intelligent racing agents using both classic heuristic methods and Deep Reinforcement Learning. All experiments are conducted in Unity (2021.3.45f1 LTS) using ML-Agents Toolkit. Code, results, and guides are provided for MSc research and reproducibility.

---

## Project Structure

```
SPEED-Intelligent-Racing-Agents/
├── Unity-Environment/        # Unity project (2021.3.45f1)
│   ├── Assets/
│   ├── ProjectSettings/
│   └── Packages/
├── Python-Training/          # DRL training & evaluation (PyTorch, SB3)
│   ├── ppo_agent.py
│   ├── sac_agent.py
│   └── ...
├── Heuristic-Agent-Project/  # Classic rule-based agent (Python)
│   └── ...
├── Results-and-Analysis/
└── README.md
```

---

## Quick Setup

**Requirements:**
- Unity Hub + Unity 2021.3.45f1 LTS   
- Python 3.8+  
- pip (install dependencies)

### 1. Clone this repo
```
git clone https://github.com/Sujyeet/SPEED-Intelligent-Racing-Agents.git
cd SPEED-Intelligent-Racing-Agents
```

### 2. Python Environment
```
python -m venv racing_env
# Activate (Windows)
racing_env\Scripts\activate
# Activate (Linux/macOS)
source racing_env/bin/activate
pip install -r requirements.txt
```

### 3. Open in Unity (Editor v2021.3.45f1 ONLY)
- Open Unity Hub
- Click 'Add project' and select `Unity-Environment/` folder
- Wait for dependencies to import

---

## Running/Training Agents

### Training PPO (Unity ML-Agents)
```
cd Python-Training
mlagents-learn training/ppo_config.yaml --run-id=ppo_racing
```

### Training SAC
```
mlagents-learn training/sac_config.yaml --run-id=sac_racing
```

### Run Heuristic Agent (Python)
```
cd Heuristic-Agent-Project
python run_heuristic.py --track oval --episodes 100
```

---

## Tips & Notes
- **Unity folders**: Only `Assets`, `ProjectSettings`, and `Packages` are required for others to open/test your Unity project.
- **Tested on**: Windows 10/11, Unity 2021.3.45f1, CUDA GPU (optional for DRL speed).
- **Issues?** Open an Issue on this repo!

---

## Academic Context
- **Author:** Sujyeet
- **Degree:** MSc in [Your MSc Program]
- **University:** [Your University]
- **Supervisor:** [Supervisor Name]
- **Contact:** [your.email@university.edu]

Please cite if using in research:
```bibtex
@mastersthesis{sujyeet2025speed,
  title={SPEED: Intelligent Racing Agents Using Deep Reinforcement Learning and Unity ML-Agents},
  author={Sujyeet, [Full Name]},
  year={2025},
  school={[University Name]},
  type={MSc Thesis}
}
```

---
License: MIT. Ongoing project for academic use. Pull requests welcome!

---
