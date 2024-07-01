using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;
using Unity.Burst;

[BurstCompile]

public class ForestManager : MonoBehaviour {
    public int newTreeID = 0;
    public int sizeClasses;
    private int newSizeClass;
    private int numberOfTrees;
    public int maxTrees;
    public int sideLength;
    public string speciesName;
    private RaycastHit terrainHit;
    public GameObject[] treeModels;
    public List<Vector3>[] gridPositions;
    public List<Vector3>[] unusedPositions;
    public List<GameObject>[] atRiskTrees;
    public GameObject[] allTrees;
    public AudioSource forestAudio;
    public PineScriptableObject species;
    public TimeManager timeManager;
    private GameObject treeObject;
    private TreeProperties treeProperties;
    private TreeProperties oldTreeProperties;
    public InGameUI UI;

    public NativeArray<Vector3> treePositionResults;
    private JobHandle jobHandle;

/// <summary>
/// Get species values and instantiate lists
/// </summary>
    void Start() {
        species = (PineScriptableObject)ScriptableObject.CreateInstance(typeof(PineScriptableObject));
        forestAudio.Play(); 
        sizeClasses = species.sizeClasses;
        gridPositions = new List<Vector3>[sizeClasses];
        unusedPositions = new List<Vector3>[sizeClasses];
        atRiskTrees = new List<GameObject>[sizeClasses];
        Physics.queriesHitTriggers = true;
        for (int i = 0; i < sizeClasses; i++) {
            gridPositions[i] = new List<Vector3>();
            InitialTrees(i);  }
    }

    private void InitialTrees(int index) {
        int maxPositions = (int)(sideLength/species.canopyDistance[index]) * (int)(sideLength/species.canopyDistance[index]);
        treePositionResults = new NativeArray<Vector3>(maxPositions, Allocator.Persistent);
        PositionGeneratorJob positionGeneratorJob = new PositionGeneratorJob(
                sideLength, species.canopyDistance[index], (uint)Random.Range(1, 1000), treePositionResults);
        jobHandle = positionGeneratorJob.Schedule();
        atRiskTrees[index] = new List<GameObject>();
        StartCoroutine(JobComplete());
        jobHandle.Complete();
        gridPositions[index].AddRange(treePositionResults);
        unusedPositions[index] = gridPositions[index];
        maxTrees += unusedPositions[index].Count;
        numberOfTrees = (sizeClasses - index) * (sizeClasses - index);
        if (unusedPositions[index] != null && unusedPositions[index].Count > 0) {
            for (int j = 0; j < numberOfTrees; j++) {
                int rand = Random.Range(0, unusedPositions[index].Count);
                Vector3 position = unusedPositions[index][rand];
                CreateTree(position, index); } }
    }

    private IEnumerator JobComplete() {
        yield return new WaitForSeconds(0.1f);
    }

/// <summary>
/// Create new tree and assign values
/// </summary>
/// <param name="position"></param>
/// <param name="size"></param>
    public void CreateTree(Vector3 position, int size) {
        if (allTrees.Length >= maxTrees) {
            UI.Saturated(); }
        Quaternion rotation = new Quaternion(0, Random.Range(-180f, 180f), 0, 0);
        treeObject = GameObject.Instantiate(treeModels[size],
            position,
            rotation);
        unusedPositions[size].Remove(position);
        treeObject.transform.position = position;
        Physics.Raycast(position, Vector3.down, out terrainHit, 100);
        treeObject.transform.position = terrainHit.point;
        treeObject.GetComponent<TreeProperties>().treeID = newTreeID++;
        treeObject.GetComponent<TreeProperties>().sizeClass = size;
        treeObject.SetActive(true);
/*         if (allTrees != null && allTrees.Length > 100) {
            DestroyTree(newSizeClass); } */
    }

/// <summary>
/// Update the model and properties of a tree as it grows
/// </summary>
/// <param name="tree"></param>
    public void UpdateTree(GameObject tree) {
        tree.SetActive(false);
        oldTreeProperties = tree.GetComponent<TreeProperties>();
        newSizeClass = oldTreeProperties.sizeClass + 1;
        if (allTrees.Length >= maxTrees) {
            UI.Saturated(); }
        treeObject = GameObject.Instantiate(treeModels[newSizeClass],
		    tree.transform.position,
		    Quaternion.identity);
        treeObject.SetActive(true);
        treeProperties = treeObject.GetComponent<TreeProperties>();
        treeProperties.treeID = oldTreeProperties.treeID;
        treeProperties.position = tree.transform.position;
        treeProperties.sizeClass = newSizeClass;
        treeProperties.growth = 0;
        treeProperties.growthRate = species.growthRate[newSizeClass];
        treeProperties.atRisk = false;
        atRiskTrees[newSizeClass - 1].Remove(tree);
        Destroy(tree);
        // For each tree that increases in sizeclass, a tree of the next sizeclass must die
/*         if (allTrees != null && allTrees.Length > 100) {
            DestroyTree(newSizeClass);
        } */
    }

/// <summary>
/// Manage a list of trees that are at risk of dying
/// </summary>
/// <param name="tree"></param>
    public void AddToAtRisk(GameObject tree) {
        int size = tree.GetComponent<TreeProperties>().sizeClass;
        if (!atRiskTrees[size].Contains(tree)) {
            tree.GetComponent<TreeProperties>().atRisk = true;
            atRiskTrees[size].Add(tree); }
    }

/// <summary>
/// Destroy at risk tree and re-add its position to the grid
/// </summary>
/// <param name="size"></param>
    public void DestroyTree(int size) {
        List<GameObject> atRiskTreesInSize = atRiskTrees[size];
        for (int i = 0; i < atRiskTreesInSize.Count; i++) {
            treeObject = atRiskTreesInSize[i];
            if (treeObject != null) { break;} }
        if (treeObject != null) { 
            treeProperties = treeObject.GetComponent<TreeProperties>();
            unusedPositions[size].Add(treeObject.transform.position);
            atRiskTrees[size].Remove(treeObject);
            Destroy(treeObject); }
    }
}