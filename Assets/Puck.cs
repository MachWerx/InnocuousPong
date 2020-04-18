﻿using UnityEngine;

public class Puck : MonoBehaviour {
  [SerializeField] GameState m_GameState = null;

  public bool OutOfBounds {
    get {
      const float bounds = 5.5f;
      Vector3 pos = transform.localPosition;
      if (Mathf.Abs(pos.x) > bounds ||
          Mathf.Abs(pos.y) > bounds ||
          Mathf.Abs(pos.z) > bounds) {
        return true;
      }
      return false;
    }
  }

  private Vector3 m_Velocity;

  // Do physics in late update so that the puck comes after the paddle.
  void LateUpdate() {
    Vector3 pos = transform.localPosition;
    float speed = m_Velocity.magnitude;
    speed = 0.1f * (m_GameState.GetPuckTargetSpeed() - speed) + speed;
    m_Velocity = speed * m_Velocity.normalized;
    pos += m_Velocity * Time.deltaTime;

    foreach (var paddle in m_GameState.GetPaddles()) {
      paddle.DoIntersection(ref pos, transform.localScale, ref m_Velocity);
    }

    transform.localPosition = pos;
  }

  public void Reset() {
    transform.localPosition = Vector3.zero;
    float angle = (Random.value - 0.5f) * 0.5f * Mathf.PI + Mathf.PI;
    m_Velocity = 3f * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
  }
}
