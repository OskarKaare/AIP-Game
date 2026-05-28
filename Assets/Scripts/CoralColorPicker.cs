using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoralColorPicker : MonoBehaviour
{
    public Color[] coralColors;
    private new List<MeshRenderer> renderer;
    void Start()
    {
        renderer = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        var randomColor = coralColors[Random.Range(0, coralColors.Length)];
        renderer.ForEach(t => t.material.color = randomColor);
    }


}
