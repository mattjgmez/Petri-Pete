using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionShoot : AIAction
{
    /// if true, the Character will face the target (left/right) when shooting
    public bool FaceTarget = true;
    /// if true the Character will aim at the target when shooting
    public bool AimAtTarget = false;

    protected CharacterOrientation _orientation;
    protected Character _character;
    protected CharacterWeaponHandler _characterWeaponHandler;
    protected WeaponAim _weaponAim;
    protected ProjectileWeapon _projectileWeapon;
    protected Vector3 _weaponAimDirection;
    protected int _numberOfShoots = 0;
    protected bool _shooting = false;

    /// <summary>
    /// On init we grab our CharacterHandleWeapon ability
    /// </summary>
    protected override void Initialization()
    {
        _character = GetComponent<Character>();
        _orientation = _character.GetAbility<CharacterOrientation>();
        _characterWeaponHandler = _character.GetAbility<CharacterWeaponHandler>();
    }

    /// <summary>
    /// On PerformAction we face and aim if needed, and we shoot
    /// </summary>
    public override void PerformAction()
    {
        MakeChangesToTheWeapon();
        TestFaceTarget();
        TestAimAtTarget();
        Shoot();
    }

    /// <summary>
    /// Sets the current aim if needed
    /// </summary>
    protected virtual void Update()
    {
        if (_characterWeaponHandler.CurrentWeapon != null)
        {
            if (_weaponAim != null)
            {
                if (_shooting)
                {
                    _weaponAim.SetCurrentAim(_weaponAimDirection);
                }
                else
                {
                    if (_orientation != null)
                    {
                        if (_orientation.IsFacingRight)
                        {
                            _weaponAim.SetCurrentAim(Vector3.right);
                        }
                        else
                        {
                            _weaponAim.SetCurrentAim(Vector3.left);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Makes changes to the weapon to ensure it works ok with AI scripts
    /// </summary>
    protected virtual void MakeChangesToTheWeapon()
    {
        if (_characterWeaponHandler.CurrentWeapon != null)
        {
            _characterWeaponHandler.CurrentWeapon.TimeBetweenUsesReleaseInterruption = true;
        }
    }

    /// <summary>
    /// Faces the target if required
    /// </summary>
    protected virtual void TestFaceTarget()
    {
        if (!FaceTarget)
        {
            return;
        }

        if (this.transform.position.x > _brain.Target.position.x)
        {
            _orientation.FaceDirection(-1);
        }
        else
        {
            _orientation.FaceDirection(1);
        }
    }

    /// <summary>
    /// Aims at the target if required
    /// </summary>
    protected virtual void TestAimAtTarget()
    {
        if (!AimAtTarget)
        {
            return;
        }

        if (_characterWeaponHandler.CurrentWeapon != null)
        {
            if (_weaponAim == null)
            {
                _weaponAim = _characterWeaponHandler.CurrentWeapon.gameObject.GetComponent<WeaponAim>();
            }

            if (_weaponAim != null)
            {
                if (_projectileWeapon != null)
                {
                    _projectileWeapon.DetermineSpawnPosition();
                    _weaponAimDirection = _brain.Target.position - (_character.transform.position);
                }
                else
                {
                    _weaponAimDirection = _brain.Target.position - _character.transform.position;
                }
            }
        }
    }

    /// <summary>
    /// Activates the weapon
    /// </summary>
    protected virtual void Shoot()
    {
        if (_numberOfShoots < 1)
        {
            _characterWeaponHandler.ShootStart();
            _numberOfShoots++;
        }
    }

    /// <summary>
    /// When entering the state we reset our shoot counter and grab our weapon
    /// </summary>
    public override void OnEnterState()
    {
        base.OnEnterState();
        _numberOfShoots = 0;
        _shooting = true;
        _weaponAim = _characterWeaponHandler.CurrentWeapon.gameObject.GetComponent<WeaponAim>();
        _projectileWeapon = _characterWeaponHandler.CurrentWeapon.gameObject.GetComponent<ProjectileWeapon>();
    }

    /// <summary>
    /// When exiting the state we make sure we're not shooting anymore
    /// </summary>
    public override void OnExitState()
    {
        base.OnExitState();
        _characterWeaponHandler.ShootStop();
        _shooting = false;
    }
}
