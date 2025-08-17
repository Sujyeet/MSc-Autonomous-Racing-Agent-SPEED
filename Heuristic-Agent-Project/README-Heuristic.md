# Heuristic-Agent-Project

A rule-based autonomous racing agent implementing waypoint-following algorithms for scientific comparison against deep reinforcement learning approaches.

## Project Overview

### Purpose
This project implements a heuristic-based autonomous racing agent that serves as a baseline for comparative analysis with deep reinforcement learning (DRL) methods. The agent employs traditional control algorithms and geometric path-following techniques to navigate racing environments, providing interpretable decision-making processes and deterministic behavior.

### Context
Developed as part of an MSc research project investigating autonomous racing strategies, this heuristic agent addresses the need for a transparent, rule-based comparison point against black-box machine learning approaches. The implementation focuses on waypoint-following navigation, which represents a fundamental approach to autonomous vehicle control.

### Significance
The heuristic agent serves several critical functions in the research framework:
- Establishes performance baselines for DRL agent evaluation
- Provides deterministic behavior for reproducible experiments
- Offers interpretable decision-making processes for analysis
- Demonstrates classical control theory applications in racing scenarios
- Enables comparative studies between rule-based and learning-based approaches

## Waypoint-Following Logic

### Implementation Architecture
The waypoint-following system is implemented through two main components:
- **WaypointKartAI**: Primary navigation controller
- **ArcadeKart**: Vehicle physics and control interface

### Step-by-Step Algorithm

#### 1. Waypoint Detection and Selection
```
FOR each frame:
  - Scan ahead for the next target waypoint
  - Calculate distance to current target
  - IF distance < waypoint_threshold:
    - Advance to next waypoint in sequence
    - Update target reference
```

#### 2. Steering Calculation
```
Vector to_target = target_waypoint.position - current_position
Float target_angle = atan2(to_target.y, to_target.x)
Float angle_difference = normalize_angle(target_angle - current_heading)
Float steering_input = clamp(angle_difference * turn_sensitivity, -1.0, 1.0)
```

#### 3. Speed Management
```
Float distance_to_target = magnitude(to_target)
Float base_speed = max_speed

// Reduce speed for sharp turns
IF abs(angle_difference) > turn_threshold:
  base_speed *= turn_speed_factor

// Apply distance-based modulation
Float speed_input = clamp(base_speed * distance_factor, 0.0, 1.0)
```

#### 4. Control Output
```
kart.SetInputs(
  steering: steering_input,
  acceleration: speed_input,
  brake: 0.0
)
```

### WaypointKartAI Implementation Details

The WaypointKartAI class manages:
- Waypoint sequence traversal
- Real-time target selection
- Steering angle calculations
- Speed modulation based on geometry
- Safety bounds enforcement

### ArcadeKart Integration

The ArcadeKart component handles:
- Vehicle physics simulation
- Control input processing
- Collision detection
- Performance metrics collection

## Requirements and Dependencies

### System Requirements
- Unity 2022.3 LTS or later
- Windows 10/11, macOS 10.15+, or Ubuntu 18.04+
- Minimum 8GB RAM
- DirectX 11 compatible graphics card

### Unity Packages
- Unity ML-Agents Toolkit
- Cinemachine
- Input System
- Universal Render Pipeline (URP)

### Runtime Dependencies
- .NET Framework 4.7.1+
- Unity Physics Engine
- ML-Agents Communicator

## Setup Instructions

### 1. Environment Initialization

```bash
# Clone the repository
git clone https://github.com/Sujyeet/MSc-Autonomous-Racing-Agent-SPEED.git
cd MSc-Autonomous-Racing-Agent-SPEED/Heuristic-Agent-Project

# Open in Unity Editor
# File -> Open Project -> Select Heuristic-Agent-Project folder
```

### 2. Waypoint Configuration

1. **Scene Setup**:
   - Open the main racing scene
   - Locate the "Waypoints" parent object in hierarchy
   - Ensure waypoints are properly sequenced (numbered)

2. **Waypoint Placement**:
   - Position waypoints along desired racing line
   - Maintain consistent spacing (recommended: 10-15 Unity units)
   - Verify waypoint order matches intended path

3. **Agent Assignment**:
   - Select the kart GameObject
   - Ensure WaypointKartAI script is attached
   - Assign waypoint parent object to "waypoints" field

### 3. Running the Agent

1. **Play Mode Execution**:
   - Press Play in Unity Editor
   - Agent will automatically begin waypoint following
   - Monitor performance through Scene view

2. **Build and Run**:
   - Build settings: Standalone platform
   - Resolution: 1920x1080 recommended
   - Quality: High for accurate physics

### 4. Expected Outputs

- **Visual**: Smooth waypoint-to-waypoint navigation
- **Console**: Performance metrics and debug information
- **Data**: Lap times, speeds, and trajectory data (if logging enabled)

## Configuration Parameters

### Core Parameters

| Parameter | Type | Default | Recommendation | Description |
|-----------|------|---------|----------------|--------------|
| `maxSpeed` | float | 20.0 | 15-25 | Maximum forward velocity |
| `turnSpeed` | float | 300.0 | 250-350 | Angular velocity multiplier |
| `waypointThreshold` | float | 3.0 | 2-5 | Distance to trigger next waypoint |
| `steeringSensitivity` | float | 2.0 | 1.5-3.0 | Steering response strength |
| `brakingDistance` | float | 10.0 | 8-15 | Distance to begin speed reduction |

### Advanced Tuning

| Parameter | Type | Default | Recommendation | Description |
|-----------|------|---------|----------------|--------------|
| `turnSpeedFactor` | float | 0.6 | 0.4-0.8 | Speed reduction in turns |
| `accelerationCurve` | AnimationCurve | Linear | Custom | Speed ramp profile |
| `steeringDeadzone` | float | 0.1 | 0.05-0.2 | Minimum steering threshold |
| `lookAheadDistance` | float | 15.0 | 10-20 | Waypoint selection range |

### Waypoint Spacing Guidelines

- **Straight sections**: 15-20 Unity units
- **Gentle curves**: 10-15 Unity units
- **Sharp turns**: 5-10 Unity units
- **Complex sections**: 3-8 Unity units

## Logging and Results

### Performance Metrics
The agent automatically logs:
- Lap completion times
- Average speed per sector
- Waypoint traversal efficiency
- Steering input statistics
- Collision events

### Data Output Format
```json
{
  "session_id": "heuristic_run_001",
  "timestamp": "2025-08-17T01:02:00Z",
  "metrics": {
    "lap_time": 45.32,
    "average_speed": 18.7,
    "max_speed_achieved": 24.1,
    "waypoints_hit": 24,
    "waypoints_missed": 0,
    "total_distance": 847.3
  }
}
```

### Log File Locations
- **Windows**: `%USERPROFILE%/AppData/LocalLow/CompanyName/ProjectName/`
- **macOS**: `~/Library/Logs/CompanyName/ProjectName/`
- **Linux**: `~/.config/unity3d/CompanyName/ProjectName/`

## Scientific Comparison Framework

### Role as Baseline
This heuristic agent serves as the control group in comparative studies with DRL approaches by providing:

1. **Deterministic Baseline**: Consistent, reproducible performance for statistical analysis
2. **Interpretable Decisions**: Clear cause-and-effect relationships in agent behavior
3. **Classical Validation**: Verification that fundamental racing principles are sound
4. **Performance Floor**: Minimum expected capability using traditional methods

### Experimental Integration
- Same environmental conditions as DRL agents
- Identical physics simulation parameters
- Consistent evaluation metrics
- Standardized testing scenarios

## Strengths and Limitations

### Strengths

**Transparency and Interpretability**
- All decision-making logic is explicitly coded and auditable
- Parameter effects are predictable and analyzable
- Debugging and optimization are straightforward

**Computational Efficiency**
- Minimal computational overhead
- Real-time performance guaranteed
- No training or inference latency

**Consistency and Reliability**
- Deterministic behavior across runs
- No performance degradation over time
- Robust to environmental variations

**Implementation Speed**
- Rapid development and deployment
- No training data requirements
- Immediate applicability to new tracks

### Limitations

**Adaptability Constraints**
- Limited ability to learn from experience
- Requires manual tuning for different tracks
- Cannot automatically optimize racing lines

**Dynamic Environment Handling**
- Struggles with unexpected obstacles
- Limited multi-agent interaction capabilities
- Reduced performance in changing conditions

**Optimization Boundaries**
- Performance ceiling limited by algorithmic approach
- Cannot discover non-intuitive racing strategies
- Manual parameter tuning required for optimization

**Scalability Challenges**
- Complexity increases significantly with added features
- Difficult to handle edge cases comprehensively
- Limited generalization across different racing scenarios

## Comparison Metrics

For scientific evaluation against DRL agents, the following metrics are collected:

- **Performance**: Lap times, sector splits, top speeds
- **Consistency**: Standard deviation across multiple runs
- **Robustness**: Performance under varying conditions
- **Efficiency**: Computational resource usage
- **Interpretability**: Decision transparency scores

## Future Development

Potential enhancements to strengthen the baseline:
- Advanced path smoothing algorithms
- Predictive speed control
- Multi-waypoint lookahead
- Dynamic parameter adjustment
- Enhanced obstacle avoidance

## Contributing

When modifying the heuristic agent:
1. Maintain deterministic behavior
2. Document all parameter changes
3. Preserve logging functionality
4. Ensure compatibility with evaluation framework

## License

This project is part of academic research. Please refer to the main repository license for usage terms.
