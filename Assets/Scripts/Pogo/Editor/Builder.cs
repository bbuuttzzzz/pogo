using System;
using System.Linq;
using UnityEditor;

public static class Builder
{
    public static void BuildAll(string root)
    {
        //BuildWin64();
    }

    public static void BuildWin64(string path) => Build(path, BuildTarget.StandaloneWindows64);

    public static void BuildLinux64(string path) => Build(path, BuildTarget.StandaloneLinux64);

    public static void Build(string path, BuildTarget target)
    {
        var options = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            target = target,
            locationPathName = path,
        };

        BuildPipeline.BuildPlayer(options);
    }

    private static string[] GetScenes()
    {
        return EditorBuildSettings.scenes
            .Select(s => s.path)
            .ToArray();
    }
}