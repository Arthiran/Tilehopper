using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreText;

    // Update is called once per frame
    void FixedUpdate()
    {
        scoreText.color = Color.Lerp(scoreText.color, new Color(0, 0, 0, 0), 0.1f);
    }
}
