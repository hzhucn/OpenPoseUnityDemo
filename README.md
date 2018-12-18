# OpenPoseUnityDemo

## Overview: 
This repository uses OpenPose as base library. Link: https://github.com/CMU-Perceptual-Computing-Lab/openpose

OpenPosePlugin: This project is developed to create a OpenPose plugin for Unity users. It uses modified OpenPose dll library and provides formatted OpenPose output and some examples. 

## To run OpenPosePlugin: 
Follow the steps: 
1. Download additional files (Plugins and models) from [this Google Drive link](https://drive.google.com/drive/folders/1b4lbMjkqAJtTCszKwBAjxC_TeBKZ-dqM?usp=sharing)
2. Clone or download the project into your local machine. 
3. Put the unzipped “models” folder, from the link above, into “OpenPosePlugin/Assets/StreamingAssets” root folder.
4. Put the unzipped “Plugins” folder into “OpenPosePlugin/Assets/OpenPose” folder. 
5. Install CUDA and CUDNN as OpenPose prerequisites refering to [OpenPose Installation Guide](https://github.com/CMU-Perceptual-Computing-Lab/openpose/blob/master/doc/installation.md#prerequisites).
6. Open Unity editor and run the "Demo.unity" in "OpenPosePlugin/Assets/OpenPose/Examples/Scenes/". 
7. Read the [Detailed documentation](OpenPosePlugin/Assets/OpenPose/Documents/README.pdf) and the [UML diagram](OpenPosePlugin/Assets/OpenPose/Documents/OpenPoseUnityPlugin_UML.pdf) for more information. 

## NOTICE: 
This is an alpha release, everything is subject to change. The plugin will finally be available in Unity Assets store in the future. 
