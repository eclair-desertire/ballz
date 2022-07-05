using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    public void onValueChanged(Vector2 pos)
    {
        float xRot = pos.x;
        float pRot = pos.y;
        camera.transform.Rotate(-pRot, xRot, 0.0f);
    }
}
