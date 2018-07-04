using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour {

    [SerializeField] bool isEquipped;

    [SerializeField] float shotCooldown;
    [SerializeField] Transform firePosition;

    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] AudioSource gunAudio;
    [SerializeField] GameObject impactPrefab;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EquipData(PlayerShooting m_playerShooting, ShotEffectsManager m_shotEffectsManager)
    {
        m_playerShooting.shotCooldown = shotCooldown;
        m_playerShooting.firePosition = firePosition;
        m_shotEffectsManager.muzzleFlash = muzzleFlash;
        m_shotEffectsManager.gunAudio = gunAudio;
        m_shotEffectsManager.impactPrefab = impactPrefab;
    }
}
