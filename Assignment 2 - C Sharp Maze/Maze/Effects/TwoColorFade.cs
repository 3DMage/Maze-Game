using Microsoft.Xna.Framework;
using System;

// This comptues a color as a interpolation between two given colors.  Fades back and forth between the colors.
public class TwoColorFade
{
    // Start and end colors.
    private float startHue;
    private float endHue;

    // Transition duration in seconds.
    private float transitionDurationSeconds = 25f;

    // Constructor.
    public TwoColorFade(Color color1, Color color2)
    {
        startHue = GetHueFromColor(color1);
        endHue = GetHueFromColor(color2);
    }

    // Obtains current color interpolation of start and end colors based on game time.
    public Color GetCurrentColor(GameTime gameTime)
    {
        // Calculate the total elapsed time in seconds.
        float totalTime = (float)gameTime.TotalGameTime.TotalSeconds;

        // Calculate the position in the current cycle.
        float cycleTime = totalTime % (transitionDurationSeconds * 2); // Cycle includes both forward and backward transition.
        float fraction;

        // Determine if in the first half (forward transition) or the second half (backward transition) of the cycle.
        if (cycleTime <= transitionDurationSeconds)
        {
            // Forward transition.
            fraction = cycleTime / transitionDurationSeconds;
        }
        else
        {
            // Backward transition.
            fraction = 1 - ((cycleTime - transitionDurationSeconds) / transitionDurationSeconds);
        }

        // Interpolate the hue between startHue and endHue based on the calculated fraction.
        float hue = startHue + (endHue - startHue) * fraction;

        // Ensure hue is within the 0-360 range.
        hue = hue % 360f;

        // Create color from the interpolated hue
        Color color = HSVToRGB(hue, 1f, 1f); // Full saturation and value for vibrant colors.

        return color;
    }

    // Computes a RBG color based on HSV values.
    private Color HSVToRGB(float hue, float saturation, float value)
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

    // Computes hue from RGB color.
    private float GetHueFromColor(Color color)
    {
        // Hue to be computed.
        float hue = 0f;

        // Color components.
        float r = color.R / 255f;
        float g = color.G / 255f;
        float b = color.B / 255f;

        // Grab min and max amongst the R G B values.
        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));

        // Compute chroma.
        float chroma = max - min;
        
        // Compute hue based on chroma value.
        if (chroma != 0)
        {
            if (max == r)
            {
                // Reddish hue.
                hue = (g - b) / chroma;
            }
            else if (max == g)
            {
                // Greenish hue
                hue = (b - r) / chroma + 2;
            }
            else if (max == b)
            {
                // Blueish hue.
                hue = (r - g) / chroma + 4;
            }

            // Obtain the color "angle" on color wheel.
            hue *= 60;

            // Ensure hue falls between 0 - 360.
            if (hue < 0)
            {
                hue += 360;
            }
        }

        return hue;
    }
}
