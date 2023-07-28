using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionAimWeaponAtTarget : AIAction
{
    protected TopDownController _controller;
    protected CharacterWeaponHandler _characterWeaponHandler;
    protected WeaponAim _weaponAim;
    protected AIActionShoot _aiActionShoot;
    protected Vector3 _weaponAimDirection;
    protected AIBrain _aiBrain;

    /// <summary>
    /// On init we grab our components
    /// </summary>
    protected override void Initialization()
    {
        _characterWeaponHandler = this.gameObject.GetComponent<CharacterWeaponHandler>();
        _aiActionShoot = this.gameObject.GetComponent<AIActionShoot>();
        _controller = this.gameObject.GetComponent<TopDownController>();
        _aiBrain = this.gameObject.GetComponent<AIBrain>();
    }

    /// <summary>
    /// if we're not shooting, we aim at our target or at our current movement
    /// </summary>
    public override void PerformAction()
    {
        if (!Shooting())
        {
            if (_aiBrain != null && _aiBrain.Target != null)
            {
                _weaponAimDirection = (_aiBrain.Target.position - this.transform.position).normalized;
            }
            else
            {
                _weaponAimDirection = _controller.CurrentDirection;
            }

            if (_weaponAim == null)
            {
                GrabWeaponAim();
            }
            if (_weaponAim == null)
            {
                return;
            }
            _weaponAim.SetCurrentAim(_weaponAimDirection);
        }
    }

    /// <summary>
    /// Returns true if shooting, returns false otherwise
    /// </summary>
    /// <returns></returns>
    protected bool Shooting()
    {
        if (_aiActionShoot != null)
        {
            return _aiActionShoot.ActionInProgress;
        }
        return false;
    }

    protected virtual void GrabWeaponAim()
    {
        if (_characterWeaponHandler.CurrentWeapon != null)
        {
            _weaponAim = _characterWeaponHandler.CurrentWeapon.gameObject.GetComponent<WeaponAim>();
        }
    }

    /// <summary>
    /// When entering the state we grab our weapon
    /// </summary>
    public override void OnEnterState()
    {
        base.OnEnterState();
        GrabWeaponAim();
    }
}
