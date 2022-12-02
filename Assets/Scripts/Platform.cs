using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private Renderer material;

    private void Start()
    {
        material.material.color = Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        material.material.color = Color.Lerp(material.material.color, new Color(0, 0, 0, 0), 0.03f);
    }
}
