# MSc Autonomous Racing Agent SPEED

## Project Overview

This repository contains the comprehensive implementation and research for an MSc project focused on developing and comparing autonomous racing agents. The project explores both traditional heuristic approaches and modern Deep Reinforcement Learning (DRL) techniques for autonomous vehicle control in racing environments.

## 🏁 Research Objectives

- **Primary Goal**: Develop and compare autonomous racing agents using heuristic and DRL approaches
- **Performance Analysis**: Evaluate agents across multiple metrics including speed, safety, and adaptability
- **Comparative Study**: Analyze the trade-offs between interpretable heuristic methods and learning-based approaches
- **Real-world Applications**: Investigate scalability and practical deployment considerations

## 🏗️ Project Structure

```
MSc-Autonomous-Racing-Agent-SPEED/
├── 📁 Heuristic-Agent-Project/          # Traditional rule-based agent implementation
│   ├── README-Heuristic.md             # Heuristic agent documentation
│   ├── src/                             # Source code for heuristic algorithms
│   ├── config/                          # Configuration files
│   └── tests/                           # Unit tests
│
├── 📁 DRL-Agent-Project/                # Deep Reinforcement Learning agent
│   ├── README-DRL.md                    # DRL agent documentation
│   ├── src/                             # Neural network implementations
│   ├── training/                        # Training scripts and utilities
│   ├── Trained-Models/                  # Saved model weights and checkpoints
│   │   └── .gitkeep                     # Ensures directory tracking
│   └── evaluation/                      # Model evaluation scripts
│
├── 📁 Documentation/                    # Project documentation
│   ├── Setup-Instructions.md           # Installation and setup guide
│   ├── API-Reference.md                 # Code documentation
│   ├── Research-Methods.md             # Methodology documentation
│   └── User-Guide.md                   # Usage instructions
│
├── 📁 Results-and-Analysis/             # Experimental results and analysis
│   ├── README.md                        # Analysis documentation
│   ├── Performance-Metrics/             # Quantitative results
│   ├── Comparison-Studies/              # Comparative analysis
│   ├── Visualizations/                  # Plots and charts
│   └── Statistical-Analysis/            # Statistical tests and analysis
│
├── 📄 .gitignore                        # Git ignore file
├── 📄 README.md                         # This file
├── 📄 requirements.txt                  # Python dependencies
├── 📄 setup.py                          # Package installation script
└── 📄 LICENSE                           # License information
```

## 🚀 Quick Start

### Prerequisites

- Python 3.8 or higher
- pip package manager
- Git
- CUDA-capable GPU (recommended for DRL training)

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/Sujyeet/MSc-Autonomous-Racing-Agent-SPEED.git
   cd MSc-Autonomous-Racing-Agent-SPEED
   ```

2. **Create virtual environment**:
   ```bash
   python -m venv venv
   source venv/bin/activate  # On Windows: venv\Scripts\activate
   ```

3. **Install dependencies**:
   ```bash
   pip install -r requirements.txt
   ```

4. **Follow detailed setup instructions**:
   See [`Documentation/Setup-Instructions.md`](Documentation/Setup-Instructions.md) for complete setup guide.

## 🧠 Agent Implementations

### Heuristic Agent
- **Path Planning**: A* algorithm with dynamic obstacle avoidance
- **Speed Control**: PID controllers optimized for racing scenarios
- **Decision Making**: Rule-based system with safety prioritization
- **Advantages**: Interpretable, fast execution, predictable behavior

### DRL Agent
- **Algorithms**: DQN, PPO, SAC implementations
- **Neural Architecture**: Custom CNN-LSTM networks for temporal reasoning
- **Training Environment**: Custom racing simulator with realistic physics
- **Advantages**: Adaptive learning, complex strategy development

## 📊 Performance Metrics

### Primary Metrics
- **Lap Time**: Average and best performance across tracks
- **Success Rate**: Percentage of completed laps without incidents
- **Safety Score**: Collision avoidance and track boundary adherence
- **Efficiency**: Computational resource utilization

### Evaluation Tracks
- Multiple track configurations (oval, road course, street circuit)
- Varying weather and lighting conditions
- Different traffic and obstacle scenarios

## 🔬 Research Methodology

1. **Literature Review**: Comprehensive analysis of existing approaches
2. **Implementation**: Development of both agent types with comparable interfaces
3. **Experimentation**: Systematic testing across multiple scenarios
4. **Statistical Analysis**: Rigorous comparison using appropriate statistical tests
5. **Validation**: Cross-validation and generalization testing

## 📈 Key Findings

*[This section will be populated as research progresses]*

### Preliminary Results
- Heuristic agents show consistent performance with lower variance
- DRL agents demonstrate superior adaptability to new environments
- Trade-offs observed between interpretability and performance
- Computational requirements vary significantly between approaches

## 🛠️ Technology Stack

### Core Technologies
- **Python 3.8+**: Primary programming language
- **PyTorch/TensorFlow**: Deep learning frameworks
- **OpenAI Gym**: Reinforcement learning environment interface
- **NumPy/SciPy**: Numerical computing and analysis
- **Matplotlib/Seaborn**: Data visualization

### Racing Simulation
- **Physics Engine**: Custom integration with realistic vehicle dynamics
- **Rendering**: OpenGL-based visualization
- **Sensors**: Simulated LIDAR, cameras, and IMU data

## 📋 Usage Examples

### Running Heuristic Agent
```bash
cd Heuristic-Agent-Project/
python src/main.py --track oval --config config/default.yaml
```

### Training DRL Agent
```bash
cd DRL-Agent-Project/
python training/train_dqn.py --env racing-v1 --episodes 10000
```

### Comparing Agents
```bash
python Results-and-Analysis/compare_agents.py --agents heuristic,drl --trials 100
```

## 🤝 Contributing

This is an academic research project. For collaboration or questions:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/research-enhancement`)
3. Commit changes (`git commit -am 'Add new analysis method'`)
4. Push to branch (`git push origin feature/research-enhancement`)
5. Create Pull Request

## 📚 Publications and Documentation

- **Research Paper**: [Link to published paper]
- **Conference Presentations**: [Links to presentations]
- **Technical Documentation**: Available in [`Documentation/`](Documentation/) folder
- **API Reference**: Generated from source code documentation

## 🏆 Acknowledgments

- Academic supervisors and research committee
- Open-source communities for frameworks and tools
- Racing simulation software providers
- Research participants and collaborators

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Contact

- **Author**: Sujyeet
- **Institution**: [University Name]
- **Email**: [Contact Email]
- **Project Duration**: [Start Date] - [End Date]

## 🔗 Related Work

- [Link to related research papers]
- [References to competing approaches]
- [Citations and acknowledgments]

---

**Note**: This repository represents ongoing research. Results, methodologies, and implementations are subject to updates as the research progresses.

**Last Updated**: August 2025
