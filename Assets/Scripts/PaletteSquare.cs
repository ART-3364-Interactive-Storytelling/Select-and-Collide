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