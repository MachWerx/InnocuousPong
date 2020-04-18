using UnityEngine;

public class Puck : MonoBehaviour {
  [SerializeField] GameState m_GameState = null;

  private Vector3 m_Velocity;

  void Start() {
    float angle = (Random.value - 0.5f) * 0.5f * Mathf.PI + Mathf.PI;
    m_Velocity = 3f * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
  }

  // Do physics in late update so that the puck comes after the paddle.
  void LateUpdate() {
    Vector3 pos = transform.localPosition;
    pos += m_Velocity * Time.deltaTime;

    foreach (var paddle in m_GameState.GetPaddles()) {
      paddle.DoIntersection(ref pos, transform.localScale, ref m_Velocity);
    }

    transform.localPosition = pos;
  }
}
