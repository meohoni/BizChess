using UnityEngine;
using System.Collections;

public class Dice : MonoBehaviour {

    private static GameObject dieA;
    private static GameObject dieB;

    private static int dieANum;
    private static int dieBNum;

    private static bool isDiceRunning = false;
    private static int counter = 0;

    private static int i = 0;

    // material cache
    private static ArrayList matNames = new ArrayList();
	private static ArrayList materials = new ArrayList();

    public static int DieANum { get => dieANum; }
    public static int DieBNum { get => dieBNum; }
    public static bool IsDiceRunning { get => isDiceRunning; }

    //------------------------------------------------------------------------------------------------------------------------------
    // public methods
    //------------------------------------------------------------------------------------------------------------------------------	

    private void Start()
    {
        Dice.InitDice();
    }

    private static void InitDice()
    {
        // create the new die
        Vector3 offsetY = new Vector3(0, -1.0f, 0);
        Vector3 dieAPos = Vector3.zero - new Vector3(0.9f, 0, 0) + offsetY;
        Vector3 dieBPos = Vector3.zero - new Vector3(-0.9f, 0, 0) + offsetY;
        dieA = Dice.prefab("d6", dieAPos, Vector3.up, new Vector3(0.6f, 0.6f, 0.6f), "d6-black-dots");
        dieB = Dice.prefab("d6", dieBPos, Vector3.up, new Vector3(0.6f, 0.6f, 0.6f), "d6-black-dots");
    }

    public IEnumerator RollDice()
    {
        isDiceRunning = true;
        counter = 0;


        StartCoroutine(RollDie(dieA, (num) => dieANum = num));
        StartCoroutine(RollDie(dieB, (num) => dieBNum = num));

        while (counter < 2)
        {
            yield return null;
        }

        isDiceRunning = false;
        //print("die1Index " + dieANum);
        //print("die2Index " + dieBNum);

        //dieBNum = 0;

        // test station
        //if (i <= 1)
        //{
        //    dieANum = 3;       
        //    i++;
        //}
        //else if (i == 2 || i == 3)
        //{
        //    dieANum = 10;
        //    i++;
        //}
        //else if (i == 4 || i == 5)
        //{
        //    dieANum = 8;
        //    i++;
        //}
        //else if (i == 6 || i == 7)
        //{
        //    dieANum = 10;
        //    i++;
        //}

        // test utility
        //if (i <= 1)
        //{
        //    dieANum = 9;
        //    i++;
        //}
        //else if (i == 2 || i == 3)
        //{
        //    dieANum = 19;
        //    i++;
        //}

        // test prison
        //if (i <= 1)
        //{
        //    dieANum = 8;
        //    i++;
        //}
        //else if (i == 2)
        //{
        //    dieANum = 17;
        //    i++;
        //}
        //else if (i == 3)
        //{
        //    dieANum = 36;
        //    i++;
        //}

        // test pass start no money
        //if (i <= 1)
        //{
        //    dieANum = 10;
        //    i++;
        //}
        //else if (i == 2)
        //{
        //    dieANum = 26;
        //    i++;
        //}
        //else if (i == 3)
        //{
        //    dieANum = 26;
        //    i++;
        //}

        //else
        //{
        //    dieANum = 10;
        //    dieBNum = 26;
        //}
    }

    private IEnumerator RollDie(GameObject die, System.Action<int> output)
    {
        Quaternion randomRotation;
        for (int i = 0; i < 30; i++)
        {
            randomRotation = Quaternion.Euler(UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f));
            die.transform.rotation = Quaternion.Slerp(die.transform.rotation, randomRotation, Time.deltaTime * 15.0f);
            yield return new WaitForSeconds(0.05f);
        }

        int num = UnityEngine.Random.Range(1, 7);
        Vector3 direction;
        switch (num)
        {
            case 1: // number 1
                {
                    direction = Vector3.back;
                    break;
                }
            case 2: // number 2
                {
                    direction = Vector3.down;
                    break;

                }
            case 3: // number 3
                {
                    direction = Vector3.left;
                    break;
                }
            case 4: // number 4
                {
                    direction = Vector3.right;
                    break;
                }
            case 5: // number 5
                {
                    direction = Vector3.up;
                    break;
                }
            case 6: // number 6
                {
                    direction = Vector3.forward;
                    break;
                }
            default:
                {
                    direction = Vector3.right;
                    break;
                }
        }

        die.transform.rotation = Quaternion.LookRotation(direction);
        counter++;
        output(num);
    }

    /// <summary>
    /// This method will create/instance a prefab at a specific position with a specific rotation and a specific scale and assign a material
    /// </summary>
    private static GameObject prefab(string name, Vector3 position, Vector3 rotation, Vector3 scale, string mat) 
	{		
		// load the prefab from Resources
        Object pf = Resources.Load("Prefabs/" + name);
		if (pf!=null)
		{
			// the prefab was found so create an instance for it.
			GameObject inst = (GameObject) GameObject.Instantiate( pf , Vector3.zero, Quaternion.identity);
			if (inst!=null)
			{
				// the instance could be created so set material, position, rotation and scale.
				if (mat!="") inst.GetComponent<Renderer>().material = material(mat);
				inst.transform.position = position;
				inst.transform.Rotate(rotation);
				inst.transform.localScale = scale;
				// return the created instance (GameObject)
				return inst;
			}
		}
		else
			Debug.Log("Prefab "+name+" not found!");
		return null;		
	}	
	
	/// <summary>
	/// This method will perform a quick lookup for a 'cached' material. If not found, the material will be loaded from the Resources
	/// </summary>
	private static Material material(string matName)
	{
		Material mat = null;
		// check if material is cached
		int idx = matNames.IndexOf(matName);
		if (idx<0)
		{
			//  not cached so load it from Resources			
			string[] a = matName.Split('-');
			if (a.Length>1)
			{
				a[0] = a[0].ToLower();
				if (a[0].IndexOf("d")==0)
					mat = (Material) Resources.Load("Materials/"+a[0]+"/"+matName);
			}			
			if (mat==null) mat = (Material) Resources.Load("Materials/"+matName);
			if (mat!=null)
			{
				// add material to cache
				matNames.Add(matName);
				materials.Add(mat);			
			}
		}
		else
			mat = (Material) materials[idx];
		// return material - null if not found
		return mat;		
	}
	
}

