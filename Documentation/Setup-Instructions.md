# Setup Instructions - DRL Agent Training

This document provides comprehensive setup instructions for the MSc Autonomous Racing Agent SPEED project, with a focus on Deep Reinforcement Learning (DRL) agent training and implementation.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Installation](#installation)
3. [Configuration](#configuration)
4. [Training Process](#training-process)
5. [File Structure](#file-structure)
6. [Paper Reproduction](#paper-reproduction)
7. [Troubleshooting](#troubleshooting)
8. [Support](#support)
9. [Citation](#citation)

## Prerequisites

### System Requirements
- **Operating System**: Ubuntu 18.04+ (recommended), Windows 10+, or macOS 10.15+
- **Python**: Version 3.8 or higher (3.9 recommended for optimal compatibility)
- **Memory**: Minimum 8GB RAM (16GB+ recommended for training)
- **Storage**: At least 5GB free space for dependencies and models
- **GPU**: NVIDIA GPU with CUDA support (optional but highly recommended for training)

### Software Dependencies
- pip package manager
- Git version control system
- Virtual environment tool (venv, conda, or virtualenv)
- CUDA Toolkit 11.0+ (if using GPU acceleration)

### Hardware Recommendations
- **CPU**: Multi-core processor (Intel i7/AMD Ryzen 7 or equivalent)
- **GPU**: NVIDIA GTX 1060 or better for training acceleration
- **Network**: Stable internet connection for dependency installation

## Installation

### Step 1: Clone the Repository

```bash
git clone https://github.com/Sujyeet/SPEED-Intelligent-Racing-Agents.git
cd SPEED-Intelligent-Racing-Agents
```

### Step 2: Set Up Virtual Environment

**Using venv (recommended):**
```bash
python -m venv speed_env
source speed_env/bin/activate  # On Windows: speed_env\Scripts\activate
```

**Using conda (alternative):**
```bash
conda create -n speed_env python=3.9
conda activate speed_env
```

### Step 3: Install Dependencies

```bash
pip install --upgrade pip
pip install -r requirements.txt
```

### Step 4: Install Additional DRL Dependencies

```bash
# For PyTorch (GPU version)
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu118

# For stable-baselines3 and related libraries
pip install stable-baselines3[extra]
pip install gymnasium[all]
```

### Step 5: Verify Installation

```bash
python -c "import torch; print('PyTorch version:', torch.__version__)"
python -c "import stable_baselines3; print('SB3 installed successfully')"
```

## Configuration

### Environment Configuration

1. **Copy the example configuration file:**
   ```bash
   cp config/config_example.yaml config/config.yaml
   ```

2. **Edit the configuration file:**
   ```bash
   nano config/config.yaml  # or use your preferred editor
   ```

3. **Key configuration parameters:**
   ```yaml
   training:
     algorithm: "PPO"  # Options: PPO, SAC, TD3, A2C
     total_timesteps: 1000000
     learning_rate: 0.0003
     batch_size: 64
     
   environment:
     track_name: "silverstone"
     max_episode_steps: 1000
     reward_function: "progress_speed"
     
   model:
     policy: "MlpPolicy"
     net_arch: [400, 300]
     activation_fn: "relu"
   ```

### GPU Configuration (Optional)

If you have an NVIDIA GPU:

```bash
# Verify CUDA installation
nvidia-smi

# Test PyTorch GPU access
python -c "import torch; print('CUDA available:', torch.cuda.is_available())"
```

## Training Process

### Basic Training Workflow

1. **Navigate to the DRL project directory:**
   ```bash
   cd DRL-Agent-Project
   ```

2. **Start training with default parameters:**
   ```bash
   python train_agent.py --config ../config/config.yaml
   ```

3. **Monitor training progress:**
   ```bash
   # In a separate terminal
   tensorboard --logdir logs/
   ```

### Advanced Training Options

**Custom training with specific parameters:**
```bash
python train_agent.py \
  --algorithm PPO \
  --total-timesteps 2000000 \
  --learning-rate 0.0003 \
  --batch-size 128 \
  --track silverstone \
  --save-freq 10000
```

**Resume training from checkpoint:**
```bash
python train_agent.py \
  --load-model Trained-Models/ppo_racing_agent_1000000.zip \
  --total-timesteps 500000
```

### Training Monitoring

- **TensorBoard logs**: Located in `logs/` directory
- **Model checkpoints**: Saved in `DRL-Agent-Project/Trained-Models/`
- **Training metrics**: Available via TensorBoard dashboard
- **Console output**: Real-time training statistics

## File Structure

```
SPEED-Intelligent-Racing-Agents/
├── DRL-Agent-Project/                 # Deep Reinforcement Learning implementation
│   ├── agents/                        # Agent implementations
│   │   ├── ppo_agent.py              # Proximal Policy Optimization
│   │   ├── sac_agent.py              # Soft Actor-Critic
│   │   └── base_agent.py             # Base agent class
│   ├── environments/                  # Racing environment implementations
│   │   ├── speed_env.py              # Main racing environment
│   │   ├── track_loader.py           # Track configuration loader
│   │   └── reward_functions.py       # Custom reward functions
│   ├── utils/                         # Utility functions
│   │   ├── preprocessing.py          # Data preprocessing
│   │   ├── visualization.py          # Training visualization
│   │   └── callbacks.py              # Training callbacks
│   ├── Trained-Models/                # Pre-trained and saved models
│   │   ├── ppo_racing_agent.zip      # PPO trained model
│   │   ├── sac_racing_agent.zip      # SAC trained model
│   │   └── model_metadata.json       # Model information
│   ├── train_agent.py                 # Main training script
│   ├── evaluate_agent.py              # Model evaluation script
│   ├── hyperparameter_tuning.py       # Automated hyperparameter optimization
│   └── README.md                      # DRL-specific documentation
├── Heuristic-Agent-Project/           # Rule-based agent implementation
├── Documentation/                     # Project documentation
│   ├── Setup-Instructions.md          # This file
│   ├── installation.md               # Basic installation guide
│   └── project-overview.md           # High-level project overview
├── Results-and-Analysis/              # Experimental results and analysis
├── config/                           # Configuration files
│   ├── config.yaml                   # Main configuration
│   └── config_example.yaml           # Example configuration template
├── logs/                             # Training logs and TensorBoard data
├── requirements.txt                  # Python dependencies
└── README.md                         # Main project documentation
```

## Paper Reproduction

### Reproducing Published Results

1. **Download the exact model configurations:**
   ```bash
   # The configurations used in the paper are available in config/paper_configs/
   cp config/paper_configs/ppo_paper.yaml config/config.yaml
   ```

2. **Run the paper's training configuration:**
   ```bash
   python train_agent.py --config config/config.yaml --seed 42
   ```

3. **Evaluate using paper benchmarks:**
   ```bash
   python evaluate_agent.py \
     --model Trained-Models/ppo_racing_agent.zip \
     --episodes 100 \
     --track silverstone
   ```

### Benchmark Datasets

- **Training tracks**: Silverstone, Monaco, Spa-Francorchamps
- **Evaluation metrics**: Lap time, completion rate, collision frequency
- **Baseline comparisons**: Heuristic agent, random policy

## Troubleshooting

### Common Issues and Solutions

#### Issue: CUDA Out of Memory
**Solution:**
```bash
# Reduce batch size in config.yaml
batch_size: 32  # instead of 64

# Or use CPU training
export CUDA_VISIBLE_DEVICES=""
```

#### Issue: Training Divergence
**Solution:**
- Lower the learning rate (e.g., from 0.0003 to 0.0001)
- Increase the number of training environments
- Check reward function scaling

#### Issue: Environment Import Errors
**Solution:**
```bash
# Reinstall gymnasium with all dependencies
pip uninstall gymnasium
pip install gymnasium[all]

# Install additional racing environment dependencies
pip install pygame PyOpenGL PyOpenGL_accelerate
```

#### Issue: Slow Training Performance
**Solutions:**
- Enable GPU acceleration if available
- Increase the number of parallel environments
- Use vectorized environments for faster sampling
- Reduce observation space complexity

#### Issue: Model Loading Errors
**Solution:**
```bash
# Verify model file integrity
python -c "from stable_baselines3 import PPO; model = PPO.load('path/to/model.zip')"

# Check model compatibility with current environment
```

### Performance Optimization Tips

1. **Use appropriate hyperparameters for your hardware**
2. **Monitor GPU/CPU utilization during training**
3. **Implement proper early stopping criteria**
4. **Use learning rate scheduling for better convergence**
5. **Regularly save and backup model checkpoints**

### Debugging Tools

- **TensorBoard**: For training visualization and debugging
- **Gym Monitor**: For episode recording and analysis
- **Profiler**: For performance bottleneck identification

## Support

### Getting Help

- **GitHub Issues**: Report bugs and request features at [GitHub Issues](https://github.com/Sujyeet/SPEED-Intelligent-Racing-Agents/issues)
- **Documentation**: Comprehensive guides available in the `Documentation/` folder
- **Community**: Join discussions in the project's discussion forum

### Contributing

We welcome contributions! Please:
1. Fork the repository
2. Create a feature branch
3. Make your changes with proper documentation
4. Submit a pull request with detailed description

### Reporting Issues

When reporting issues, please include:
- Operating system and Python version
- Complete error traceback
- Steps to reproduce the issue
- Configuration file used
- Hardware specifications (CPU, GPU, RAM)

## Citation

If you use this project in your research, please cite:

```bibtex
@misc{speed_racing_agents_2025,
  title={SPEED: Intelligent Racing Agents for Autonomous Vehicle Control},
  author={Sujyeet Kumar},
  year={2025},
  url={https://github.com/Sujyeet/SPEED-Intelligent-Racing-Agents},
  note={MSc Thesis Project - Advanced Deep Reinforcement Learning for Autonomous Racing}
}
```

### Related Publications

This project builds upon established research in:
- Deep Reinforcement Learning for Autonomous Driving
- Multi-Agent Racing Environments
- Reward Function Design for Racing Tasks
- Transfer Learning in Autonomous Vehicle Control

---

**Last Updated**: August 2025  
**Version**: 2.0  
**Maintainer**: Sujyeet Kumar  
**License**: MIT License

For the most up-to-date information, please refer to the [main repository](https://github.com/Sujyeet/SPEED-Intelligent-Racing-Agents) and check the latest releases.
