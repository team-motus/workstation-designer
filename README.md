# Workstation Designer

The MODS Workstation Designer is a program for creating virtual representations of factory workstations for simulation in the main MODS application, which is in development a different team at OSU. Together, these tools will help modular construction factory managers and designers prototype the layout of their factory workstations in order to improve efficiency. Specially, these tools will help them visualize the full effect of design changes without needing to physically implement them.

(Image of something cool here?)

---

## Setup
1. Install Unity 2020.3.2f1.
2. Clone this repository.
3. Open this repository in Unity with Unity version 2020.3.2f1.
4. In `/Assets/Scenes`, open either `TitleScreen.scene` or `MainScene.scene`. This will allow experimentation in the Unity editor.
5. [Optional] Create C# project files: In the Unity Editor, navigate to `Edit > Preferences > External Tools`, and click the `Regenerate Project Files` button to generate the .csproj files for the project. This allows easy integration of Visual Studio for the C# script development.
6. [Optional] Build the project: In the Unity Editor, navigate to `File > Build And Run` and select a folder to place the built project files.

### VR
To use the workstation designer program in VR download Valve's Steam VR from [here](https://www.steamvr.com/en/), set it up and then run it while the workstation designer application is running.
If you are a Oculus PCVR user you can follow this [tutorial](https://support.steampowered.com/kb_article.php?ref=3180-UPHK-0900) to setup Steam VR for Oculus devices.

---

## Usage
(How to section, use lots of images we have already taken of the UI).

To create a new workstation click on the new workstation button (Image of main menu here).

On the right side of the screen you will see a UI that can be used to place objects that will help your workers complete their task (Image of objects UI here).

At the top of the screen you will see a toolbar that can be used for a number of actions including:
- Saving/Loading
- Entering VR
- Exiting the program
- Measuring tool
(Image of toolbar here)

### Controls
In `Edit Mode` users can use WASD and their mouse to navigate the factory environment.

Edit Mode Control List:
- W | Move Forward
- A | Move Left
- S | Move Backwards
- D | Move Right
- Shift | Move Up
- Ctrl | Move Down
- Mouse Left click-and-hold | Rotation

---

## To-do
- Implementation of `Simulation Mode`
- Animations

---

## Inspiration
The VR portion of the project was inspired by the tutorials of [Valem](https://www.youtube.com/channel/UCPJlesN59MzHPPCp0Lg8sLw/videos).
