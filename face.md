# Integrating facial recognition

- [Prerequisites](#prerequisites)
- [Face detection and bounding boxes](#face-detection-and-bounding-boxes)
- [Face recognition](#face-recognition)

## Prerequisites

- Unity project configured as described in [setup.md](setup.md)

## Face detection and bounding boxes

The facial recognition pipeline requires a 128x128 cropped image of a face, 
so an additional bounding box detection stage is needed first.

1. Download the `.onnx` model file found in the `models` folder of the Unity BlazeFace Hugging Face repository [here](https://huggingface.co/unity/inference-engine-blaze-face/tree/main).
2. Also download the `anchors.csv` file found in the `data` folder of the same repository.

## Face recognition

1. 
