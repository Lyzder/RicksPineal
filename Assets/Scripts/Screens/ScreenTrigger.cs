using UnityEngine;

public class ScreenTrigger : MonoBehaviour
{
    public ScreenData screenData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //ScreenTransitionManager.Instance.MoveToScreen(screenData);
        }
    }
}
