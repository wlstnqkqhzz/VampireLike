using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Locks Android builds to normal portrait before Unity generates the final APK.
/// </summary>
public sealed class AndroidOrientationBuildPostprocessor : IPreprocessBuildWithReport, IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 1000;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android)
            return;

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
    }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string gradleRoot = Directory.GetParent(path)?.FullName ?? path;
        int changedCount = 0;

        foreach (string manifestPath in Directory.GetFiles(gradleRoot, "AndroidManifest.xml", SearchOption.AllDirectories))
        {
            if (ForcePortraitManifest(manifestPath))
                changedCount++;
        }

        if (changedCount > 0)
            Debug.Log($"Android orientation manifest patch applied: {changedCount} file(s).");
    }

    private static bool ForcePortraitManifest(string manifestPath)
    {
        if (!File.Exists(manifestPath))
            return false;

        XDocument document = XDocument.Load(manifestPath);
        XNamespace android = "http://schemas.android.com/apk/res/android";

        bool changed = false;
        foreach (XElement activity in document.Descendants("activity"))
        {
            string activityName = activity.Attribute(android + "name")?.Value ?? string.Empty;
            if (!activityName.Contains("UnityPlayer") && !activityName.Contains("VampireLikeActivity"))
                continue;

            activity.SetAttributeValue(android + "screenOrientation", "portrait");
            activity.SetAttributeValue(android + "resizeableActivity", "false");
            changed = true;
        }

        if (changed)
            document.Save(manifestPath);

        return changed;
    }
}
