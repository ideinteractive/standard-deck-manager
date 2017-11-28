using UnityEngine;

// contains a list of functions for GUI editor styles
public class EditorStyle
{
    // create a texture background
    public static Texture2D SetBackground(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pixels);
        result.Apply();

        return result;
    }
}