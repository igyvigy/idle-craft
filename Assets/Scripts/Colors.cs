using System;
using System.Collections.Generic;
using UnityEngine;

public class Colors
{
    private static Dictionary<string, Color> colors = new Dictionary<string, Color>();
    private static Color GetColor(string hash)
    {
        if (colors.ContainsKey(hash))
        {
            return colors[hash];
        }
        else
        {
            var color = GetColorFromString(hash);
            colors[hash] = color;
            return color;
        }
    }
    public static Color green
    {
        get { return GetColor("5DF40D"); }
    }

    public static Color orange
    {
        get { return GetColor("F4C70D"); }
    }

    public static Color red
    {
        get { return GetColor("F55442"); }
    }

    private static Color GetColorFromString(string color)
    {
        float red = Hex_to_Dec01(color.Substring(0, 2));
        float green = Hex_to_Dec01(color.Substring(2, 2));
        float blue = Hex_to_Dec01(color.Substring(4, 2));
        float alpha = 1f;
        if (color.Length >= 8)
        {
            // Color string contains alpha
            alpha = Hex_to_Dec01(color.Substring(6, 2));
        }
        return new Color(red, green, blue, alpha);
    }

    private static float Hex_to_Dec01(string hex)
    {
        return Hex_to_Dec(hex) / 255f;
    }

    private static int Hex_to_Dec(string hex)
    {
        return Convert.ToInt32(hex, 16);
    }
}