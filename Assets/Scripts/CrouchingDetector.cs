using UnityEngine;
using UnityEngine.UI;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;
using System;

public class CrouchingDetector : MonoBehaviour
{
    [SerializeField] Transform HMD_Camera;
    [SerializeField] JumpExecutor jumpExecutor;
    [SerializeField] MoveSpeedChanger moveSpeedChanger;
    [SerializeField] XROrigin XR_Origin;
    [SerializeField] MapManager mapManager;
    [SerializeField] Button readyButton;
    public float CrouchingBorder = 1.0f;
    public float playerCrouchingMoveSpeedMultiplier = 0.9f;
    private bool _isCrouched = false;
    private long _crouchStartTime;
    private bool _doCrouchingMove = false;
    private float _crouchingHeightOffset = 0;
    private float _standingBorder = 0.8f;
    private bool _isStandingHeightSet = false;
    private bool _isCrouchingHeightSet = false;

    public void SetStandingHeight()
    {
        XR_Origin.CameraYOffset += 1.6f - HMD_Camera.position.y;
        _isStandingHeightSet = true;
        CheckReady();
    }

    public void SetCrouchingHeight()
    {
        CrouchingBorder = (1.6f + HMD_Camera.position.y) / 2;
        _crouchingHeightOffset = HMD_Camera.position.y - 0.8f;
        _standingBorder = CrouchingBorder - _crouchingHeightOffset;
        _isCrouchingHeightSet = true;
        CheckReady();
    }

    public void ResetSettings()
    {
        _isStandingHeightSet = false;
        _isCrouchingHeightSet = false;
        XR_Origin.CameraYOffset = 1.2f;
        CrouchingBorder = 1.0f;
        _crouchingHeightOffset = 0;
        _standingBorder = 0.8f;
        CheckReady();
    }

    private void CheckReady()
    {
        readyButton.interactable = (_isStandingHeightSet && _isCrouchingHeightSet);
    }

    void Update()
    {
        if(!_isCrouched && HMD_Camera.position.y <= CrouchingBorder)
        {
            _isCrouched = true;
            _crouchStartTime = DateTime.Now.Ticks;
            moveSpeedChanger.MultiplyPlayerSpeed(playerCrouchingMoveSpeedMultiplier);
            XR_Origin.CameraYOffset -= _crouchingHeightOffset;
        }
        else if(_isCrouched)
        {
            if(HMD_Camera.position.y >= _standingBorder && mapManager.getCellState(HMD_Camera.position.x, HMD_Camera.position.z) != 3)
            {
                if(!_doCrouchingMove)
                {
                    jumpExecutor.Jump();
                }
                _isCrouched = false;
                _doCrouchingMove = false;
                moveSpeedChanger.DividePlayerSpeed(playerCrouchingMoveSpeedMultiplier);
                XR_Origin.CameraYOffset += _crouchingHeightOffset;
            }
            else if((new TimeSpan(DateTime.Now.Ticks - _crouchStartTime)).Seconds >= 2)
            {
                _doCrouchingMove = true;
            }
        }
    }
}