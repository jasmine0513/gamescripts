using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    public Camera playerCamera; // Reference to the player's camera
    public float throwForce = 500f; // Force at which the object is thrown
    public float pickUpRange = 5f; // How far the player can pick up the object from
    private float rotationSensitivity = 1f; // How fast/slow the object is rotated in relation to mouse movement
    private GameObject heldObj; // Object which we pick up
    private Rigidbody heldObjRb; // Rigidbody of object we pick up
    private bool canDrop = true; // Prevents throwing/dropping object when rotating
    private int LayerNumber; // Layer index

    // Reference to mouse look script
    private MonoBehaviour mouseLookScript;

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Find the mouse look script on the player or camera
        mouseLookScript = player.GetComponent<MonoBehaviour>();
        if (mouseLookScript == null)
        {
            mouseLookScript = playerCamera.GetComponent<MonoBehaviour>();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.tag == "canPickUp")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                    if (hit.transform.gameObject.tag == "canPickUp(C)")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                    if (hit.transform.gameObject.tag == "canPickUp(FV)")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                    if (hit.transform.gameObject.tag == "canPickUp(P)")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                    if (hit.transform.gameObject.tag == "canPickUp(G)")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                    if (hit.transform.gameObject.tag == "canPickUp(W)")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    StopClipping();
                    DropObject();
                }
            }
        }
        if (heldObj != null)
        {
            MoveObject();
            RotateObject();
            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop == true)
            {
                StopClipping();
                ThrowObject();
            }
        }
    }
    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform;
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }
    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObj = null;
    }
    void MoveObject()
    {
        heldObj.transform.position = holdPos.transform.position;
    }
    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))
        {
            canDrop = false;

            // Disable mouse look script to prevent camera movement
            if (mouseLookScript != null)
            {
                mouseLookScript.enabled = false;
            }

            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;

            // Fix inverted controls and use proper rotation
            // Mouse X (left/right) rotates around Y axis
            heldObj.transform.Rotate(0, XaxisRotation, 0, Space.World);
            // Mouse Y (up/down) rotates around X axis (inverted)
            heldObj.transform.Rotate(-YaxisRotation, 0, 0, Space.World);
        }
        else
        {
            canDrop = true;

            // Re-enable mouse look script
            if (mouseLookScript != null)
            {
                mouseLookScript.enabled = true;
            }
        }
    }
    void ThrowObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        Vector3 throwDirection = playerCamera.transform.forward;
        heldObjRb.AddForce(throwDirection * throwForce);
        heldObj = null;
    }
    void StopClipping()
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }
}