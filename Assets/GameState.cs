using UnityEngine;

public class GameState : MonoBehaviour {
  [SerializeField] private Puck m_Puck = null;
  [SerializeField] private Paddle m_PaddleLeft = null;
  [SerializeField] private Paddle m_PaddleRight = null;
  [SerializeField] private Paddle m_PaddleDown = null;
  [SerializeField] private Paddle m_PaddleUp = null;
  [SerializeField] private TMPro.TextMeshPro m_ScoreText = null;
  [SerializeField] private TMPro.TextMeshPro m_LevelText = null;

  public enum Direction {
    None,
    Left,
    Right,
    Down,
    Up,
    Back,
    Fore
  }

  // Each enty specifies whether the given paddle (Left, Right, Down, Up, Back, Fore) is static for
  // that level.
  private bool[][] kPaddleStaticMap = new bool[][] {
    new bool[] {false, true },  // level 1
    new bool[] {false, false},  // level 2
    new bool[] {false, false},  // level 3
  };

  // Each enty specifies the horizontal axis for the given paddle (Left, Right, Down, Up, Back,
  // Fore).
  private Direction[][] kHorizontalAxisMap = new Direction[][] {
    new Direction[] {Direction.None, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 1
    new Direction[] {Direction.None, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 2
    new Direction[] {Direction.None, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 2
  };

  // Each enty specifies the vertical axis for the given paddle (Left, Right, Down, Up, Back,
  // Fore).
  private Direction[][] kVerticalAxisMap = new Direction[][] {
    new Direction[] {Direction.Up, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 1
    new Direction[] {Direction.Up, Direction.Up, Direction.None, Direction.None, Direction.None, Direction.None},  // level 2
    new Direction[] {Direction.Up, Direction.Down, Direction.None, Direction.None, Direction.None, Direction.None},  // level 2
  };

  public int Score {
    get { return m_Score; }
    set {
      m_Score = value;
      m_ScoreText.text = m_Score.ToString();

      Level = (m_Score / 3) + 1;
    }
  }

  public int Level {
    get { return m_Level; }
    set {
      if (m_Level != value) {
        m_Level = value;
        m_LevelText.text = $"Level {m_Level}!";
        m_LevelText.alpha = 1;

        AdjustPaddles();
      }
    }
  }

  private Paddle[] m_Paddles;
  private int m_Level;
  private int m_Score;
  private float m_Difficulty;

  void Awake() {
    m_Paddles = new Paddle[] {
      m_PaddleLeft,
      m_PaddleRight,
      m_PaddleDown,
      m_PaddleUp,
    };
  }

  private void Start() {
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

  public Direction GetHorizontalAxis(Paddle.Type type) {
    if (Level > 0) {
      int mapLevel = Level <= kPaddleStaticMap.Length
          ? Level - 1
          : kPaddleStaticMap.Length - 1;
      return kHorizontalAxisMap[mapLevel][(int)type];
    }

    return Direction.None;
  }

  public Direction GetVerticalAxis(Paddle.Type type) {
    if (Level > 0) {
      int mapLevel = Level <= kPaddleStaticMap.Length
          ? Level - 1
          : kPaddleStaticMap.Length - 1;
      return kVerticalAxisMap[mapLevel][(int)type];
    }

    return Direction.None;
  }

  private void ResetGame() {
    Score = 0;
    Level = 1;
    m_LevelText.alpha = 1;
    m_Difficulty = 0.0f;

    m_Puck.Reset();
  }

  private void AdjustPaddles() {
    float horizontalValue = m_PaddleLeft.transform.localPosition.y;
    float verticalValue = m_PaddleLeft.transform.localPosition.y;
    if (Level > 0) {
      int mapLevel = Level <= kPaddleStaticMap.Length
          ? Level - 1
          : kPaddleStaticMap.Length - 1;
      bool[] staticMap = kPaddleStaticMap[mapLevel];
      m_PaddleLeft.SetStatic(staticMap[0]);
      m_PaddleRight.SetStatic(staticMap[1]);
    }

    foreach (var paddle in GetPaddles()) {
      paddle.Horizontal = horizontalValue;
      paddle.Vertical = verticalValue;
    }
  }
}
