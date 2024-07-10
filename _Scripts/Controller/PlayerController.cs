using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : PlayerController.cs
 * Desc     : 플레이어의 움직임
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class PlayerController : DontDestroyObject<PlayerController>, IDamageable
{
    private Rigidbody _myRigid;
    private bool _grounded = false;
    private int _groundLayerMask;

    [SerializeField]
    private Transform _playerBody;
    [SerializeField]
    private Transform _cameraArm;
    [SerializeField]
    private Transform _camera;
    [SerializeField]
    private Transform _footPos;

    private Quaternion _originRotation;

    protected override void Awake()
    {
        base.Awake();

        _myRigid = GetComponent<Rigidbody>();
        _groundLayerMask = 1 << (int)EnumTypes.LayerIndex.Ground;
    }

    private void Start()
    {
        _originRotation = _camera.localRotation;
    }

    private void Update()
    {
        //Vector3 direction = (this.transform.position - _camera.position).normalized;
        //RaycastHit[] hits = Physics.RaycastAll(_camera.position, direction, Mathf.Infinity, 1 << (int)EnumTypes.LayerIndex.Ground);

        //foreach (var hit in hits)
        //{

        //}
        Debug.DrawRay(_camera.position, this.transform.position, Color.red);
    }

    private void FixedUpdate()
    {
        #region Move
        Vector2 moveInput = InputManager.Instance.MoveVector;
        bool isMove = (moveInput.magnitude != 0f);

        if (isMove && PlayerEntity.StateMachine.CurrentState != PlayerEntity.States[(int)EnumTypes.PlayerState.Die])
        {
            _myRigid.constraints = RigidbodyConstraints.FreezeRotation;

            Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z);
            Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z);
            Vector3 moveDir = (lookForward * moveInput.y) + (lookRight * moveInput.x);

            if (PlayerEntity.StateMachine.CurrentState == PlayerEntity.States[(int)EnumTypes.PlayerState.Run])
            {
                _myRigid.velocity = new Vector3(moveDir.x * DataManager.Instance.PlayerStatus.RunSpeed, _myRigid.velocity.y, moveDir.z * DataManager.Instance.PlayerStatus.RunSpeed);
            }
            else
            {
                _myRigid.velocity = new Vector3(moveDir.x * DataManager.Instance.PlayerStatus.WalkSpeed, _myRigid.velocity.y, moveDir.z * DataManager.Instance.PlayerStatus.WalkSpeed);
            }

            _playerBody.forward = moveDir;
        }
        else
        {
            _myRigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
        #endregion

        #region Jump
        if (Physics.Raycast(_footPos.position, Vector3.down, 0.2f, _groundLayerMask))
        {
            _grounded = true;
        }
        else
        {
            _grounded = false;
        }

        if (InputManager.Instance.IsJump && _grounded)
        {
            _myRigid.velocity = Vector3.up * DataManager.Instance.PlayerStatus.JumpHeight;
        }
        #endregion
    }

    private void LateUpdate()
    {
        #region LookARound

        if (!Cursor.visible)
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            Vector3 cameraAngles = _cameraArm.rotation.eulerAngles; // Quternion => euler
            float cameraAngleX = (cameraAngles.x - mouseDelta.y);

            // 180 보다 작으면 위쪽으로 회전하는 경우
            if (cameraAngleX < 180f)
            {
                cameraAngleX = Mathf.Clamp(cameraAngleX, -1f, 50f);
            }
            else
            {
                cameraAngleX = Mathf.Clamp(cameraAngleX, 300f, 361f);
            }

            _cameraArm.rotation = Quaternion.Euler(cameraAngleX, cameraAngles.y + mouseDelta.x, transform.rotation.z);
        }
        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.MonsterWeapon)&& !DataManager.Instance.PlayerStatus.IsHit)
        {
            MonsterStatus monsterStatus = other.GetComponentInParent<MonsterStatus>();

            float monsterDamage = monsterStatus.AttackDamage;
            float monsterCriticalDamage = monsterStatus.CriticalDamage;

            float ciriticalChance = Random.Range(0f, 101f);

            if (monsterStatus.CriticalChance >= ciriticalChance)
            {
                DataManager.Instance.PlayerStatus.CurrentHp -= HitDamageCalculation(monsterCriticalDamage);
            }
            else
            {
                DataManager.Instance.PlayerStatus.CurrentHp -= HitDamageCalculation(monsterDamage);
            }
        }
    }

    public IEnumerator Hit()
    {
        float timer = 0f;
        float magnitudeRot = 100f;

        DataManager.Instance.PlayerStatus.IsHit = true;
        UIManager.Instance.BloodUI.SetActive(true);
        while (timer < DataManager.Instance.PlayerStatus.InvincibilityTime)
        {
            Vector3 shakeRotate = new Vector3(_camera.localEulerAngles.x, _camera.localEulerAngles.z, Mathf.PerlinNoise(Time.time * magnitudeRot, 0f));
            Camera.main.transform.localRotation = Quaternion.Euler(shakeRotate);

            timer += Time.deltaTime;
            yield return null;
        }

        _camera.localRotation = _originRotation;

        DataManager.Instance.PlayerStatus.IsHit = false;
        UIManager.Instance.BloodUI.SetActive(false);
    }

    public float HitDamageCalculation(float damage)
    {
        float hitDamage = 0f;

        StartCoroutine(Hit());

        if (!(DataManager.Instance.PlayerStatus.Defense > damage))
        {
            hitDamage = (damage - (DataManager.Instance.PlayerStatus.Defense / 2));
        }
        else
        {
            hitDamage = 1;
        }

        return hitDamage;
    }
}
