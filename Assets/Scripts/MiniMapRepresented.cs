using UnityEngine;
using UnityEngine.UI;

public class MiniMapRepresented : MonoBehaviour
{
    private GameState _gameState;
    public Sprite miniMapSprite;
    [SerializeField] private GameObject _iconObj;
    private RectTransform _rect;

    private void Awake()
    {
        _gameState = FindFirstObjectByType<GameState>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnMiniMapIcon();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = Objective.Vec3ToLongLat(transform.position);
        float lon = pos.x;
        float lat = pos.y;
        // print("lat:" + lat + "lon:" + lon);

        lon = ((lon * -1 + 90+360) % 360) / 360f;
        lat = lat / 180f;

        float x = (lon - .5f) * _gameState.miniMapTransform.rect.width;
        float y = (lat) * _gameState.miniMapTransform.rect.height;

        _rect.anchoredPosition = new Vector2(x, y);
    }


    private void SpawnMiniMapIcon()
    {
        _iconObj = Instantiate(_gameState.prefabMiniMapIcon, _gameState.miniMapTransform);
        Image image = _iconObj.GetComponent<Image>();
        image.sprite = miniMapSprite;
        _rect = _iconObj.GetComponent<RectTransform>();
    }
}