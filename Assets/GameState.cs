using UnityEngine;

public class GameState : MonoBehaviour {
  [SerializeField] private Puck m_Puck = null;
  [SerializeField] private Paddle m_PaddleLeft = null;
  [SerializeField] private Paddle m_PaddleRight = null;
  [SerializeField] private Paddle m_PaddleDown = null;
  [SerializeField] private Paddle m_PaddleUp = null;
  [SerializeField] private TMPro.TextMeshPro m_ScoreText = null;
  [SerializeField] private TMPro.TextMeshPro m_LevelText = null;

  public int Score {
    get { return m_Score; }
    set {
      m_Score = value;
      m_ScoreText.text = m_Score.ToString();

      Level = (m_Score / 10) + 1;
    }
  }

  public int Level {
    get { return m_Level; }
    set {
      if (m_Level != value) {
        m_Level = value;
        m_LevelText.text = $"Level {m_Level}!";
        m_LevelText.alpha = 1;
      }
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

    if (m_LevelText.alpha > 0) {
      m_LevelText.alpha -= Time.deltaTime * 2.0f;
      if (m_LevelText.alpha < 0) {
        m_LevelText.alpha = 0;
      }
    }
  }

  public Paddle[] GetPaddles() {
    return m_Paddles;
  }

  private void ResetGame() {
    Score = 0;
    Level = 1;
    m_LevelText.alpha = 1;
    m_Puck.Reset();
  }
}
