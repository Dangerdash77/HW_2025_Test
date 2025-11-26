# 🎮 Doofus Adventure Game  
### A Unity 3D Assignment for Hitwicket – HW 2025 Test  
Created by **Manav Kalola**

---

## 📌 Overview
**Doofus Adventure Game** is a fast-paced 3D survival puzzle where **Doofus**, a rolling ball, must keep moving across disappearing platforms (called *Pulpits*).  
Each pulpit has a limited lifetime and only **two** exist at any time.  
The challenge: **survive as long as possible and score points by stepping on new pulpits** before the floor collapses.

This project meets all requirements for Hitwicket’s **Game Developer – HW_2025_Test**, including:

✔ Character movement  
✔ Pulpit spawning logic based on JSON rules  
✔ Score tracking  
✔ Game Over screen  
✔ Main Menu  
✔ Audio (BGM + UI sounds)  
✔ Volume toggle system  
✔ Clean modular scripts  
✔ Unity 6 project with proper folder structure  
✔ Frequent Git commits  

---

## 🕹️ Gameplay Features

### 🟢 **Player Movement**
- Controlled using **WASD / Arrow keys**
- Uses **Rigidbody physics** for real rolling movement
- Attached **camera follow** system

### 🟩 **Pulpit System**
- Only **2 pulpits** active at a time  
- Spawn is adjacent (+X, -X, +Z, -Z)  
- Lifetime + spawn timings read from `StreamingAssets/doofus_diary.json`

### 📘 **JSON Configuration**
The game reads gameplay parameters from:
{
  "player_data": {
    "speed": 5.0
  },
  "pulpit_data": {
    "min_pulpit_destroy_time": 4.0,
    "max_pulpit_destroy_time": 8.0,
    "pulpit_spawn_time": 2.0
  }
}


⭐ Scoring
Score increments each time Doofus steps on a new pulpit
UI updates instantly


💀 Game Over
Triggered when:
Player falls off, OR All pulpits disappear
Game Over UI shows:
Final Score, Restart, Main Menu


🎛 Main Menu
Contains:
Start Game
Volume On/Off Toggle (saved in PlayerPrefs)
Exit Game


🔊 Audio
Background music plays during whole game
UI click sound
Global Mute/Unmute button
Audio state persists across sessions


🧩 Project Structure
Assets/
 ├── Materials/
 ├── Prefabs/
 │    ├── Pulpit.prefab
 │    ├── PlayerBall.prefab
 │
 ├── Scenes/
 │    ├── MainMenu.unity
 │    ├── GameScene.unity
 │
 ├── Scripts/
 │    ├── PlayerController.cs
 │    ├── CameraFollow.cs
 │    ├── Pulpit.cs
 │    ├── PulpitManager.cs
 │    ├── DiaryLoader.cs
 │    ├── AudioManager.cs
 │    ├── UIButtonSound.cs
 │    ├── MainMenuManager.cs
 │
 ├── UI/
 │    ├── Score UI
 │    ├── GameOver Panel
 │
 ├── StreamingAssets/
      ├── doofus_diary.json


▶️ How to Run
Clone the repo
git clone https://github.com/Dangerdash77/HW_2025_Test.git
Open Unity Hub → Select Unity 6+
Load the project
Open MainMenu.unity
Press Play 🎮


GamePlay Screenshots:

Main Menu:
<img width="1919" height="1019" alt="image" src="https://github.com/user-attachments/assets/7c77859b-e5db-423d-b987-f7b8769fd4d4" />

Level:
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/3940b417-6505-4fd9-a089-e63fba1cb5c8" />

Game Over Page:
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/c9a62071-9551-47dc-804a-6fd3949c5b8c" />


GamePlay Video:
https://github.com/user-attachments/assets/a0d24733-0b47-465e-8243-20f8b6b21788


📌 Notes
Game parameters are NOT hardcoded — they come from JSON.
UI and Audio work across all platforms.
Easy to extend with:
Particles effects
More pulpit types
More levels

👤 Author
Manav Kalola
VIT Vellore
Game Developer (Unity/Unreal) | Full-Stack Developer
