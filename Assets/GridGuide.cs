using UnityEngine;

public class GridGuide : MonoBehaviour {
  [SerializeField] private Puck m_Puck = null;
  [SerializeField] private Paddle m_PaddleLeft = null;
  [SerializeField] private Paddle m_PaddleRight = null;
  [SerializeField] private Paddle m_PaddleDown = null;
  [SerializeField] private Paddle m_PaddleUp = null;
  [SerializeField] private Paddle m_PaddleBack = null;
  [SerializeField] private Paddle m_PaddleFront = null;
  [SerializeField] private MeshRenderer m_QuadLeft = null;
  [SerializeField] private MeshRenderer m_QuadRight = null;
  [SerializeField] private MeshRenderer m_QuadDown = null;
  [SerializeField] private MeshRenderer m_QuadUp = null;
  [SerializeField] private MeshRenderer m_QuadBack = null;
  [SerializeField] private MeshRenderer m_QuadFront = null;
  private Paddle[] m_Paddles;
  private Material[] m_Materials;
  private Matrix4x4[] m_Matrices;

  void Start() {
    m_Paddles = new Paddle[] {
      m_PaddleLeft ,
      m_PaddleRight,
      m_PaddleDown ,
      m_PaddleUp   ,
      m_PaddleBack ,
      m_PaddleFront,
    };
    m_Materials = new Material[] {
      m_QuadLeft .material,
      m_QuadRight.material,
      m_QuadDown .material,
      m_QuadUp   .material,
      m_QuadBack .material,
      m_QuadFront.material,
    };
    // we need to pass in the matrices so that we can preserve object space
    m_Matrices = new Matrix4x4[] {
      Matrix4x4.TRS(.5f * Vector3.left, Quaternion.Euler(0, 90, 0), Vector3.one),
      Matrix4x4.TRS(.5f * Vector3.right, Quaternion.Euler(0, 90, 0), Vector3.one),
      Matrix4x4.TRS(.5f * Vector3.down, Quaternion.Euler(90, 0, 0), Vector3.one),
      Matrix4x4.TRS(.5f * Vector3.up, Quaternion.Euler(90, 0, 0), Vector3.one),
      Matrix4x4.TRS(.5f * Vector3.back, Quaternion.Euler(0, 0, 0), Vector3.one),
      Matrix4x4.TRS(.5f * Vector3.forward, Quaternion.Euler(0, 0, 0), Vector3.one),
    };
  }

  void Update() {
    for (int i = 0; i < m_Paddles.Length; i++) {
      Vector4 pos = new Vector4(
          m_Paddles[i].transform.localPosition.x / transform.localScale.x,
          m_Paddles[i].transform.localPosition.y / transform.localScale.y,
          m_Paddles[i].transform.localPosition.z / transform.localScale.z,
          1);
      m_Materials[i].SetVector("_PaddlePosition", pos);
      m_Materials[i].SetMatrix("_QuadAdjust", m_Matrices[i]);
    }
  }
}
