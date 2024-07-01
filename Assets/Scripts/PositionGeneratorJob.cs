using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using static UnityEngine.Mathf;

[BurstCompile]

public struct PositionGeneratorJob : IJob { 

    private int sideLength;
    private float canopyDistance;
    private uint seed;

    public NativeArray<Vector3> treePositionResults;

    public PositionGeneratorJob(int _sideLength, 
                float _canopyDistance, 
                uint _seed, 
                NativeArray<Vector3> _treePositionResults) {
        sideLength = _sideLength;
        canopyDistance = _canopyDistance;
        seed = _seed;
        treePositionResults = _treePositionResults;
    }

    public void Execute() {
        Vector3 position = Vector3.zero;
        float x = 0;
        float z = 0;
        int index = 0; 
        var random = new Unity.Mathematics.Random(seed);
        while (x < sideLength * 2) { 
            x += canopyDistance;
            while (z < sideLength) {
                z += canopyDistance;
                position = new Vector3(x, 0, z);
                if (index < treePositionResults.Length) {
                    treePositionResults[index++] = new Vector3(
                        position.x + random.NextFloat(-canopyDistance, canopyDistance),
                        100, 
                        position.z + random.NextFloat(-canopyDistance, canopyDistance));
                } 
            }
            z = 0; 
        }
    }
} 
