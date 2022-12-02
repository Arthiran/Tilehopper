using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public Transform FollowObject;
    public Vector3 Offset;
    public Camera thisCamera;

    float speed = 0.04f;

    private void LateUpdate()
    {
        if (FollowObject != null)
        {
            //transform.position = FollowObject.position + Offset;
            transform.position = Vector3.Lerp(transform.position, FollowObject.position + Offset, Time.deltaTime * 3f);
        }
        float t = (Mathf.Sin(Time.time * speed) + 1) / 2f;
        thisCamera.backgroundColor = Color.HSVToRGB(Mathf.Lerp(0f, 1f, t), 1f, 0.1f);
    }
}
