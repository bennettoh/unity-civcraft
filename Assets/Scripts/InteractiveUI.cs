using UnityEngine;

public class InteractiveUI : MonoBehaviour
{
    [SerializeField] GameObject SpawnButton;
    [SerializeField] GameObject BuildButton;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.intent == "tile")
        {
            setVisibility(true);
        }
        else
        {
            setVisibility(false);
        }
    }

    private void setVisibility(bool visible)
    {
        if (visible)
        {
            BuildButton.SetActive(true);
            SpawnButton.SetActive(true);
        }
        else
        {
            BuildButton.SetActive(false);
            SpawnButton.SetActive(false);
        }

    }
}
