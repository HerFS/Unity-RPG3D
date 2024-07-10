using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * File     : ItemToolTipUI.cs
 * Desc     : Item의 정보를 알려주는 UI
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class ItemToolTipUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _titleText;
    [SerializeField]
    private TextMeshProUGUI _descriptionText;
    [HideInInspector]
    public RectTransform MyRectTransform;

    private void Awake()
    {
        MyRectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        HideToolTip();
    }

    private void Update()
    {
        Rect rect = MyRectTransform.rect;

        Vector2 leftBottom = transform.TransformPoint(rect.min);
        Vector2 rightTop = transform.TransformPoint(rect.max);
        Vector2 uiSize = rightTop - leftBottom;

        rightTop = new Vector2(Screen.width, Screen.height) - uiSize;

        float x = Mathf.Clamp(leftBottom.x, 0, rightTop.x);
        float y = Mathf.Clamp(leftBottom.y, 0, rightTop.y);

        Vector2 offset = (Vector2)transform.position - leftBottom;
        
        transform.position = new Vector2(x, y) + offset;
    }

    public void UpdateToolTip(ItemData data)
    {
        _titleText.text = data.Name;
        _descriptionText.text = $"{data.Description} \n 판매 가격 : {data.SalePrice}";
    }

    public void ShowToolTip()
    {
        this.gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        this.gameObject.SetActive(false);
    }
}
