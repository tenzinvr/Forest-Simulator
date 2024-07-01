using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using static UnityEngine.Mathf;
using Unity.Burst;

[BurstCompile]

public class TreeProperties : MonoBehaviour 
{
    public float age;
    public float timeOfBirth;
    public float timeSinceBloom = 0;
    public float timeAtBloom;
    public float growth = 0;
    public int treeID;
    public int maxSprouts;
    public Vector3 position;
    public Vector3 newPosition;
    private Vector3 childPosition;
    public int sizeClass;
    public int sizeClasses;
    private int maturity;
    private float maxAge;
    public Collider[] overlappingTrees;
    public bool atRisk = false;
    public bool propogated = false;
    public float growthRate;
    public float mortalityRate;
    public InGameUI UI;
    public PineScriptableObject species;
    public ForestManager forestManager;
    public TimeManager timeManager;
    public TreeProperties treeProperties;
    public GameObject treeObject;
    public NativeArray<Vector3> unusedPositions;
    public NativeArray<Vector3> newSproutPositions;
    private JobHandle bloomJobHandle;

/// <summary>
/// Instantiate relevant values for a tree
/// </summary>
    void Start() 
    {
        //species = forestManager.species;
        sizeClasses = species.sizeClasses;
        maxSprouts = species.maxSprouts;
        maturity = species.maturity;
        position = transform.position;
        growthRate = species.growthRate[0];
        timeOfBirth = timeManager.time;
        if (sizeClass == 0) { age = 0; }
        else { age = 1 / species.growthRate[sizeClass - 1] + Random.Range(0, 1/ species.growthRate[sizeClass]); }
        maxAge = 1 / species.growthRate[sizeClasses - 1] + Random.Range(6000, 12000);
        RaycastHit terrain;
        Physics.Raycast(transform.position, Vector3.down, out terrain, Mathf.Infinity);
        transform.position = new Vector3(transform.position.x, terrain.point.y, transform.position.z);
        GetOverlappingTrees();
    }

/// <summary>
/// Called on each tick, updates all time related properties
/// </summary>
/// <param name="tick"></param>
    public void UpdateTime(int tick) 
    {
        age += tick;
        timeSinceBloom += tick;
        if (sizeClass < sizeClasses - 1 && !atRisk) {
            growth = growthRate * age;
            if (growth > 1) {
                forestManager.UpdateTree(treeObject); } }
        if (timeSinceBloom > 365 && age > maturity) { 
            int sprouts = Random.Range(0, maxSprouts);
            unusedPositions = new NativeArray<Vector3>(forestManager.unusedPositions[sizeClass].ToArray(), Allocator.Persistent);
            newSproutPositions = new NativeArray<Vector3>(sprouts, Allocator.Persistent);
            BloomJob bloomJob = new BloomJob (sprouts, (uint)Random.Range(0, 1000), species.canopyDistance[sizeClass], unusedPositions, transform.position, newSproutPositions);
            bloomJobHandle = bloomJob.Schedule();
            StartCoroutine(JobComplete());
            bloomJobHandle.Complete();
            timeSinceBloom -= 365; 
            foreach (Vector3 newSprout in newSproutPositions) { forestManager.CreateTree(newSprout, 0); } }
        if (age > maxAge) { 
            forestManager.AddToAtRisk(treeObject); 
        }
        GetOverlappingTrees();
    }

    private IEnumerator JobComplete() {
        yield return new WaitForSeconds(0.1f);
    }

/// <summary>
/// Add tree to at risk if it is competing with a larger tree
/// </summary>
    public void GetOverlappingTrees() 
    {
        if (!atRisk) {
            Collider[] overlappingTrees = Physics.OverlapSphere(position, sizeClass / 2);
            foreach (Collider collider in overlappingTrees) {
                if (collider.gameObject.tag == "Tree") {
                    treeProperties = collider.gameObject.GetComponent<TreeProperties>();
                    if (!treeProperties.atRisk) {
                        if (treeProperties.sizeClass > sizeClass) {
                            forestManager.AddToAtRisk(treeObject); 
                            break; 
                        } 
                    } 
                } 
            } 
        } 
    }
}