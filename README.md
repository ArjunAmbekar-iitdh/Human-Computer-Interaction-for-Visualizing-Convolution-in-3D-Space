# Human-Computer Interaction for Visualizing Convolution in 3D Space

An interactive real-time 3D visualization system for understanding Convolutional Neural Networks (CNNs) using **PyTorch**, **ONNX**, **Unity Sentis**, and **Human-Computer Interaction (HCI)** concepts.

This project transforms CNNs from black-box mathematical models into an explorable and interactive 3D environment where users can literally *watch the network think*.

---

## Project Overview

Deep learning models are powerful but difficult to interpret.  
Most CNNs operate as invisible black boxes, making it hard for students and researchers to understand how features evolve inside the network.

This project introduces an interactive visualization framework that:

- Trains a CNN on MNIST
- Exports the model to ONNX
- Runs real-time inference inside Unity using Sentis
- Extracts intermediate activations
- Visualizes feature maps as 3D cube structures
- Animates inference propagation layer-by-layer
- Allows users to draw digits interactively and observe activations in real-time

The system combines:
- Computer Vision
- Human-Computer Interaction (HCI)
- Deep Learning
- 3D Visualization
- Game Engine Rendering

---

# Features

- Real-time CNN visualization in 3D
- Interactive handwritten digit input
- Layer-by-layer activation rendering
- CNN inference animation wave propagation
- ONNX model integration
- Unity Sentis inference pipeline
- Feature map visualization
- Flatten layer representation
- Prediction confidence visualization
- PCA-based dimensionality reduction concepts
- Modular architecture for extensibility

---

# System Pipeline

```text
PyTorch CNN Training
        ↓
ONNX Export
        ↓
Unity Sentis Inference
        ↓
Intermediate Layer Extraction
        ↓
3D Cube Rendering
        ↓
Interactive Visualization
```

---

# CNN Architecture

```text
Input           : 1 × 28 × 28
Conv1 + ReLU    : 8 × 28 × 28
MaxPool         : 8 × 14 × 14
Conv2 + ReLU    : 16 × 14 × 14
MaxPool         : 16 × 7 × 7
Flatten         : 784
Dense Layer     : 10
Output          : Digit Prediction
```

The architecture was intentionally kept lightweight for educational visualization and real-time rendering.

:contentReference[oaicite:0]{index=0}

---

# Core Technologies

| Technology | Purpose |
|---|---|
| PyTorch | CNN training |
| ONNX | Model interoperability |
| Unity | 3D rendering engine |
| Unity Sentis | Neural network inference |
| C# | Visualization logic |
| Python | Training pipeline |
| TextMeshPro | UI rendering |

---

# Repository Structure

```text
├── cnn2.py
├── CNNVisualizer.cs
├── CNNAnimator.cs
├── DrawableCanvas.cs
├── CNN_LAST.pptx
├── model.onnx
├── Assets/
├── Scenes/
└── README.md
```

---

# Project Components

## 1. CNN Training (PyTorch)

The CNN is trained on the MNIST dataset using PyTorch.

Features:
- 2 convolution layers
- ReLU activations
- MaxPooling
- Fully connected output layer
- ONNX export support

The trained model is exported for Unity inference.

:contentReference[oaicite:1]{index=1}

---

## 2. Unity Sentis Visualization

The Unity visualization engine:
- Loads the ONNX model
- Extracts intermediate tensors
- Converts activations into 3D cube grids
- Updates activations in real-time

Feature maps are rendered as volumetric activation spaces.

:contentReference[oaicite:2]{index=2}

---

## 3. Interactive Drawing Canvas

Users can:
- Draw digits directly on screen
- Perform live inference
- Observe activations update dynamically

The drawing system supports:
- Continuous brush interpolation
- Real-time updates
- Interactive feedback

:contentReference[oaicite:3]{index=3}

---

## 4. Wave Propagation Animation

Inference is animated as a wave flowing through the CNN layers.

Features:
- Smooth layer transitions
- Progressive activation updates
- Dynamic scaling
- Temporal inference visualization

:contentReference[oaicite:4]{index=4}

---

# Visualization Design

The visualization follows several important design principles:

## Spawn Once, Update Colors

All ~12,500 cubes are instantiated only once at startup to maintain high FPS performance.

Only:
- colors,
- activations,
- and scaling

are updated during inference.

---

## Intermediate Layer Extraction

Using Unity Sentis:

```csharp
model.AddOutput("conv1_out", 10);
model.AddOutput("pool1_out", 15);
model.AddOutput("conv2_out", 25);
model.AddOutput("pool2_out", 30);
```

Intermediate activations are directly extracted from the ONNX computational graph.

:contentReference[oaicite:5]{index=5}

---

# Human-Computer Interaction (HCI)

This project focuses heavily on educational interaction.

Goals:
- Make neural networks tangible
- Improve CNN intuition
- Allow interactive experimentation
- Reduce abstraction barriers
- Transform CNNs into explorable systems

---

# PCA and Dimensionality Reduction

The project also explores dimensionality reduction concepts using PCA for visualizing high-dimensional feature spaces.

The flatten layer (784 dimensions) can be projected into 3D PCA space to visualize:
- digit clustering,
- class separability,
- latent feature evolution.

:contentReference[oaicite:6]{index=6}

---

# Performance

- Real-time inference: 30+ FPS
- ~12,500 active cubes
- Real-time feature extraction
- GPU-assisted rendering
- Interactive user feedback

---

# Applications

- AI Education
- CNN Visualization
- Explainable AI (XAI)
- Human-AI Interaction
- Deep Learning Teaching
- Interactive ML Demonstrations
- Research Visualization Systems

---

# Limitations

Current limitations include:
- Scalability for very large networks
- High object count rendering overhead
- Memory requirements for large models

Potential improvements:
- GPU instancing
- Top-K neuron visualization
- Level-of-detail rendering
- Saliency-based filtering

:contentReference[oaicite:7]{index=7}

---

# Future Work

- Transformer visualization
- Vision Transformer (ViT) support
- Attention map rendering
- 3D latent space navigation
- VR/AR integration
- Real-time explainability tools
- Large-scale CNN optimization

---

# Research Contribution

This project demonstrates how game engines and machine learning frameworks can be combined to create intuitive educational tools for deep learning understanding.

The work bridges:
- Computer Vision
- Human-Computer Interaction
- Explainable AI
- Scientific Visualization
- Interactive Learning Systems

---

# Authors

### Arjun Ambekar
PhD Research Scholar  
Department of EECE  
IIT Dharwad

### Nicholas Khumukcham
PhD Research Scholar  
Department of CSE  
IIT Dharwad

Under the guidance of:

**Dr. Shashaank Aswatha Mattur**  
Department of EECE  
IIT Dharwad

:contentReference[oaicite:8]{index=8}

---

# References

- Harley, A. W. (2015). Interactive visualization of CNNs.
- Kahng et al. (2019), IEEE TVCG
- Unity Sentis Documentation
- PyTorch Documentation
- ONNX Runtime Documentation

---

# License

This project is intended for academic, educational, and research purposes.

---

# Acknowledgements

- IIT Dharwad
- Unity Sentis
- PyTorch
- MNIST Dataset
- Open-source AI community
