<<<<<<< HEAD
# Unity Deep RL Search and Rescue

A comprehensive deep reinforcement learning project implementing intelligent agents for search and rescue scenarios in Unity. This project explores multi-agent systems, adversarial training, and emergent behaviors in complex 3D environments.

## Project Overview

This Unity ML-Agents project develops and trains autonomous agents for search and rescue operations using Proximal Policy Optimization (PPO). The research explores increasingly complex scenarios to understand agent cooperation, competition, and emergent behaviors.

## Training Scenarios

### Single Agent, Single Target (SAST)
**Objective**: Train a single agent to efficiently locate and reach a target in a 3D environment.

- **Algorithm**: Proximal Policy Optimization (PPO)
- **Training Time**: 2M steps (5 hours 1 minute)
- **Network**: Untrained neural network specifically designed for this task
- **Focus**: Basic navigation, target detection, and pathfinding

### Multiple Agent, Single Target (MAST)  
**Objective**: Coordinate multiple agents to collaboratively search for a single target.

- **Algorithm**: PPO with multi-agent considerations
- **Training Time**: 2M steps (5 hours 11 minutes)  
- **Challenge**: Agent coordination without explicit communication
- **Emergent Behaviors**: Efficient area coverage and implicit cooperation

### Adversarial Agent vs Seeker Agent (AvS)
**Objective**: Train competing agents where one hides while another seeks.

- **Hider Agent Training**: 4.6M steps (15 hours 24 minutes)
- **Seeker Agent Training**: 4.8M steps (16 hours 37 minutes)
- **Innovation**: Tailored reward systems for opposing objectives
- **Complexity**: Dynamic environment with adaptive strategies

### Environment Features
- **3D Simulation**: Realistic physics and collision detection
- **Dynamic Scenarios**: Randomized target and spawn locations
- **Visual Sensors**: Ray-casting and camera-based perception
- **Reward Engineering**: Custom reward functions for each scenario type

## Research Applications

This project demonstrates applications in:
- **Emergency Response**: Real-world search and rescue operations
- **Robotics**: Multi-robot coordination and navigation
- **Game AI**: Intelligent NPCs with emergent behaviors
- **Military**: Tactical coordination and surveillance
- **Research**: Understanding cooperation and competition in AI systems

## Results and Analysis

The project successfully demonstrated:
- Effective single-agent navigation and search capabilities
- Emergent cooperative behaviors in multi-agent scenarios
- Complex adversarial strategies without explicit programming
=======
# SearchNRescue_Unity
SearchNRescue Reinforcement Learning environment in unity
>>>>>>> 83f827caf0e26b9cb3dac050af9631ed52ceecfb
