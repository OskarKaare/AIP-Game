using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoralColorPicker : MonoBehaviour
{
    private GameObject[] coralObjects;
    public Color[] coralColors;
    private new List<MeshRenderer> renderer;
    void Start()
    {

        coralObjects = GameObject.FindGameObjectsWithTag("Coral");


        for (int i = 0; i < coralObjects.Length; i++)
        {
            var randomColor = coralColors[Random.Range(0, coralColors.Length)];
            renderer = new List<MeshRenderer>(coralObjects[i].GetComponentsInChildren<MeshRenderer>());
            renderer.ForEach(t => t.material.color = randomColor);
        }

    }


}
