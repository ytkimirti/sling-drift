﻿using System.Collections;
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
    [Space]
    public int minRoadsPerKnob;
    public int maxRoadsPerKnob;
    [Space]
    public int knobsPerBoost;

    int spawnedKnobsInARow;//Resets when a boost spawns
    int spawnedRoadsInARow;//Resets when a knob spawns

    [Space]
    public Transform knobPrefab;
    public Transform roadPrefab;
    public Transform boostPrefab;

    public Transform spawnerTrans;

    bool facedWithAKnob;

    int currSpawnScore;

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
        bool isBoost = false;

        if (spawnedKnobsInARow >= knobsPerBoost)
        {
            isBoost = true;
        }

        if (!isBoost)
        {
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
            {
                spawnedRoadsInARow = 0;
                spawnedKnobsInARow++;
            }
        }
        else
        {
            spawnedKnobsInARow = 0;
        }

        SpawnBlock(spawnerTrans.position, spawnerTrans.eulerAngles.y, isKnob, isBoost);
    }

    void SpawnBlock(Vector3 pos, float angle, bool isKnob, bool isBoost)
    {
        //Damn the spawner code is just soo messy :P
        Transform prefab = isKnob ? knobPrefab : roadPrefab;

        if (isBoost)
            prefab = boostPrefab;

        Transform blockTrans = EZ_PoolManager.Spawn(prefab, pos, Quaternion.Euler(0, angle, 0));

        Transform blockChild = blockTrans.GetChild(0);

        currBlocks.Insert(0, blockTrans);


        if (isBoost)
        {
            currSpawnScore++;
        }
        else if (isKnob)
        {
            Knob knob = blockTrans.GetComponent<Knob>();

            knob.isLeft = M.Flip();//It's like flipping a coin :P

            knob.UpdateDirection();

            currSpawnScore++;

            knob.SetScore(currSpawnScore);

            facedWithAKnob = true;
        }
        else
        {
            //So we move currPos

            //spawnerTrans.Translate(0, 0, roadSpawnOffset, Space.Self);

        }

        spawnerTrans.position = blockChild.position;
        spawnerTrans.eulerAngles = blockChild.eulerAngles;
    }
}
