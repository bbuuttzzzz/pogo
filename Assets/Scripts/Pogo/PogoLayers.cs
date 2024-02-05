using UnityEngine;

public static class LAYERMASK
{
    // LevelChange Layers
    public const int Default = 1 << LAYER.Default;
    public const int TransparentFX = 1 << LAYER.TransparentFX;
    public const int IgnoreRaycast = 1 << LAYER.IgnoreRaycast;
    public const int Special = 1 << LAYER.Special;
    public const int Water = 1 << LAYER.Water;
    public const int UI = 1 << LAYER.UI;

    public const int player = 1 << LAYER.player;

    public const int SOLIDS_ONLY = Default;
    public const int ALL = ~0;

    public static int MaskForLayer(int layer)
    {
        int currentMask = 0;

        for (int otherLayer = 0; otherLayer <= LAYER.finalLayer; otherLayer++)
        {
            currentMask += Physics.GetIgnoreLayerCollision(layer, otherLayer) ? 0 : (1 << otherLayer);
        }

        return currentMask;
    }
};
public static class LAYER
{
    public const int finalLayer = player;

    public const int Default = 0;
    public const int TransparentFX = 1;
    public const int IgnoreRaycast = 2;
    public const int Special = 3;
    public const int Water = 4;
    public const int UI = 5;
    public const int player = 6;
    public const int trigger = 6;
}
