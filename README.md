<h1><img src="./Misc/Logo.png" width="100" height="100" /> AR Country Data Explorer</h1>

*Visualize and explore data between countries with AR*

<img src="./Misc/Screenshots/screenshot%20(4).png" width="200" style="float: right"/>

## How AR was used
- Technologies used
  - Unity 2022.3.14f1
  - AR Foundation 5.1.1
  - Google Pixel 8/Android 14
- AR Features used
  - Plane Detection
  - Gesture Interactions (i.e. Touchscreen Taps)
  - Interactable Placement
- Other features
  - Graph algorithm
  - Inter-country connection animation (according to sibling data)

## How to interact

- Users can place countries on a surface (e.g. floor, table) via taps
- Users can drag countries around the area to get a good overview (select first!)
- Users can change and delete existing countries (floating UI buttons)
- Users can choose between data categories to be visualized (static top right button)
- Users can reset the AR session (static top left button)

## Hardware requirements

Android or iOS device with a camera

For detailed platform support information visit [the ARFoundation docs](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.0/manual/index.html#platform-support).

## Build instructions
- Open project
- File -> Build Settings
- Select Android
- Connect Android phone with Debugging enabled
- Click Build and Run (first compilation takes a while!)

## Learning outcomes
- Deep understanding of AR Foundation features in Unity
  - Tracker-based approach was discarded due to bad detection performance
  - Plane detection as a good alternative
  - Setup can be tedious:
    - Android Debugging Mode
    - Input/Event systems
    - Interaction Managers
    - XR Package vs AR Foundation
- (Unity) UI is painful
    - Unity UI Toolkit is easier to use
    - Differentiating between UI taps and "AR" taps can be difficult
- Visualization of connections was tedious
  - Unity's [Spline Package](https://docs.unity3d.com/Packages/com.unity.splines@2.3/manual/index.html) is not well documented and in an early stage
  - Logic for country connections was time-consuming to implement
- Light estimation was tested but worsened the readability of the visualization -> discarded

## Used datasets

Austria's 2023 Import/Export data ([Source](https://www.statistik.at/atlas/itgs/), accessed on 14.06.2024)

Estimated migration flows 2015-2020 ([Source](https://www.bib.bund.de/DE/Fakten/Tools/Migration/Globalflows.html), accessed on 14.06.2024)


## Notes
This project was part of an AR lecture at [my university](https://www.tuwien.at/).

Flag icons from https://github.com/lipis/flag-icons/ ([license](https://github.com/lipis/flag-icons/?tab=MIT-1-ov-file))

## Screenshots

<img src="./Misc/Screenshots/screenshot%20(1).png" width="200" />
<img src="./Misc/Screenshots/screenshot%20(2).png" width="200" />
<img src="./Misc/Screenshots/screenshot%20(3).png" width="200" />
<img src="./Misc/Screenshots/screenshot%20(4).png" width="200" />
<img src="./Misc/Screenshots/screenshot%20(5).png" width="200" />
<img src="./Misc/Screenshots/screenshot%20(6).png" width="200" />