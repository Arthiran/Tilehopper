using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlashingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    float speed = 2.5f;

    private void LateUpdate()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1) / 2f;
        Text.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, t));
    }
}
