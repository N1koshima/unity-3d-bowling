using UnityEngine;

public class Pin : MonoBehaviour
{
    //Флаг чтобы кегля не засчиталась несколько раз
    private bool counted = false;
    private float fallenTimer = 0f;

    //сбита ли кегля
    void Update()
    {
        if (counted)
            return;
        float angle = Vector3.Angle(transform.up, Vector3.up);
        //угол
        if (angle > 25f)
        {
            fallenTimer += Time.deltaTime;
            if (fallenTimer >= 0.3f)
            {
                counted = true;
                GameManager.instance.PinDestroyed();
                Destroy(gameObject, 0.3f);
            }
        }
        else
        {

            fallenTimer = 0f;
        }
    }
}
