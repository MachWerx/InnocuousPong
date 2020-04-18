using UnityEngine;

public class Paddle : MonoBehaviour {
  private float kMouseSpeed = 0.02f;
  private float kBorder = 4.75f;
  private Vector2 m_MousePositionPrev;
  private float paddleSize;

  // Start is called before the first frame update
  void Start() {
    m_MousePositionPrev = Input.mousePosition;
    paddleSize = transform.localScale.y;
  }

  // Update is called once per frame
  void Update() {
    Vector2 mousePosition = Input.mousePosition;
    Vector3 pos = transform.localPosition;
    Debug.Log($"mouse y: {pos.y}");
    pos.y += kMouseSpeed * (mousePosition.y - m_MousePositionPrev.y);

    float boundary = kBorder - paddleSize / 2;

    if (pos.y < -boundary) {
      pos.y = -boundary;
    } else if (pos.y > boundary) {
      pos.y = boundary;
    }

    transform.localPosition = pos;
    m_MousePositionPrev = mousePosition;

  }

  public Vector3 CheckIntersection(Vector3 puckPos, float puckSize) {
    Vector3 pos = transform.localPosition;
    Vector3 size = transform.localScale;
    if (puckPos.x - puckSize < pos.x + size.x &&
        puckPos.x + puckSize > pos.x - size.x &&
        puckPos.y - puckSize < pos.y + size.y &&
        puckPos.y + puckSize > pos.y - size.y &&
        puckPos.z - puckSize < pos.z + size.z &&
        puckPos.z + puckSize > pos.z - size.z) {
      return Vector3.one;
    }

    return Vector3.zero;
  }
}
