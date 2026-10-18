using UnityEngine;
using UnityEngine.Tilemaps;

public class GeradorAgua : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tileAgua;

    [ContextMenu("Gerar Agua")]
    public void GerarAgua()
    {
        for (int x = 0; x < 125; x++)
        {
            for (int y = 0; y < 125; y++)
            {
                tilemap.SetTile(
                    new Vector3Int(x, y, 0),
                    tileAgua
                );
            }
        }
    }
}
