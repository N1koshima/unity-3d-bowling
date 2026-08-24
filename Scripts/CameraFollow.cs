using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //скрипт камеры
    //следование шара до броска
    //возле кеглей переход в кинематографический режим
    public Transform target;

    public float smoothSpeed = 5f;

    public Vector3 followOffset = new Vector3(0, 6, -12);
    public Vector3 cinematicPosition = new Vector3(0, 12, 8);
    public float transitionZ = 12f;

    private Vector3 startPosition;
    private quaternion startRotation;
    private bool cinematicMode = false;


    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    //Переход в кинематографический режим возле кеглей
    void LateUpdate()
    {
        if (!cinematicMode)
        {
            //камера следует за шаром
            Vector3 desiredPosition = new Vector3(target.position.x, followOffset.y, target.position.z + followOffset.z);
            //перемещение камеры
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.LookAt(target.position + Vector3.up * 1.5f);
            if (target.position.z > transitionZ)
            {
                cinematicMode = true;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, cinematicPosition, smoothSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(0, 2, 12));
        }
    }

    //возврат камеры
    public void ResetCameraInstant()
    {
        cinematicMode = false;
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
