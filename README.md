# Rope Physics with Verlet Integration

A custom rope physics simulation built from scratch using **Verlet Integration** — a numerical method used in professional game engines for cloth, hair, and rope simulation. This project was built to understand the mathematics behind physics engines rather than relying on Unity's built-in physics components.

![Verlet Rope Demo](screenshots/firstgif.gif)
*Real-time rope simulation using Verlet Integration with collision detection*

## 🎯 Project Goals

After building a rope system with ConfigurableJoints in my previous project, I wanted to understand **how physics engines actually work** under the hood. This project implements rope physics from first principles using mathematical integration.

**Learning Objectives:**
- Understand **numerical integration methods** (Verlet vs Euler)
- Implement **constraint solving** for distance preservation
- Build **collision detection** without Unity's physics components
- Optimize performance with **sub-stepping** and iteration counts

## 🧮 What is Verlet Integration?

Verlet Integration is a numerical method for integrating equations of motion. It's widely used in game physics because it's:
- ✅ **More stable** than Euler integration
- ✅ **Simpler** than Runge-Kutta methods
- ✅ **Implicitly handles velocity** without storing it
- ✅ **Time-reversible** (important for accuracy)

### The Math

**Standard Verlet Formula:**
```
newPosition = 2 * currentPosition - previousPosition + acceleration * dt²
```

**Why This Works:**
```
Velocity is implicit: velocity ≈ (currentPosition - previousPosition) / dt
Acceleration is explicit: gravity, forces, etc.
Position updates directly without velocity storage
```

**Comparison to Euler Integration:**
```
Euler (less stable):
  velocity += acceleration * dt
  position += velocity * dt

Verlet (more stable):
  position = 2 * position - prevPosition + acceleration * dt²
```

## 🔧 How It Works

The implementation follows a simple three-step process each frame:

### 1. Verlet Integration
Update each node's position using the Verlet formula:
```
newPosition = 2 × currentPosition - previousPosition + gravity × dt²
```

The beauty of Verlet: **velocity is implicit** in the difference between positions, so we don't need to store or calculate it separately.

### 2. Constraint Solving
After integration, nodes might drift apart. We fix this by:
- Calculating distance between adjacent nodes
- Moving both nodes toward each other equally to maintain target distance
- Pinning endpoints to their attachment points

Multiple iterations per frame ensure the rope stays together.

### 3. Collision Detection
Each node checks for collisions with nearby objects:
- Uses `OverlapSphereNonAlloc` for performance (no garbage allocation)
- Calculates penetration depth when overlapping
- Pushes nodes out along collision normal

### Sub-stepping
Instead of one large physics step, we take multiple smaller steps:
```
4 sub-steps × 10 constraint iterations = stable rope
```

This prevents stretching and keeps the simulation accurate even during fast movement

## 📊 Verlet vs ConfigurableJoint Comparison

Having built rope systems both ways, here's what I learned:

| Aspect | ConfigurableJoint | Verlet Integration |
|--------|------------------|-------------------|
| **Setup Complexity** | High (many parameters) | Low (just math) |
| **Performance** | Moderate (Unity PhysX) | High (custom code) |
| **Stability** | Good | Excellent (with tuning) |
| **Control** | Limited | Complete |
| **Understanding** | Black box | Full transparency |
| **Collision** | Built-in | Manual implementation |

**When to use Verlet:**
- Need maximum performance
- Want full control over simulation
- Building custom physics
- Learning how engines work

**When to use ConfigurableJoint:**
- Need quick prototype
- Want built-in collision
- Complex joint requirements

## 🎓 What I Learned

### Mathematical Physics
- **Numerical integration** is the foundation of all physics engines
- **Verlet method** trades velocity storage for stability
- **Constraint solving** is just iterative correction
- **Sub-stepping** is essential for accuracy

### Performance Optimization
- `OverlapSphereNonAlloc` prevents garbage allocation
- Reusable arrays for repeated operations
- Iteration count vs accuracy trade-off
- Sub-stepping increases cost but improves quality

### Practical Implementation
- **Pin nodes** to prevent drift at attachment points
- **Interleave collision** with constraint solving
- **Small penetration buffer** (×1.01) prevents jitter
- **Skip endpoint collision** since they're fixed

## 🔍 Challenges & Solutions

**Rope Stretching Under Fast Movement**
- Increased iteration count and sub-steps
- More iterations = tighter constraints

**Collision Jitter**
- Added small penetration buffer (×1.01)
- Reset velocity when resolving collision
- Prevents vibration on surfaces

**Performance with Many Nodes**
- Used `OverlapSphereNonAlloc` to avoid garbage
- Interleaved collision checks (every 2nd iteration)
- Skipped collision for fixed endpoints

## 🛠️ Technical Stack

- **Unity 6000.2.6f2**
- **Pure C#** (no external physics)
- **Verlet Integration** algorithm
- **Custom collision detection**

## 📂 Code Structure

```
Assets/
└── Rope.cs                # Complete Verlet rope implementation
    ├── CalculateNewPositions()   # Verlet integration step
    ├── FixNodeDistances()        # Constraint solving
    └── ResolveCollision()        # Custom collision detection
```

## 💡 Key Takeaways

**What Worked:**
- Verlet integration is **surprisingly simple** to implement
- Sub-stepping dramatically improves stability
- Custom physics gives complete control
- No velocity storage = less state to manage

**What I'd Improve:**
- Add damping for energy dissipation
- Implement bending constraints for stiffness
- Use spatial hashing for collision optimization
- Add visual mesh generation (currently just Gizmos)

**Biggest Insight:**
Physics engines aren't magic — they're just math loops. Verlet integration showed me that complex-looking physics is built from simple principles:
1. Update positions based on previous positions
2. Apply constraints
3. Handle collisions
4. Repeat

Understanding this foundation makes all physics systems less intimidating.

## 📚 Learning Resources

This implementation was inspired by and learned from:

**Primary Resource:**
- [Simulating Rope Physics in Unity](https://blog.devgenius.io/verlet-integration-simulating-rope-physics-in-unity-b4f0ffde38fb) by **DevGenius** — Excellent breakdown of Verlet integration for rope simulation

**Additional Reading:**
- *Game Physics Engine Development* by Ian Millington
- Jakobsen's 2001 GDC paper on Verlet integration
- Unity Physics documentation

## 🎯 Future Improvements

- [ ] Visual mesh generation for rope rendering
- [ ] Bending constraints for realistic rope stiffness
- [ ] Wind forces and external force application
- [ ] Spatial partitioning for collision optimization
- [ ] Distance field collision for complex meshes

## ⚠️ Project Status

This is a **learning implementation** focused on understanding the mathematics behind rope physics. It demonstrates core concepts but would need optimization for production use (mesh rendering, LOD, spatial optimization).

---

**Developer**: Mert Özzencir  
**GitHub**: [MertOzzencir](https://github.com/MertOzzencir)  
**Learning Focus**: Numerical integration, constraint solving, custom physics implementation  
**Credits**: Inspired by [DevGenius's Verlet Integration tutorial](https://blog.devgenius.io/verlet-integration-simulating-rope-physics-in-unity-b4f0ffde38fb)