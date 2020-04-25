using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour {
  [SerializeField] GameState m_GameState = null;
  [SerializeField] GameObject m_MainBody = null;
  [SerializeField] GameObject m_FragmentTemplate = null;
  [SerializeField] Transform m_GuideX = null;
  [SerializeField] Transform m_GuideY = null;
  [SerializeField] Transform m_GuideZ = null;

  public bool IsDead {
    get;
    private set;
  }

  private Vector3 m_Velocity;
  private bool m_IsOut;
  private bool m_IsExploding;
  private float m_DeathCountdown;
  private GameObject[] m_Fragments;
  private Rigidbody[] m_FragmentRigidbodies;
  private float m_FragmentMaxSize;

  private void Awake() {
    // instantiate some fragments
    var fragmentList = new List<GameObject>();
    var rigidbodyList = new List<Rigidbody>();
    m_FragmentMaxSize = m_FragmentTemplate.transform.localScale.x;
    fragmentList.Add(m_FragmentTemplate);
    rigidbodyList.Add(m_FragmentTemplate.GetComponent<Rigidbody>());
    var parent = m_FragmentTemplate.transform.parent;
    for (int i = 0; i < 100; i++) {
      GameObject newFragment = Instantiate(m_FragmentTemplate, parent);
      fragmentList.Add(newFragment);
      rigidbodyList.Add(newFragment.GetComponent<Rigidbody>());
    }
    m_Fragments = fragmentList.ToArray();
    m_FragmentRigidbodies = rigidbodyList.ToArray();
  }

  // Do physics in late update so that the puck comes after the paddle.
  void LateUpdate() {
    if (m_DeathCountdown > 0) {
      m_DeathCountdown -= Time.deltaTime;
      if (m_DeathCountdown <= 0) {
        m_DeathCountdown = 0;
        IsDead = true;
      }

      if (!m_IsExploding) {
        if (m_DeathCountdown < 2.75f) {
          Debug.Log($"exploding {Time.timeSinceLevelLoad}");
          // explode the puck
          m_IsExploding = true;
          m_MainBody.SetActive(false);
          m_GameState.Explode();
          Vector3 lastVelDir = m_Velocity.normalized;
          for (int i = 0; i < m_Fragments.Length; i++) {
            m_Fragments[i].SetActive(true);
            m_Fragments[i].transform.localPosition = transform.localPosition;
            m_Fragments[i].transform.localScale = (.8f * Random.value + .2f) * m_FragmentMaxSize * Vector3.one;
            float explodiness = Random.value;
            Vector3 explodeVector = 5.0f * Mathf.Pow(explodiness, 4) * m_Velocity + (3f * (Mathf.Pow(Random.value, 3) + .5f)) * Random.insideUnitSphere;
            m_FragmentRigidbodies[i].velocity = explodeVector;
            m_Fragments[i].transform.localPosition += m_FragmentRigidbodies[i].velocity * Time.deltaTime;

            m_FragmentRigidbodies[i].drag = 0.5f * (1 - explodiness) + .5f;
          }
          m_Velocity = Vector3.zero;
        }
      }
    }

    Vector3 pos = transform.localPosition;
    float speed = m_Velocity.magnitude;
    speed = 0.1f * (m_GameState.GetPuckTargetSpeed() - speed) + speed;
    m_Velocity = speed * m_Velocity.normalized;
    pos += m_Velocity * Time.deltaTime;

    if (!m_IsOut) {
      foreach (var paddle in m_GameState.GetPaddles()) {
        paddle.DoIntersection(ref pos, transform.localScale, ref m_Velocity);
      }

      // check for out of bounds
      const float kBorder = 5.5f;
      const float puckSize = 0.5f;
      const float kPaddleSize = 0.5f;
      const float bounds = kBorder - 0.5f * puckSize - 0.5f * kPaddleSize;
      if (Mathf.Abs(pos.x) > bounds ||
          Mathf.Abs(pos.y) > bounds ||
          Mathf.Abs(pos.z) > bounds) {
        // puck has gone out of bounds
        m_IsOut = true;
        m_GuideX.gameObject.SetActive(false);
        m_GuideY.gameObject.SetActive(false);
        m_GuideZ.gameObject.SetActive(false);
        m_DeathCountdown = 3.0f;
      }
    }

    transform.localPosition = pos;

    if (!m_IsOut) {
      m_GuideX.localPosition = new Vector3(0, pos.y, pos.z);
      m_GuideY.localPosition = new Vector3(pos.x, 0, pos.z);
      m_GuideZ.localPosition = new Vector3(pos.x, pos.y, 0);
    }
  }

  public void SetGuides(bool active) {
    m_GuideX.gameObject.SetActive(active);
    m_GuideY.gameObject.SetActive(active);
    m_GuideZ.gameObject.SetActive(active);
  }

  public Vector3 GetTargetPosition() {
    Ray ray = new Ray(transform.position, m_Velocity.normalized);
    RaycastHit hitInfo;
    int layerMask = 1 << 9;
    if (Physics.Raycast(ray, out hitInfo, 10.0f, layerMask)) {
      return hitInfo.point;
    } else {
      return transform.localPosition;
    }
  }

  public void Reset() {
    transform.localPosition = Vector3.zero;
    float angle = (Random.value - 0.5f) * 0.5f * Mathf.PI + Mathf.PI;
    m_Velocity = 3f * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
    m_MainBody.SetActive(true);
    for (int i = 0; i < m_Fragments.Length; i++) {
      m_Fragments[i].SetActive(false);
    }
    m_IsOut = false;
    m_IsExploding = false;
    IsDead = false;
  }
}
