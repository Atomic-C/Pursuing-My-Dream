using UnityEditor;
using UnityEngine;

/// <summary>
/// Class created as an exercise with Master D course
/// To be able to change the properties of an asset, when imported
/// In this case, changing the properties of imported textures
/// </summary>
public class PixelImportProcessor : AssetPostprocessor
{
    /// <summary>
    /// Bool used to determine if the functionality of this script will be used or not
    /// </summary>
    public bool useScript;

    public void OnPreprocessTexture()
    {
        // Deactivated for now
        useScript = false;

        if (useScript)
        {
            TextureImporter importer = (TextureImporter)assetImporter;

            //importer.textureType = TextureImporterType.Sprite;
            //importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            //importer.alphaIsTransparency = true;
            //importer.spritePixelsPerUnit = 32;

            importer.SaveAndReimport();
        }  
    }
}
