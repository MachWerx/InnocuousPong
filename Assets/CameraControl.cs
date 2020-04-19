using UnityEngine;

public class CameraControl : MonoBehaviour {
  [SerializeField] private Camera m_Camera = null;

  private Quaternion m_SrcRot;
  private Quaternion m_DstRot;
  private float m_TransitionValue;

  void Start() {
    m_TransitionValue = 1;
  }

  void Update() {
    if (m_TransitionValue < 1) {
      m_TransitionValue += 3 * Time.deltaTime;
      if (m_TransitionValue > 1) {
        m_TransitionValue = 1;
      }
      transform.localRotation = Quaternion.Slerp(m_SrcRot, m_DstRot, m_TransitionValue);
    }
  }

  public void TransitionTo(Quaternion dstRot) {
    m_SrcRot = transform.localRotation;
    m_DstRot = dstRot;
    m_TransitionValue = 0;
  }
}
