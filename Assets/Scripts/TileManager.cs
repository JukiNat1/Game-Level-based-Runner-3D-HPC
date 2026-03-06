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
        // Read tile count from ProgressManager if available (level-based mode)
        // Falls back to Inspector value when ProgressManager is absent (backward compatible)
        if (ProgressManager.Instance != null)
        {
            int idx = ProgressManager.Instance.currentLevelIndex;
            if (idx >= 0 && idx < LevelData.TileCounts.Length)
                finishAfterTiles = LevelData.TileCounts[idx];
        }

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
        // Stop spawning new tiles once the finish gate has been placed
        if (finishSpawned) return;

        if (playerTransform.position.z - 35 > zSpawn - (numberOfTiles * tileLength))
        {
            // Check if enough tiles have been spawned to place the finish gate
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

        Vector3 gatePos = transform.forward * zSpawn;
        gatePos.y = 0;

        // Spawn 5 straight tiles AFTER the gate so the player doesn't fall off
        for (int i = 0; i < 5; i++)
        {
            SpawnTile(0);
        }

        if (finishGatePrefab != null)
        {
            GameObject gate = Instantiate(finishGatePrefab, gatePos, Quaternion.identity);
            EnsureFinishLineTrigger(gate);
        }
        else
        {
            CreateProceduralHPCGate(gatePos);
        }
    }

    /// <summary>Creates an HPC school gate using Unity primitives (no prefab needed)</summary>
    private void CreateProceduralHPCGate(Vector3 position)
    {
        GameObject gate = new GameObject("FinishGate_HPC");
        gate.transform.position = position;

        // --- Colors ---
        Material matRed    = CreateMat(new Color(0.85f, 0.08f, 0.08f)); // Red pillar
        Material matYellow = CreateMat(new Color(1f,    0.82f, 0f));     // Yellow crossbar
        Material matWhite  = CreateMat(Color.white);                     // Sign background
        Material matBlue   = CreateMat(new Color(0.05f, 0.2f, 0.6f));   // HPC text color

        float halfWidth = 4.5f; // khoảng cách từ giữa đến cột
        float pillarH   = 7f;
        float pillarW   = 0.8f;

        // --- Left pillar ---
        MakeCube(gate, "PillarLeft",  new Vector3(-halfWidth, pillarH / 2f, 0),
                 new Vector3(pillarW, pillarH, pillarW), matRed);

        // --- Right pillar ---
        MakeCube(gate, "PillarRight", new Vector3(halfWidth, pillarH / 2f, 0),
                 new Vector3(pillarW, pillarH, pillarW), matRed);

        // --- Top crossbar ---
        float barY = pillarH + 0.5f;
        MakeCube(gate, "TopBar", new Vector3(0, barY, 0),
                 new Vector3(halfWidth * 2 + pillarW, 1f, pillarW), matYellow);

        // --- Sign board (wider to fit the text) ---
        float signW = halfWidth * 2f - pillarW; // chiều rộng giữa 2 cột = 8.2f
        float signH = 3.0f;
        float signY = pillarH - 1.5f;           // thấp hơn một chút để cân đối
        MakeCube(gate, "SignBoard", new Vector3(0, signY, -0.05f),
                 new Vector3(signW, signH, 0.15f), matWhite);

        // --- 3D text on sign ---
        GameObject textObj = new GameObject("SignText");
        textObj.transform.SetParent(gate.transform);
        textObj.transform.localPosition = new Vector3(0, signY, -0.22f);
        textObj.transform.localRotation = Quaternion.identity;
        // Scale down so text fits the sign board
        textObj.transform.localScale    = new Vector3(0.32f, 0.32f, 0.32f);

        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.text          = "TRƯỜNG CAO ĐẰNG\nCÔNG NGHỆ\nBÁCH KHOA HÀ NỘI";
        tm.fontSize      = 11;
        tm.characterSize = 1f;
        tm.color         = new Color(0.05f, 0.15f, 0.55f);
        tm.alignment     = TextAlignment.Center;
        tm.anchor        = TextAnchor.MiddleCenter;
        tm.fontStyle     = FontStyle.Bold;

        // --- 2 decorative yellow vertical bars ---
        MakeCube(gate, "DecoLeft",  new Vector3(-halfWidth + 1.5f, pillarH * 0.55f, 0),
                 new Vector3(0.35f, pillarH * 0.5f, 0.35f), matYellow);
        MakeCube(gate, "DecoRight", new Vector3( halfWidth - 1.5f, pillarH * 0.55f, 0),
                 new Vector3(0.35f, pillarH * 0.5f, 0.35f), matYellow);

        // --- Trigger BoxCollider (detects Player) ---
        BoxCollider bc = gate.AddComponent<BoxCollider>();
        bc.center  = new Vector3(0, pillarH / 2f, 0);
        bc.size    = new Vector3(halfWidth * 2 + 2f, pillarH + 1f, 2f);
        bc.isTrigger = true;

        // --- Attach FinishLine script ---
        gate.AddComponent<FinishLine>();

        Debug.Log("FinishGate_HPC đã được tạo tại " + position);
    }

    /// <summary>Ensures the gate prefab has a FinishLine component and a trigger collider.</summary>
    private void EnsureFinishLineTrigger(GameObject gate)
    {
        if (gate.GetComponent<FinishLine>() == null)
            gate.AddComponent<FinishLine>();
        if (gate.GetComponent<Collider>() == null)
        {
            BoxCollider bc = gate.AddComponent<BoxCollider>();
            bc.size = new Vector3(10f, 7f, 2f);
            bc.isTrigger = true;
        }
    }

    private GameObject MakeCube(GameObject parent, string name, Vector3 localPos, Vector3 scale, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent.transform);
        obj.transform.localPosition = localPos;
        obj.transform.localScale    = scale;
        obj.GetComponent<Renderer>().material = mat;
        // Remove child colliders (keep only the root trigger collider)
        Destroy(obj.GetComponent<BoxCollider>());
        return obj;
    }

    private Material CreateMat(Color color)
    {
        Material m = new Material(Shader.Find("Standard"));
        m.color = color;
        return m;
    }

    public void SpawnRoadsideTiles(int leftTileIndex, int rightTileIndex)
    {
        GameObject left = Instantiate(roadsideTilePrefabs[leftTileIndex], new Vector3(0.5F + (tileLength / 4), 0, tileLength * roadsideTilesCreated), transform.rotation);
        GameObject right = Instantiate(roadsideTilePrefabs[rightTileIndex], new Vector3(-(0.5F + (tileLength / 4)), 0, tileLength * roadsideTilesCreated), transform.rotation);
        
        activeTiles.Add(left);
        activeTiles.Add(right);

        roadsideTilesCreated++;
    }

    // Removes the 3 oldest tiles (1 main + 2 roadside) to prevent memory overflow.
    // Fix: previously deleted index 0 then index 1, but after RemoveAt(0) indices shift.
    // Correct approach: delete index 0 three times in a row.
    private void DeleteTiles()
    {
        for (int i = 0; i < 3; i++)
        {
            if (activeTiles.Count == 0) break;
            Destroy(activeTiles[0]);
            activeTiles.RemoveAt(0);
        }
    }
}
