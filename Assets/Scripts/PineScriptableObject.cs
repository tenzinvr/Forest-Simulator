using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PineScriptableObject", menuName = "Species/Pine")]
public class PineScriptableObject : ScriptableObject
{
    public int sizeClasses = 7;
    public int maxSprouts = 10;
    public int maturity = 600;
    public float[] stemRadii = new float[]{0.3f, 0.4f, 0.6f, 1f, 1.5f, 1.8f, 2.8f};
    public float[] growthRate = new float[] {1/150f, 1/250f, 1/400f, 1/1000f, 1/2500f, 1/4000, 1/8000};
    public float spaceFilling = 3f;
    public float canopyGeometry = 0.1f;
    public float[] stemDistance;
    public float[] canopyDistance;

    void Awake() {
        stemDistance = new float[sizeClasses];
        canopyDistance = new float[sizeClasses];
        for (int i = 0; i < sizeClasses; i++) {
            stemDistance[i] = spaceFilling * stemRadii[i];
            canopyDistance[i] = GetCanopyDistance(i); }
    }

    public float GetCanopyDistance(int size) {
        return stemDistance[size] * (1 - 2 * (canopyGeometry / (spaceFilling * Mathf.Pow(stemRadii[size], (1/3f)))));
    }
}
