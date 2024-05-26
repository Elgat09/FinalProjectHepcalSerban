using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    enum CurrentPlayer
    {
        Player1, Player2
    }

    CurrentPlayer currentPlayer;
    bool isWinningShotForPlayer1 = false;
    bool isWinningShotForPlayer2 = false;
    int player1BallsRemaining = 7;
    int player2BallsRemaining = 7;
    bool isWaitingForMovement = false;
    bool isGameOver = false;
    bool willSwapPlayer = false;
    bool ballPocketed = false;
    [SerializeField] float shotTimer = 3f;
    private float currentTimer;

    [SerializeField] TextMeshProUGUI player1BallsRemainingText;
    [SerializeField] TextMeshProUGUI player2BallsRemainingText;
    [SerializeField] TextMeshProUGUI currentTurnText;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] float movementThreshold;

    [SerializeField] GameObject restartButton;
    [SerializeField] Transform headPosition;

    [SerializeField] Camera cueStickCamera;
    [SerializeField] Camera overheadCamera;
    Camera currentCamera;

    void Start()
    {
        currentPlayer = CurrentPlayer.Player1;
        currentCamera = cueStickCamera;
        currentTimer = shotTimer;
        UpdateUI();
    }

    void Update()
    {
        if (isWaitingForMovement && !isGameOver)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer > 0)
            {
                return;
            }
            bool allStopped = true;

            foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball"))
            {
                if (ball.GetComponent<Rigidbody>().velocity.magnitude >= movementThreshold)
                {
                    allStopped = false;
                    break;
                }
            }

            if (allStopped)
            {
                isWaitingForMovement = false;
                if (willSwapPlayer || !ballPocketed)
                {
                    NextPlayerTurn();
                }
                else
                {
                    SwitchCameras(); // Switch to cue stick camera without resetting the camera
                }
                currentTimer = shotTimer;
                ballPocketed = false;
            }
        }
    }

    public void SwitchCameras()
    {
        if (currentCamera == cueStickCamera)
        {
            cueStickCamera.enabled = false;
            overheadCamera.enabled = true;
            currentCamera = overheadCamera;
            isWaitingForMovement = true;
        }
        else
        {
            cueStickCamera.enabled = true;
            overheadCamera.enabled = false;
            currentCamera = cueStickCamera;
            CameraController cameraController = currentCamera.gameObject.GetComponent<CameraController>();
            if (cameraController != null)
            {
                cameraController.ResetCamera();
            }
        }
    }


    public void RestartTheGame()
    {
        SceneManager.LoadScene(0);
    }

    bool Scratch()
    {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            if (isWinningShotForPlayer1)
            {
                ScratchOnWinningBall("Player 1");
                return true;
            }
        }
        else
        {
            if (isWinningShotForPlayer2)
            {
                ScratchOnWinningBall("Player 2");
                return true;
            }
        }
        willSwapPlayer = true;
        return false;
    }

    void EarlyEightBall()
    {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            Lose("Player 1 hit the black ball first and has lost");
        }
        else
        {
            Lose("Player 2 hit the black ball first and has lost");
        }
    }

    void ScratchOnWinningBall(string player)
    {
        Lose(player + " scratched on their final shot and lost");
    }

    bool CheckBall(Ball ball)
    {
        if (ball.IsCueBall())
        {
            if (Scratch())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (ball.IsEightBall())
        {
            if (currentPlayer == CurrentPlayer.Player1)
            {
                if (isWinningShotForPlayer1)
                {
                    Win("Player 1");
                    return true;
                }
            }
            else
            {
                if (isWinningShotForPlayer2)
                {
                    Win("Player 2");
                    return true;
                }
            }
            EarlyEightBall();
        }
        else
        {
            if (ball.IsBallRed())
            {
                player1BallsRemaining--;
                player1BallsRemainingText.text = "Player 1 Balls Remaining: " + player1BallsRemaining;
                if (player1BallsRemaining <= 0)
                {
                    isWinningShotForPlayer1 = true;
                }
                if (currentPlayer != CurrentPlayer.Player1)
                {
                    isWaitingForMovement = true;
                    willSwapPlayer = true;
                }
            }
            else
            {
                player2BallsRemaining--;
                player2BallsRemainingText.text = "Player 2 Balls Remaining: " + player2BallsRemaining;
                if (player2BallsRemaining <= 0)
                {
                    isWinningShotForPlayer2 = true;
                }
                if (currentPlayer != CurrentPlayer.Player2)
                {
                    isWaitingForMovement = true;
                    willSwapPlayer = true;
                }
            }
        }
        return true;
    }

    void Lose(string message)
    {
        isGameOver = true;
        messageText.gameObject.SetActive(true);
        messageText.text = message;
        restartButton.SetActive(true);
    }

    void Win(string player)
    {
        isGameOver = true;
        messageText.gameObject.SetActive(true);
        messageText.text = player + " has won";
        restartButton.SetActive(true);
    }

    void NextPlayerTurn()
    {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            currentPlayer = CurrentPlayer.Player2;
            currentTurnText.text = "Current Turn: Player 2";
        }
        else
        {
            currentPlayer = CurrentPlayer.Player1;
            currentTurnText.text = "Current Turn: Player 1";
        }
        willSwapPlayer = false;
        SwitchCameras();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            ballPocketed = true;
            if (CheckBall(other.gameObject.GetComponent<Ball>()))
            {
                Destroy(other.gameObject);
            }
            else
            {
                other.gameObject.transform.position = headPosition.position;
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }

    private void UpdateUI()
    {
        player1BallsRemainingText.text = "Player 1 Balls Remaining: " + player1BallsRemaining;
        player2BallsRemainingText.text = "Player 2 Balls Remaining: " + player2BallsRemaining;
        currentTurnText.text = "Current Turn: Player 1";
    }
}
