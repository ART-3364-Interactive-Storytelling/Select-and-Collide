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