using UnityEngine;

public class Paddle : MonoBehaviour {
  [SerializeField] GameState m_GameState = null;
  [SerializeField] Type m_Type = Type.Left;
  [SerializeField] bool m_IsStatic = false;

  public enum Type {
    Left,
    Right,
    Down,
    Up,
    Back,
    Fore
  }

  public float Horizontal {
    get {
      switch (m_GameState.GetHorizontalAxis(m_Type)) {
        case GameState.Direction.Left:
          return -transform.localPosition.x;
        case GameState.Direction.Right:
          return transform.localPosition.x;
        case GameState.Direction.Down:
          return -transform.localPosition.y;
        case GameState.Direction.Up:
          return transform.localPosition.y;
        case GameState.Direction.Back:
          return -transform.localPosition.z;
        case GameState.Direction.Fore:
          return transform.localPosition.z;
      }
      return 0;
    }

    set {
      Vector3 pos = transform.localPosition;
      switch (m_GameState.GetHorizontalAxis(m_Type)) {
        case GameState.Direction.Left:
          pos.x = -value;
          break;
        case GameState.Direction.Right:
          pos.x = value;
          break;
        case GameState.Direction.Down:
          pos.y = -value;
          break;
        case GameState.Direction.Up:
          pos.y = value;
          break;
        case GameState.Direction.Back:
          pos.z = -value;
          break;
        case GameState.Direction.Fore:
          pos.z = value;
          break;
      }
      transform.localPosition = pos;
    }
  }

  public float Vertical {
    get {
      switch (m_GameState.GetVerticalAxis(m_Type)) {
        case GameState.Direction.Left:
          return -transform.localPosition.x;
        case GameState.Direction.Right:
          return transform.localPosition.x;
        case GameState.Direction.Down:
          return -transform.localPosition.y;
        case GameState.Direction.Up:
          return transform.localPosition.y;
        case GameState.Direction.Back:
          return -transform.localPosition.z;
        case GameState.Direction.Fore:
          return transform.localPosition.z;
      }
      return 0;
    }

    set {
      Vector3 pos = transform.localPosition;
      switch (m_GameState.GetVerticalAxis(m_Type)) {
        case GameState.Direction.Left:
          pos.x = -value;
          break;
        case GameState.Direction.Right:
          pos.x = value;
          break;
        case GameState.Direction.Down:
          pos.y = -value;
          break;
        case GameState.Direction.Up:
          pos.y = value;
          break;
        case GameState.Direction.Back:
          pos.z = -value;
          break;
        case GameState.Direction.Fore:
          pos.z = value;
          break;
      }
      transform.localPosition = pos;
    }
  }

  private float kMouseSpeed = 0.5f;
  private float kBorder = 4.75f;
  private Vector3 m_Velocity;
  private float paddleSize;
  private Vector3 m_InitialPosition;

  void Awake() {
    paddleSize = transform.localScale.y;
    m_InitialPosition = transform.localPosition;
    Debug.Log($"set init: {m_InitialPosition} {name}");
  }

  void Update() {
    Vector2 mousePosition = Input.mousePosition;
    Vector3 pos = transform.localPosition;

    if (!m_IsStatic) {
      Vector3 oldPos = transform.localPosition;
      Vertical += kMouseSpeed * (Input.GetAxis("Mouse Y"));
      Debug.Log($"vert: {Vertical}");
      float boundary = kBorder - paddleSize / 2;

      if (Vertical < -boundary) {
        Vertical = -boundary;
      } else if (Vertical > boundary) {
        Vertical = boundary;
      }

      m_Velocity = (transform.localPosition - oldPos) / Time.deltaTime;
    }
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

      // add to the score
      m_GameState.Score++;
    }
  }

  public void SetStatic(bool value) {
    m_IsStatic = value;
    if (m_IsStatic) {
      transform.localPosition = m_InitialPosition;
      Debug.Log($"read init: {m_InitialPosition} {name}");
    }
    Vector3 size = 0.5f * Vector3.one;
    switch (m_Type) {
      case Type.Left:
      case Type.Right:
        size.y = m_IsStatic ? 10.5f : 2.0f;
        break;
      case Type.Down:
      case Type.Up:
        size.x = m_IsStatic ? 10.5f : 2.0f;
        break;
    }
    transform.localScale = size;
  }
}
