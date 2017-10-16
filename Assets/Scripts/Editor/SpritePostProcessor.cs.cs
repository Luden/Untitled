using UnityEngine;
using UnityEditor;

public class SpritePostProcessor : AssetPostprocessor
{
	int pixelsPerUnit = 16;
	FilterMode filterMode = FilterMode.Point;
	SpriteAlignment spriteAlignment = SpriteAlignment.BottomCenter;

	void OnPostprocessTexture(Texture2D texture)
	{
		TextureImporter ti = (assetImporter as TextureImporter);
		ti.spritePixelsPerUnit = pixelsPerUnit;
		ti.filterMode = filterMode;

		TextureImporterSettings texSettings = new TextureImporterSettings();
		ti.ReadTextureSettings(texSettings);
		texSettings.spriteAlignment = (int)spriteAlignment;
		ti.SetTextureSettings(texSettings);
	}
}