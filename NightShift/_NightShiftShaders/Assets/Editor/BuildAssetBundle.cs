using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public static class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    private static void BuildAllAssetBundles()
    {
        string outputPath = Path.GetFullPath(Application.dataPath + "/../../Assets/Shaders");

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        foreach (string filePath in Directory.GetFiles(outputPath)) {
            File.Delete(filePath);
        }

        AssetBundleBuild[] bundles = new AssetBundleBuild[1];
        bundles[0] = new AssetBundleBuild
        {
            assetBundleName = "NightShift",
            assetNames = RecursiveGetAllAssetsInDirectory("Assets/Shaders").ToArray()
        };

        const BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle;
        BuildPipeline.BuildAssetBundles(outputPath, bundles, buildOptions, BuildTarget.StandaloneWindows);

        foreach (string filePath in Directory.GetFiles(outputPath)) {
            if (filePath.EndsWith(".meta")) {
                File.Delete(filePath);
            }
        }

        string shaderBundle = Path.Combine(outputPath, "nightshift");

        if (File.Exists(shaderBundle))
        {
            File.Move(shaderBundle, shaderBundle + ".shab");
        }

        if (File.Exists(shaderBundle + ".manifest"))
        {
            File.Move(shaderBundle + ".manifest", shaderBundle + ".shab.manifest");
        }

        foreach (string filePath in Directory.GetFiles(outputPath)) {
            if (Path.GetFileName(filePath).StartsWith("Shaders")) {
                File.Delete(filePath);
            }
        }
    }

    private static List<string> RecursiveGetAllAssetsInDirectory(string path)
    {
        List<string> assets = new List<string>();
        foreach (string f in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            if (Path.GetExtension(f) != ".meta" &&
                Path.GetExtension(f) != ".cs" &&  // Scripts are not supported in AssetBundles
                Path.GetExtension(f) != ".unity") // Scenes cannot be mixed with other file types in a bundle
                assets.Add(f);

        return assets;
    }
}