using UnityEngine;

public class GameState : MonoBehaviour {
  [SerializeField] private Puck m_Puck = null;
  [SerializeField] private Paddle m_PaddleLeft = null;
  [SerializeField] private Paddle m_PaddleRight = null;
  [SerializeField] private Paddle m_PaddleDown = null;
  [SerializeField] private Paddle m_PaddleUp = null;
  [SerializeField] private TMPro.TextMeshPro m_ScoreText = null;

  public int Score {
    get { return m_Score; }
    set {
      m_Score = value;
      m_ScoreText.text = m_Score.ToString();
    }
  }

  private Paddle[] m_Paddles;
  private int m_Level;
  private int m_Score;

  void Start() {
    m_Paddles = new Paddle[] {
      m_PaddleLeft,
      m_PaddleRight,
      m_PaddleDown,
      m_PaddleUp,
    };

    ResetGame();
  }

  void Update() {
    if (m_Puck.OutOfBounds) {
      ResetGame();
    }
  }

  public Paddle[] GetPaddles() {
    return m_Paddles;
  }

  private void ResetGame() {
    m_Level = 1;
    Score = 0;
    m_Puck.Reset();
  }
}
