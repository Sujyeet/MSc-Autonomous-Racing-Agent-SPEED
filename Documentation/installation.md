# Installation Guide

## SPEED: Intelligent Racing Agents Setup

### Overview

This guide provides step-by-step instructions for setting up the SPEED project environment across different operating systems. The setup includes Unity Editor, ML-Agents framework, Python dependencies, and project-specific configurations.

### System Requirements

#### Minimum Requirements
- **OS**: Windows 10/11, macOS 10.15+, or Ubuntu 18.04+
- **CPU**: Intel i5-4590 / AMD FX 8350 equivalent or better
- **RAM**: 8 GB minimum, 16 GB recommended
- **Storage**: 10 GB free space
- **GPU**: DirectX 11 compatible (optional but recommended for faster training)

#### Recommended Specifications
- **CPU**: Intel i7-8700 / AMD Ryzen 5 3600 or better
- **RAM**: 16 GB or more
- **GPU**: NVIDIA GTX 1060 / AMD RX 580 or better
- **Storage**: 20 GB free space (SSD recommended)

### Installation Steps

#### Step 1: Unity Installation

##### Option A: Unity Hub (Recommended)

1. **Download Unity Hub**
   ```bash
   # Visit https://unity3d.com/get-unity/download
   # Download and install Unity Hub for your OS
   ```

2. **Install Unity 2022.3 LTS**
   - Open Unity Hub
   - Go to "Installs" tab
   - Click "Install Editor"
   - Select "Unity 2022.3.0f1" (LTS)
   - Add modules:
     - WebGL Build Support (optional)
     - Documentation
     - Visual Studio Community (Windows) or Visual Studio for Mac

##### Option B: Direct Download

```bash
# Windows
# Download from: https://unity3d.com/get-unity/download/archive

# macOS
brew install --cask unity

# Linux (using Unity Hub)
# Download Unity Hub AppImage from Unity website
```

#### Step 2: Python Environment Setup

##### Install Python 3.8-3.10

**Windows:**
```powershell
# Using winget (Windows 10 1809+)
winget install Python.Python.3.10

# Or download from: https://python.org/downloads/
```

**macOS:**
```bash
# Using Homebrew
brew install python@3.10

# Using pyenv (recommended)
brew install pyenv
pyenv install 3.10.0
pyenv global 3.10.0
```

**Linux (Ubuntu/Debian):**
```bash
# Update package list
sudo apt update

# Install Python 3.10
sudo apt install python3.10 python3.10-venv python3.10-dev

# Install pip
sudo apt install python3-pip
```

##### Create Virtual Environment

```bash
# Navigate to project directory
cd SPEED-Intelligent-Racing-Agents

# Create virtual environment
python -m venv speed-env

# Activate virtual environment
# Windows
speed-env\Scripts\activate

# macOS/Linux
source speed-env/bin/activate
```

#### Step 3: ML-Agents Installation

##### Install ML-Agents Python Package

```bash
# Ensure virtual environment is activated
# Install ML-Agents release 20
pip install mlagents==0.30.0

# Install additional dependencies
pip install -r requirements.txt

# Verify installation
mlagents-learn --help
```

##### Install ML-Agents Unity Package

1. **Open Unity Project**
   - Launch Unity Hub
   - Click "Add" and select the project folder
   - Open the project (may take several minutes first time)

2. **Install ML-Agents Package**
   - In Unity: Window → Package Manager
   - Change scope to "Unity Registry"
   - Search for "ML-Agents"
   - Install "ML-Agents" (Release 20 or 2.0.1)

3. **Verify Package Installation**
   ```csharp
   // Check if ML-Agents namespace is available
   using Unity.MLAgents;
   using Unity.MLAgents.Actuators;
   using Unity.MLAgents.Sensors;
   ```

#### Step 4: Project Configuration

##### Unity Project Settings

1. **Configure Build Settings**
   - File → Build Settings
   - Platform: PC, Mac & Linux Standalone
   - Target Platform: Current OS
   - Architecture: x86_64

2. **Player Settings Configuration**
   - Edit → Project Settings → Player
   - Company Name: "SPEED Research"
   - Product Name: "SPEED Racing Agents"
   - Default Icon: (optional custom icon)

3. **Quality Settings**
   - Edit → Project Settings → Quality
   - Set default quality level to "Good" or "Beautiful"
   - Ensure VSync Count is set to "Every V Blank"

##### ML-Agents Configuration

1. **Verify Scene Setup**
   - Open `Assets/Karting/Scenes/MainScene.unity`
   - Check that KartAgent GameObjects have:
     - Behavior Parameters component
     - Decision Requester component
     - Ray Perception Sensor components

2. **Training Configuration**
   - Review `config/trainer_config.yaml`
   - Ensure behavior names match Unity scene
   - Adjust hyperparameters if needed

#### Step 5: Verification and Testing

##### Environment Test

1. **Python Environment**
   ```bash
   # Test ML-Agents installation
   python -c "import mlagents; print('ML-Agents installed successfully')"
   
   # Test Unity MLAgents communication
   mlagents-learn --help
   ```

2. **Unity Environment**
   - Open MainScene.unity
   - Press Play button
   - Verify no console errors
   - Check agent behavior in Scene view

##### Quick Start Test

```bash
# Test heuristic agent (no ML-Agents needed)
# In Unity: Set Behavior Type to "Heuristic Only" and press Play

# Test ML-Agents connection
mlagents-learn config/trainer_config.yaml --run-id=test --no-graphics
# Then press Play in Unity (should connect automatically)
```

### Platform-Specific Instructions

#### Windows Additional Setup

1. **Visual Studio Build Tools**
   ```powershell
   # Install Visual Studio Build Tools 2019 or later
   # Required for some Python packages compilation
   winget install Microsoft.VisualStudio.2022.BuildTools
   ```

2. **Git for Windows**
   ```powershell
   winget install Git.Git
   ```

3. **Windows Defender Exclusion** (Optional, for performance)
   - Add project folder to Windows Defender exclusions
   - Settings → Update & Security → Windows Security → Virus & threat protection

#### macOS Additional Setup

1. **Xcode Command Line Tools**
   ```bash
   xcode-select --install
   ```

2. **Homebrew** (if not installed)
   ```bash
   /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
   ```

#### Linux Additional Setup

1. **Additional Dependencies**
   ```bash
   # Ubuntu/Debian
   sudo apt install build-essential git curl
   
   # CentOS/RHEL
   sudo yum groupinstall "Development Tools"
   sudo yum install git curl
   ```

2. **Graphics Drivers** (for GPU acceleration)
   ```bash
   # NVIDIA drivers
   sudo apt install nvidia-driver-470
   
   # AMD drivers
   sudo apt install mesa-vulkan-drivers
   ```

### Troubleshooting

#### Common Issues and Solutions

**Unity ML-Agents Package Not Found**
```bash
# Solution: Add package via Git URL
# In Unity Package Manager:
# Add package from Git URL: com.unity.ml-agents
```

**Python mlagents-learn Command Not Found**
```bash
# Ensure virtual environment is activated
source speed-env/bin/activate  # or speed-env\Scripts\activate on Windows

# Reinstall ML-Agents
pip uninstall mlagents
pip install mlagents==0.30.0
```

**Unity Console Errors on Play**
```csharp
// Check Behavior Parameters component settings:
// - Behavior Name: "KartAgent"
// - Vector Observation Size: 14
// - Actions: Continuous, Size: 3
```

**Training Not Starting**
```bash
# Check Unity console for connection messages
# Ensure trainer_config.yaml behavior name matches Unity scene
# Try restarting both Unity and mlagents-learn command
```

#### Performance Optimization

1. **Unity Settings for Training**
   ```csharp
   // In training mode, reduce graphics quality:
   Time.timeScale = 20f;  // Speed up training
   QualitySettings.SetQualityLevel(0);  // Lowest quality
   ```

2. **System Resource Management**
   - Close unnecessary applications during training
   - Monitor CPU/GPU usage
   - Use task manager to prioritize Unity/Python processes

### Next Steps

After successful installation:

1. **Explore Documentation**
   - Read `Documentation/experiment-guide.md` for running experiments
   - Review `Documentation/agent-descriptions.md` for agent details

2. **Run Sample Experiments**
   ```bash
   # Start with heuristic agent evaluation
   # Then try DRL agent training
   mlagents-learn config/trainer_config.yaml --run-id=first-training
   ```

3. **Check Results**
   - Training logs: `results/first-training/`
   - TensorBoard monitoring: `tensorboard --logdir results`
   - Evaluation data: `Results-and-Analysis/`

### Support and Resources

- **Unity ML-Agents Documentation**: https://unity-technologies.github.io/ml-agents/
- **Unity Forum**: https://forum.unity.com/
- **Project Issues**: Use GitHub Issues for project-specific problems
- **Community**: ML-Agents Discord and Reddit communities

---

**Installation Time Estimate**: 30-60 minutes  
**Last Updated**: August 2025  
**Tested On**: Windows 11, macOS 13, Ubuntu 22.04  
**Maintainer**: Sujyeet
