using UnityEngine;



public class GameSetUp : MonoBehaviour
{

    int redBallsRemaining = 7;
    int blueBallsRemaining = 7;
    float ballDiameter;
    float ballRadius;
    float ballDiameterWithBuffer;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] Transform cueBallPosition;
    [SerializeField] Transform headballPosition;
    private void Awake()
    {
        ballRadius = ballPrefab.GetComponent<SphereCollider>().radius * 100f;
        ballDiameter = ballRadius * 2f;
        ballDiameterWithBuffer = ballDiameter * 1.1f;
        PlaceAllBalls();
    }

    void PlaceAllBalls()
    {

        PlaceCueBall();
        PlaceRandomBalls();
    }
    void PlaceCueBall()
    {
        if (ballPrefab == null)
        {
            Debug.LogError("ballPrefab is not set in the inspector!");
            return;
        }
        if (cueBallPosition == null)
        {
            Debug.LogError("cueBallPosition is not set in the inspector!");
            return;
        }

        GameObject ball = Instantiate(ballPrefab, cueBallPosition.position, Quaternion.identity);
        if (ball == null)
        {
            Debug.LogError("Failed to instantiate the cue ball prefab!");
            return;
        }

        Ball ballComponent = ball.GetComponent<Ball>();
        if (ballComponent == null)
        {
            Debug.LogError("The Ball component is not found on the instantiated ball prefab!");
            return;
        }

        ballComponent.MakeCueBall();
    }
    void PlaceEightBall(Vector3 position)
    {
        GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
        ball.GetComponent<Ball>().MakeEightBall();
    }

    void PlaceRandomBalls()
    {
        int numInThisRow = 1;
        Vector3 firstInRowPosition = headballPosition.position;
        Vector3 currentPosition = firstInRowPosition;

        void PlaceRedBall(Vector3 position)
        {
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.GetComponent<Ball>().BallSetup(true);
            redBallsRemaining--;
        }

        void PlaceBlueBall(Vector3 position)
        {
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.GetComponent<Ball>().BallSetup(false);
            blueBallsRemaining--;
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < numInThisRow; j++)
            {
                
                if (i == 2 && j == 1)
                {
                    PlaceEightBall(currentPosition);
                }
                else if (redBallsRemaining > 0 && blueBallsRemaining > 0)
                {
                    int rand = Random.Range(0, 2);
                    if (rand == 0)
                    {
                        PlaceRedBall(currentPosition);
                    }
                    else
                    {
                        PlaceBlueBall(currentPosition);
                    }
                }
                else if (redBallsRemaining > 0)
                {
                    PlaceRedBall(currentPosition);
                }
                else
                {
                    PlaceBlueBall(currentPosition);
                }

                currentPosition += new Vector3(ballDiameterWithBuffer, 0, 0);
            }

            firstInRowPosition += new Vector3(-ballDiameterWithBuffer / 2, 0, -Mathf.Sqrt(3) * ballRadius);
            currentPosition = firstInRowPosition;
            numInThisRow++; 
        }
    }
}