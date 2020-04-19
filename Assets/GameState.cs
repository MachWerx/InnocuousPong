using UnityEngine;

public class GameState : MonoBehaviour {
  [SerializeField] private Puck m_Puck = null;
  [SerializeField] private Paddle m_PaddleLeft = null;
  [SerializeField] private Paddle m_PaddleRight = null;
  [SerializeField] private Paddle m_PaddleDown = null;
  [SerializeField] private Paddle m_PaddleUp = null;
  [SerializeField] private Paddle m_PaddleBack = null;
  [SerializeField] private Paddle m_PaddleFront = null;
  [SerializeField] private TMPro.TextMeshPro m_ScoreText = null;
  [SerializeField] private TMPro.TextMeshPro m_LevelText = null;

  public enum Direction {
    None,
    Left,
    Right,
    Down,
    Up,
    Back,
    Front
  }

  // Each enty specifies whether the given paddle (Left, Right, Down, Up, Back, Fore) is static for
  // that level.
  private bool[][] kPaddleStaticMap = new bool[][] {
    new bool[] {false, true , true , true , true , true },  // level 1, just left
    new bool[] {false, false, true , true , true , true },  // level 2, left and right, linked
    new bool[] {false, false, true , true , true , true },  // level 3, left and right, opposite
    new bool[] {false, false, false, false, true , true },  // level 4, left/right and down/up, linked to y and x
    new bool[] {false, false, false, false, true , true },  // level 5, left/right and down/up, just y
    new bool[] {false, false, false, false, false, false},  // level 6, THREE DIMENSIONS!
  };

  // Each enty specifies the horizontal axis for the given paddle (Left, Right, Down, Up, Back,
  // Fore).
  private Direction[][] kHorizontalAxisMap = new Direction[][] {
    new Direction[] {Direction.None, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 1
    new Direction[] {Direction.None, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 2
    new Direction[] {Direction.None, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 3
    new Direction[] {Direction.None, Direction.None, Direction.Right, Direction.Right, Direction.None, Direction.None},  // level 4
    new Direction[] {Direction.None, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 5
    new Direction[] {Direction.Back, Direction.Front, Direction.Left, Direction.Right, Direction.Down, Direction.Up},  // level 6
  };

  // Each enty specifies the vertical axis for the given paddle (Left, Right, Down, Up, Back,
  // Fore).
  private Direction[][] kVerticalAxisMap = new Direction[][] {
    new Direction[] {Direction.Up, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 1
    new Direction[] {Direction.Up, Direction.Up, Direction.None, Direction.None, Direction.None, Direction.None},  // level 2
    new Direction[] {Direction.Up, Direction.Down, Direction.None, Direction.None, Direction.None, Direction.None},  // level 3
    new Direction[] {Direction.Up, Direction.Up, Direction.None, Direction.None, Direction.None, Direction.None},  // level 4
    new Direction[] {Direction.Up, Direction.Down, Direction.Left, Direction.Right, Direction.None, Direction.None},  // level 5
    new Direction[] {Direction.Up, Direction.Down, Direction.Back, Direction.Front, Direction.Left, Direction.Right},  // level 6
  };

  public int Score {
    get { return m_Score; }
    set {
      m_Score = value;
      m_ScoreText.text = m_Score.ToString();

      Level = (m_Score / 2) + 1;
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
    Cursor.visible = false;
    m_Paddles = new Paddle[] {
      m_PaddleLeft,
      m_PaddleRight,
      m_PaddleDown,
      m_PaddleUp,
      m_PaddleBack,
      m_PaddleFront
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

  public float GetPuckTargetSpeed() {
    return 0.2f * (2 + Level / 4.0f);
  }

  private void ResetGame() {
    Level = 0;
    Score = 9;
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
      m_PaddleDown.SetStatic(staticMap[2]);
      m_PaddleUp.SetStatic(staticMap[3]);
      m_PaddleBack.SetStatic(staticMap[4]);
      m_PaddleFront.SetStatic(staticMap[5]);
    }

    //foreach (var paddle in GetPaddles()) {
    //  paddle.Horizontal = horizontalValue;
    //  paddle.Vertical = verticalValue;
    //}
  }
}
