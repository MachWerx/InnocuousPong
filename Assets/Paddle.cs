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
    Front
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
        case GameState.Direction.Front:
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
        case GameState.Direction.Front:
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
        case GameState.Direction.Front:
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
        case GameState.Direction.Front:
          pos.z = value;
          break;
      }
      transform.localPosition = pos;
    }
  }

  private const float kMouseSpeed = 0.5f;
  private const float kBorder = 4.75f;
  private static readonly Vector3[] kNormals = new Vector3[] {
    Vector3.right,
    Vector3.left,
    Vector3.up,
    Vector3.down,
    Vector3.forward,
    Vector3.back
  };

  private Vector3 m_Velocity;
  private Vector3 m_InitialPosition;

  void Awake() {
    m_InitialPosition = transform.localPosition;
  }

  void Update() {
    Vector2 mousePosition = Input.mousePosition;
    Vector3 pos = transform.localPosition;

    if (!m_IsStatic) {
      Vector3 oldPos = transform.localPosition;

      Vertical += kMouseSpeed * (Input.GetAxis("Mouse Y"));
      if (Vertical < -kBorder) {
        Vertical = -kBorder;
      } else if (Vertical > kBorder) {
        Vertical = kBorder;
      }

      Horizontal += kMouseSpeed * (Input.GetAxis("Mouse X"));
      if (Horizontal < -kBorder) {
        Horizontal = -kBorder;
      } else if (Horizontal > kBorder) {
        Horizontal = kBorder;
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
      Vector3 normal = kNormals[(int)m_Type];
      Vector3 boundaryPoint = pos + 0.5f * normal;
      float distance = Vector3.Dot(puckPos - boundaryPoint, normal);
      if (distance < 0) {
        float puckVelNormalComponent = Vector3.Dot(puckVel, normal);
        float t = distance / puckVelNormalComponent;
        puckPos -= puckVel * t;
        pos -= m_Velocity * t;
        if (!m_IsStatic) {
          // tweak normals for paddles
          const float kFudgeRadius = 5.0f;  // the radius of a sphere to fudge the normal with
          normal = (puckPos - (pos - kFudgeRadius * normal)).normalized;
        }
        puckVel -= 2 * Vector3.Dot(puckVel, normal) * normal;
        puckPos += puckVel * t;
      }
      m_GameState.Score++;
    }
  }

  public void ResetPosition() {
    transform.localPosition = m_InitialPosition;
  }

  public void SetStatic(bool value) {
    m_IsStatic = value;
    if (m_IsStatic) {
      ResetPosition();
    }
    Vector3 size = 0.5f * Vector3.one;
    switch (m_Type) {
      case Type.Left:
      case Type.Right:
        size.y = size.z = m_IsStatic ? 10.5f : 2.0f;
        break;
      case Type.Down:
      case Type.Up:
        size.x = size.z = m_IsStatic ? 10.5f : 2.0f;
        break;
      case Type.Back:
      case Type.Front:
        size.x = size.y = m_IsStatic ? 10.5f : 2.0f;
        break;
    }
    transform.localScale = size;
  }
}
