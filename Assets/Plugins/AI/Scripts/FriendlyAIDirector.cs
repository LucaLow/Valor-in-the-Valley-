using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FriendlyAIDirector : MonoBehaviour
{
    public KeyCode toggle = KeyCode.E;

    [Space]

    public Transform pathSprite;
    public Transform arrowHeadSprite;
    public Transform destinationIndicator;
    [Space]

    public float targetHeight = 40f;
    public float radius = 10;
    public LayerMask friendlyAgentsLayer;

    private bool isDrawingPath;
    private Vector3 pathOrigin;

    private bool isDirecting = false;
    private Vector3 beforePosition;
    private Vector3 beforeRotation;

    private Coroutine startDirectingAnimation;
    private Coroutine stopDirectingAnimation;

    private Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggle))
        {
            if (isDirecting == false)
            {
                if (stopDirectingAnimation != null)
                    return;

                beforePosition = playerCamera.transform.localPosition;
                beforeRotation = playerCamera.transform.eulerAngles;


                isDirecting = true;
                startDirectingAnimation = StartCoroutine(StartDirecting());
            }
            else
            {
                if (startDirectingAnimation != null)
                    return;

                isDirecting = false;
                stopDirectingAnimation = StartCoroutine(StopDirecting());
            }
        }

        if (Input.GetMouseButtonDown(0) && isDirecting)
        {
            RaycastHit hit;
            Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit);
            pathOrigin = hit.point;
            pathOrigin.y += 1f;

            // Reset positions and scales
            arrowHeadSprite.position = pathOrigin;
            arrowHeadSprite.localScale = Vector3.zero;
            pathSprite.position = pathOrigin;
            pathSprite.localScale = Vector3.one;

            isDrawingPath = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isDrawingPath)
            {
                FinalisePath();
            }
            
            isDrawingPath = false;
        }

        if (isDirecting)
        {
            destinationIndicator.gameObject.SetActive(true);

            RaycastHit hit;
            Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit);
            Vector3 mousePosition = hit.point;
            mousePosition.y += 0.5f;

            destinationIndicator.position = Vector3.Lerp(destinationIndicator.position, mousePosition, 30 * Time.deltaTime);
        }
        else
        {
            destinationIndicator.gameObject.SetActive(false);
        }

        if (isDrawingPath)
        {
            pathSprite.gameObject.SetActive(true);
            arrowHeadSprite.gameObject.SetActive(true);

            // Raycast from cursor
            RaycastHit hit;
            Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit);
            Vector3 mousePosition = hit.point;
            mousePosition.y += 1f;

            // Change in mouse position
            Vector3 delta = pathOrigin - mousePosition;

            // Scaling
            Vector3 pathScale = new Vector3(delta.magnitude, pathSprite.localScale.y, pathSprite.localScale.z);
            pathSprite.localScale = Vector3.Lerp(pathSprite.localScale, pathScale, 30 * Time.deltaTime);
            arrowHeadSprite.localScale = Vector3.Lerp(arrowHeadSprite.localScale, Vector3.one, 20 * Time.deltaTime);

            // Positioning
            Vector3 pathPosition = pathOrigin - delta * 0.5f;
            pathSprite.position = Vector3.Lerp(pathSprite.position, pathPosition, 20 * Time.deltaTime);
            arrowHeadSprite.position = Vector3.Lerp(arrowHeadSprite.position, mousePosition, 20 * Time.deltaTime);

            // Calculating and applying rotations
            float angle = -Mathf.Atan2(delta.z, delta.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 eulerAngle = rotation.eulerAngles;
            Vector3 pathOffset = new Vector3(90f, 0f, 0f);
            Vector3 headOffset = new Vector3(90f, -90f, 0f);
            float pX = Mathf.LerpAngle(pathSprite.eulerAngles.x, eulerAngle.x + pathOffset.x, 20 * Time.deltaTime);
            float pY = Mathf.LerpAngle(pathSprite.eulerAngles.y, eulerAngle.y + pathOffset.y, 20 * Time.deltaTime);
            float pZ = Mathf.LerpAngle(pathSprite.eulerAngles.z, eulerAngle.z + pathOffset.z, 20 * Time.deltaTime);
            pathSprite.eulerAngles = new Vector3(pX, pY, pZ);
            float hX = Mathf.LerpAngle(arrowHeadSprite.eulerAngles.x, eulerAngle.x + headOffset.x, 8 * Time.deltaTime);
            float hY = Mathf.LerpAngle(arrowHeadSprite.eulerAngles.y, eulerAngle.y + headOffset.y, 8 * Time.deltaTime);
            float hZ = Mathf.LerpAngle(arrowHeadSprite.eulerAngles.z, eulerAngle.z + headOffset.z, 8 * Time.deltaTime);
            arrowHeadSprite.eulerAngles = new Vector3(hX, hY, hZ);
        }
        else
        {
            pathSprite.gameObject.SetActive(false);
            arrowHeadSprite.gameObject.SetActive(false);
        }
    }

    private void FinalisePath()
    {
        Collider[] colliders = Physics.OverlapSphere(pathOrigin, radius, friendlyAgentsLayer);

        foreach (Collider collider in colliders)
        {
            AIAgent agent = collider.GetComponentInParent<AIAgent>();
            if (agent != null)
            {
                // Raycast from cursor
                RaycastHit hit;
                Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit);
                Vector3 mousePosition = hit.point;
                mousePosition.y += 0.5f;

                if (agent.wanderingState.wanderCoroutine != null)
                    agent.StopCoroutine(agent.wanderingState.wanderCoroutine);
                agent.wanderingState.wanderCoroutine = null;

                agent.wanderingState.wanderRangePosition = mousePosition;
            }
        }
    }

    private IEnumerator StartDirecting()
    {
        float time = 0;

        // Disable player & camera movement
        PlayerController.active = false;
        // Show the cursor
        Cursor.lockState = CursorLockMode.None;

        // Calculate the target position
        Vector3 targetPosition = playerCamera.transform.localPosition;
        targetPosition.y = targetHeight;
        // Calculate the target rotation
        Vector3 targetRotation = new Vector3(90f, 0f, 0f);

        // 1 second transition time
        while (time <= 1f)
        {
            // Translate the camera to the target position
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPosition, 10 * Time.deltaTime);

            // Calculate the interpolated rotation
            Vector3 rotation = playerCamera.transform.eulerAngles;
            rotation.x = Mathf.LerpAngle(rotation.x, targetRotation.x, 10 * Time.deltaTime);
            rotation.y = Mathf.LerpAngle(rotation.y, targetRotation.y, 10 * Time.deltaTime);
            rotation.z = Mathf.LerpAngle(rotation.z, targetRotation.z, 10 * Time.deltaTime);
            // Rotate the camera to the target rotation
            playerCamera.transform.eulerAngles = rotation;

            // Increment time variable
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Snap the camera to the final target position & rotation
        playerCamera.transform.localPosition = targetPosition;
        playerCamera.transform.eulerAngles = targetRotation;
        // Reset the coroutine
        startDirectingAnimation = null;
    }

    private IEnumerator StopDirecting()
    {
        float time = 0;

        while (time <= 1f)
        {
            // Translate the camera to the target position
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, beforePosition, 10 * Time.deltaTime);

            // Calculate the interpolated rotation
            Vector3 rotation = playerCamera.transform.eulerAngles;
            rotation.x = Mathf.LerpAngle(rotation.x, beforeRotation.x, 10 * Time.deltaTime);
            rotation.y = Mathf.LerpAngle(rotation.y, beforeRotation.y, 10 * Time.deltaTime);
            rotation.z = Mathf.LerpAngle(rotation.z, beforeRotation.z, 10 * Time.deltaTime);
            // Rotate the camera to the target rotation
            playerCamera.transform.eulerAngles = rotation;

            // Increment time variable
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Snap the camera to the final target position & rotation
        playerCamera.transform.localPosition = beforePosition;
        playerCamera.transform.eulerAngles = beforeRotation;

        // Disable the cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Enable player & camera movement
        PlayerController.active = true;
        // Reset the coroutine
        stopDirectingAnimation = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
