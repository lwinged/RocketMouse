using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MouseController : MonoBehaviour {

    [SerializeField] float jetpackForce = 75.0f;
    [SerializeField] float forwardMovementSpeed = 3.0f;
    Rigidbody2D playerRigidbody;
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] LayerMask groundCheckLayerMask;
    bool isGrounded;
    Animator mouseAnimator;
    [SerializeField] ParticleSystem jetpack;
    bool isDead = false;
    uint coins = 0;
    [SerializeField] Text coinsCollectedLabel;
    [SerializeField] Button restartButton;
    [SerializeField] AudioClip coinCollectSound;
    [SerializeField] AudioSource jetpackAudio;
    [SerializeField] AudioSource footstepsAudio;
    [SerializeField] ParallaxScroll parallax;

    // Use this for initialization
    void Start () {
        playerRigidbody = GetComponent<Rigidbody2D>();
        mouseAnimator = GetComponent<Animator>();
	}

    void FixedUpdate()
    {
        bool jetpackActive = Input.GetButton("Fire1") && !isDead;

        if (jetpackActive) {
            playerRigidbody.AddForce(new Vector2(0, jetpackForce));
        }

        if (!isDead) {
            Vector2 newVelocity = playerRigidbody.velocity;
            newVelocity.x = forwardMovementSpeed;
            playerRigidbody.velocity = newVelocity;
        }

        UpdateGroundedStatus();
        AdjustJetpack(jetpackActive);

        if (isDead && isGrounded)
            restartButton.gameObject.SetActive(true);
        AdjustFootstepsAndJetpackSound(jetpackActive);

        parallax.offset = transform.position.x;
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Coins"))
            CollectCoin(collider);
        else 
            HitByLaser(collider);
    }


    public void RestartGame() {
        SceneManager.LoadScene("SampleScene");
    }

    void CollectCoin(Collider2D coinCollider) {
        ++coins;
        coinsCollectedLabel.text = coins.ToString();
        Destroy(coinCollider.gameObject);
        AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);
    }

    void HitByLaser(Collider2D laserCollider) {

        if (!isDead) {
            AudioSource laserZap = laserCollider.gameObject.GetComponent<AudioSource>();
            laserZap.Play();
        }

        isDead = true;
        mouseAnimator.SetBool("isDead", isDead);
    }

    void UpdateGroundedStatus() {
        isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f);
        mouseAnimator.SetBool("isGrounded", isGrounded);
    }

    void AdjustJetpack(bool jetpackActive) {
        var jetpackEmission = jetpack.emission;
        jetpackEmission.enabled = !isGrounded;
        jetpackEmission.rateOverTime = (jetpackActive) ? 300.0f : 75.0f;
    }

    void AdjustFootstepsAndJetpackSound(bool jetpackActive) {
        footstepsAudio.enabled = !isDead && isGrounded;
        jetpackAudio.enabled = !isDead && !isGrounded;
        jetpackAudio.volume = (jetpackActive) ? 1.0f : 0.5f;
    }
}
