using UnityEngine;

public class Paddle : MonoBehaviour {
  private float kMouseSpeed = 0.02f;
  private float kBorder = 4.75f;
  private Vector2 m_MousePositionPrev;
  private Vector3 m_Velocity;
  private float paddleSize;

  void Start() {
    m_MousePositionPrev = Input.mousePosition;
    paddleSize = transform.localScale.y;
  }

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

  public void DoIntersection(ref Vector3 puckPos, Vector3 puckSize, ref Vector3 puckVel) {
    Bounds puckBounds = new Bounds(puckPos, puckSize);
    Vector3 pos = transform.localPosition;
    Vector3 size = transform.localScale;
    Bounds bounds = new Bounds(pos, size);

    if (bounds.Intersects(puckBounds)) {
      // find intersection time
      float minT = -Time.deltaTime;
      float maxT = 0;
      float t = 0.5f * (minT + maxT);
      for (int i = 0; i < 10; i++) {
        puckBounds = new Bounds(puckPos + t * puckVel, puckSize);
        bounds = new Bounds(pos + t * m_Velocity, size);
        if (bounds.Intersects(puckBounds)) {
          maxT = t;
        } else {
          minT = t;
        }
        t = 0.5f * (minT + maxT);
      }

      // rewind to intersection time
      pos += t * m_Velocity;
      bounds = new Bounds(pos, size);
      puckPos += t * puckVel;

      // figure out normal at intersection point
      Vector3 normal = Vector3.zero;
      if (puckPos.x < bounds.min.x) {
        normal += Vector3.left;
      } else if (puckPos.x > bounds.max.x) {
        normal += Vector3.right;
      }
      if (puckPos.y < bounds.min.y) {
        normal += Vector3.down;
      } else if (puckPos.y > bounds.max.y) {
        normal += Vector3.up;
      }
      if (puckPos.z < bounds.min.z) {
        normal += Vector3.back;
      } else if (puckPos.z > bounds.max.z) {
        normal += Vector3.forward;
      }
      normal.Normalize();

      // calculate new puck velocity
      puckVel -= m_Velocity;
      puckVel -= 2 * Vector3.Dot(puckVel, normal) * normal;
      puckVel += m_Velocity;

      // fast foward to current time
      pos -= t * m_Velocity;
      puckPos -= t * puckVel;
    }
  }
}
