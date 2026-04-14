# 2048 Puzzle Game - C# Implementation

A fully functional clone of the classic 2048 puzzle game, built with C# and focusing on algorithmic efficiency and smooth User Experience (UX).

## 🚀 Overview
This project was developed as a personal challenge to master **Object-Oriented Programming (OOP)** and **Matrix manipulation** in C#. The goal is to slide numbered tiles on a grid to combine them to create a tile with the number 2048.

## ✨ Key Features
- **Efficient Game Logic:** Used 2D arrays and optimized matrix traversal algorithms for tile movement and merging.
- **Event-Driven Input:** Seamless keyboard controls (Arrow keys/WASD) implemented through Event Handlers.
- **Score Tracking:** Real-time score calculation and persistent High-Score storage.
- **UX/UI Focused:** Applied **Nielsen’s Heuristics** to ensure a minimalist, intuitive interface with clear visual feedback for "Game Over" and "Victory" states.
- **Responsive Animations:** (Optional: Nếu bạn có làm hiệu ứng) Smooth tile transitions and merging effects.

## 🛠 Tech Stack
- **Language:** C#
- **Framework:** .NET / WinForms / Unity (Chọn cái bạn dùng)
- **IDE:** Antigravity / Visual Studio
- **Version Control:** Git & GitHub

## 🧠 Technical Highlights
### Matrix Transformation
Instead of writing 4 different functions for each direction, I implemented a generic movement logic combined with **Matrix Rotation** to handle Up, Down, Left, and Right moves efficiently, reducing code redundancy.

### UX Implementation
Applying Don Norman’s principles, I focused on:
- **Visibility of system status:** Always showing the current score and game state.
- **User control and freedom:** (Optional: Nếu có nút Undo) Simple "New Game" and "Undo" functionalities.

## 📸 Screenshots
*(Include screenshots of your game here to make it more visual)*
![Game Preview](https://via.placeholder.com/400x400.png?text=2048+Game+Preview)

## 🏗 How to Run
1. Clone this repository:
   ```bash
   git clone [https://github.com/DangoTheCat/2048-Puzzle-Game.git](https://github.com/DangoTheCat/2048-Puzzle-Game.git)
