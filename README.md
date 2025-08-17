# MSc Autonomous Racing Agent SPEED

## Introduction

This repository presents the comprehensive implementation and research for an MSc project exploring autonomous racing agents through two distinct methodological approaches: traditional heuristic algorithms and modern Deep Reinforcement Learning (DRL) techniques. The research addresses fundamental questions in autonomous vehicle control within high-performance racing environments, where split-second decision-making, precise control, and adaptive strategies are paramount.

The project emerged from the growing intersection of autonomous vehicle research and high-performance motorsports, where traditional control theory meets cutting-edge machine learning. By developing and rigorously comparing both heuristic and learning-based approaches, this work contributes to our understanding of when interpretable rule-based systems excel versus when adaptive neural approaches provide superior performance.

## Research Context & Objectives

### Primary Research Questions
- **Performance Comparison**: How do traditional heuristic approaches compare against modern DRL techniques across diverse racing scenarios?
- **Interpretability vs. Adaptability**: What are the fundamental trade-offs between explainable heuristic methods and adaptive learning-based systems?
- **Generalization**: Which approach demonstrates better transferability across different track configurations and environmental conditions?
- **Computational Efficiency**: How do the computational requirements and real-time performance constraints differ between methodologies?

### Academic Contributions
- Systematic comparison framework for autonomous racing agents
- Novel heuristic algorithms optimized for racing scenarios
- Custom DRL architectures incorporating temporal reasoning for sequential decision-making
- Comprehensive evaluation methodology with statistical validation
- Open-source implementations enabling reproducible research

## 🏗️ Project Architecture

```
MSc-Autonomous-Racing-Agent-SPEED/
├── 📁 Heuristic-Agent-Project/          # Rule-based agent implementation
│   ├── README-Heuristic.md             # Detailed heuristic methodology
│   ├── src/                             # Core algorithms & control logic
│   ├── config/                          # Parameter configurations
│   └── tests/                           # Unit and integration tests
│
├── 📁 DRL-Agent-Project/                # Deep Reinforcement Learning agent
│   ├── README-DRL.md                    # DRL methodology & architecture
│   ├── src/                             # Neural network implementations
│   ├── training/                        # Training pipelines & utilities
│   ├── Trained-Models/                  # Model checkpoints & weights
│   └── evaluation/                      # Performance evaluation scripts
│
├── 📁 Documentation/                    # Comprehensive project documentation
│   ├── Setup-Instructions.md           # Environment setup & installation
│   ├── API-Reference.md                 # Code documentation & interfaces
│   ├── Research-Methods.md              # Methodology & experimental design
│   └── User-Guide.md                    # Usage examples & tutorials
│
├── 📁 Results-and-Analysis/             # Experimental results & analysis
│   ├── Performance-Metrics/             # Quantitative performance data
│   ├── Comparison-Studies/              # Statistical comparative analysis
│   ├── Visualizations/                  # Charts, plots, & visual analysis
│   └── Statistical-Analysis/            # Hypothesis testing & validation
│
├── 📄 requirements.txt                  # Python dependencies
├── 📄 setup.py                          # Package installation configuration
└── 📄 README.md                         # This documentation
```

## Installation & Setup

### Prerequisites
- **Python**: 3.8 or higher (3.9+ recommended for optimal performance)
- **Hardware**: CUDA-capable GPU strongly recommended for DRL training
- **System**: 8GB+ RAM, 10GB+ available disk space
- **Dependencies**: Git, pip, virtual environment support

### Quick Installation

1. **Clone & Navigate**
   ```bash
   git clone https://github.com/Sujyeet/MSc-Autonomous-Racing-Agent-SPEED.git
   cd MSc-Autonomous-Racing-Agent-SPEED
   ```

2. **Environment Setup**
   ```bash
   # Create virtual environment
   python -m venv racing_env
   
   # Activate environment
   # Linux/MacOS:
   source racing_env/bin/activate
   # Windows:
   racing_env\Scripts\activate
   ```

3. **Install Dependencies**
   ```bash
   # Install core requirements
   pip install -r requirements.txt
   
   # For development (optional)
   pip install -e .
   ```

4. **Verify Installation**
   ```bash
   python -c "import torch; print('PyTorch available:', torch.cuda.is_available())"
   ```

### Detailed Setup
For comprehensive installation instructions including environment-specific configurations, GPU setup, and troubleshooting, see [`Documentation/Setup-Instructions.md`](Documentation/Setup-Instructions.md).

## Agent Implementations

### 🧠 Heuristic Agent
The heuristic approach implements traditional control theory and algorithmic decision-making:

**Core Components:**
- **Path Planning**: A* pathfinding with dynamic obstacle avoidance
- **Speed Control**: Tuned PID controllers optimized for racing dynamics
- **Decision Logic**: Rule-based system prioritizing safety and performance
- **Sensor Processing**: Real-time LIDAR and camera data interpretation

**Advantages**: Interpretable decisions, consistent performance, fast execution, predictable behavior under known conditions.

### 🤖 Deep Reinforcement Learning Agent
The DRL approach leverages neural networks for adaptive learning:

**Architecture:**
- **Algorithms**: DQN, PPO, and SAC implementations with custom modifications
- **Neural Networks**: CNN-LSTM hybrid architecture for spatial-temporal reasoning
- **Training Environment**: Custom racing simulator with realistic physics
- **Reward Engineering**: Multi-objective reward function balancing speed, safety, and efficiency

**Advantages**: Adaptive learning, complex strategy development, superior generalization to unseen scenarios.

## Usage Guide

### Training Agents

**Heuristic Agent Configuration:**
```bash
cd Heuristic-Agent-Project/
python src/main.py --track oval --config config/aggressive.yaml --episodes 100
```

**DRL Agent Training:**
```bash
cd DRL-Agent-Project/
# Start training with default parameters
python training/train_ppo.py --env racing-v1 --episodes 10000

# Advanced training with custom configuration
python training/train_ppo.py --config configs/ppo_advanced.yaml --tensorboard
```

### Evaluation & Comparison

**Individual Agent Evaluation:**
```bash
# Evaluate heuristic agent
python Heuristic-Agent-Project/src/evaluate.py --model saved_models/heuristic_best.pkl

# Evaluate DRL agent
python DRL-Agent-Project/evaluation/evaluate.py --model Trained-Models/ppo_best.pt
```

**Comparative Analysis:**
```bash
# Run comprehensive comparison
python Results-and-Analysis/compare_agents.py --agents heuristic,drl --trials 100 --tracks all

# Generate performance report
python Results-and-Analysis/generate_report.py --output results_report.html
```

## Research Methodology

### Experimental Design
1. **Controlled Testing Environment**: Standardized racing scenarios with consistent evaluation metrics
2. **Multiple Track Configurations**: Oval, road course, street circuit, and mixed-condition tracks
3. **Statistical Validation**: Rigorous hypothesis testing with appropriate confidence intervals
4. **Cross-validation**: K-fold validation ensuring robust performance estimates
5. **Ablation Studies**: Component-wise analysis identifying key performance drivers

### Performance Metrics
- **Primary**: Lap time consistency, success rate (completed laps), safety scores
- **Secondary**: Computational efficiency, memory usage, training time requirements
- **Qualitative**: Strategy interpretability, behavior predictability, failure modes

## Key Findings

*[This section will be updated as research progresses with peer-reviewed results]*

### Preliminary Observations
- **Consistency**: Heuristic agents demonstrate lower performance variance across trials
- **Adaptability**: DRL agents show superior performance in novel or dynamic environments
- **Computational Trade-offs**: Heuristic agents require ~10x less computational resources during execution
- **Training Requirements**: DRL agents require extensive training data but show better long-term learning curves

## Academic Citation

If you use this work in your research, please cite:

```bibtex
@mastersthesis{sujyeet2025racing,
  title={Autonomous Racing Agents: A Comparative Study of Heuristic and Deep Reinforcement Learning Approaches},
  author={Sujyeet, [Full Name]},
  year={2025},
  school={[University Name]},
  type={MSc Thesis},
  url={https://github.com/Sujyeet/MSc-Autonomous-Racing-Agent-SPEED}
}
```

## Contributing & Collaboration

This research welcomes academic collaboration and contributions:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/research-enhancement`)
3. **Implement** your changes with appropriate tests and documentation
4. **Commit** with descriptive messages (`git commit -m 'Add statistical significance testing'`)
5. **Submit** a pull request with detailed description

### Research Collaboration
For academic partnerships, dataset sharing, or research discussions, please reach out through the contact information below.

## Acknowledgments

- **Academic Supervisors**: [Supervisor Names] for invaluable guidance and expertise
- **Research Community**: Open-source contributors to PyTorch, OpenAI Gym, and racing simulation frameworks
- **University Resources**: [University Name] High-Performance Computing facilities
- **Industry Partners**: [If applicable] for providing real-world validation data

## License & Usage Rights

This project is released under the MIT License, enabling both academic and commercial use while maintaining attribution requirements. See [`LICENSE`](LICENSE) for complete details.

**Academic Use**: Freely available for research purposes with appropriate citation
**Commercial Use**: Permitted under MIT License terms
**Modification**: Encouraged with attribution to original work

## Contact & Support

**Primary Author**: Sujyeet  
**Institution**: [University Name]  
**Academic Email**: [institutional.email@university.edu]  
**Project Duration**: [Start Date] - [Expected Completion]  
**Research Area**: Autonomous Systems, Machine Learning, Control Theory

**Technical Support**: For implementation questions, please use GitHub Issues  
**Research Inquiries**: Direct academic questions to the email above  
**Collaboration**: Open to research partnerships and joint publications

## Project Status

**Current Phase**: Active Development & Experimentation  
**Latest Update**: August 2025  
**Expected Completion**: [Date]  
**Publication Status**: Thesis in progress, conference submissions planned

---

*This repository represents ongoing academic research. Methodologies, results, and implementations are continuously refined as the research progresses. For the most current findings and implementations, please check the latest releases and documentation updates.*
