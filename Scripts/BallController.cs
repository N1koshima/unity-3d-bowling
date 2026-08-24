using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public float moveSpeed = 4f;
    //максимальная сила броска
    public float maxThrowForce = 370f;
    //скорость заполнения шкалы силы
    public float forceBuildSpeed = 150f;

    private Vector3 startPosition;
    private bool waitingForReset = false;
    private bool returnScheduled = false;
    private float currentForce = 0f;
    private float stoppedTime = 0f;
    private Rigidbody rb;
    private bool thrown = false;
    public Slider powerBar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        powerBar.maxValue = maxThrowForce;
        powerBar.value = 0;
    }

    void Update()
    {
        if (!thrown) 
        {
            //влево и вправо
            MoveBall();
            //индикатор силы
            BuildThrowForce();
            //бросок
            ThrowBall();
        }
        //остановка шара
        CheckBallStopped();
    }

    //движение шара влево и вправо до броска
    void MoveBall() 
    {
        float horisontal = Input.GetAxis("Horizontal");

        Vector3 pos = transform.position;
        pos.x += horisontal * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -4f, 4f);
        transform.position = pos;

        
    }

    //накопление силы броска при удержании Space
    void BuildThrowForce() 
    {
        if(Input.GetKey(KeyCode.Space)) 
        {
            currentForce += forceBuildSpeed * Time.deltaTime;

            currentForce = Mathf.Clamp(currentForce, 0, maxThrowForce);

            powerBar.value = currentForce;
        }
    }

    //выполнение броска
    void ThrowBall() 
    { 
        if(Input.GetKeyUp(KeyCode.Space)) 
        {
            GameManager.instance.StartThrow();
            rb.AddForce(Vector3.forward * currentForce, ForceMode.Impulse);

            thrown = true;

            waitingForReset = true;

        }
    }

    //возвращение шара на стартовую позицию
    public void ResetBall() 
    {
        rb.isKinematic = true;
        transform.position = startPosition;
        CameraFollow cam  = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
        {
            cam.ResetCameraInstant();
        }
        transform.rotation = Quaternion.identity;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        thrown = false;
        waitingForReset = false;
        returnScheduled = false;

        currentForce = 0f;
        stoppedTime = 0f;

        powerBar.value = 0;

        Invoke(nameof(EnablePhysics), 0.1f);
    }

    void EnablePhysics()
    {
        rb.isKinematic = false;
    }

    //проверка, остановился ли шар
    void CheckBallStopped()
    {
        if (!waitingForReset)
            return;
        if (rb.linearVelocity.magnitude < 0.4f)
        {
            stoppedTime += Time.deltaTime;

            if (stoppedTime >= 2f && !returnScheduled)
            {
                returnScheduled = true;
                waitingForReset = false;
                stoppedTime = 0f;

                Invoke(nameof(ReturnBall), 2f);
            }
        }
        else
        {
            stoppedTime = 0f;
        }
    }

    void ReturnBall()
    {
        ResetBall();

        GameManager.instance.BallReturned();
    }

    public void ForceReturn()
    {
        CancelInvoke();
        waitingForReset = false;
        Invoke(nameof(ReturnBall), 3f);
    }
}
