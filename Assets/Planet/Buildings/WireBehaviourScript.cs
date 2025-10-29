using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;

public class WireBehaviourScript : MonoBehaviour
{
    [SerializeField]
   bool if_put = false;
   private bool is_ground;
   private GameObject wirePrefab;
   private GameObject[] wires;
   private static int wireCount = 0;
   private int energyvalue = 0;
   private FixedJoint fixedJoint;
   private Rigidbody rigidbody;
   private float breakforce = Mathf.Infinity;
   private float breaktorque = Mathf.Infinity;
   private GameObject nearWire = null;
    public void Put(Vector3 position)
    {
        Collider[] nearbythings= Physics.OverlapSphere(position,1.0f);
        bool playerNearby=false;
        bool wireNearby = false;
       
        if (if_put)
        {
            foreach (Collider col in nearbythings)
            {
                if (col.CompareTag("Player"))
                {
                    playerNearby = true;
                    break;
                }
            }
            foreach (Collider col in nearbythings)
            {
                if (col.CompareTag("Wire"))
                {
                    wireNearby = true;
                    nearWire = col.gameObject;
                    break;
                }
            }
            if (playerNearby)
            {
                GameObject instance = GameObject.Instantiate(wirePrefab);
                instance.SetActive(true);
                instance.transform.position = position;
                DataBaseManger.RegisterBuildingData("Wire" + wireCount, position, energyvalue);
                if (wireNearby)
                {
                    if (nearWire!=null) {
                        energyvalue = nearWire.GetComponent<WireBehaviourScript>().GetEnergy();
                        transform.rotation= nearWire.transform.rotation;
                    }
                }
            }
        }
        wireCount += 1;
    }
    public void IsPut()
    {
         if_put = true;
    }
    public void SetEnergy(int value)
    {
        energyvalue = value;
    }
    public int GetEnergy()
    {
        return energyvalue;
    }
    private void AlignToGrand()
   {
        foreach (GameObject wire in wires)
        {

            RaycastHit hit;

            Vector3 rayStart = wire.transform.position + Vector3.up * 0.1f;

            if (Physics.SphereCast(rayStart, 0.5f, Vector3.down, out hit))
            {
                is_ground = true;
                rigidbody = wire.GetComponent<Rigidbody>();
                fixedJoint = wire.GetComponent<FixedJoint>();
                if (rigidbody == null || fixedJoint == null)
                {
                    rigidbody = wire.AddComponent<Rigidbody>();
                    fixedJoint = wire.AddComponent<FixedJoint>();
                }
                else
                {
                    if (hit.collider.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        fixedJoint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody>();
                    }
                    else
                    {
                        hit.collider.gameObject.AddComponent<Rigidbody>();
                        fixedJoint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody>();
                        fixedJoint.breakForce = breakforce;
                        fixedJoint.breakTorque = breaktorque;
                    }
                }
            }
            wire.transform.position = hit.point;
            wire.transform.rotation = Quaternion.FromToRotation(wire.transform.position, hit.normal) * wire.transform.rotation;
        }
    }
    private void ApplyGravty()
    {
        foreach (GameObject wire in wires)
        {
            rigidbody = wire.GetComponent<Rigidbody>();

            if (!is_ground)
            {
                if (rigidbody != null)
                {
                    rigidbody.AddForce(Physics.gravity * rigidbody.mass);
                }
                else
                {
                    wire.AddComponent<Rigidbody>();
                    rigidbody.AddForce(Physics.gravity * rigidbody.mass);
                }
            }
        }
    }
    public void TransformEnergy()
    {
        if (energyvalue > 0 && nearWire != null)
        {
            nearWire.GetComponent<WireBehaviourScript>().SetEnergy(energyvalue);
        }
    }
        // Start is called before the first frame update
        void Start()
        {
            wires = GameObject.FindGameObjectsWithTag("Wire");
            foreach (GameObject wire in wires)
            {
                wire.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
            AlignToGrand();
            ApplyGravty();
        }
    }
