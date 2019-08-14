using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using EZ_Pooling;

public class LevelSpawner : MonoBehaviour
{
    List<Transform> currBlocks = new List<Transform>();
    public int maxBlockCount;

    [Space]

    [Slider(0, 100)]
    public float knobSpawnChancePerRoad;

    public float roadSpawnOffset;
    public int minRoadsPerKnob;
    public int maxRoadsPerKnob;

    int spawnedRoadsInARow;

    [Space]
    public Transform knobPrefab;
    public Transform roadPrefab;

    public Transform spawnerTrans;

    bool facedWithAKnob;

    public static LevelSpawner main;

    bool firstTimeKnobFinishes = true;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        SpawnUntilAKnob();
        SpawnUntilAKnob();
        SpawnUntilAKnob();
    }

    void Update()
    {
        if (false && currBlocks.Count > maxBlockCount)
        {
            EZ_PoolManager.Despawn(currBlocks[currBlocks.Count - 1]);
            currBlocks.RemoveAt(currBlocks.Count - 1);
        }
    }

    public void OnKnobFinished()//When player passes a knob
    {
        SpawnUntilAKnob();

        if (!firstTimeKnobFinishes)
        {
            DespawnUntilAKnob();
        }

        firstTimeKnobFinishes = false;
    }

    void DespawnUntilAKnob()//Despwabs from the end of the list until there is a knob
    {
        for (int i = currBlocks.Count - 1; i >= 0; i--)
        {
            string blockTag = currBlocks[i].tag;

            EZ_PoolManager.Despawn(currBlocks[i]);
            currBlocks.RemoveAt(i);

            if (blockTag == "Knob")
                break;

        }
    }

    void SpawnUntilAKnob()
    {
        facedWithAKnob = false;

        while (!facedWithAKnob)
        {
            SpawnBlock();
        }

        facedWithAKnob = false;
    }

    [Button]
    void SpawnBlock()
    {
        bool isKnob = M.Change(knobSpawnChancePerRoad);

        if (spawnedRoadsInARow <= minRoadsPerKnob)
        {
            isKnob = false;
        }
        else if (spawnedRoadsInARow > maxRoadsPerKnob)//If its more than the max amount of roads, just spawn a knob
        {
            isKnob = true;
        }

        if (!isKnob)
            spawnedRoadsInARow++;
        else
            spawnedRoadsInARow = 0;

        SpawnBlock(spawnerTrans.position, spawnerTrans.eulerAngles.y, isKnob);
    }

    void SpawnBlock(Vector3 pos, float angle, bool isKnob)
    {
        Transform prefab = isKnob ? knobPrefab : roadPrefab;

        Transform blockTrans = EZ_PoolManager.Spawn(prefab, pos, Quaternion.Euler(0, angle, 0));

        currBlocks.Insert(0, blockTrans);

        if (isKnob)
        {
            Knob knob = blockTrans.GetComponent<Knob>();

            knob.isLeft = M.Flip();//It's like flipping a coin :P

            knob.UpdateDirection();

            spawnerTrans.position = knob.spawnerEndTrans.position;
            spawnerTrans.eulerAngles = knob.spawnerEndTrans.eulerAngles;

            facedWithAKnob = true;
        }
        else
        {
            //So we move currPos

            spawnerTrans.Translate(0, 0, roadSpawnOffset, Space.Self);

        }
    }
}
