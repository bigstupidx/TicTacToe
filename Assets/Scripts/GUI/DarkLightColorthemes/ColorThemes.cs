using UnityEngine;

public static class ColorThemes {

    public static PreferencesScript.ColorTheme DefaultTheme
        = new PreferencesScript.ColorTheme(Color.red, Color.blue, new Color(0f, 0.58824f, 0.53333f), new Color(1f, 0.75686f, 0.02745f), "Default");

    public static PreferencesScript.ColorTheme DefaultAltTheme
        = new PreferencesScript.ColorTheme(new Color(1f, 0.59608f, 0f), new Color(0.40392f, 0.22745f, 0.71765f), new Color(0.96078f, 0f, 0.34118f), new Color(0.46275f, 1f, 0.01176f), "DefaultAlt");

    public static PreferencesScript.ColorTheme AltAltTheme
        = new PreferencesScript.ColorTheme(new Color(0.47451f, 0.33333f, 0.28235f), new Color(1f, 0.34118f, 0.13333f), new Color(1f, 0.92157f, 0.23137f), new Color(0.39608f, 0.12157f, 1f), "DefaultAltAlt");

    public static PreferencesScript.ColorTheme AAAltTheme
        = new PreferencesScript.ColorTheme(new Color(1f, 0.18824f, 0f), new Color(0f, 0.69804f, 0.4549f), new Color(1f, 0.75686f, 0.01569f), new Color(0.01569f, 0.18431f, 1f), "DefaultAAAlt");

    public static PreferencesScript.ColorTheme GetTheme(string name) {
        switch (name) {
            case "DefaultTheme": return DefaultTheme;
            case "DefaultAltTheme": return DefaultAltTheme;
            case "DefaultAltAltTheme": return AltAltTheme;
            case "DefaultAAAltTheme": return AAAltTheme;
        }

        return DefaultTheme;
    }

}
