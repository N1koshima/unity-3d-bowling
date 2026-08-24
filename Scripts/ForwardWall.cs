using UnityEngine;

public class ForwardWall : MonoBehaviour
{
    private bool resetting = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<BallController>().ForceReturn();
        }
    }

    private BallController currentBall;
    void ResetBall()
    {
        if (currentBall != null)
        {
            currentBall.ResetBall();
        }
        GameManager.instance.BallReturned();
        resetting = false;
    }
}
