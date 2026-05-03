using UnityEngine;

public class RadialLayout : MonoBehaviour
{
    public float radius = 1.5f;

    void OnEnable()
    {
        Arrange();
    }

    public void Arrange()
    {
        int count = transform.childCount;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);

            float angle = i * angleStep * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(
                Mathf.Cos(angle),
                Mathf.Sin(angle),
                0f
            ) * radius;

            child.localPosition = pos;
        }
    }
}