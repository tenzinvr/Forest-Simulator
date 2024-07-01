using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateSpecies : MonoBehaviour
{
    SpeciesScriptableObject species;

    // Start is called before the first frame update
    void Start()
    {
        species = (SpeciesScriptableObject)ScriptableObject.
                CreateInstance(typeof(SpeciesScriptableObject));
        
    }
}
