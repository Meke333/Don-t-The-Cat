using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    public Transform cameraRoot;
    //RaycastHit hit;
    LayerMask layerMask;
    [SerializeField] float length;
    GameObject leverParent;
    GameObject lampParent;
    GameObject[] leverObjects;
    GameObject[] lampObjects;

    [SerializeField] Material red;
    [SerializeField] Material green;

    Transform[] leverTransforms;
    Transform[] lampTransforms;
    Animator[,] anims;
    MeshRenderer[,] renderers;
    GameObject[,] lamps;

    Lever[,] levers;

    //Button
    GameObject button;
    Animator buttonAnim;

    //check lamps
    //level
    GameObject[] levelObjects;
    GameObject levelParent;
    bool[,,] levels3D;
    int levelIndex = 0;


    class Lever {

        public bool isUp = true;
        public int x;
        public int y;
        public GameObject leverObject;
        
        public Lever(int x, int y, GameObject obj)
        {
            this.x = x;
            this.y = y;
            leverObject = obj;
        }
    };

    void Start()
    {
        levelParent = GameObject.FindWithTag("Levels");
        levelObjects = new GameObject[10];
        assignObjects(levelParent.transform, levelObjects);
        
        
        leverParent = GameObject.FindWithTag("LeverParent");
        lampParent = GameObject.FindWithTag("LampParent");
        button = GameObject.FindWithTag("Button");
        buttonAnim = button.GetComponent<Animator>();

        leverObjects = new GameObject[16];
        lampObjects = new GameObject[16];
        leverTransforms = new Transform[16];
        lampTransforms = new Transform[16];

        assignObjects(leverParent.transform, leverObjects);
        assignObjects(lampParent.transform, lampObjects);

        levers = new Lever[4, 4];
        anims = new Animator[4, 4];
        renderers = new MeshRenderer[4, 4];
        
        
        levels3D = new bool[10, 4, 4]
        {
            { { false, false, true, true }, { false, false, true, false }, { false, true, false, false }, { false, true, false, false } },
            { { true, false, false, false }, { false, true, false, false }, { false, true, true, true }, { true, false, false, false } },
            { { false, false, false, true }, { false, false, false, true }, { false, false, false, true }, { true, true, true, true } },
            { { true, false, true, false }, { false, true, true, false }, { false, false, false, true }, { true, true, true, false } },
            { { false, true, true, false }, { false, true, false, false }, { true, true, true, true }, { false, false, true, false } },
            { { false, false, false, false }, { false, false, true, false }, { false, false, true, false }, { true, false, true, true } },
            { { false, true, true, true }, { false, true, true, false }, { false, true, false, false }, { false, true, false, false } },
            { { false, true, true, true }, { false, true, true, true }, { false, false, false, false }, { false, false, false, false } },
            { { false, true, false, true }, { true, false, false, false }, { false, true, false, false }, { false, true, false, true } },
            { { true, true, true, true }, { true, true, true, true }, { true, true, true, true }, { true, true, true, true } }
        };

        int k = 0;
        for(int i = 0; i<4; i++)
        {
            for(int j = 0; j<4; j++)
            {
                anims[i, j] = leverObjects[k].GetComponent<Animator>();
                levers[i, j] = new Lever(i, j, leverObjects[k]);
                renderers[i, j] = lampObjects[k].GetComponent<MeshRenderer>();
                k++;
            }
        }
        //cameraRoot = transform.Find("PlayerCameraRoot");
        layerMask = 1 << 6;
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, length, layerMask))
            {
                //LEVER SOUND HERE
                
                Debug.DrawRay(ray.origin,ray.direction, Color.green, 0.05f);

                string s = hit.collider.tag;
                char xChar = s[7];
                char yChar = s[5];
                int x = (int)(xChar - '0') - 1;
                int y = (int)(yChar - '0') - 1;

                if (levers[y, x].isUp == true)
                {
                    anims[y, x].SetTrigger("Down");
                    levers[y, x].isUp = false;
                    StartCoroutine(changeMaterial(renderers[y, x], green));
                    StartCoroutine(disableCollider(hit.collider, 0.1f, Time.time));
                }
                else
                {
                    anims[y, x].SetTrigger("Up");
                    levers[y, x].isUp = true;
                    StartCoroutine(changeMaterial(renderers[y, x], red));
                }
            }
            else if(Physics.Raycast(ray, out RaycastHit hit2, length, 1<<7))
            {
                // BUTTON PUSH SOUND HERE
                
                buttonAnim.SetTrigger("Push");
                disableCollider(hit2.collider, 3, Time.time);
                
                if (checkLevel())
                {
                    StartCoroutine(switchLevels());
                }
            }
            
            else
            {
                Debug.DrawLine(cameraRoot.position, cameraRoot.position + cameraRoot.forward * 3, Color.red, 0.05f);
            }
        }
    }

    IEnumerator disableCollider(Collider coll, float duration, float startTime)
    {
        if(Time.time < startTime + duration)
        {
            coll.enabled = false;
            yield return null;
        }
        coll.enabled = true;
        yield return null;
    }

    IEnumerator changeMaterial(MeshRenderer rend, Material mat, float waitDuration = 0.15f)
    {
        yield return new WaitForSeconds(waitDuration);
        rend.material = mat;
        yield return null;
    }
    
    IEnumerator switchLevels()
    {
        levelObjects[levelIndex].SetActive(false);
        yield return new WaitForSeconds(1f);
        levelObjects[levelIndex+1].SetActive(true);
        levelIndex++;
        yield return null;
    }

    void assignObjects(Transform parent, GameObject[] array)
    {
        int i = 0;
        foreach(Transform child in parent)
        {
            array[i] = child.gameObject;
            i++;        
        }
    }
    
    bool checkLevel()
    {
        for(int i = 0; i<4; i++)
        {
            for(int j = 0; j<4; j++)
            {
                if(levers[i, j].isUp == levels3D[levelIndex, i, j])
                {
                    
                    GameEventManager.Instance.onPlayerDied?.Invoke();
                    
                    //Your Dead SOUND HERE
                    
                    return false;
                }
            }
        }

        if (levelIndex > 8)
        {
            GameEventManager.Instance.onWonGame?.Invoke();
        }
        
        //richtige Antwort SOUND HERE
        
        
        return true;
    }
}
