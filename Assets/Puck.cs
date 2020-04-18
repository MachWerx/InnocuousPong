using UnityEngine;

public class Puck : MonoBehaviour {
  private Vector3 m_Velocity;
  private float kBorder = 4.5f;

  // Start is called before the first frame update
  void Start() {

    float angle = (Random.value - 0.5f) * 0.5f * Mathf.PI + Mathf.PI;
    m_Velocity = 2 * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
  }

  // Update is called once per frame
  void Update() {
    Vector3 pos = transform.localPosition;
    pos += m_Velocity * Time.deltaTime;

    if (pos.x < -kBorder) {
      pos.x = -2 * kBorder - pos.x;
      m_Velocity.x *= -1;
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
