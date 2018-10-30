# OpenPoseUnityDemo

## Overview: 
This repository uses OpenPose as base library. Link: https://github.com/CMU-Perceptual-Computing-Lab/openpose
This repository contains two projects: 
1. unitydemo: This project was intended to be used in CVPR 2018 for OpenPose demonstration. It only involves OpenPose 3D reconstruction, including visualizing streaming data, loading BVH file and JSON file (loading FBX file is not supported yet). 
2. OpenPosePlugin: This project is developed to create a OpenPose plugin for Unity users. It uses modified OpenPose dll library and provides formatted OpenPose output and some examples. 

## To run OpenPosePlugin: 
You need to get additional files (dll and model files) from [this Google Drive link](https://drive.google.com/drive/folders/1b4lbMjkqAJtTCszKwBAjxC_TeBKZ-dqM?usp=sharing)

And follow the steps: 
1. Install CUDA and CUDNN as OpenPose prerequisites refering to [OpenPose Installation Guide](https://github.com/CMU-Perceptual-Computing-Lab/openpose/blob/master/doc/installation.md#prerequisites).
2. Clone or download the project into your local machine. 
3. Put the unzipped “models” folder, from the link above, into “OpenPosePlugin/Assets/StreamingAssets” root folder.
4. Put the unzipped “Plugins” folder into “OpenPosePlugin/Assets/OpenPose” folder. 
5. Try to run the Output2D scene in Unity editor. 

NOTICE: 

This is only a beta version so far, everything is subject to change in later versions. The plugin will finally be available on Unity Assets store after release. 
