using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;
using System;
using System.Reflection;

public class Swim : MonoBehaviour {
    private FirstPersonController _firstPersonController;
    private CharacterController _characterController;
    private Blur _blur;
    private Color _fogColorWater;

    private float _defWalkSpeed, _defJumpSpeed, _defRunSpeed, _defGravityMultiplier;
    private FogMode _defFogMode;
    private float _defFogDensity;
    private Color _defFogColor;
    private bool _defFogEnabled;
    private Camera _camera;
    private bool _isInWater = false;
    private float _waterSurfacePosY = 5.0f;
    public float _aboveWaterTolerance = 0.5f;
    [Range(0.5f, 3.0f)]
    public float _upDownSpeed = 1.0f;

	// Use this for initialization
	void Start () {
        _firstPersonController = GetComponent<FirstPersonController>();
        _characterController = GetComponent<CharacterController>();
        _fogColorWater = new Color(0.2f, 0.65f, 0.75f, 0.5f);
        Transform fpChar = transform.FindChild("FirstPersonCharacter");
        _blur = fpChar.GetComponent<Blur>();
        _camera = fpChar.GetComponent<Camera>();

        _defWalkSpeed = _firstPersonController.m_WalkSpeed;
        _defRunSpeed = _firstPersonController.m_RunSpeed;
        _defJumpSpeed = _firstPersonController.m_JumpSpeed;
        _defGravityMultiplier = _firstPersonController.m_GravityMultiplier;

        _defFogMode = RenderSettings.fogMode;
        _defFogDensity = RenderSettings.fogDensity;
        _defFogColor = RenderSettings.fogColor;
        _defFogEnabled = RenderSettings.fog;
	}
	
	// Update is called once per frame
	void Update () {

        if(IsUnderWater())
        {
            SetRenderDiving();
        }
        else
        {
            SetRenderDefault();
        }

        if(_isInWater)
        {
            if(IsUnderWater())
            {
                DoDiving();
            }
            else
            {
                if(_characterController.isGrounded)
                {
                    DoWalking();
                }
                else
                {
                    HandleUpDownMovement();
                }
            }
            
        }
        else
        {
            DoWalking();
        }
    }
    private bool IsUnderWater()
    {
        return _camera.gameObject.transform.position.y < (_waterSurfacePosY);
    }
    private void DoWalking()
    {
        _firstPersonController.m_StickToGroundForce = 10.0f;
        _firstPersonController.m_WalkSpeed = Mathf.Lerp(_firstPersonController.m_WalkSpeed, _defWalkSpeed, Time.deltaTime * 3.0f);
        _firstPersonController.m_RunSpeed = Mathf.Lerp(_firstPersonController.m_RunSpeed, _defRunSpeed, Time.deltaTime * 3.0f);
        _firstPersonController.m_JumpSpeed = _defJumpSpeed;
        _firstPersonController.m_GravityMultiplier = _defGravityMultiplier;
        _firstPersonController.m_UseHeadBob = true;
       // _firstPersonController.m_AudioSource.Play();

    }
    private void DoDiving()
    {
        _firstPersonController.m_WalkSpeed = 1.0f;
        _firstPersonController.m_RunSpeed = 2.0f;
        _firstPersonController.m_JumpSpeed = 0.0f;
        _firstPersonController.m_UseHeadBob = false;
        _firstPersonController.m_AudioSource.Stop();
        HandleUpDownMovement();

    }
    private void HandleUpDownMovement()
    {
        _firstPersonController.m_StickToGroundForce = 0.0f;
        _firstPersonController.m_GravityMultiplier = 0.1f;
        Vector3 mv = _firstPersonController.m_MoveDir;
        
        if(Input.GetKey(KeyCode.E))
        {
            //upwards in water
            if(_camera.gameObject.transform.position.y < _waterSurfacePosY + _aboveWaterTolerance)
            {
                mv.y = _upDownSpeed;
            }
        }
        else if(Input.GetKey(KeyCode.Q))
        {
            //downwards
            mv.y = -_upDownSpeed;
        }
        _firstPersonController.m_MoveDir = mv;
        


    }
    private void SetRenderDiving()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = _fogColorWater;
        RenderSettings.fogDensity = 0.1f;
        RenderSettings.fogMode = FogMode.Exponential;
        _blur.enabled = true;
    }
    private void SetRenderDefault()
    {
        RenderSettings.fog = _defFogEnabled;
        RenderSettings.fogColor = _defFogColor;
        RenderSettings.fogDensity = _defFogDensity;
        RenderSettings.fogMode = _defFogMode;
        _blur.enabled = false;

    }
    public void OnTriggerEnter(Collider other)
    {
        if(LayerMask.LayerToName(other.gameObject.layer)=="Water")
        {
            _isInWater = true;
            Debug.Log("is in water");
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if(LayerMask.LayerToName(other.gameObject.layer)=="Water" && _isInWater)
        {
            _waterSurfacePosY = other.transform.position.y;
            float fpsPosY = this.transform.position.y;
            if(fpsPosY > _waterSurfacePosY)
            {
                _isInWater = false;
            }
            Debug.Log("Left water");
        }
    }
    
}
