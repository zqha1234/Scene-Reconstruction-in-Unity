using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading;


public class Pos_Rota
{
    public double pos1 { get; set; }
    public double pos2 { get; set; }
    public double pos3 { get; set; }
    public double rota1 { get; set; }
    public double rota2 { get; set; }
    public double rota3 { get; set; }
}

public class MID
{
    public int id { get; set; }
}

public class MyData
{
    public List<MID> markerID { get; set; }
    public List<Pos_Rota> position { get; set; }

}

public class test : MonoBehaviour
{
    private object dataLock = new object(); // Lock object for synchronization
    public Vector3 cameraPosition;
    public Quaternion cameraRotation;
    private Thread receiveThread;
    private UdpClient listener;
    private IPEndPoint groupEP;
    private int listenPort;
    public string receivedData;
    public GameObject cupPrefab;
    public GameObject bottlePrefab;
    public GameObject cup;
    public Quaternion cupRotation;
    public Vector3 cupPosition;
    public GameObject bottle;
    public Quaternion bottleRotation;
    public Vector3 bottlePosition;
    public Dictionary<int, GameObject> obj_dic = new Dictionary<int, GameObject>();
    public Vector3 objectPostion;
    public GameObject gObject;

    // Start is called before the first frame update
    void Start()
    {
        cameraPosition = transform.position;
        cameraRotation = transform.rotation;

        cupRotation = Quaternion.Euler(0, 0, 0);// * cameraRotation;
        cupPosition = new Vector3(0, 0, 0);
        bottleRotation = Quaternion.Euler(0, 0, 0);// * cameraRotation;
        bottlePosition = new Vector3(0, 0, 0);

        // cup = Instantiate(cupPrefab, cupPosition, cupRotation);
        // Debug.Log(cup.transform.position);

        //cup.transform.position = cameraPosition + new Vector3(2, 4, 12);
        receivedData = "NULL";
        listenPort = 8899;
        listener = new UdpClient(listenPort);
        groupEP = new IPEndPoint(IPAddress.Any, listenPort);
        receiveThread = new Thread(new ThreadStart(Listen));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // Listener
    void Listen()
    {
        
        while (true)
        {
            byte[] data = listener.Receive(ref groupEP);
            receivedData = Encoding.UTF8.GetString(data);
        }
    }

    // Update is called once per frame
    void Update()
    {
        lock (dataLock)
        {
            HashSet<int> id_hashset = new HashSet<int>();
            List<int> keysToRemove = new List<int>();
            if (receivedData != "NULL")
            {
                // Deserialize the JSON string into a MyData object
                MyData myData = JsonConvert.DeserializeObject<MyData>(receivedData);
                // Accessing values
                for (int i = 0; i < myData.markerID.Count; i++)
                {
                    //Debug.Log("markerID Count:");
                    //Debug.Log(myData.markerID.Count);
                    MID mid = myData.markerID[i];
                    Pos_Rota posRota = myData.position[i];
                    id_hashset.Add(mid.id);
                    if (obj_dic.ContainsKey(mid.id)) 
                    {

                        // objectPostion = cameraPosition + new Vector3((float)posRota.pos1 * (1), (float)posRota.pos2 * (1), (float)posRota.pos3);
                        // obj_dic[mid.id].transform.position = Vector3.Lerp(obj_dic[mid.id].transform.position, objectPostion, Time.deltaTime * 2f);
                        obj_dic[mid.id].transform.position = cameraPosition + new Vector3((float)(posRota.pos1 * (1) / 10.0), (float)(posRota.pos2 * (-1) / 10.0), (float)(posRota.pos3 / 10.0));
                        obj_dic[mid.id].transform.rotation = Quaternion.Euler((float)posRota.rota1-180, (float)posRota.rota2+180, (float)posRota.rota3) * cameraRotation;
                        // add rotation here

                    } else
                    {
                        gObject = CreateObject(mid.id);
                        obj_dic.Add(mid.id, gObject);
                    }
                    //Debug.Log($"Marker ID: {mid.id}, Position: pos1={posRota.pos1}, pos2={posRota.pos2}, pos3={posRota.pos3}");
                    //Debug.Log($"Rotation: rota1={posRota.rota1}, rota2={posRota.rota2}, rota3={posRota.rota3}");
                    //cupPosition = cameraPosition + new Vector3((float)posRota.pos1*(1), (float)posRota.pos2*(1), (float)posRota.pos3);
                    //cupRotation = Quaternion.Euler(0, 0, 0) * cameraRotation;
                }
                
                receivedData = "NULL";
            }
            //foreach (var key in obj_dic.Keys)
            //{
            //    if (!id_hashset.Contains(key))
            //    {
            //        Destroy(obj_dic[key]);
            //        keysToRemove.Add(key);
            //    }                  
            //}

            // Now remove the keys
            //foreach (var key in keysToRemove)
            //{
            //    obj_dic.Remove(key);
            //}
        }
    }

    // FixedUpdate is called once per physic update
    private void FixedUpdate()
    {
        //cup.transform.position = Vector3.Lerp(cup.transform.position, cupPosition, Time.deltaTime * 2f);
        //obj_dic[mid.id].transform.position = Vector3.Lerp(obj_dic[mid.id].transform.position, objectPostion, Time.deltaTime * 2f);
    }

    private GameObject CreateObject(int id)
    {
        if (id == 11)
        {
            cup = Instantiate(cupPrefab, cupPosition, cupRotation);
            return cup;
        } else if (id == 10)
        {
            bottle = Instantiate(bottlePrefab, bottlePosition, bottleRotation);
            return bottle;
        }
        return null;
    }

    void OnDestory()
    {
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }

        listener.Close();
    }

}
