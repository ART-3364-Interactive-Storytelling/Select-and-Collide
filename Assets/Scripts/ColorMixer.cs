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