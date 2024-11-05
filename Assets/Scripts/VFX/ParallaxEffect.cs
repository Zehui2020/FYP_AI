using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private Vector2 startPos;

    [SerializeField] private bool updateY;
    [SerializeField] private Transform followPos;
    [SerializeField] private Transform player;
    [SerializeField] private float parallaxEffect;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private string levelName;
    [SerializeField] private string filename;
    private ImageSaver imageSaver;

    private float mapOriginY;
    private float mapSizeY;

    [SerializeField] private float YDiff;

    public void InitParallaxEffect(float mapSizeY)
    {
        this.mapSizeY = mapSizeY;

        mapOriginY = mapSizeY / 2f;
        transform.position = player.transform.position;
        startPos = transform.position;
    }

    private void Awake()
    {
        imageSaver = GetComponent<ImageSaver>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (levelName == string.Empty)
            spriteRenderer.sprite = imageSaver.GetSpriteFromLocalDisk(filename + "_" + GameData.Instance.currentLevel);
        else
            spriteRenderer.sprite = imageSaver.GetSpriteFromLocalDisk(filename + "_" + levelName);
    }

    private void LateUpdate()
    {
        // Parallax X
        float dist = followPos.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPos.x - dist, transform.position.y, transform.position.z);

        // Parallax Y
        if (mapSizeY == 0 || !updateY)
            return;

        float diff = transform.position.y - mapOriginY;
        float offsetY = Mathf.Abs(transform.position.y - mapSizeY) / mapSizeY * YDiff;
        offsetY = diff < 0 ? offsetY * parallaxEffect : -offsetY * parallaxEffect;
        offsetY = Mathf.Clamp(offsetY, -YDiff, YDiff);
        transform.position = new Vector3(transform.position.x, followPos.transform.position.y + offsetY, transform.position.z);
    }
}