# Project Notes - Quick Reference

## 📁 Project Structure

```
Assets/
├── Animations/          # Animation clips and controllers
│   ├── Bee.controller
│   ├── Player.controller
│   └── *.anim files
├── Art/                 # Sprites and visual assets
├── Prefabs/            # Reusable game objects
│   └── Bee.prefab
├── Scenes/             # Game scenes
│   └── SampleScene.unity
├── Scripts/
│   ├── Mobs/           # Enemy scripts
│   │   ├── BeeEnemy.cs
│   │   └── Health.cs
│   └── Player/         # Player scripts
│       ├── HealthBarUI.cs
│       ├── PlayerAnimator.cs
│       ├── PlayerAttack.cs
│       ├── PlayerInputHandler.cs
│       └── PlayerMovement.cs
└── Settings/           # Project settings
```

---

## 🎮 Key Components

### Player Components (Required on Player GameObject)
1. `PlayerMovement` - handles physics and movement
2. `PlayerInputHandler` - processes input from keyboard/gamepad
3. `PlayerAnimator` - controls animation states and sprite flipping
4. `PlayerAttack` - handles attack logic and damage
5. `Health` - health management with events
6. `Rigidbody2D` - physics body
7. `Collider2D` - collision detection
8. `Animator` - animation controller

### Bee Enemy Components (Required on Bee GameObject)
1. `BeeEnemy` - AI state machine (patrol, chase, attack)
2. `Health` - health management
3. `Rigidbody2D` - physics body (Gravity Scale = 0!)
4. `Collider2D` - collision detection (set as Trigger)
5. `Animator` - animation controller

---

## ⚙️ Important Settings

### Bee Enemy (Prefab Settings)
```
Rigidbody2D:
  - Body Type: Dynamic
  - Gravity Scale: 0 (IMPORTANT!)
  - Collision Detection: Continuous

CircleCollider2D:
  - Is Trigger: true
```

### Layer Setup
- **Player Layer:** Used for player detection
- **Ground Layer:** Used for platform collision detection
- **Enemy Layer:** Used for enemy detection in player attacks

---

## 🔧 Animation Events Setup

### Player Attack Animation
- Event: `OnAttackHit()` - place at attack impact frame

### Bee Attack Animation
- Event: `OnAttackHit()` - place at attack impact frame (middle)
- Event: `OnAttackEnd()` - place at animation end

---

## 🎯 Current Parameters

### Bee Enemy (BeeEnemy.cs)
```csharp
hoverHeightOffset = 1.5f    // Height above player target
patrolSpeed = 1.5f          // Patrol movement speed
chaseSpeed = 3.5f           // Chase movement speed
attackRange = 1f            // Distance to start attack
attackCooldown = 1f         // Time between attacks
damage = 15                 // Damage per attack
knockbackForce = 3f         // Knockback force on player
detectionRadius = 4f        // How far bee can see player
```

### Player (PlayerMovement.cs)
```csharp
moveSpeed = 5f              // Horizontal movement speed
jumpForce = 10f             // Jump force
```

### Player Attack (PlayerAttack.cs)
```csharp
attackRange = 0.8f          // Attack radius
damage = 25                 // Damage per attack
attackCooldown = 0.5f       // Time between attacks
```

---

## 🐛 Common Issues & Solutions

### Bee Falls Through Platforms
**Solution:** Set `Rigidbody2D.Gravity Scale = 0` on Bee prefab

### Bee Flickers/Spins
**Solution:** Adjust `hoverHeightOffset` and `attackRange` to avoid state switching

### Player Can't Attack
**Solution:** Check Animator transitions - `Any State → Attack` should have `Has Exit Time = false`

### Animation Won't Play
**Solution:** Verify Animation Events are set up correctly in animation clips

### Knockback Causes Wrong Animation
**Solution:** `PlayerAnimator` should read input, not velocity

---

## 📚 Code Architecture

### Event System
The project uses C# events for decoupled communication:
- `Health.OnHealthChanged` - fired when health changes
- `Health.OnDied` - fired when health reaches zero

### State Machine Pattern
`BeeEnemy` uses enum-based state machine:
- `Patrol` → `Chase` → `Attack` → `Dead`

### Animation Event Pattern
Damage is applied through Animation Events, not immediately on button press.

---

## 🎓 Teaching Notes

### Concepts Covered
- ✅ Physics-based movement (Rigidbody2D, velocity)
- ✅ State machines for AI
- ✅ Event-driven architecture
- ✅ Animation system (Animator, transitions, parameters)
- ✅ Animation Events for precise timing
- ✅ Input System (new Unity Input System)
- ✅ UI updates from game events
- ✅ Component-based architecture

### Good Examples in Code
- **BeeEnemy.cs**: Clean state machine implementation
- **Health.cs**: Simple event system
- **PlayerAnimator.cs**: Separation of animation logic from movement

---

## 🔄 Quick Update Workflow

1. **Read DEVLOG.md** - see what was done last session
2. **Check TODO.md** - see what's planned next
3. **Make changes** - implement features
4. **Update DEVLOG.md** - document changes and problems solved
5. **Update TODO.md** - check off completed items, add new ones
6. **Commit to git** - save progress

---

*This file provides quick reference for project structure and settings.*
