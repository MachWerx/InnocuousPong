using UnityEngine;

public class GameState : MonoBehaviour {
  [SerializeField] private CameraControl m_CameraControl = null;
  [SerializeField] private Puck m_Puck = null;
  [SerializeField] private Paddle m_PaddleLeft = null;
  [SerializeField] private Paddle m_PaddleRight = null;
  [SerializeField] private Paddle m_PaddleDown = null;
  [SerializeField] private Paddle m_PaddleUp = null;
  [SerializeField] private Paddle m_PaddleBack = null;
  [SerializeField] private Paddle m_PaddleFront = null;
  [SerializeField] private GridGuide m_GridGuide = null;
  [SerializeField] private Light m_Light = null;
  [SerializeField] private AudioSource m_AudioHit = null;
  [SerializeField] private AudioSource m_AudioExplosion = null;

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

  private const int kPointsBetweenLevels = 3;
  private const int kMaxLevel = 6;

  private float[] kPuckBaseSpeed = new float[] {
    5.0f,  // level 0, just left
    5.0f,  // level 1, left and right, linked
    4.0f,  // level 2, left and right, opposite
    4.0f,  // level 3, left/right and down/up, linked to y and x
    3.0f,  // level 4, left/right and down/up, just y
    2.0f,  // level 5, THREE DIMENSIONS!
  };

  // Each enty specifies whether the given paddle (Left, Right, Down, Up, Back, Fore) is static for
  // that level.
  private bool[][] kPaddleStaticMap = new bool[][] {
    new bool[] {false, true , true , true , true , true },  // level 0, just left
    new bool[] {false, false, true , true , true , true },  // level 1, left and right, linked
    new bool[] {false, false, true , true , true , true },  // level 2, left and right, opposite
    new bool[] {false, false, false, false, true , true },  // level 3, left/right and down/up, linked to y and x
    new bool[] {false, false, false, false, true , true },  // level 4, left/right and down/up, just y
    new bool[] {false, false, false, false, false, false},  // level 5, THREE DIMENSIONS!
  };

  // Each enty specifies the horizontal axis for the given paddle (Left, Right, Down, Up, Back,
  // Fore).
  private Direction[][] kHorizontalAxisMap = new Direction[][] {
    new Direction[] {Direction.None, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 0
    new Direction[] {Direction.None, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 1
    new Direction[] {Direction.None, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 2
    new Direction[] {Direction.None, Direction.None, Direction.Right, Direction.Right, Direction.None, Direction.None},  // level 3
    new Direction[] {Direction.None, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 4
    //new Direction[] {Direction.Back, Direction.Front, Direction.Left, Direction.Right, Direction.Down, Direction.Up},  // level 5, too hard?
    new Direction[] {Direction.Back, Direction.Back, Direction.Right, Direction.Right, Direction.Right, Direction.Right},  // level 5
  };

  // Each enty specifies the vertical axis for the given paddle (Left, Right, Down, Up, Back,
  // Fore).
  private Direction[][] kVerticalAxisMap = new Direction[][] {
    new Direction[] {Direction.Up, Direction.None, Direction.None, Direction.None, Direction.None, Direction.None},  // level 0
    new Direction[] {Direction.Up, Direction.Up, Direction.None, Direction.None, Direction.None, Direction.None},  // level 1
    new Direction[] {Direction.Up, Direction.Down, Direction.None, Direction.None, Direction.None, Direction.None},  // level 2
    new Direction[] {Direction.Up, Direction.Up, Direction.None, Direction.None, Direction.None, Direction.None},  // level 3
    new Direction[] {Direction.Up, Direction.Down, Direction.Left, Direction.Right, Direction.None, Direction.None},  // level 4
    //new Direction[] {Direction.Up, Direction.Down, Direction.Back, Direction.Front, Direction.Left, Direction.Right},  // level 5, too hard?
    new Direction[] {Direction.Up, Direction.Up, Direction.Front, Direction.Front, Direction.Up, Direction.Up},  // level 5, too hard?
  };

  public int Score {
    get { return m_Score; }
    set {
      if (m_Score != value) {
        if (value > m_Score) {
          m_AudioHit.volume = .2f;
          m_AudioHit.Play();
        }
        m_Score = value;
        m_ScoreText.text = m_Score.ToString();

        Level = (m_Score / kPointsBetweenLevels);
      }
    }
  }

  public int Level {
    get { return m_Level; }
    set {
      if (value >= kMaxLevel) {
        value = kMaxLevel - 1;
      }
      if (m_Level != value) {
        m_Level = value;
        //m_LevelText.text = $"Level {m_Level}!";
        //m_LevelText.alpha = 1;

        AdjustForNewLevel();
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
    if (m_Puck.IsDead) {
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
    return kHorizontalAxisMap[Level][(int)type];
  }

  public Direction GetVerticalAxis(Paddle.Type type) {
    return kVerticalAxisMap[Level][(int)type];
  }

  public float GetPuckTargetSpeed() {
    float adjustFactor = ((float)Score - Level * kPointsBetweenLevels) / kPointsBetweenLevels;
    return kPuckBaseSpeed[Level] * (1 + .8f * adjustFactor);
  }

  private void AdjustForNewLevel() {
    AdjustPaddles();
    AdjustCamera();
    float lightFactor = (float)Level / (kMaxLevel - 1);
    m_Light.intensity = Mathf.Lerp(1.0f, 0.02f, lightFactor);

    // show guides after level 4
    if (Level > 4) {
      m_Puck.SetGuides(true);
      m_GridGuide.gameObject.SetActive(true);
    } else {
      m_Puck.SetGuides(false);
      m_GridGuide.gameObject.SetActive(false);
    }
  }

  private void AdjustPaddles() {
    float horizontalValue = m_PaddleLeft.transform.localPosition.y;
    float verticalValue = m_PaddleLeft.transform.localPosition.y;
    bool[] staticMap = kPaddleStaticMap[Level];
    m_PaddleLeft.SetStatic(staticMap[0]);
    m_PaddleRight.SetStatic(staticMap[1]);
    m_PaddleDown.SetStatic(staticMap[2]);
    m_PaddleUp.SetStatic(staticMap[3]);
    m_PaddleBack.SetStatic(staticMap[4]);
    m_PaddleFront.SetStatic(staticMap[5]);

    if (Level < 5) {
      m_PaddleBack.transform.localScale = Vector3.zero;
      m_PaddleFront.transform.localScale = Vector3.zero;
    }

    foreach (var paddle in GetPaddles()) {
      paddle.Horizontal = horizontalValue;
      paddle.Vertical = verticalValue;
    }
  }

  private void AdjustCamera() {
    if (Level < 5) {
      m_CameraControl.TransitionTo(Quaternion.Euler(0, 0, 0));
    } else {
      m_CameraControl.TransitionTo(Quaternion.Euler(30, 30, 0));
    }
  }

  public void Explode() {
    m_AudioExplosion.Play();
  }

  private void ResetGame() {
    Score = 0;
    m_LevelText.alpha = 0;
    m_Difficulty = 0.0f;

    m_Puck.Reset();
    foreach (var paddle in GetPaddles()) {
      paddle.ResetPosition();
    }
    AdjustForNewLevel();
  }
}
