# DT-Reconfigurable-Soft-Robot

Welcome to this code repository, designed to support the implementation of the research paper titled "An Augmented Reality-enabled Digital Twin System for Reconfigurable Soft Robots: Visualization, Simulation, and Interaction," which has been published in the journal *Computers in Industry*.

In the rapidly evolving field of soft robotics, advancements in new materials, structural designs, and conceptual frameworks have propelled the rise of soft robot technology, particularly towards a highly versatile modular architecture with vast potential applications across various industries. However, one of the main challenges in this domain is the shape-morphing issue, as existing visualization and simulation tools struggle to adequately represent the complex and continuous deformation behaviors of soft robots. Furthermore, there is a distinct lack of intuitive, user-friendly platforms for visualizing and interactively controlling the shape-shifting capabilities of these robots.

In response to these challenges, this paper introduces an innovative Digital Twin (DT) system specifically designed for reconfigurable soft robots, operating within an Augmented Reality (AR) environment. This system facilitates a more natural and accurate depiction of 3D soft deformations while providing an intuitive interface for simulation. We utilize a parameterized curve-driven method to dynamically adapt the DT in the AR space, ensuring smooth transitions between various 3D shape-morphing states. We identify three fundamental shape-morphing patterns—stretching, bending, and twisting—and create advanced visualization tools to precisely demonstrate these morphological changes. 

To enhance real-time representation of shape-morphing, we employ sensor fusion to detect and depict the soft robot's structural changes as parameterized curves. Our system is fully operational in an AR environment, empowering users to conduct immersive examinations and simulate reconfigurations of real-world soft robotic systems. The source code will be released after it has been organized.

## Framework

![image](https://github.com/user-attachments/assets/71f707f9-2e47-4f6d-9508-94a39ea0f753)

## Demo



https://github.com/user-attachments/assets/19ca5b17-90c0-46e3-80ee-d75cc41bc1d9



## Project Overview

This project is a HoloLens2-compatible MRTK template, integrated with UDP web modules, based on the following technologies:

- Unity 2020.3.42f1 (LTS)
- OpenXR features
- Windows Mixed Reality Toolkit (MRTK) 2.7.2

## 3D Models

The required 3D models for the project can be found in the following zip file:

- [Download liao_model.zip](<https://github.com/yuanzero/DT-Reconfigurable-Soft-Robot/blob/master/liao_model.rar>)

## User Guide

1. **Download the project** and deploy it directly to HoloLens2.
2. In the Unity Editor, navigate to the top menu: Mixed Reality -> Toolkit -> Utilities -> Configure Project for MRTK -> Apply Settings. This step will help reconfigure your project for MRTK and avoid errors.
3. In the Unity Editor, locate the scene "Success_scene" in the "Scenes" folder. In this scene, you will find the game object "UDP Communication" as an example.
4. In the Arduino and ESP32 file, set your Wi-Fi and IP address, and upload it to the hardware.

For any questions or feedback, please contact:

**Zhongyuan Liao**  
**Hong Kong University of Science and Technology (HKUST)**  

Thank you for your interest!
