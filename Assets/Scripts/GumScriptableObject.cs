using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GumScriptableObject", menuName = "Species/Gum")]
public class GumScriptableObject : ScriptableObject
{        
    public int sizeClasses = 5;
    public int maxSprouts = 5;
    public int maturity = 800;
    public float[] stemRadii = new float[]{0.4f, 0.6f, 1.2f, 2.8f, 3f};
    public float[] growthRate = new float[] {1/365f, 1/490f, 1/1100f, 1/2500f, 1/7000f};
    public float spaceFilling = 5f;
    public float canopyGeometry = 0.05f;
    public float[] stemDistance;
    public float[] canopyDistance;

    void Awake() {
        stemDistance = new float[sizeClasses];
        canopyDistance = new float[sizeClasses];
        for (int i = 0; i < sizeClasses; i++) {
            stemDistance[i] = spaceFilling * stemRadii[i];
            canopyDistance[i] = GetCanopyDistance(i); 
        }
    }

    public float GetCanopyDistance(int size) {
        return stemDistance[size] * (1 - 2 * (canopyGeometry / (spaceFilling * Mathf.Pow(stemRadii[size], (1/3f)))));
    }
}
