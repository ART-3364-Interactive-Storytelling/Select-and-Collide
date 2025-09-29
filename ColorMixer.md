# ColorMixer Script

## Introduction
This document provides a detailed explanation of the `ColorMixer` script, written in C# for the Unity game engine. The script is designed to manage the color of a sprite in a Unity scene, allowing for additive color mixing and resetting to a default white color.

## Overview of the Script
The `ColorMixer` script is attached to a GameObject in the Unity scene, which must have a `SpriteRenderer` and a `Collider2D` component. The script uses the `SpriteRenderer` to control the color of the sprite.

### Variables and Properties
The script has several variables and properties:
* `currentColor`: a public `Color` variable that stores the current color of the sprite. It is initialized to white and can be changed in the Unity Inspector (line 6: `public Color currentColor = Color.white;`).
* `sr`: a private `SpriteRenderer` variable that references the `SpriteRenderer` component attached to the GameObject (line 7: `private SpriteRenderer sr;`).

### Initialization
The `Awake` method is called when the script is initialized (line 10: `void Awake()`). It:
1. Retrieves the `SpriteRenderer` component and stores it in the `sr` variable (line 11: `sr = GetComponent<SpriteRenderer>();`).
2. Sets the color of the sprite to the `currentColor` (line 12: `sr.color = currentColor;`).
3. Logs a message to the console indicating that the script has been initialized and the starting color (line 13: `Debug.Log($"[ColorMixer] Awake. Starting color = {ColorToStr(currentColor)}");`).

### Synchronization with the Inspector
The `OnValidate` method is called when the `currentColor` variable is changed in the Unity Inspector (line 16: `void OnValidate()`). It:
1. Retrieves the `SpriteRenderer` component if it has not been initialized yet (line 17: `if (sr == null) sr = GetComponent<SpriteRenderer>();`).
2. Sets the color of the sprite to the updated `currentColor` (line 18: `if (sr != null) sr.color = currentColor;`).

### Additive Color Mixing
The `AddColor` method adds a new color to the current color of the sprite (line 22: `public void AddColor(Color c)`). It:
1. Calculates the new color by averaging the current color and the new color for each color channel (red, green, and blue) (lines 23-26: `currentColor = new Color(...)`).
2. Sets the `currentColor` to the new color.
3. Updates the color of the sprite (line 27: `sr.color = currentColor;`).

### Resetting to White
The `ResetColor` method resets the color of the sprite to white (line 31: `public void ResetColor()`). It:
1. Sets the `currentColor` to white (line 32: `currentColor = Color.white;`).
2. Retrieves the `SpriteRenderer` component if it has not been initialized yet (line 33: `if (sr == null) sr = GetComponent<SpriteRenderer>();`).
3. Sets the color of the sprite to white (line 34: `if (sr != null) sr.color = currentColor;`).
4. Logs a message to the console indicating that the color has been reset (line 35: `Debug.Log("[ColorMixer] ResetColor -> White");`).

### Testing
The `TestAddRed` method is a test function that can be called from the Unity Inspector (line 39: `[ContextMenu("Test: Add Red")]` and line 40: `void TestAddRed()`). It adds the color red to the current color of the sprite (line 41: `AddColor(Color.red);`).

### Color Conversion
The `ColorToStr` method converts a `Color` object to a string representation in the format `(r:x.x, g:x.x, b:x.x)` (line 45: `private string ColorToStr(Color c) => $"(r:{c.r:0.##}, g:{c.g:0.##}, b:{c.b:0.##})";`).
