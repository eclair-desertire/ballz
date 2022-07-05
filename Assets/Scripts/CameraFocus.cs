using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
  [SerializeField] private Transform m_Target;
  [SerializeField] private Vector3 m_Distance = Vector3.back;

  void LateUpdate()
  {
      transform.position = m_Target.position + m_Distance;
  }
}
