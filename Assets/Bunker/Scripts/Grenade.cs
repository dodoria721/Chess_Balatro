using UnityEngine;

public class Grenade : MonoBehaviour, IProjectile
{
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float flightTime;
    private float elapsedTime;
    private float arcHeight;

    public void Init(Vector3 start, Vector3 target, float time, float height)
    {
        startPosition = start;
        targetPosition = target;
        flightTime = time;
        arcHeight = height;
        elapsedTime = 0;
    }

    void Update()
    {
        if (elapsedTime >= flightTime)
        {
            gameObject.SetActive(false);
            return;
        }

        float t = elapsedTime / flightTime;

        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, t);
        currentPos.y += arcHeight * Mathf.Sin(t * Mathf.PI);

        transform.position = currentPos;

        elapsedTime += Time.deltaTime;
    }
}
