using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {

    [SerializeField] private float footStepSoundInterval = 0.1f;
    [SerializeField] private float footStepVolume = 0.5f;

    private Player player;
    private float footstepTimer;

    private void Awake() {
        player = GetComponent<Player>();
    }

    private void Update() {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer < 0f) {
            footstepTimer = footStepSoundInterval;
            if (player.IsWalking()) {
                SoundManager.Instance.PlayFootstepsSound(player.transform.position, footStepVolume);
            }
        }
    }
}