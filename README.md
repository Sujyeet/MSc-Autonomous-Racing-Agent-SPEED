# SPEED: Intelligent Racing Agents - Comparative Analysis of Deep Reinforcement Learning and Heuristic Controllers

This repository presents a comprehensive empirical study comparing deep reinforcement learning (DRL) agents with deterministic heuristic controllers in autonomous racing environments. The research provides a rigorous benchmarking framework for evaluating classical versus modern AI approaches in high-speed sequential decision-making tasks, with full experimental reproducibility for academic research.

## Research Motivation and Objectives

Autonomous racing represents a challenging testbed for intelligent systems, requiring integration of real-time perception, sequential decision-making, and adaptive control under dynamic constraints. This study addresses fundamental research questions in autonomous agent design:

- **Performance Trade-offs**: Under what conditions do engineered heuristic controllers outperform learned policies, and vice versa?
- **Learning Efficiency**: How do different reward structures and neural architectures affect DRL agent convergence, stability, and generalization capabilities?
- **Behavioral Fidelity**: Can deep reinforcement learning agents achieve human-like racing behaviors while maintaining competitive performance?
- **Reproducibility**: How can controlled experimental frameworks enable reliable comparison between classical and modern AI approaches?

## Key Contributions

### Deterministic Heuristic Controller
A fully deterministic waypoint-following agent employing adaptive speed control and heading correction algorithms. The controller demonstrates consistent lap completion with parameterized behavior for comparative analysis against learned policies.

### Deep Reinforcement Learning Agent
A Proximal Policy Optimization (PPO) based agent utilizing multi-modal sensor inputs (raycast-derived environmental perception) with curriculum learning protocols. Multiple architectural configurations were evaluated through systematic ablation studies.

### Controlled Experimental Framework
Rigorous benchmarking methodology incorporating statistical significance testing, multi-seed evaluation protocols, and standardized performance metrics for objective comparison between agent types.

### Human Performance Baseline
Integrated human demonstration collection system enabling direct behavioral comparison through quantitative metrics including lap times, steering smoothness, and trajectory deviation analysis.

## Methodology and Experimental Design

### Controlled Testing Environment
All evaluations conducted on standardized racing tracks with fixed random seeds to ensure reproducible results. Performance metrics include lap completion times, collision rates, path efficiency, and behavioral similarity indices measured across multiple evaluation episodes.

### Agent Architecture Comparison

**Heuristic Controller Architecture:**
- Waypoint-based path planning with PID control loops
- Adaptive speed regulation based on track curvature analysis
- Deterministic decision-making with parameter sensitivity analysis

**DRL Agent Architecture:**
- PPO algorithm with customizable neural network depth (2-layer/128 units vs 3-layer/256 units)
- Multi-dimensional observation space incorporating raycast sensors and velocity vectors
- Reward function ablation study comparing complex multi-term rewards against simplified progress-based rewards

### Systematic Ablation Studies

The research conducted comprehensive ablation studies across multiple dimensions:

1. **Reward Structure Analysis**: Comparison between complex multi-term reward functions versus simplified progress-based rewards
2. **Neural Architecture Evaluation**: Performance impact of network depth and width on learning stability
3. **Curriculum Learning Effects**: Analysis of training progression from simple to complex track layouts
4. **Generalization Assessment**: Cross-track evaluation to measure out-of-distribution performance

## Results Summary

### Performance Comparison (Representative Data)

| Agent Type | Mean Lap Time (s) | Std Deviation | Collision Rate | Human Similarity Score |
|------------|------------------|---------------|----------------|----------------------|
| Heuristic Controller | 41.52 | 0.09 | 0% | Low |
| PPO Agent (Baseline) | 39.81 | 1.40 | <2% | High |
| PPO Agent (Human-like) | 43.01 | 1.10 | <1% | Very High |
| Human Baseline | 44.20 | 2.80 | ~3% | Reference |

### Key Experimental Findings

**Learning Efficiency**: Simplified reward structures and lightweight neural architectures consistently demonstrated superior training stability and convergence rates compared to complex multi-term rewards and deep networks.

**Performance Trade-offs**: DRL agents achieved faster lap times than both heuristic controllers and human players while maintaining collision rates below 2%. However, heuristic controllers provided perfect reliability with zero variability.

**Behavioral Analysis**: Quantitative analysis of steering smoothness and trajectory deviation revealed DRL agents could closely approximate human-like racing behavior while exceeding human performance in terms of consistency and speed.

**Generalization Capabilities**: Cross-track evaluation demonstrated that DRL agents trained with curriculum learning showed significantly better adaptation to novel track layouts compared to fixed-parameter heuristic controllers.

## Technical Implementation

### Development Environment
- **Game Engine**: Unity 2021.3.45f1 LTS
- **ML Framework**: Unity ML-Agents 0.30.0 (PPO implementation)
- **Programming Languages**: C# (Unity scripting), Python 3.8+
- **Hardware Requirements**: 8GB+ RAM, CUDA-compatible GPU recommended for DRL training
- **Analysis Tools**: TensorBoard for training visualization, custom Python scripts for statistical analysis

### Repository Structure
```
SPEED-Intelligent-Racing-Agents/
├── Heuristic-Agent-Project/     # Deterministic controller implementation
│   ├── Assets/                  # Unity scenes, scripts, and prefabs
│   ├── ProjectSettings/         # Unity configuration files
│   └── Packages/               # Unity package dependencies
├── DRL-Agent-Project/          # Deep reinforcement learning implementation
│   ├── Assets/                 # ML-Agents scenes and training environments
│   ├── Trained-Models/         # Pre-trained .onnx model files
│   └── ProjectSettings/        # Unity ML-Agents configuration
├── Results-and-Analysis/       # Experimental data and statistical analysis
│   ├── Performance-Logs/       # Raw performance data (CSV format)
│   └── Statistical-Analysis/   # Python analysis scripts and plots
├── Documentation/              # Research documentation and setup guides
│   ├── SPEED-Dissertation.pdf # Complete research paper
│   └── Setup-Instructions.md  # Detailed reproduction guide
└── requirements.txt           # Python dependencies specification
```

## Reproducibility and Setup

### Prerequisites
- Unity 2021.3.45f1 LTS (exact version required for compatibility)
- Python 3.8+ with pip package manager
- Unity ML-Agents 0.30.0 (`pip install mlagents==0.30.0`)

### Installation Protocol
1. **Repository Cloning**:
   ```bash
   git clone https://github.com/Sujyeet/SPEED-Intelligent-Racing-Agents.git
   cd SPEED-Intelligent-Racing-Agents
   ```

2. **Unity Project Setup**:
   - Import either Heuristic-Agent-Project or DRL-Agent-Project in Unity Hub
   - Ensure all package dependencies are resolved during initial import
   - Verify scene loading and agent prefab configurations

3. **Python Environment Configuration** (for DRL training):
   ```bash
   python -m venv racing_env
   source racing_env/bin/activate  # Linux/Mac
   # racing_env\Scripts\activate   # Windows
   pip install -r requirements.txt
   ```

### Experimental Reproduction

**Heuristic Agent Evaluation**:
- Load designated race scenes in Heuristic-Agent-Project
- Execute evaluation runs with logged performance metrics
- All parameters documented for exact reproduction

**DRL Agent Assessment**:
- Utilize pre-trained models from Trained-Models/ directory
- Configure inference mode for evaluation scenarios
- Training reproduction via documented hyperparameters and scripts

**Statistical Analysis**:
- Execute analysis scripts in Results-and-Analysis/ for result validation
- All experimental data and analysis code provided for verification

## Future Research Directions

This framework establishes foundation for several research extensions:

- **Multi-Agent Racing**: Competitive and cooperative multi-agent scenarios
- **Transfer Learning**: Domain adaptation between different racing environments
- **Explainable AI**: Interpretability analysis of learned racing policies
- **Real-World Validation**: Sim-to-real transfer studies with physical racing platforms

## Resources and Documentation

### Academic Resources
- **Complete Research Paper**: `Documentation/SPEED-Dissertation.pdf`
- **Detailed Setup Guide**: `Documentation/Setup-Instructions.md`
- **Raw Experimental Data**: `Results-and-Analysis/Performance-Logs/`
- **Statistical Analysis Scripts**: `Results-and-Analysis/Statistical-Analysis/`

### Reproducibility Guarantee
All experiments utilize documented random seeds and configuration files. Every result presented in the accompanying research paper can be reproduced using the provided codebase and documented procedures.

## Citation

If this research contributes to your work, please cite:

```bibtex
@mastersthesis{sujyeet2025speed,
  title={SPEED: Intelligent Racing Agents Using Deep Reinforcement Learning and Heuristic Controllers - A Comparative Analysis},
  author={Sujyeet [Full Name]},
  year={2025},
  school={[University Name]},
  type={MSc Thesis},
  note={Available at: https://github.com/Sujyeet/SPEED-Intelligent-Racing-Agents}
}
```

## Author Information

**Principal Investigator**: Sujyeet [Full Name]  
**Institution**: [University Name]  
**Academic Level**: MSc Research Project  
**Contact**: [academic.email@domain.edu]  

For technical inquiries, experimental reproduction support, or academic collaboration, please open a GitHub issue or contact via institutional email.

## License

This research project and all associated code, data, and documentation are released under the MIT License to facilitate academic reproduction and collaborative research. See LICENSE file for complete terms.
