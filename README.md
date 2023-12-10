# Unity Deep RL Search and Rescue

## Overview

This Unity Deep RL project focuses on training agents for a search and rescue task in increasingly complex scenarios. The scenarios include Single Agent, Single Target (SAST), Multiple Agent, Single Target (MAST), and an Adversarial Agent vs Seeker Agent (AvS) challenge.

## Approach

- **SAST/MAST:** Utilized Proximal Policy Optimization (PPO) algorithm with separate untrained networks for each task.
  - SAST Training Time: 2M steps, 5 hr 1 min
  - MAST Training Time: 2M steps, 5 hr 11 min

- **AvS:** Presented a unique challenge with tailored rewards and penalties.
  - Hider Training Time: 4.6M steps/15hr 24min
  - Seeker Training Time: 4.8M steps/16hr 37min

## Results

Achieved efficient searching in MAST scenarios and observed emergent behaviors in all tasks. Comprehensive results and discussion available in my [portfolio](https://sites.google.com/view/danielcheney/portfolio/deep-rl-in-unity?authuser=0).
