using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCamSpider : MonoBehaviour
{
    public float introDuration = 3.0f; // Duration of the intro animation
    public float cameraDistance = 5.0f; // Distance between the camera and the enemy
    public Vector3 introPositionOffset = new(0, 2, -2); // Offset from the enemy's position during intro
    private Transform initialCameraTransform;
    private float introTimer = 0.0f;
    private bool introCompleted = false;
    [SerializeField] private GameObject introCard;

    // Start is called before the first frame update
    void Start()
    {
        initialCameraTransform = LevelManager.global.SceneCamera.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!introCompleted && GetComponent<SpiderBoss>().startIntro)
        {
            PlayerController.global.characterAnimator.SetBool("Moving", false);
            introTimer += Time.deltaTime;
            CameraFollow.global.bossCam = true;
            // Calculate the interpolation factor
            float introProgress = Mathf.Clamp01(introTimer / introDuration);
            PlayerController.global.playerCanMove = false;
            // Perform the intro animation
            Vector3 targetPosition = transform.position + introPositionOffset;
            Vector3 cameraPosition = Vector3.Lerp(initialCameraTransform.position, targetPosition - initialCameraTransform.forward, introProgress);
            LevelManager.global.SceneCamera.transform.position = cameraPosition;

            introCard.SetActive(true);

            if (introProgress >= 1.0f)
            {
                introCompleted = true;
            }
        }
        else if (introCompleted)
        {
            GetComponent<SpiderBoss>().awoken = true;
            CameraFollow.global.bossCam = false;
            PlayerController.global.playerCanMove = true;
            LevelManager.global.HUD.SetActive(true);
            introCard.SetActive(false);
            GetComponent<BossSpawner>().BossEncountered(true);
        }
    }
}
