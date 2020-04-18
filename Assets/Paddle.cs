using UnityEngine;

public class Paddle : MonoBehaviour {
  private float kMouseSpeed = 0.02f;
  private float kBorder = 4.75f;
  private Vector2 m_MousePositionPrev;
  private Vector3 m_Velocity;
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
    pos.y += kMouseSpeed * (mousePosition.y - m_MousePositionPrev.y);

    float boundary = kBorder - paddleSize / 2;

    if (pos.y < -boundary) {
      pos.y = -boundary;
    } else if (pos.y > boundary) {
      pos.y = boundary;
    }

    m_Velocity = (pos - transform.localPosition) / Time.deltaTime;
    transform.localPosition = pos;
    m_MousePositionPrev = mousePosition;

  }

  public Vector3 CheckIntersection(Vector3 puckPos, Vector3 puckSize, Vector3 puckVel) {
    Bounds puckBounds = new Bounds(puckPos, puckSize);
    Vector3 pos = transform.localPosition;
    Vector3 size = transform.localScale;
    Bounds bounds = new Bounds(pos, size);

    if (bounds.Intersects(puckBounds)) {
      return (puckPos - pos).normalized;
    }

    return Vector3.zero;
  }
}
