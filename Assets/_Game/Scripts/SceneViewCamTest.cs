using UnityEngine;

public class SceneViewCamTest : MonoBehaviour
{
    void Update()
    {
        Vector2 DELTARoll = 20f * 1f * Time.deltaTime * Vector3.up;
        transform.localRotation *= Quaternion.Euler(DELTARoll);
    }
}
