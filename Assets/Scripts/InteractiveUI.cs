using UnityEngine;

public class InteractiveUI : MonoBehaviour
{
    [SerializeField] GameObject SpawnButton;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.intent == "tile")
        {
            SpawnButton.SetActive(true);
        }
        else
        {
            SpawnButton.SetActive(false);
        }
    }
}
