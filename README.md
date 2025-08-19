# SPEED-Intelligent-Racing-Agents
• **Result**: Simpler reward and lightweight networks consistently improved DRL reliability. See main paper for summary tables (e.g., Table IV, Table V) and cumulative reward learning curves.
### Performance Metrics (example table)
| Agent | Mean Lap Time (s) | Std Dev | Collision Rate | Human-Likeness |
|-------|------------------|---------|----------------|----------------|
| Heuristic Agent | 41.52 | 0.09 | 0% | Low |
| PPO DRL Agent (Baseline) | 39.8 | 1.4 | <2% | High |
| DRL Agent (Enhanced Humanlike) | 43.0 | 1.1 | <1% | Very High |
| Human Player | 44.2 | 2.8 | ~3% | Baseline |
### Behavioral Analysis
Beyond speed, the work quantifies "natural" behavior:
• **Path deviation and steering variance**—plotted time series show DRL agents closely track human smoothness, unlike the mechanical, deterministic baseline
• **Error and recovery**: Analysis of wall-collisions and recovery strategies
• **Comprehensive logging**: All logs available for reproduction in Results-and-Analysis/
### Key Lessons
• Over-complexity (deep networks, multi-term rewards) led to instability and slow convergence
• Well-tuned heuristics provide reliability but limited adaptability to novel track layouts
• Simplified DRL with targeted reward shaping yields both superior lap times and higher human-likeness scores in controlled experiments

## Running and Evaluation

### Overview
This section provides step-by-step guidance for running and evaluating each agent type in the SPEED racing environment. All experiments are conducted within Unity Editor using ML-Agents toolkit.

### Prerequisites
- Unity 2022.3.0f1 or later
- ML-Agents Release 20 package installed via Package Manager
- Python 3.8+ with ML-Agents Python package (`pip install mlagents`)
- All required dependencies as listed in [Documentation/Setup-Instructions.md](Documentation/Setup-Instructions.md)

### Unity Scene Setup
1. **Load the Main Scene**:
   - Navigate to `Assets/Karting/Scenes/MainScene.unity`
   - Double-click to open in Unity Editor
   - Verify all GameObjects are properly loaded (KartClassic, TrackManager, MLAgents components)

2. **Configure Console Output**:
   - Open Unity Console: `Window > General > Console`
   - Enable "Collapse" and "Clear on Play" for cleaner log output
   - Set Console to capture ML-Agents training logs and episode statistics

### Agent Configuration

#### 1. Heuristic Agent
**Setup Steps**:
1. In Unity Editor, select the `KartAgent` GameObject in the scene hierarchy
2. In Inspector panel, locate the `Behavior Parameters` component:
   - Set `Behavior Name` to "KartAgent"
   - Set `Behavior Type` to "Heuristic Only"
   - Verify `Vector Observation Space Size` is set correctly (typically 14)
3. Ensure `Decision Requester` component has `Decision Period` = 5

**Running the Heuristic Agent**:
1. Press Play button in Unity Editor
2. **Expected Behavior**: Agent follows pre-programmed racing logic
3. **Console Output**: Look for messages like:
   ```
   [INFO] Heuristic agent initialized
   [INFO] Episode 1 started - lap time tracking active
   [INFO] Lap completed: Time = 41.52s, Collisions = 0
   ```
4. **Performance Monitoring**: Lap times and collision data logged to `Results-and-Analysis/Heuristic/`

**Controls & Settings**:
- Modify heuristic parameters in `KartHeuristicController.cs`
- Adjust speed limits, turning sensitivity via Inspector sliders
- Change evaluation length: Set `Max Step` in `Behavior Parameters` (default: 10000 steps)

#### 2. DRL (Deep Reinforcement Learning) Agent
**Model Assignment**:
1. Navigate to `Assets/TensorFlowModels/` directory
2. Locate the trained `.onnx` model file (e.g., `KartAgent_trained.onnx`)
3. Select `KartAgent` GameObject → Inspector → `Behavior Parameters`:
   - Set `Behavior Type` to "Inference Only"
   - Drag the `.onnx` model into the `Model` field
   - Verify `Inference Device` is set appropriately (CPU/GPU based on your system)

**Running the DRL Agent**:
1. Ensure model is properly assigned (Inspector should show model name)
2. Press Play in Unity Editor
3. **Expected Behavior**: 
   - Agent loads neural network weights
   - Begins autonomous racing using learned policy
   - Smooth, adaptive driving behavior visible
4. **Console Output**:
   ```
   [INFO] Loading ONNX model: KartAgent_trained.onnx
   [INFO] Model loaded successfully - inference ready
   [INFO] Episode 1: Action=[0.7, -0.2, 0.0] Reward=1.24
   [INFO] Lap completed: Time = 39.8s, Reward = 245.6
   ```
5. **Log Output**: Detailed episode data saved to `Results-and-Analysis/DRL/YYYY-MM-DD_HH-MM/`

**Hyperparameter Configuration**:
- Training hyperparameters: Edit `config/trainer_config.yaml`
- Runtime inference settings: Modify `MLAgents.PolicyFactory` parameters
- Output directory: Set via `--results-dir` in training command or `Training.resultsDir` in config

#### 3. Human Agent
**Setup Steps**:
1. Select `KartAgent` GameObject → Inspector → `Behavior Parameters`:
   - Set `Behavior Type` to "Heuristic Only"
2. Ensure `KartUserControl.cs` script is enabled and properly configured
3. Verify input mappings in `Project Settings > Input Manager`:
   - Horizontal: A/D keys or Left/Right arrows
   - Vertical: W/S keys or Up/Down arrows
   - Brake: Space bar

**Running Human Control**:
1. Press Play button
2. **Expected Behavior**: 
   - Manual keyboard/controller input controls the kart
   - Responsive steering and acceleration
   - Real-time performance feedback
3. **Controls**:
   - `W/↑`: Accelerate
   - `S/↓`: Brake/Reverse
   - `A/D` or `←/→`: Steer left/right
   - `Space`: Emergency brake
4. **Console Output**:
   ```
   [INFO] Human control active - input system ready
   [INFO] Lap started - timer active
   [INFO] Lap completed: Time = 44.2s, Player input recorded
   ```
5. **Data Logging**: Human driving patterns saved to `Results-and-Analysis/Human/session_YYYY-MM-DD/`

### Troubleshooting

#### Common Issues and Solutions:

**Package Installation Errors**:
- **Problem**: "ML-Agents package not found"
- **Solution**: 
  1. Open Package Manager (`Window > Package Manager`)
  2. Switch to "Unity Registry"
  3. Search for "ML-Agents" and install Release 20+
  4. If not found, add via Git URL: `com.unity.ml-agents`

**Missing Models**:
- **Problem**: "ONNX model not found" or "Model field is empty"
- **Solution**:
  1. Check `Assets/TensorFlowModels/` contains `.onnx` files
  2. If missing, run training first: `mlagents-learn config/trainer_config.yaml --run-id=training_run`
  3. Models generated in `results/training_run/KartAgent.onnx`

**Scene Loading Issues**:
- **Problem**: "MainScene.unity not found" or missing GameObjects
- **Solution**:
  1. Verify project integrity: `Assets/Karting/` folder should contain complete karting package
  2. Reimport assets: Right-click `Assets/Karting` → "Reimport"
  3. Check Unity version compatibility (requires 2022.3+)

**Performance Issues**:
- **Problem**: Low FPS or stuttering during training/inference
- **Solution**:
  1. Reduce simulation time scale: `Time.timeScale = 1.0f`
  2. Lower graphics quality: `Edit > Project Settings > Quality`
  3. Use CPU inference for ONNX models if GPU memory is limited

**Log Output Issues**:
- **Problem**: Missing or incomplete logs
- **Solution**:
  1. Ensure `Results-and-Analysis/` directory exists with write permissions
  2. Check `Debug.Log` statements are enabled in build settings
  3. Verify session ID generation in `ExperimentManager.cs`

### Advanced Configuration

**Result Consistency**:
- **Fixed Seeds**: Set `Random.seed = 12345` in `Start()` method for reproducible results
- **Session Identification**: Each run generates unique timestamp-based session ID
- **Log Locations**: 
  - Training logs: `results/[run-id]/`
  - Evaluation data: `Results-and-Analysis/[AgentType]/[SessionID]/`
  - Performance metrics: `*.csv` files with lap times, rewards, collision data

**Hyperparameter Tuning**:
- **Learning Rate**: Modify `learning_rate` in `config/trainer_config.yaml`
- **Network Architecture**: Adjust `hidden_units` and `num_layers`
- **Reward Shaping**: Edit reward function in `KartAgent.cs` → `AddReward()` calls
- **Episode Length**: Set `max_steps` (default: 10000 for ~2-3 laps)

**Evaluation Protocols**:
- **Standard Evaluation**: 100 episodes per agent type
- **Statistical Analysis**: Mean ± Standard Deviation reported
- **Comparison Metrics**: Lap time, collision rate, path deviation, human-likeness score
- **Output Format**: CSV files with episode-by-episode data for statistical analysis

### Command-Line Automation

For full automation, retraining procedures, and batch evaluation scripts, see the comprehensive guide:
**[Documentation/Setup-Instructions.md](Documentation/Setup-Instructions.md)**

This includes:
- Automated training pipelines
- Batch evaluation scripts
- Hyperparameter sweep configurations
- Result aggregation and analysis tools
- Integration with external evaluation frameworks

---

## Full Reproducibility Commitment
• All experiments use documented seeds and config files; every result in the paper can be regenerated from scripts in this repository
• **Paper draft**: Documentation/SPEED-Dissertation.pdf (complete analyses, figures, and extended results)
• **Installation and reproducibility guide**: Documentation/Setup-Instructions.md (step-by-step for every OS)
## Collaboration & Academic Use
• **Academic collaboration**: Designed for MSc and PhD research use; contact author for dataset sharing, integration studies, or advanced experiments
• **Contributing**: Issue tracker and pull requests are welcome for reproducibility, cross-validation, or improved agent design studies
## Citation
If this work, code, or methodology contributes to your research, please cite:
```bibtex
@mastersthesis{sujyeet2025speed,
  title={SPEED: Intelligent Racing Agents Using Deep Reinforcement Learning and Unity ML-Agents},
  author={Sujyeet, [Your Full Name]},
  year={2025},
  school={[Your University]},
  type={MSc Thesis}
}
```
## Author & Contact
• Sujyeet (MSc Autonomous Racing, [your.email@domain])
• Queen Mary University of London (for academic coordination)
• For technical questions, open a GitHub Issue or contact by email
## License
All project source, code, models, data, and documentation are released under the MIT License. See LICENSE for details. Intended for research, education, and open collaboration.
