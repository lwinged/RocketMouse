using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {

    [SerializeField] Sprite laserOnSprite;
    [SerializeField] Sprite laserOffSprite;
    [SerializeField] float toggleInterval = 0.5f;
    [SerializeField] float rotationSpeed = 0.0f;
    bool isLaserOn = true;
    float timeUntilNextToggle;
    Collider2D laserCollider;
    SpriteRenderer laserRenderer;


    // Use this for initialization
    void Start () {
        timeUntilNextToggle = toggleInterval;

        laserCollider = gameObject.GetComponent<Collider2D>();
        laserRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update () {
        timeUntilNextToggle -= Time.deltaTime;

        if (timeUntilNextToggle <= 0) {
            isLaserOn = !isLaserOn;
            laserCollider.enabled = isLaserOn;
            laserRenderer.sprite = (isLaserOn) ? laserOnSprite : laserOffSprite;
            timeUntilNextToggle = toggleInterval;
        }

        transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
