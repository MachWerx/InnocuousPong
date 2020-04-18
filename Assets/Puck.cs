using UnityEngine;

public class Puck : MonoBehaviour {
  [SerializeField] Paddle m_PaddleLeft = null;

  private float kBorder = 4.5f;
  private Vector3 m_Velocity;

  void Start() {
    float angle = (Random.value - 0.5f) * 0.5f * Mathf.PI + Mathf.PI;
    m_Velocity = 3f * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
  }

  // Do physics in late update so that the puck comes after the paddle.
  void LateUpdate() {
    Vector3 pos = transform.localPosition;
    pos += m_Velocity * Time.deltaTime;

    if (pos.x < -kBorder) {
      m_PaddleLeft.DoIntersection(ref pos, transform.localScale, ref m_Velocity);
    } else if (pos.x > kBorder) {
      pos.x = 2 * kBorder - pos.x;
      m_Velocity.x *= -1;
    }

    if (pos.y < -kBorder) {
      pos.y = -2 * kBorder - pos.y;
      m_Velocity.y *= -1;
    } else if (pos.y > kBorder) {
      pos.y = 2 * kBorder - pos.y;
      m_Velocity.y *= -1;
    }

    if (pos.z < -kBorder) {
      pos.z = -2 * kBorder - pos.z;
      m_Velocity.z *= -1;
    } else if (pos.z > kBorder) {
      pos.z = 2 * kBorder - pos.z;
      m_Velocity.z *= -1;
    }

    transform.localPosition = pos;
  }
}
