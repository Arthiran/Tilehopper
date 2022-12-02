using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using AndroidAudioBypass;
using TMPro;

public enum GameState
{
    ReadyForInput,
    Moving,
    Lost
}

public class GameManager : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Transform StartPlatform;
    public Transform GlassPlatform;
    public Transform UpdateScoreText;

    [SerializeField] private float JumpForce;

    [SerializeField] private Transform Player;
    [SerializeField] private BoxCollider playerCollider;
    [SerializeField] private GameObject EndObject;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI HighScoreText;
    [SerializeField] private GameObject NewHighScoreText;
    private Vector3 TargetDestination = Vector3.zero;
    private Quaternion TargetRotation = Quaternion.identity;

    private GameState CurrentState;

    private Transform CurrentPlatform = null;

    private int currentScore = 0;

    private float playerMoveDist = 2f;
    private float playerYValue = -0.5f;
    private float distToGround;

    [SerializeField] private NativeAudioManager AudioManagerInstance;
    int jumpSoundId = -1;
    string soundFileName = "Swoosh.wav";

    private int PlayerHighscore = 0;

    private GooglePlayServices playServices;


    // Start is called before the first frame update
    void Start()
    {
        playServices = FindObjectOfType<GooglePlayServices>();

        if (PlayerPrefs.HasKey("HighScore"))
        {
            PlayerHighscore = PlayerPrefs.GetInt("HighScore");
        }
        else
        {
            PlayerPrefs.SetInt("HighScore", 0);
            PlayerHighscore = 0;
        }

        distToGround = playerCollider.bounds.extents.y + 0.2f;
        jumpSoundId = AudioManagerInstance.RegisterSoundFile(soundFileName);
        CurrentPlatform = StartPlatform;
        StartCoroutine(SpawnPlaform());
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                LoadMenuScene();
                return;
            }
        }

        if (CurrentState == GameState.Lost)
        {
            if (EndObject.activeSelf != true)
            {
                if (PlayerPrefs.GetInt("HighScore") < currentScore)
                {
                    PlayerPrefs.SetInt("HighScore", currentScore);
                    HighScoreText.text = currentScore.ToString();
                    PlayerHighscore = currentScore;
                    if (playServices != null)
                    {
                        if (playServices.connectedToGooglePlay)
                        {
                            Social.ReportScore(currentScore, GPGSIds.leaderboard_tiles_hopped, LeaderboardUpdate);
                        }
                    }

                    NewHighScoreText.SetActive(true);
                }
                else
                {
                    HighScoreText.text = PlayerHighscore.ToString();
                    NewHighScoreText.SetActive(false);
                }

                Destroy(Player.gameObject);
                EndObject.SetActive(true);
            }
        }

        if (Player != null)
        {
            if (Player.position.y < -1f)
            {
                CurrentState = GameState.Lost;
            }
        }
    }

    private void FixedUpdate()
    {
        if (CurrentState == GameState.Moving)
        {
            Player.position = Vector3.MoveTowards(Player.position, new Vector3(TargetDestination.x, Player.position.y, TargetDestination.z), 0.2f);
            Player.rotation = Quaternion.RotateTowards(Player.rotation, Quaternion.Euler(0, TargetRotation.eulerAngles.y, 0), 10f);

            if (Vector2.Distance(new Vector2(Player.position.x, Player.position.z), new Vector2(TargetDestination.x, TargetDestination.z)) < 0.1f && Physics.Raycast(Player.position, -Vector2.up, distToGround))
            {
                Player.position = TargetDestination;
                Player.rotation = Quaternion.Euler(0, TargetRotation.eulerAngles.y, 0);
                currentScore++;
                SetScoreText(currentScore);
                CurrentState = GameState.ReadyForInput;
            }
        }
    }

    private void LeaderboardUpdate(bool success)
    {
        return;
    }

    private void SetScoreText(int _InValue)
    {
        if (Player != null)
        {
            Transform newText = Instantiate(UpdateScoreText, new Vector3(Player.position.x, Player.position.y + 3, Player.position.z), UpdateScoreText.rotation);
            Destroy(newText.gameObject, 0.25f);
        }

        ScoreText.text = _InValue.ToString();
    }

    private void CreatePlatform(Transform _Platform, int _InValue)
    {
        if (_InValue == 0)
        {
            CurrentPlatform = Instantiate(GlassPlatform, new Vector3(_Platform.position.x, -0.6f, _Platform.position.z + 2), Quaternion.identity);
        }
        else if (_InValue == 1)
        {
            CurrentPlatform = Instantiate(GlassPlatform, new Vector3(_Platform.position.x, -0.6f, _Platform.position.z - 2), Quaternion.identity);
        }
        else if (_InValue == 2)
        {
            CurrentPlatform = Instantiate(GlassPlatform, new Vector3(_Platform.position.x + 2, -0.6f, _Platform.position.z), Quaternion.identity);
        }
        else
        {
            CurrentPlatform = Instantiate(GlassPlatform, new Vector3(_Platform.position.x - 2, -0.6f, _Platform.position.z), Quaternion.identity);
        }
    }

    private IEnumerator SpawnPlaform()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(CurrentPlatform.gameObject, 0.5f);
        if (CurrentState != GameState.Lost)
        {
            int newValue = Random.Range(0, 4);
            CreatePlatform(CurrentPlatform, newValue);
            StartCoroutine(SpawnPlaform());
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private enum DraggedDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    private void GetDragDirection(Vector3 dragVector)
    {
        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        DraggedDirection draggedDir;
        if (positiveX > positiveY)
        {
            draggedDir = (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
        }
        else
        {
            draggedDir = (dragVector.y > 0) ? DraggedDirection.Up : DraggedDirection.Down;
        }

        if (CurrentState != GameState.Lost && CurrentState == GameState.ReadyForInput)
        {
            if (draggedDir == DraggedDirection.Up)
            {
                AudioManagerInstance.PlaySound(jumpSoundId, 1, 1, 1, 0, 1);
                Player.gameObject.GetComponent<Rigidbody>().AddForce(JumpForce * Vector3.up);
                TargetDestination = Player.position + new Vector3(0, playerYValue, playerMoveDist);
                TargetRotation = Quaternion.Euler(0, 180, 0);
                CurrentState = GameState.Moving;
            }
            if (draggedDir == DraggedDirection.Left)
            {
                AudioManagerInstance.PlaySound(jumpSoundId, 1, 1, 1, 0, 1);
                Player.gameObject.GetComponent<Rigidbody>().AddForce(JumpForce * Vector3.up);
                TargetDestination = Player.position + new Vector3(-playerMoveDist, playerYValue, 0);
                TargetRotation = Quaternion.Euler(0, 90, 0);
                CurrentState = GameState.Moving;
            }
            if (draggedDir == DraggedDirection.Down)
            {
                AudioManagerInstance.PlaySound(jumpSoundId, 1, 1, 1, 0, 1);
                Player.gameObject.GetComponent<Rigidbody>().AddForce(JumpForce * Vector3.up);
                TargetDestination = Player.position + new Vector3(0, playerYValue, -playerMoveDist);
                TargetRotation = Quaternion.Euler(0, 0, 0);
                CurrentState = GameState.Moving;
            }
            if (draggedDir == DraggedDirection.Right)
            {
                AudioManagerInstance.PlaySound(jumpSoundId, 1, 1, 1, 0, 1);
                Player.gameObject.GetComponent<Rigidbody>().AddForce(JumpForce * Vector3.up);
                TargetDestination = Player.position + new Vector3(playerMoveDist, playerYValue, 0);
                TargetRotation = Quaternion.Euler(0, -90, 0);
                CurrentState = GameState.Moving;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        GetDragDirection(dragVectorDirection);
    }
}
