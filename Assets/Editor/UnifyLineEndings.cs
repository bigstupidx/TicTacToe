#if UNITY_EDITOR_WIN

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class UnifyLineEndings {
    /// <summary>
    /// All supported line ending modes.
    /// </summary>
    private enum LineEndingMode {
        Windows,
        Mac,
        Unix
    }

    /// <summary>
    /// The file extensions to unify.
    /// </summary>
    private static readonly string[] s_fileExtensions = { "*.cs" };

    /// <summary>
    /// The line endings mode to use.
    /// </summary>
    private static readonly LineEndingMode s_lineEndingMode = LineEndingMode.Windows;

    /// <summary>
    /// A map between the line endings mode and the actual ascii representation.
    /// </summary>
    private static readonly Dictionary<LineEndingMode, string> s_lineEndingsMap = new Dictionary<LineEndingMode, string>();

    /// <summary>
    /// Static constructor to init the line endings map.
    /// </summary>
    static UnifyLineEndings() {
        s_lineEndingsMap[LineEndingMode.Windows] = "\r\n";
        s_lineEndingsMap[LineEndingMode.Mac] = "\r";
        s_lineEndingsMap[LineEndingMode.Unix] = "\n";
    }

    /// <summary>
    /// Called by Unity when scripts are reloaded by the editor.
    /// </summary>
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded() {
        var scripts = new HashSet<string>();

        // Find all scripts that match the specified file extensions.
        var rootFolder = Application.dataPath;
        foreach (var extension in s_fileExtensions) {
            scripts.UnionWith(Directory.GetFiles(rootFolder, extension, SearchOption.AllDirectories));
        }

        // Set the line endings on all the scripts found.
        foreach (var script in scripts) {
            SetScriptLineEndings(script, s_lineEndingMode);
        }
    }

    /// <summary>
    /// Loads the script at the specified path, replaces all the line endings
    /// then if the contents of the file have changed, write the file back out.
    /// </summary>
    /// <param name="scriptPath">The path to the script to unify.</param>
    /// <param name="mode">The line ending mode to use.</param>
    private static void SetScriptLineEndings(string scriptPath, LineEndingMode mode) {
        var scriptContents = File.ReadAllText(scriptPath);
        var unifiedContents = Regex.Replace(scriptContents, @"\r\n|\n\r|\n|\r", s_lineEndingsMap[mode]);

        if (scriptContents.Equals(unifiedContents, StringComparison.CurrentCultureIgnoreCase)) {
            return;
        }

        Debug.Log("Unifyed script line endings: " + scriptPath);
        File.WriteAllText(scriptPath, unifiedContents);
    }
}

#endif