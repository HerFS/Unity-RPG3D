using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * File     : InputManager.cs
 * Desc     : 플레이어 키 입력
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class InputManager : Singleton<InputManager>
{
    public Stack<GameObject> UIObject = new Stack<GameObject>();

    private PlayerInputs _playerInput;

    private Vector2 _moveVector;

    private bool _isLeftShift;
    private bool _isRun;
    private bool _isJump;
    private bool _isAttack;
    private bool _isPickup;
    private bool _isInteraction;

    public Vector2 MoveVector { get { return _moveVector; } set { _moveVector = value; } }

    public bool IsLeftShift { get { return _isLeftShift; } }
    public bool IsRun { get { return _isRun; } set { _isRun = value; } }
    public bool IsJump { get { return _isJump; } }
    public bool IsAttack { get { return _isAttack; } }
    public bool IsPickup { get { return _isPickup; } }
    public bool IsInteraction { get { return _isInteraction; } }

    protected override void Awake()
    {
        base.Awake();

        _playerInput = new PlayerInputs();
    }

    private void OnEnable()
    {
        _playerInput.PlayerActionMap.Enable();

        #region Setup Move
        _playerInput.PlayerActionMap.Move.Enable();

        _playerInput.PlayerActionMap.Move.performed += OnMove;
        _playerInput.PlayerActionMap.Move.canceled += OnMove;
        #endregion

        #region Setup Run
        _playerInput.PlayerActionMap.Run.Enable();

        _playerInput.PlayerActionMap.Run.started += OnRun;
        _playerInput.PlayerActionMap.Run.canceled += OnRun;
        #endregion

        #region Setup Jump
        _playerInput.PlayerActionMap.Jump.Enable();

        _playerInput.PlayerActionMap.Jump.started += OnJump;
        _playerInput.PlayerActionMap.Jump.canceled += OnJump;
        #endregion

        #region Setup Pickup
        _playerInput.PlayerActionMap.Pickup.Enable();

        _playerInput.PlayerActionMap.Pickup.started += OnPickup;
        _playerInput.PlayerActionMap.Pickup.canceled += OnPickup;
        #endregion

        #region Setup Attack
        _playerInput.PlayerActionMap.Attack.Enable();

        _playerInput.PlayerActionMap.Attack.started += OnAttack;
        _playerInput.PlayerActionMap.Attack.canceled += OnAttack;
        #endregion

        #region Setup Interaction
        _playerInput.PlayerActionMap.Interaction.Enable();

        _playerInput.PlayerActionMap.Interaction.started += OnInteraction;
        _playerInput.PlayerActionMap.Interaction.canceled += OnInteraction;
        #endregion

        #region Setup Inventory UI
        _playerInput.PlayerActionMap.InventoryUI.Enable();

        _playerInput.PlayerActionMap.InventoryUI.started += OnInventoryUI;
        #endregion

        #region Setup Status UI
        _playerInput.PlayerActionMap.StatusUI.Enable();

        _playerInput.PlayerActionMap.StatusUI.started += OnStatusUI;
        _playerInput.PlayerActionMap.StatusUI.canceled += OnStatusUI;
        #endregion

        #region Setup Status UI
        _playerInput.PlayerActionMap.EquipmentUI.Enable();

        _playerInput.PlayerActionMap.EquipmentUI.started += OnEquipmentUI;
        _playerInput.PlayerActionMap.EquipmentUI.canceled += OnEquipmentUI;
        #endregion

        #region Setup Quest View UI
        _playerInput.PlayerActionMap.QuestViewUI.Enable();

        _playerInput.PlayerActionMap.QuestViewUI.started += OnQuestViewUI;
        _playerInput.PlayerActionMap.QuestViewUI.canceled += OnQuestViewUI;
        #endregion

        #region Setup Achievement View UI
        _playerInput.PlayerActionMap.AchievementViewUI.Enable();

        _playerInput.PlayerActionMap.AchievementViewUI.started += OnAchievementViewUI;
        _playerInput.PlayerActionMap.AchievementViewUI.canceled += OnAchievementViewUI;
        #endregion

        #region Setup MouseControl
        _playerInput.PlayerActionMap.MouseControl.Enable();

        _playerInput.PlayerActionMap.MouseControl.started += OnMouseControl;
        _playerInput.PlayerActionMap.MouseControl.canceled += OnMouseControl;
        #endregion

        #region Setup DivideItem
        _playerInput.PlayerActionMap.DivideItem.Enable();

        _playerInput.PlayerActionMap.DivideItem.started += OnDivideItem;
        _playerInput.PlayerActionMap.DivideItem.canceled += OnDivideItem;
        #endregion
    }

    private void OnDisable()
    {
        Release();
    }

    private void OnApplicationQuit()
    {
        Release();
    }

    private void Release()
    {
        #region Release Move
        _playerInput.PlayerActionMap.Move.performed -= OnMove;
        _playerInput.PlayerActionMap.Move.canceled -= OnMove;

        _playerInput.PlayerActionMap.Move.Disable();
        #endregion

        #region Release Run

        _playerInput.PlayerActionMap.Run.started -= OnRun;
        _playerInput.PlayerActionMap.Run.canceled -= OnRun;

        _playerInput.PlayerActionMap.Run.Disable();
        #endregion

        #region Release Jump
        _playerInput.PlayerActionMap.Jump.Disable();

        _playerInput.PlayerActionMap.Jump.started -= OnJump;
        _playerInput.PlayerActionMap.Jump.canceled -= OnJump;
        #endregion

        #region Release Pickup
        _playerInput.PlayerActionMap.Pickup.Disable();

        _playerInput.PlayerActionMap.Pickup.started -= OnPickup;
        _playerInput.PlayerActionMap.Pickup.canceled -= OnPickup;
        #endregion

        #region ReleaseAttack
        _playerInput.PlayerActionMap.Attack.Disable();

        _playerInput.PlayerActionMap.Attack.started -= OnAttack;
        _playerInput.PlayerActionMap.Attack.canceled -= OnAttack;
        #endregion

        #region Release Interaction
        _playerInput.PlayerActionMap.Interaction.Disable();

        _playerInput.PlayerActionMap.Interaction.started -= OnInteraction;
        _playerInput.PlayerActionMap.Interaction.canceled -= OnInteraction;
        #endregion

        #region Release Inventory UI
        _playerInput.PlayerActionMap.InventoryUI.Disable();

        _playerInput.PlayerActionMap.InventoryUI.started -= OnInventoryUI;
        #endregion

        #region Release Status UI
        _playerInput.PlayerActionMap.StatusUI.Disable();

        _playerInput.PlayerActionMap.StatusUI.started -= OnStatusUI;
        _playerInput.PlayerActionMap.StatusUI.canceled -= OnStatusUI;
        #endregion

        #region Release Status UI
        _playerInput.PlayerActionMap.EquipmentUI.Disable();

        _playerInput.PlayerActionMap.EquipmentUI.started -= OnEquipmentUI;
        _playerInput.PlayerActionMap.EquipmentUI.canceled -= OnEquipmentUI;
        #endregion

        #region Release Quest View UI
        _playerInput.PlayerActionMap.QuestViewUI.Disable();

        _playerInput.PlayerActionMap.QuestViewUI.started -= OnQuestViewUI;
        _playerInput.PlayerActionMap.QuestViewUI.canceled -= OnQuestViewUI;
        #endregion

        #region Release Achievement View UI
        _playerInput.PlayerActionMap.AchievementViewUI.Disable();

        _playerInput.PlayerActionMap.AchievementViewUI.started -= OnAchievementViewUI;
        _playerInput.PlayerActionMap.AchievementViewUI.canceled -= OnAchievementViewUI;
        #endregion

        #region Release MouseControl
        _playerInput.PlayerActionMap.MouseControl.Disable();

        _playerInput.PlayerActionMap.MouseControl.started -= OnMouseControl;
        _playerInput.PlayerActionMap.MouseControl.canceled -= OnMouseControl;
        #endregion

        #region Release DivideItem
        _playerInput.PlayerActionMap.DivideItem.Disable();

        _playerInput.PlayerActionMap.DivideItem.started -= OnDivideItem;
        _playerInput.PlayerActionMap.DivideItem.canceled -= OnDivideItem;
        #endregion
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _moveVector = context.ReadValue<Vector2>();
        }

        if (context.canceled)
        {
            _moveVector = Vector2.zero;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isRun = true;
        }

        if (context.canceled)
        {
            _isRun = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isJump = true;
        }

        if (context.canceled)
        {
            _isJump = false;
        }
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isPickup = true;
        }

        if (context.canceled)
        {
            _isPickup = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isAttack = true;
        }

        if (context.canceled)
        {
            _isAttack = false;
        }
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isInteraction = true;
        }

        if (context.canceled)
        {
            _isInteraction = false;
        }
    }

    public void OnInventoryUI(InputAction.CallbackContext context)
    {
        GameObject inventoryUIPanel = UIManager.Instance.InventoryPanel;

        if (context.started)
        {
            if (inventoryUIPanel.activeSelf)
            {
                inventoryUIPanel.SetActive(false);
                if (UIObject.Count != 0)
                {
                    UIObject.Pop();
                }
                UIManager.Instance.ItemToolTipPanel.HideToolTip();
                UIManager.Instance.ItemPopupPanel.HideThrowUI();
                UIManager.Instance.ItemPopupPanel.HideQuantityUI();
            }
            else
            {
                inventoryUIPanel.SetActive(true);
                UIObject.Push(inventoryUIPanel);
                GameManager.Instance.ShowMouse();
            }

            GameManager.Instance.HideMouse();
        }
    }
    
    public void OnStatusUI(InputAction.CallbackContext context)
    {
        GameObject statusUIObject = UIManager.Instance.StatusPanel;

        if (context.started)
        {
            if (statusUIObject.activeSelf)
            {
                statusUIObject.SetActive(false);
                if (UIObject.Count != 0)
                {
                    UIObject.Pop();
                }
            }
            else
            {
                statusUIObject.SetActive(true);
                UIObject.Push(statusUIObject);
                GameManager.Instance.ShowMouse();
            }

            GameManager.Instance.HideMouse();
        }
    }

    public void OnEquipmentUI(InputAction.CallbackContext context)
    {
        GameObject equipmentUIObject = UIManager.Instance.EquipmentPanel.gameObject;

        if (context.started)
        {
            if (equipmentUIObject.activeSelf)
            {
                equipmentUIObject.SetActive(false);
                if (UIObject.Count != 0)
                {
                    UIObject.Pop();
                }
                UIManager.Instance.ItemToolTipPanel.HideToolTip();
            }
            else
            {
                equipmentUIObject.SetActive(true);
                UIObject.Push(equipmentUIObject);
                GameManager.Instance.ShowMouse();
            }

            GameManager.Instance.HideMouse();
        }
    }

    public void OnQuestViewUI(InputAction.CallbackContext context)
    {
        GameObject questUIObject = UIManager.Instance.QuestViewPanel.gameObject;

        if (context.started)
        {
            if (questUIObject.activeSelf)
            {
                questUIObject.SetActive(false);
                if (UIObject.Count != 0)
                {
                    UIObject.Pop();
                }
            }
            else
            {
                questUIObject.SetActive(true);
                UIObject.Push(questUIObject);
                GameManager.Instance.ShowMouse();
            }

            GameManager.Instance.HideMouse();
        }
    }

    public void OnAchievementViewUI(InputAction.CallbackContext context)
    {
        GameObject achievementUIObject = UIManager.Instance.AchievementViewPanel.gameObject;

        if (context.started)
        {
            if (achievementUIObject.activeSelf)
            {
                achievementUIObject.SetActive(false);

                if (UIObject.Count != 0)
                {
                    UIObject.Pop();
                }
            }
            else
            {
                achievementUIObject.SetActive(true);
                UIObject.Push(achievementUIObject);
                GameManager.Instance.ShowMouse();
            }

            GameManager.Instance.HideMouse();
        }
    }

    public void OnMouseControl(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Cursor.visible)
            {
                GameManager.Instance.HideMouse();
            }
            else
            {
                GameManager.Instance.ShowMouse();
            }
        }
    }

    public void OnDivideItem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (context.started)
            {
                _isLeftShift = true;
            }

            if (context.canceled)
            {
                _isLeftShift = false;
            }
        }
    }

    public void PopUIObject()
    {
        if (UIObject.Count > 0)
        {
            UIObject.Pop();
        }
    }
}
