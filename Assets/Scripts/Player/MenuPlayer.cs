using UnityEngine;

public class MenuPlayer : MonoBehaviour
{
    [SerializeField] private Transform holdButtonPos;
    [SerializeField] private Transform launchButtonPos;

    private WorldSpaceButton currentButton;
    public WorldSpaceButton heldButton;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && heldButton != null)
        {
            heldButton.ReleaseButton();
            heldButton = null;
        }
        else if (Input.GetKeyDown(KeyCode.E) && currentButton != null)
        {
            heldButton = currentButton;
            currentButton.PickupButton(holdButtonPos);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent<WorldSpaceButton>(out currentButton);

        if (collision.CompareTag("Pipe") && heldButton != null)
            heldButton.LaunchButton(launchButtonPos);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.TryGetComponent<WorldSpaceButton>(out currentButton);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<WorldSpaceButton>(out currentButton))
            currentButton = null;
    }
}