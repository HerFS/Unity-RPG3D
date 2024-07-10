using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * File     : EqipmentIcon.cs
 * Desc     : Equipment에서 Item의 Icon을 관리
 * Date     : 2024-05-30
 * Writer   : 정지훈
 */

public class EquipmentIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private ItemToolTipUI _itemToolTipUI;
    public Transform ParentAfterDrag;

    private int _itmeSlotCount;

    public Image ItemImage;

    private Texture2D _handCursor;
    private Texture2D _originalCursor;


    private void Awake()
    {
        #region InitValues
        ItemImage = GetComponent<Image>();

        ParentAfterDrag = this.transform.parent;
        #endregion

        #region Cursor
        _handCursor = Resources.Load<Texture2D>(Globals.CursorPath.Hand);
        _originalCursor = Resources.Load<Texture2D>(Globals.CursorPath.Original);
        #endregion
    }

    private void Start()
    {
        _itmeSlotCount = DataManager.Instance.Inventory.ItemSlots.Length;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown((int)EnumTypes.MouseButton.Right))
        {
            EquipmentSlot parentEquipmentSlot = ParentAfterDrag.GetComponent<EquipmentSlot>();
            
            for (int i = 0; i < _itmeSlotCount; ++i)
            {
                if (!DataManager.Instance.Inventory.ItemSlots[i].HasItem)
                {
                    DataManager.Instance.Inventory.ItemSlots[i].Item = parentEquipmentSlot.EquipmentItem;
                    DataManager.Instance.Inventory.ItemSlots[i].ItemQuantity = 1;
                    DataManager.Instance.Inventory.ItemSlots[i].UpdateIcon();
                    parentEquipmentSlot.ClearSlot();
                    break;
                }
            }
        }
        else
        {
            Cursor.SetCursor(_handCursor, new Vector2((_handCursor.width / 3f), 0f), CursorMode.Auto);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Cursor.SetCursor(_originalCursor, new Vector2(0f, 0f), CursorMode.Auto);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton((int)EnumTypes.MouseButton.Left))
        {
            this.transform.position = Input.mousePosition;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ParentAfterDrag = this.transform.parent;
        this.transform.SetParent(transform.root);
        this.transform.SetAsLastSibling();
        ItemImage.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ParentAfterDrag.GetComponent<EquipmentSlot>().IsEquipped)
        {
            ItemImage.raycastTarget = true;
        }

        this.transform.SetParent(ParentAfterDrag);
    }
}
