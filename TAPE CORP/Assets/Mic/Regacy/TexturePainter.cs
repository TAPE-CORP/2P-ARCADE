using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class TexturePainter : MonoBehaviour
{
    [Header("페인트 설정")]
    [Tooltip("칠할 색상")]
    public Color paintColor = Color.red;
    [Range(0f, 1f), Tooltip("브러시 최대 불투명도")]
    public float brushAlpha = 0.5f;

    [Header("페이드 설정")]
    [Tooltip("페인트가 완전히 사라지기까지 걸리는 시간 (초)")]
    public float fadeDuration = 2f;

    [Header("흩뿌리기 설정")]
    [Tooltip("흩뿌릴 점 개수")]
    public int splatterCount = 50;
    [Tooltip("흩뿌리기 한 점당 반경 (픽셀 단위)")]
    public int splatterRadiusPx = 2;

    private SpriteRenderer _sr;
    private Collider2D _col;
    private Texture2D _tex;
    private Vector2Int _texSizePx;
    private Bounds _bounds;
    private Color[] _originalPixels;
    private float[] _overlayAlphas;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();
        _bounds = _sr.bounds;

        // 읽기/쓰기 가능한 RGBA32 텍스처 생성
        var srcTex = _sr.sprite.texture;
        var rect = _sr.sprite.textureRect;
        _texSizePx = new Vector2Int((int)rect.width, (int)rect.height);
        var readable = new Texture2D(_texSizePx.x, _texSizePx.y, TextureFormat.RGBA32, false);

        // 원본 영역 복사
        var pix = srcTex.GetPixels((int)rect.x, (int)rect.y, _texSizePx.x, _texSizePx.y);
        readable.SetPixels(pix);
        readable.Apply();

        // 스프라이트 교체
        _sr.sprite = Sprite.Create(
            readable,
            new Rect(0, 0, _texSizePx.x, _texSizePx.y),
            new Vector2(0.5f, 0.5f),
            _sr.sprite.pixelsPerUnit
        );

        _tex = readable;
        _originalPixels = _tex.GetPixels();
        _overlayAlphas = new float[_originalPixels.Length];
    }

    void Update()
    {
        bool dirty = false;
        float delta = Time.deltaTime / fadeDuration;
        var newPixels = new Color[_originalPixels.Length];

        for (int i = 0; i < _overlayAlphas.Length; i++)
        {
            if (_overlayAlphas[i] > 0f)
            {
                _overlayAlphas[i] = Mathf.Max(_overlayAlphas[i] - delta, 0f);
                dirty = true;
            }
            newPixels[i] = Color.Lerp(_originalPixels[i], paintColor, _overlayAlphas[i]);
        }

        if (dirty)
        {
            _tex.SetPixels(newPixels);
            _tex.Apply();
        }
    }

    /// <summary>
    /// 파동이 닿은 영역 내에서만, 스프라이트의 가장자리 픽셀에만 페인트가 흩뿌려집니다.
    /// </summary>
    public void PaintSplatterOnEdges(Vector2 centerWorld, float radiusWorld)
    {
        for (int i = 0; i < splatterCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * radiusWorld;
            Vector2 worldPoint = centerWorld + offset;

            if (!_col.OverlapPoint(worldPoint))
                continue;

            float u = (worldPoint.x - _bounds.min.x) / _bounds.size.x;
            float v = (worldPoint.y - _bounds.min.y) / _bounds.size.y;

            int cx = Mathf.RoundToInt(u * (_texSizePx.x - 1));
            int cy = Mathf.RoundToInt(v * (_texSizePx.y - 1));

            // 가장자리 픽셀인지 확인
            int idxCenter = cy * _texSizePx.x + cx;
            if (_originalPixels[idxCenter].a <= 0f)
                continue;

            bool isEdge = false;
            // 4-방향 이웃 검사
            var dirs = new Vector2Int[] {
                new Vector2Int(1,0), new Vector2Int(-1,0),
                new Vector2Int(0,1), new Vector2Int(0,-1)
            };
            foreach (var d in dirs)
            {
                int nx = cx + d.x, ny = cy + d.y;
                if (nx < 0 || nx >= _texSizePx.x || ny < 0 || ny >= _texSizePx.y)
                {
                    isEdge = true;
                    break;
                }
                int nidx = ny * _texSizePx.x + nx;
                if (_originalPixels[nidx].a <= 0f)
                {
                    isEdge = true;
                    break;
                }
            }
            if (!isEdge)
                continue;

            // 에지 픽셀 주변 작은 원형 브러시 적용
            for (int oy = -splatterRadiusPx; oy <= splatterRadiusPx; oy++)
            {
                for (int ox = -splatterRadiusPx; ox <= splatterRadiusPx; ox++)
                {
                    int px = cx + ox, py = cy + oy;
                    if (px < 0 || px >= _texSizePx.x || py < 0 || py >= _texSizePx.y)
                        continue;
                    if (ox * ox + oy * oy > splatterRadiusPx * splatterRadiusPx)
                        continue;

                    int idx = py * _texSizePx.x + px;
                    float a = Random.Range(brushAlpha * 0.5f, brushAlpha);
                    _overlayAlphas[idx] = Mathf.Max(_overlayAlphas[idx], a);
                }
            }
        }
    }
}
