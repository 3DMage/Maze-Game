using Microsoft.Xna.Framework;
using System;

// This computes a color somewhere on color wheel based on game time.  Makes a rainbow effect.
public class RainbowEffect
{
    // Obtains current color on color wheel based on game time.
    public Color GetColor(GameTime gameTime)
    {
        // Calculate hue based on the current time, cycling every 10 seconds.
        float seconds = (float)gameTime.TotalGameTime.TotalSeconds;

        // Compute hue based on seconds elapsed.
        float hue = seconds * 5 * 360f / 10f % 360f;

        // Obtain RGB color from HSV values.
        Color color = HSVtoRGB(hue, 1f, 1f); // Full saturation and value for vibrant colors

        return color;
    }

    // Computes a RBG color based on HSV values.
    private Color HSVtoRGB(float hue, float saturation, float value)
    {
        // Compute region of color wheel.
        int hueIndex = Convert.ToInt32(Math.Floor(hue / 60)) % 6;

        // Compute how much in the color wheel is interpolated.
        double hueFactor = hue / 60 - Math.Floor(hue / 60);

        // Convert value into 0-255 range.
        value = value * 255;

        // Color components to construct the final color.
        int valueComponent = Convert.ToInt32(value);
        int lowestIntensity = Convert.ToInt32(value * (1 - saturation));
        int transitionFactor = Convert.ToInt32(value * (1 - hueFactor * saturation));
        int inverseTransitionFactor = Convert.ToInt32(value * (1 - (1 - hueFactor) * saturation));

        if (hueIndex == 0)
        {
            // Redd-ish color.
            return new Color(valueComponent, inverseTransitionFactor, lowestIntensity);
        }
        else if (hueIndex == 1)
        {
            // Yellow-ish color.
            return new Color(transitionFactor, valueComponent, lowestIntensity);
        }
        else if (hueIndex == 2)
        {
            // Green-ish color.
            return new Color(lowestIntensity, valueComponent, inverseTransitionFactor);
        }
        else if (hueIndex == 3)
        {
            // Cyan-ish color.
            return new Color(lowestIntensity, transitionFactor, valueComponent);
        }
        else if (hueIndex == 4)
        {
            // Blue-ish color.
            return new Color(inverseTransitionFactor, lowestIntensity, valueComponent);
        }
        else
        {
            // Magenta-ish color.
            return new Color(valueComponent, lowestIntensity, transitionFactor);
        }
    }
}


