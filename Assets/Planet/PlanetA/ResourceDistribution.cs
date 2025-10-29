using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDistribution : MonoBehaviour
{
    [SerializeField]
    private int resourceAmount = 0;
    private int counter = 0;
    public void DistributeResources(Vector3 position)
    {
       float Amount= Random.Range(1,10);
       resourceAmount=Mathf.FloorToInt(Amount)*10000;
       DataBaseManger.RegisterBuildingData("ResourceNode"+counter, position, resourceAmount);
       counter += 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        List<Vector3> resourcePositions = new List<Vector3>();
        for (int i=0;i<3;i++)
        {
            for (int j=0;j<3;j++)
            {
                resourcePositions.Add(new Vector3(-i*1000,0,-j*1000));
            }
        }
        foreach (Vector3 pos in resourcePositions)
        {
            DistributeResources(pos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
