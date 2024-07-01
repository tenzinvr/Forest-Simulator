using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using static UnityEngine.Mathf;

[BurstCompile]

public struct BloomJob : IJob
{
    private int sprouts;
    private uint randomSeed;
    private float canopyDistance;
    private Vector3 newPosition;
    private Vector3 position;
    
    public NativeArray<Vector3> unusedPositions;
    public NativeArray<Vector3> newSproutPositions;

    public BloomJob(int _sprouts, uint _randomSeed, float _canopyDistance, 
            NativeArray<Vector3> _unusedPositions, Vector3 _position,
            NativeArray<Vector3> _newSproutPositions) 
    {
        sprouts = _sprouts;
        randomSeed = _randomSeed;
        canopyDistance = _canopyDistance;
        unusedPositions = _unusedPositions;
        position = _position;
        newSproutPositions = _newSproutPositions;
        newPosition = Vector3.zero;
    }

    public void Execute()  
    {
        var random = new Unity.Mathematics.Random(randomSeed);
        for (int j = 0; j < sprouts; j++) {
            for (int i = 0; i < 200; i++) {
                if (unusedPositions.Length > i) {
                    newPosition = unusedPositions[random.NextInt(0, unusedPositions.Length)];
                    float distance = Mathf.Sqrt(
                        Mathf.Pow(newPosition.x - position.x, 2f)
                        + Mathf.Pow(newPosition.y - position.y, 2f));
                if (distance < canopyDistance) { break; } 
                } 
            }
            newSproutPositions[j] = newPosition;
        }
        
    }
}
