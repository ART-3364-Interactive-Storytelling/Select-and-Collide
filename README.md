# Select-and-Collide

# Unity 6 Tutorial: Color Mixing with Squares

This tutorial guides you through building a simple Unity 6 (2D) project to use Unity’s object system, scripts, and color blending.


## Goal

- One **white square** (the “canvas”).
- Eight **colored squares** (the “palette”).
- When a palette square is clicked, it follows the mouse.
- Clicking again:
  - If over the white square the square’s color is **added** to the white square.
  - If not over the palette square snaps back.

---

## 1. Project Setup

1. Create a new **2D project** in Unity 6.
2. Main Camera → Projection: **Orthographic**.
3. In the **Project** right click and create a ```2D ->Sprite → Square```.
4. Add a **WhiteSquare**:
   - Drag the sprite into the scene.
   - Rename it `WhiteSquare`.
   - Set **SpriteRenderer → Color = White**.
   - In the **Inspector** select ```Add Component``` and select ```Physics 2D -> BoxCollider2D```.
   - Scale to **(2, 2, 1)**.
   - Position at **(0, 0, 0)**.

---

## 2. Palette Squares

1. Duplicate the square sprite and name it `ColorSquare`.
2. Scale to **(2, 2, 1)**.
3. In the **Inspector** select ```Add Component``` and select ```Physics 2D -> BoxCollider2D```.
4. In the **Hierarchy** duplicate it until you have **8 total**.
5. Lay them out in a around the white square.
6. Give each one a unique color (red, green, blue, yellow, cyan, magenta, orange, purple).

---

## 3. Scripts

Create a `Scripts` folder and add these three scripts.

### `ColorMixer.cs` (attach to WhiteSquare)

```csharp
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class ColorMixer : MonoBehaviour
{
    [Tooltip("Current color of the white square (starts as white).")]
    public Color currentColor = Color.white;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = currentColor;
        Debug.Log($"[ColorMixer] Awake. Starting color = {ColorToStr(currentColor)}");
    }

    // If you tweak currentColor in the Inspector, keep the sprite in sync.
    void OnValidate()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = currentColor;
    }

    /// <summary>Add color additively (clamped to 1 per channel).</summary>
    public void AddColor(Color c)
    {
        // Simple average toward the new color
        currentColor = new Color(
            (currentColor.r + c.r) * 0.5f,
            (currentColor.g + c.g) * 0.5f,
            (currentColor.b + c.b) * 0.5f,
            1f
        );
        sr.color = currentColor;
    }

    /// <summary>Reset to pure white.</summary>
    public void ResetColor()
    {
        currentColor = Color.white;
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = currentColor;
        Debug.Log("[ColorMixer] ResetColor -> White");
    }

    // Quick manual test from the Inspector (gear menu).
    [ContextMenu("Test: Add Red")]
    void TestAddRed()
    {
        AddColor(Color.red);
    }

    private string ColorToStr(Color c) => $"(r:{c.r:0.##}, g:{c.g:0.##}, b:{c.b:0.##})";
}
```

### `PaletteSquare.cs` (attach to each colored square)

```csharp
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class PaletteSquare : MonoBehaviour
{
    [Header("Setup")]
    public Color squareColor = Color.red;
    public ColorMixer mixer;

    [Header("Visuals")]
    public float zWhileDragging = -1f;

    private SpriteRenderer sr;
    private Collider2D col;
    private Vector3 startPos;
    private bool dragging = false;
    private Vector3 dragOffset;
    private Camera cam;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        sr.color = squareColor;
        startPos = transform.position;
        cam = Camera.main;
        if (cam == null) cam = FindAnyObjectByType<Camera>();
    }

    void OnMouseDown()
    {
        if (dragging) return;
        if (cam == null) return;

        // Compute grab offset so it doesn't snap its center to the cursor
        Vector3 worldUnderPointer = ScreenToWorldOnObjectPlane(Input.mousePosition);
        dragOffset = transform.position - worldUnderPointer;
        dragging = true;
    }

    void OnMouseUp()
    {
        if (!dragging) return;
        TryApplyToMixer();
        SnapBack();
    }

    void Update()
    {
        if (!dragging || cam == null) return;

        Vector3 world = ScreenToWorldOnObjectPlane(Input.mousePosition);
        world += dragOffset;
        world.z = zWhileDragging;
        transform.position = world;
    }

    Vector3 ScreenToWorldOnObjectPlane(Vector3 screenPos)
    {
        // Convert on the plane where the square lives.
        // If camera z = -10 and object z = 0, distance from camera is 10.
        float distance = Mathf.Abs((transform.position.z) - cam.transform.position.z);
        screenPos.z = distance;
        return cam.ScreenToWorldPoint(screenPos);
    }

    void TryApplyToMixer()
    {
        if (mixer == null || col == null) { Debug.LogWarning("[PaletteSquare] Mixer or my collider missing."); return; }
        var mixerCol = mixer.GetComponent<Collider2D>();
        if (mixerCol == null) { Debug.LogWarning("[PaletteSquare] Mixer has no Collider2D."); return; }

        if (Overlaps2D(col.bounds, mixerCol.bounds)) // <-- your 2D overlap helper
        {
            Debug.Log($"[PaletteSquare] Applying {squareColor} to mixer.");
            mixer.AddColor(squareColor);
        }
        else
        {
            Debug.Log("[PaletteSquare] Dropped but NOT overlapping mixer.");
        }
    }

    // AABB overlap on X/Y only (ignores Z difference)
    bool Overlaps2D(Bounds a, Bounds b)
    {
        bool overlapX = a.max.x >= b.min.x && b.max.x >= a.min.x;
        bool overlapY = a.max.y >= b.min.y && b.max.y >= a.min.y;
        return overlapX && overlapY;
    }

    void SnapBack()
    {
        dragging = false;
        transform.position = startPos;
    }
}
```

### `SceneBinder.cs` (attach to a GameManager object)

```csharp
using UnityEngine;

public class SceneBinder : MonoBehaviour
{
    public ColorMixer mixer;

    void Awake()
    {
        if (mixer == null)
        {
            Debug.LogWarning("SceneBinder: Mixer not assigned.");
            return;
        }

        var squares = FindObjectsOfType<PaletteSquare>(includeInactive: true);
        foreach (var sq in squares)
        {
            sq.mixer = mixer;
        }
    }
}
```
### Hoooking up the Scripts
1. Drag the ColorMixer script onto the WhiteSquare.
2. Create GameManager (empty GameObject).
3. Drag the SceneBinder script onto the GameManager.
4. Drag WhiteSquare into the GameManager mixer field.
5. Select each palette square:
	•	Add PaletteSquare script to each.
	•	Set squareColor to match its sprite color.
	•	Leave Mixer empty (it’s auto-assigned).


