using UnityEngine;
using UnityEngine.InputSystem;

public class RecallController : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem teleportParticlePrefab;

    [Header("Recall Settings")]
    [SerializeField] private GameObject recallMarkerPrefab; // the shadowclone left behind
    [SerializeField] private float recallCooldown = 3f;

    [Header("Audio")]
    [SerializeField] private AudioClip setSound;
    [SerializeField] private AudioClip teleportSound;

    private GameObject spawnedMarker;
    private Vector3 recallPointPosition;
    private bool isRecallPointSet = false;
    private float nextAllowedTeleport = 0f;
    private MovementController moveController;
    private Rigidbody rb;                       

    private void Start()
    {
        TryGetComponent(out rb);                // gives me Player(Clone) Rigidbody
        TryGetComponent(out moveController);    // I believe this lets me communicate to the playercontroller, which handles things like dashing, interactions and now teleporting.
    }

    // don't forget to call "OnSetOrTeleport" in PlayerController if you want this crap to work
    // AND it's gotta be the same as the name below, as well as in the input actions
    public void SetOrTeleport()
    {
        if (Time.time < nextAllowedTeleport)
        {
            Debug.Log("Recall on cooldown.");
            return;
        }

        if (isRecallPointSet == false) // if no recall point exists, we set it.
        {
            recallPointPosition = transform.position; // this script is directly placed on a gameobject, so it uses that object as it's transform.
            isRecallPointSet = true;

            Vector3 markerPosition = recallPointPosition;
            markerPosition.y -= 0.3926557f; // The clone spawns slightly too high to properly overlap the player, this fixes it.

            // use our visual reference at our location and rotation
            spawnedMarker = Instantiate(recallMarkerPrefab, markerPosition, transform.rotation);

            AudioSource.PlayClipAtPoint(setSound, transform.position);

            Debug.Log($"[RecallController] Recall point set to {recallPointPosition}.");
        }

        else // teleporting
        {
            Debug.Log($"[RecallController] Teleporting player to {recallPointPosition}.");

            // added particles that get deleted after their animation is over.
            ParticleSystem startEffect = Instantiate(teleportParticlePrefab, transform.position, transform.rotation);
            Destroy(startEffect.gameObject, startEffect.main.duration);
            
            rb.position = recallPointPosition; // move to location
            rb.angularVelocity = Vector3.zero; // this fixes a bug where the teleport would break, i think because two transform updates are fighting each other somewhere

            AudioSource.PlayClipAtPoint(teleportSound, transform.position);

            // now for the exit
            ParticleSystem endEffect = Instantiate(teleportParticlePrefab, recallPointPosition, transform.rotation);
            Destroy(endEffect.gameObject, endEffect.main.duration);

            isRecallPointSet = false; // next button press will create a new teleportation spot

            Destroy(spawnedMarker);
            nextAllowedTeleport = Time.time + recallCooldown; // start cooldown

            Debug.Log("[RecallController] Teleport complete.");
        }
    }
}