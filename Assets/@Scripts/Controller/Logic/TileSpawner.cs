using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TileSpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject GameBoardPrefab;

    [SerializeField]
    private GameObject TilemapPrefab;

    [SerializeField]
    private GameObject TilePrefab;

    public void Start()
    {
        GameObject tilemap = Instantiate(TilemapPrefab,Vector3.zero, Quaternion.Euler(90, 0, 0), GameBoardPrefab.transform);

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                GameObject spawnedTilePrefab = Instantiate(TilePrefab, Vector3.zero, Quaternion.identity, tilemap.transform);
                spawnedTilePrefab.transform.localEulerAngles = new Vector3(90, 90, -90);

                spawnedTilePrefab.name = $"t({i},{j})";
            }
        }
    }
}
