﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ToyGun))]
[RequireComponent(typeof(Animator))]
public class PuusMinigun : MonoBehaviour {

	[Header("Exclusive properties")]
	public AudioClip accelelerationClip;
	public AudioClip desaccelelerationClip;
	public AudioClip rotationClip;

	[Header("Extra components")]
	public AudioSource effectsAudioSource;

	// Components
	ToyGun toyGun;
	Animator animator;

	// Animator properties
	bool triggered = false;

	// Internal properties
	bool startedRotation;
	float triggeredTime;
	float lastShootTime;

	//

	void Start() {
		toyGun = GetComponent<ToyGun>();
		animator = GetComponent<Animator>();
	}

	void Update() {

		// Trigger check
		triggered = toyGun.Triggered;

		// Trigger exec
		if (triggered) {
			if (!startedRotation) {
				startedRotation = true;
				triggeredTime = Time.timeSinceLevelLoad;
				AccelerateBarrel();
			}
			if (Time.timeSinceLevelLoad >= triggeredTime + 1) {
				if (Time.timeSinceLevelLoad >= lastShootTime + 1 / toyGun.fireRate) {
					lastShootTime = Time.timeSinceLevelLoad + 1 / toyGun.fireRate;
					toyGun.Shoot();
					Shoot();
				}
			}
		} else {
			if (startedRotation) {
				DesaccelerateBarrel();
			}
			startedRotation = false;
		}
		SetAnimationProperties();
	}

	void Shoot() {
		RaycastHit[] hits = Physics.RaycastAll(toyGun.barrelOut.position, toyGun.barrelOut.forward);
		foreach (RaycastHit hit in hits) {
			ToyPart hittedPart = hit.collider.GetComponent<ToyPart>();
			if (hittedPart != null) {
				hittedPart.Hit(toyGun.toy, toyGun.damage);
			}
		}
	}

	void RotateBarrel() {
		if (!effectsAudioSource.clip.Equals(rotationClip) && triggered) {
			effectsAudioSource.clip = rotationClip;
			effectsAudioSource.loop = true;
			effectsAudioSource.Play();
		}
	}

	void AccelerateBarrel() {
		effectsAudioSource.clip = accelelerationClip;
		effectsAudioSource.loop = false;
		effectsAudioSource.Play();
	}

	void DesaccelerateBarrel() {
		effectsAudioSource.clip = desaccelelerationClip;
		effectsAudioSource.loop = false;
		effectsAudioSource.Play();
	}

	void SetAnimationProperties() {
		animator.SetBool("triggered", triggered);
	}
}
