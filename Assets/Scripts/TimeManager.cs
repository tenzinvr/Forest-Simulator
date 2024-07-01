using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    public float time;
    public int tick;
    public GameObject[] allTrees;

    void Start() {
        time = 0f;
    }

/// <summary>
/// Updates tree time properties and the list of all trees
/// </summary>
    public void Tick() {
        allTrees = GameObject.FindGameObjectsWithTag("Tree");
        time += tick;
        foreach (GameObject tree in allTrees) {
            tree.GetComponent<TreeProperties>().UpdateTime(tick); }
        GetComponent<ForestManager>().allTrees = allTrees;
    }
}
 