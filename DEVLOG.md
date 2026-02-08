# Development Log - Lololowkas Game

A 2D platformer game project developed with Unity as part of educational sessions.

---

## 2026-02-08 - Bee Enemy AI & Health System Improvements

### Session Goals
- Fix bee enemy behavior issues
- Improve player attack system
- Polish animation transitions

### Problems Identified & Solutions

#### 1. Bee Enemy Positioning Issue
**Problem:** Bee was positioning itself below the player during chase/attack, making combat awkward.

**Solution:**
- Added `hoverHeightOffset` parameter (default 1.5f) to make bee hover above player
- Modified `ChaseUpdate()` to target `playerPosition + Vector2.up * hoverHeightOffset`

**Files Changed:**
- `Assets/Scripts/Mobs/BeeEnemy.cs`

---

#### 2. Bee Passing Through Platforms
**Problem:** Bee was passing through tilemap platforms during movement.

**Root Cause:** Using `rb.MovePosition()` which teleports the object ignoring physics collisions.

**Solution:**
- Replaced `rb.MovePosition()` with `rb.velocity` in `PatrolUpdate()` and `ChaseUpdate()`
- Changed `rb.linearVelocity = dir * speed` for proper physics-based movement

**Files Changed:**
- `Assets/Scripts/Mobs/BeeEnemy.cs`

---

#### 3. Bee Attack Animation Issues
**Problem:**
- Attack damage was applied instantly instead of at animation hit frame
- Animation was too fast and felt unresponsive
- Bee was falling during attack (gravity pulling it down)

**Solution:**
- Added `isAttacking` flag to prevent multiple simultaneous attacks
- Created `OnAttackHit()` and `OnAttackEnd()` methods for Animation Events
- Set `rb.linearVelocity = Vector2.zero` during attack to stop movement
- **Fixed gravity:** Set `Gravity Scale = 0` in Bee Rigidbody2D (it's a flying enemy!)
- Moved damage dealing logic to `OnAttackHit()` triggered by animation event

**Files Changed:**
- `Assets/Scripts/Mobs/BeeEnemy.cs`
- `Assets/Prefabs/Bee.prefab` (Rigidbody2D settings)
- `Assets/Animations/Attack.anim` (added Animation Events)

---

#### 4. Bee Animator Not Working
**Problem:** Animator had no parameters, transitions had no conditions, animations were stuck.

**Solution - Animator Setup:**
1. **Added Parameters:**
   - `Speed` (Float)
   - `Attack` (Trigger)
   - `Hit` (Trigger)
   - `Die` (Trigger)

2. **Fixed Transitions:**
   - `Any State → Attack`: Added condition `Attack` trigger, removed Exit Time
   - `Any State → Hit`: Added condition `Hit` trigger, removed Exit Time
   - `Any State → Die`: Added condition `Die` trigger, removed Exit Time
   - `Attack → Fly`: Added with Exit Time = 0.9
   - `Hit → Fly`: Added with Exit Time = 0.9

**Files Changed:**
- `Assets/Animations/Bee.controller`

---

#### 5. Player Attack Not Working Near Bee
**Problem:** Player couldn't attack when bee was nearby or during bee's attack animation.

**Root Cause:**
- Player Animator transition `Any State → Attack` had `Has Exit Time = true`
- Knockback from bee created velocity, triggering Run animation
- Player was stuck in Run animation and couldn't transition to Attack

**Solution:**
1. **Fixed Attack Transition:**
   - `Any State → Attack`: Set `Has Exit Time = false`, `Transition Duration = 0`

2. **Fixed Input Reading:**
   - Changed `PlayerAnimator` to read **input** instead of **velocity**
   - This prevents Run animation during knockback
   - Added `PlayerInputHandler` reference to `PlayerAnimator`

**Files Changed:**
- `Assets/Animations/Player.controller`
- `Assets/Scripts/Player/PlayerAnimator.cs`

---

#### 6. Animation "Bounce" on Player Stop
**Problem:** Player_Run animation flickered when player stopped moving.

**Root Cause:**
- Velocity oscillating around zero (0.005 → -0.002 → 0.003)
- Animator transitions had too-close thresholds: Idle→Run at 0, Run→Idle at 0.01

**Solution:**
- Added "dead zone" (`speedDeadZone = 0.05f`) in `PlayerAnimator`
- Speed below threshold is treated as zero
- Prevents animation dithering

**Files Changed:**
- `Assets/Scripts/Player/PlayerAnimator.cs`

---

#### 7. Bee State Flickering & Wrong Facing Direction
**Problem:**
- With `hoverHeightOffset = 1`, bee was flickering between Chase/Attack states
- Bee sprite was flipping rapidly
- Bee was facing wrong direction

**Root Cause:**
- Distance check was to player center, but bee flew to offset position
- Caused constant state switching (distance ≈ attackRange)
- Flip threshold too sensitive (0.01)

**Solution:**
1. **Fixed Distance Calculation:**
   - Calculate distance to target position (with height offset) instead of player center
   - Added hysteresis: Chase→Attack at `attackRange`, Attack→Chase at `attackRange * 1.5`

2. **Fixed Sprite Flipping:**
   - Increased flip threshold from 0.01 to 0.1
   - Fixed flip logic: `sr.flipX = facingRight` (was inverted)

**Files Changed:**
- `Assets/Scripts/Mobs/BeeEnemy.cs`

---

### Current State
- ✅ Bee enemy AI working smoothly
- ✅ Bee hovers above player during combat
- ✅ Bee respects physics collisions
- ✅ Attack animations properly synced with damage
- ✅ Player can attack while being attacked
- ✅ Animation transitions are smooth and responsive

---

### Technical Details

#### Bee Enemy Parameters (Current Settings)
```
hoverHeightOffset = 1.5f    // Height above player
patrolSpeed = 1.5f
chaseSpeed = 3.5f
attackRange = 1f
attackCooldown = 1f
damage = 15
knockbackForce = 3f
detectionRadius = 4f
Gravity Scale = 0           // Important for flying enemies!
```

#### Player Animator Settings
```
speedDeadZone = 0.05f      // Prevents animation dithering
Uses input instead of velocity for animations
```

---

### Key Learnings
1. **Flying enemies should have Gravity Scale = 0**
2. **Use velocity, not MovePosition, for physics-based movement**
3. **Animation Events are essential for timing attacks correctly**
4. **Animator transitions need proper conditions and exit times**
5. **Read player input, not velocity, for animations to avoid knockback issues**
6. **Use hysteresis (different thresholds) to prevent state flickering**
7. **Dead zones prevent "dithering" in animations and state machines**

---

## Previous Sessions

### Session: Health System Implementation
**Date:** Before 2026-02-08 (based on git history)

**What Was Done:**
- Created `Health.cs` component with events system
- Implemented `HealthBarUI.cs` for player health display
- Added health bar with smooth animation and color transitions
- Integrated Health component with player

**Git Commits:**
- `71aacf4` - add health bar UI
- `b8c3d02` - add var for knockbackForce
- `5b4b064` - init
- `57ad08a` - Initial commit

---

## Future Sessions - Ideas & Improvements

### Potential Next Steps
- [ ] Add more enemy types
- [ ] Implement enemy spawn system
- [ ] Add player death/respawn mechanics
- [ ] Create more attack combos
- [ ] Add sound effects for combat
- [ ] Implement level progression system
- [ ] Add collectibles/power-ups
- [ ] Polish visual effects (hit effects, attack trails)

---

*This devlog is maintained to track progress between educational sessions.*
