using UnityEngine;
using UnityEngine.Assertions;

public class ScaleWithCamera : MonoBehaviour
{
    [SerializeField]
    private int pixelsPerUnit = 32;
    [SerializeField]
    private int verticalNum = 7;
    private float finalUnit;
    private new Camera camera;

    void Awake()
    {
        camera = gameObject.GetComponent<Camera>();

        SetOrthographicSize();
    }

    void SetOrthographicSize()
    {
        var tempUnitSize = Screen.height / verticalNum;

        finalUnit = GetNearestMultiple(tempUnitSize, pixelsPerUnit);

        camera.orthographicSize = Screen.height / (finalUnit * 2.0f);
    }

    int GetNearestMultiple(int value, int _pixel)
    {
        int rem = value % _pixel;
        int result = value - rem;
        if (rem > (_pixel / 2))
            result += _pixel;

        return result;
    }
}