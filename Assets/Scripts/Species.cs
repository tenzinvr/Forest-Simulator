using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Species {
    public int sizeClasses;
    public int maxSprouts;
    public int maturity;
    public float[] stemRadii;
    public float spaceFilling;
    public float canopyGeometry;
    public float[] stemDistance;
    public float[] canopyDistance;
    public float[] growthRate;
    public float[] mortalityRate;

/// <summary>
/// Initialise all relevant properties and calculate geometry of a tree in the species
/// These properties directly correlate with the greater geometry of the forest
/// </summary>
/// <param name="speciesName"></param>
    public Species(string speciesName) {
        if (speciesName == "pine") { Pine(); }
        else if (speciesName == "gum") { Gum(); }
    }

    public void Gum() {
        sizeClasses = 5;
        maxSprouts = 5;
        maturity = 800;
        stemRadii = new float[]{0.4f, 0.6f, 1.2f, 2.8f, 3f};
        growthRate = new float[] {1/365f, 1/490f, 1/1100f, 1/2500f, 1/7000f};
        spaceFilling = 5f;
        canopyGeometry = 0.05f;
        stemDistance = new float[sizeClasses];
        canopyDistance = new float[sizeClasses];
        for (int i = 0; i < sizeClasses; i++) {
            stemDistance[i] = spaceFilling * stemRadii[i];
            canopyDistance[i] = GetCanopyDistance(i); }
    }

    public void Pine() {
        sizeClasses = 7;
        maxSprouts = 10;
        maturity = 600;
        stemRadii = new float[]{0.3f, 0.4f, 0.6f, 1f, 1.5f, 1.8f, 2.8f};
        growthRate = new float[] {1/150f, 1/250f, 1/400f, 1/1000f, 1/2500f, 1/4000, 1/8000};
        spaceFilling = 3f;
        canopyGeometry = 0.1f;
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
