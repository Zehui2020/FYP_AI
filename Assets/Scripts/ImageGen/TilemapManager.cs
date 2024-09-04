using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [Header("Tile Slicing")]
    [SerializeField] private ImageSaver imageSaver;
    [SerializeField] private string fileName;
    [SerializeField] private List<Rect> tileRects;
    private List<Sprite> slicedSprites = new();

    [Header("Tilemap Manipulation")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<TileBase> targetTiles;

    private void Start()
    {
        SliceTexture();
    }

    public void SliceTexture()
    {
        if (targetTiles.Count != tileRects.Count)
        {
            Debug.LogError("Insufficient Tiles To Set!");
            return;
        }

        Texture2D texture = imageSaver.GetTextureFromLocalDisk(fileName);

        foreach (Rect rect in tileRects)
        {
            Sprite newSprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 302);
            slicedSprites.Add(newSprite);
        }

        AssignTileSprites();
    }

    public void AssignTileSprites()
    {
        for (int i = 0; i < slicedSprites.Count; i++)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = slicedSprites[i];
            tilemap.SwapTile(targetTiles[i], tile);
        }
    }
}