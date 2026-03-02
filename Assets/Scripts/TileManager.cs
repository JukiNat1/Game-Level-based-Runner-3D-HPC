using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private List<GameObject> activeTiles = new List<GameObject>();
    
    public Transform playerTransform;
    public GameObject[] tilePrefabs;
    public GameObject[] roadsideTilePrefabs;

    public float zSpawn = 0;
    public float tileLength = 30;
    
    public int numberOfTiles;
    public int numberOfRoadsideTiles;
    int roadsideTilesCreated = 0;

    [Header("Finish Line")]
    [Tooltip("Số tile tới đích (mỗi tile = tileLength units). Ví dụ: 20 tile x 30 = 600 units")]
    public int finishAfterTiles = 20;
    public GameObject finishGatePrefab;

    private bool finishSpawned = false;
    private int tilesSpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < numberOfTiles; i++)
        {
            if (i == 0)
            {
                SpawnTile(0);
                SpawnRoadsideTiles(1, 1);
            }
            else
            {
                SpawnTile(UnityEngine.Random.Range(0, tilePrefabs.Length));
                SpawnRoadsideTiles(UnityEngine.Random.Range(0, roadsideTilePrefabs.Length),
                    UnityEngine.Random.Range(0, roadsideTilePrefabs.Length));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Nếu đã spawn cổng đích rồi, không sinh thêm tile nữa
        if (finishSpawned) return;

        if (playerTransform.position.z - 35 > zSpawn - (numberOfTiles * tileLength))
        {
            // Kiểm tra đã đủ số tile chưa
            if (tilesSpawned >= finishAfterTiles)
            {
                SpawnFinishGate();
            }
            else
            {
                SpawnTile(UnityEngine.Random.Range(0, tilePrefabs.Length));
                SpawnRoadsideTiles(UnityEngine.Random.Range(0, roadsideTilePrefabs.Length),
                    UnityEngine.Random.Range(0, roadsideTilePrefabs.Length));
                DeleteTiles();
            }
        }
    }

    public void SpawnTile(int tileIndex)
    {
        GameObject go = Instantiate(tilePrefabs[tileIndex], transform.forward * zSpawn, transform.rotation);
        activeTiles.Add(go);
        zSpawn += tileLength;
        tilesSpawned++;
    }

    private void SpawnFinishGate()
    {
        finishSpawned = true;

        if (finishGatePrefab != null)
        {
            // Đặt cổng ở cuối đường, cao hơn mặt đường một chút
            Vector3 gatePos = transform.forward * zSpawn;
            gatePos.y = 0;
            Instantiate(finishGatePrefab, gatePos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("TileManager: finishGatePrefab chưa được gán! " +
                             "Hãy gán Prefab FinishGate trong Inspector.");
        }
    }

    public void SpawnRoadsideTiles(int leftTileIndex, int rightTileIndex)
    {
        GameObject left = Instantiate(roadsideTilePrefabs[leftTileIndex], new Vector3(0.5F + (tileLength / 4), 0, tileLength * roadsideTilesCreated), transform.rotation);
        GameObject right = Instantiate(roadsideTilePrefabs[rightTileIndex], new Vector3(-(0.5F + (tileLength / 4)), 0, tileLength * roadsideTilesCreated), transform.rotation);
        
        activeTiles.Add(left);
        activeTiles.Add(right);

        roadsideTilesCreated++;
    }

    //removing three tiles at once (main and two from roadsides)
    private void DeleteTiles()
    {
        for (int i = 0; i < 2; i++)
        {
            Destroy(activeTiles[i]);
            activeTiles.RemoveAt(i);
        }
    }
}
