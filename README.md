Unity version: 2018.3.1f1

### Post Processing v2 Extension
* Panini Projection
* Gaussian Blur
* Kawase Blur
* Dual Blur
* Circle Blur (used in UE4 bloom effect)

#### Blur Post-processing Compare
|     Type      |                                            Image                                            | Parameter                                 |
|:-------------:|:-------------------------------------------------------------------------------------------:|:------------------------------------------|
|    Origin     |    ![](https://github.com/douduck08/Unity-PostProcessing/blob/master/images/origin.png)     |                                           |
| Gaussian Blur | ![](https://github.com/douduck08/Unity-PostProcessing/blob/master/images/gaussian_blur.png) | dowmsample = 1, radius = 4, iteration = 1 |
|  Kawase Blur  |  ![](https://github.com/douduck08/Unity-PostProcessing/blob/master/images/kawase_blur.png)  | downsample = 0, iteration = 5             |
|   Dual Blur   |   ![](https://github.com/douduck08/Unity-PostProcessing/blob/master/images/dual_blur.png)   | iteration = 3                             |
|  Circle Blur  |  ![](https://github.com/douduck08/Unity-PostProcessing/blob/master/images/circle_blur.png)  | downsample = 2, scale = 2                 |

#### Blur Post-processing References
* Chris Oat, Real-Time 3D Scene Post-processing, GDC Europe 2003
* Filip Strugar, An investigation of fast real-time GPU-based image blur algorithms, 2014
* Marius Bjørge, Bandwidth-efficient graphics, Siggraph 2015
* Niklas Smedberg & Timothy Lottes, Epic Games, Next-gen Mobile Rendering, GDC 2014