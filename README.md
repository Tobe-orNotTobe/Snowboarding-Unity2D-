## Snowboard Rush
A dynamic 2D snowboarding game built in Unity where players slide down procedurally generated terrain, perform flips, collect coins, and try to achieve the highest score and distance possible.

### Technical Requirements
    Unity Version: Unity 6000.0.38f1
    Target Platform: Windows.
### Team Members
    _Nguyễn Quang Sơn
    _Võ Lê Đức Anh
    _Phan Đức Hậu
    _Nguyễn Minh Thành

## Core Features
![pic (5)](https://github.com/user-attachments/assets/9ddcaa8a-b341-423b-a221-0637349a7028)
![pic (4)](https://github.com/user-attachments/assets/7db1160d-42df-42bb-935c-af3e16a42308)

### Player Mechanics
- **Smooth Physics-Based Movement**: Control a snowboarder with realistic momentum and gravity
- **Intuitive Controls**: 
  - Left/Right Arrow keys to rotate
  - Spacebar to jump
- **Trick System**: 
  - Perform flips while in the air
  - Earn points for successful flips (3 points per flip)
  - Complete 360° rotations for maximum points
- **Realistic Audio**: Dynamic sound effects change based on speed and terrain

### Distance Tracking System
- **Real-time Distance Measurement**: Tracks how far you've traveled in meters
- **Record System**: Saves your farthest distance across gameplay sessions
- **Visual Markers**: Places a marker at your record-breaking point on the slope
- **Record Notifications**: Displays when you break your personal distance record

### Scoring System
- **High Score Tracking**: Saves top 5 scores across gameplay sessions
- **Score Display**: Real-time score updates during gameplay
- **Game Complete Summary**: Shows final score, highest score, and distance records

### Terrain Generation
- **Procedural Level Design**: Dynamically generates terrain as you progress
- **Varied Terrain Segments**: Multiple segment types create diverse gameplay experiences
- **Optimization**: Segments are pooled and recycled to maintain performance
- **Ski Lines**: Special paths that affect player speed and movement

### Game States
- **Main Menu**: Navigate between game options
- **Pause Functionality**: Pause the game with ESC key
- **Game Over Screen**: Displays final score, distance traveled, and records

### Collectibles & Obstacles
- **Coin Collection**: Gather coins to increase your score
- **Obstacles**: Navigate around spikes and other hazards
- **Death Detection**: Game ends when player crashes into obstacles or hazardous terrain
![Uploading pic (2).png…]()

### Visual Effects
- **Snow Particles**: Dynamic particle effects based on speed and terrain
- **Farthest Point Marker**: Visual indicator showing your record distance
- **Record Notifications**: Animated notifications when breaking records
![pic (3)](https://github.com/user-attachments/assets/041e29e2-9e15-4184-bbb2-d0e0826aa527)

## Game Flow
1. Start from the main menu
2. Play the game, performing tricks and collecting coins
3. Upon crashing, view your final score and distance
4. Try again to beat your personal records

## Technical Implementation
- **Score Persistence**: High scores and distance records saved between sessions
- **Object Pooling**: Efficient resource management for terrain segments
- **Physics-Based Controls**: Realistic snowboarding feel with torque and momentum
- **Responsive UI**: Clear feedback on points, distance, and game state

## Tips for Success
- Master the timing of jumps to perform multiple flips
- Use ski lines to maintain speed on flatter sections
- Aim to break your distance record each run
- Chain together tricks to maximize your score

