using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigidBall;

    [SerializeField]
    private GameObject ball;

    public void onValueChanged(Vector2 pos)
    {
        rigidBall.velocity= new Vector3(-pos.y,0,pos.x)*10f;
        Debug.Log(pos.ToString());
    }
}
