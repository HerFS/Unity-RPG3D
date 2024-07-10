using UnityEngine;
using UnityEngine.EventSystems;

/*
 * File     : MovableUI.cs
 * Desc     : UI를 드래그 하여 움직임
 * Date     : 2024-05-07
 * Writer   : 정지훈
 */

public class MovableUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform _rectTr;

    private Texture2D _handCursor;
    private Texture2D _originalCursor;

    [SerializeField]
    private Transform _clickUI;

    private void Awake()
    {
        _rectTr = GetComponent<RectTransform>();
        _handCursor = Resources.Load<Texture2D>(Globals.CursorPath.Hand);
        _originalCursor = Resources.Load<Texture2D>(Globals.CursorPath.Original);
    }

    private void Update()
    {
        Rect rect = _rectTr.rect;

        Vector2 leftBottom = _clickUI.TransformPoint(rect.min);
        Vector2 rightTop = _clickUI.TransformPoint(rect.max);
        Vector2 uiSize = rightTop - leftBottom;

        rightTop = new Vector2(Screen.width, Screen.height) - uiSize;

        float x = Mathf.Clamp(leftBottom.x, 0, rightTop.x);
        float y = Mathf.Clamp(leftBottom.y, 0, rightTop.y);

        Vector2 offset = (Vector2)_clickUI.position - leftBottom;

        _clickUI.position = new Vector2(x, y) + offset;

    }

    public void OnDrag(PointerEventData eventData)
    {
        _clickUI.transform.position = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Cursor.SetCursor(_handCursor, new Vector2(0f, 0f), CursorMode.Auto);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Cursor.SetCursor(_originalCursor, new Vector2(0f, 0f), CursorMode.Auto);
    }
}
