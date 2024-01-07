using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;
using PNM;
using System.Linq.Expressions;

[ScriptedImporter(1, new string[] { "pnm", "pgm", "ppm", "pbm" })]
public class PNMImporter : ScriptedImporter
{
    public TextureFormat format = TextureFormat.RGBA32;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var pnm = PortableAnyMap.FromFile(ctx.assetPath);
        if(pnm == null)
        {
            Debug.LogError("Failed to load PNM file: " + ctx.assetPath);
            return;
        } else if (pnm.Number != MagicNumber.P2 && pnm.Number != MagicNumber.P5 && pnm.BytesPerPixelChannel != 1)
        {
            Debug.LogError("currently unsupported PNM file: " + ctx.assetPath);
            return;
        }

        if(format != TextureFormat.Alpha8 && format != TextureFormat.RGBA32)
        {
            Debug.LogError("Unsupported texture format: " + format);
            return;
        }

        var texture = new Texture2D(pnm.Width, pnm.Height, format, false);

        switch(format){
            case TextureFormat.Alpha8:
            case TextureFormat.R8:
                texture.SetPixelData(pnm.Bytes, 0);
                break;
            case TextureFormat.RGBA32:
                for (int y = 0; y < pnm.Height; y++)
                {
                    for (int x = 0; x < pnm.Width; x++)
                    {
                        var grey = pnm[x, y];
                        texture.SetPixel(x, y, new Color(grey / 255f, grey / 255f, grey / 255f, 1));
                    }
                }
                break;
        }

        ctx.AddObjectToAsset("Texture", texture);
        ctx.SetMainObject(texture);
    }
}
