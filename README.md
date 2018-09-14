# OpenPoseUnityDemo

## Overview: 
This repository uses OpenPose as base library. Link: https://github.com/CMU-Perceptual-Computing-Lab/openpose
This repository contains two projects: 
1. unitydemo: This project was intended to be used in CVPR 2018 for OpenPose demonstration. It only involves OpenPose 3D reconstruction, including visualizing streaming data, loading BVH file and JSON file (loading FBX file is not supported yet). 
2. OpenPosePlugin: This project is developed to create a OpenPose plugin for Unity users. It uses modified OpenPose dll library and provides formatted OpenPose output and some examples. 

## To run OpenPosePlugin: 
You need to get additional files (dll and model files) from this Google Drive link:
https://drive.google.com/drive/folders/1b4lbMjkqAJtTCszKwBAjxC_TeBKZ-dqM?usp=sharing

And follow the steps: 
1. Clone or download the project into your local machine. 
2. Put the unzipped “models” folder, from the link above, into “OpenPosePlugin” root folder.
3. Put the unzipped “Plugins” folder into “OpenPosePlugin/Assets” folder. 
4. Try to run the Output2D scene in Unity editor. 

NOTICE: 
1. If you are trying to run the built project, you may also need to copy the models folder to the build root folder to run it.
2. This is only a beta version so far, everything is subject to change in later versions. The plugin will finally be available on Unity Assets store after release. 
