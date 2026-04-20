using UnityEngine;
using UnityEngine.UI;

public class MiniMapRepresented : MonoBehaviour
{
    private enum RepresentedState
    {
        Hidden,
        Unexplored,
        Explored,
        Dynamic
    }

    private GameState _gameState;
    private Image _image;
    private SatelliteSelector _satelliteSelector;
    private RepresentedState _state = RepresentedState.Hidden;

    public Sprite miniMapHiddenSprite;
    public Sprite miniMapSprite;

    [SerializeField] private GameObject _iconObj;
    private RectTransform _rect;

    private void Awake()
    {
        _gameState = FindFirstObjectByType<GameState>();
        SpawnMiniMapIcon();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SpawnMiniMapIcon();
    }

    // Update is called once per frame
    void Update()
    {
        if (_state == RepresentedState.Dynamic)
        {
            UpdatePos();
        }
    }

    private void UpdatePos()
    {
        Vector2 pos = Objective.Vec3ToLongLat(transform.position);
        float lon = pos.x;
        float lat = pos.y;
        // print("lat:" + lat + "lon:" + lon);

        lon = ((lon * -1 + 90 + 360) % 360) / 360f;
        lat = lat / 180f;

        float x = (lon - .5f) * _gameState.miniMapTransform.rect.width;
        float y = (lat) * _gameState.miniMapTransform.rect.height;

        _rect.anchoredPosition = new Vector2(x, y);
    }
    
    private void SpawnMiniMapIcon()
    {
        _iconObj = Instantiate(_gameState.prefabMiniMapIcon, _gameState.miniMapTransform);
        _image = _iconObj.GetComponent<Image>();
        _satelliteSelector = _iconObj.GetComponent<SatelliteSelector>();
        _image.gameObject.SetActive(false);
        _image.sprite = miniMapSprite;
        _rect = _iconObj.GetComponent<RectTransform>();
        UpdatePos();
    }

    public void Unexplored()
    {
        _image.sprite = miniMapHiddenSprite;
        _image.gameObject.SetActive(true);
        _state = RepresentedState.Unexplored;
    }
    
    public void Explored()
    {
        _image.sprite = miniMapSprite;
        _image.gameObject.SetActive(true);
        _state = RepresentedState.Explored;
    }
    
    public void Dynamic()
    {
        _image.sprite = miniMapSprite;
        _image.gameObject.SetActive(true);
        _state = RepresentedState.Dynamic;
    }

    public void SetSatellite(SatelliteInstance sat)
    {
        if (_satelliteSelector != null) _satelliteSelector.sat = sat;
    }
}