using UnityEngine;
using System.Collections;

[RequireComponent (typeof (LineRenderer))]
public class RecursiveLightning : MonoBehaviour {
	public int vertexCount = 17; //number of split points
	public Vector3[] vertices;
	public Vector3[] p;
	public Vector3 firstVertexPosition; //start position of line
	public Vector3 lastVertexPosition; //end position of line
	public float fadeOutTime = 0.3f; //initial fadde out time
	public bool strikeOnStart = false; //strike on awake flag
	public bool fadeOutAfterStrike = true; //fade out flag
	public RecursiveLightning leftBranch=null; //does not work
    public RecursiveLightning rightBranch=null;  //does not work
	int currentNumber;
	LineRenderer lineRenderer; //Line renderer for he lightning
	int leftBranchVertex = -1; //for branching does not work
	int rightBranchVertex = -1; //for branching does not work
    private float time = 0.0f; //Real world time
	private float other = 0.0f; //time between different position strikes
	public float interpolationPeriod = 2.0f; //Time between multile strikes
    private ParticleSystem cloud; //Cloud particle system
	private GameObject FlashLightObject; //flashing directional light
	private GameObject rainfall; //Praticle system for rain
	private GameObject CloudObject; 
    private bool LightEnabled = false; 
	public float total = 8.0f; //looping the scene
	private int inside = 1; 
	private int alternate = 5;
	private Vector3 startpos;
	private float overall = 0.0f; //cloud timer
	private float fall = 0.0f; //rainfall timer
	private float wait = 1.0f; //sound timer
	public AudioSource rain; //rain audio 
	public AudioSource clap; //thunder audio
	public AudioSource change; //cahnge between lightning audio

	void Start () {
        //Initialise the flashing light object
        FlashLightObject = GameObject.Find("p");
        //Initialise the rain particle system object
		rainfall = GameObject.Find("Rain");
        //Stop the rain rendering
        rainfall.SetActive(false);
        //Stop the rendering of directional light(Flashing light)
		FlashLightObject.SetActive(false);
        //Starting position of lightning
		startpos = this.transform.position;
        //Initialise a line renderer with pre-defined values
		InitializeLineRenderer();
        //If flag is on strike on awake
		if (strikeOnStart)
			StrikeLightning();
        //Play this sound effect after 5 seconds
		change.Play(441000);
        //Play this sound effect after 10 seconds
		rain.Play(441000*2);
	}




	void Update()
	{
        //timer check for rain
		fall += Time.deltaTime;
        //check for rain to start falling
		if (fall >= 19.0f && fall< 20.0f)
		{
			rainfall.SetActive(true); //set rendering on rain to true
		}
		if (overall <= 30.0f) //set the timer of lightning rendering after 30 seconds of 
        {
			overall += Time.deltaTime;
			
		}
		else
		{	
			
			time += Time.deltaTime; //keep note of real time time
			other += Time.deltaTime; //keep note for which lightning strike

			if (time >= interpolationPeriod && other <= total) //After every x seconds where x is equal to interpolation time
			{
				time = time - interpolationPeriod;
				FlashLightObject.SetActive(true); //start flashing
				if (alternate == 5)
				{
					clap.Play(); //play this sound effect for one out of 5 times this line is executed
					alternate = Random.Range(0,5);
				}
				alternate += 1; //foor the alternate audio effect
				StrikeLightning(); //strike lightning once
				StartCoroutine("Example"); //wait for some time
				fadeOutTime = 0.5f; //change fade out time back to normal

			}
			else if (other > total)
			{ //for the three different lightning strikes
				StartCoroutine("Stop"); //wait for some time
				wait = wait / 2; //decrease wait time over time
				change.Play();  //play this audio effect
				if (inside == 1) //for first lightning pre-determined values are initialised
				{
					this.transform.position = new Vector3(155.0f, 48.0f, 400.0f);
					this.transform.Rotate(0, 0, 20);
					this.transform.localScale += new Vector3(0, 1.0f, 0);
					inside = inside + 1;
				}
				else if (inside == 2) //for second lightning pre-determined values are initialised
                {	
					this.transform.position = new Vector3(350.0f, 55.0f, 200.0f);
					this.transform.Rotate(0, -50, 50);
					this.transform.localScale -= new Vector3(0, 0.2f, 0);
					inside = inside + 1;
				}
				else if (inside == 3) //for third lightning pre-determined values are initialised
                {
					this.transform.position = startpos;
					this.transform.Rotate(0, 40, -70);
					this.transform.localScale -= new Vector3(0, 1.0f, 0);
					inside = 1;
					total = total / 3;
					if (total <= 1.0f) // wrap aorund to a loop of all three striking
					{
						total = 0.0f;
					}
				}
				other = 0.0f;

			}
		}

	}




	IEnumerator Example() // wait between first strike and second
	{
		
		yield return new WaitForSeconds(0.3f);
		fadeOutTime = 0.02f; //minimize fade out time
        FlashLightObject.SetActive(true); //flash again
        StrikeLightning(); //strike again

    }
	IEnumerator Stop() //a random wait betwenswitch in different positions of the strike
	{

		yield return new WaitForSeconds(wait);

	}


	void InsertFirstAndLastNode(){ //initialise the first and last node of the line renderer
		InsertNodeInLineRenderer(0, firstVertexPosition);
		InsertNodeInLineRenderer(vertexCount-1, lastVertexPosition);
	}



	void InsertVertexBetween(int start, int end)
    {//initialise between given first and last node of the line renderer
        int currentVertexNumber = (start + end) /2;

		if(currentVertexNumber != start){
			vertices[currentVertexNumber] = (vertices[start] + vertices[end]) /2 + new Vector3(0, Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));

			InsertNodeInLineRenderer(currentVertexNumber, vertices[currentVertexNumber]);

			InsertVertexBetween(start, currentVertexNumber);
			InsertVertexBetween(currentVertexNumber, end);
		}

		if(leftBranchVertex == currentVertexNumber){ // for left branch does not work
			ConfigureBranch(leftBranch, vertices[currentVertexNumber], vertices[currentVertexNumber] + new Vector3(0.0f,-1f,-0.5f));
			leftBranch.StrikeLightning();
		}

		if(rightBranchVertex == currentVertexNumber){ // for right branch does not work
			ConfigureBranch(rightBranch, vertices[currentVertexNumber], vertices[currentVertexNumber] + new Vector3(0.0f,-1f, 0.5f));
			rightBranch.StrikeLightning();
		}
	}




	void ConfigureBranch(RecursiveLightning branch, Vector3 firstVertexPosition, Vector3 lastVertexPosition){ // create new branch for new line renderer ut does not work
		branch.firstVertexPosition = firstVertexPosition;

		if(lastVertexPosition.y < vertices[vertexCount-1].y)
			lastVertexPosition.y = vertices[vertexCount-1].y;

		branch.lastVertexPosition = lastVertexPosition;

		branch.fadeOutTime = fadeOutTime;
		branch.fadeOutAfterStrike = fadeOutAfterStrike;
	}




	void InsertNodeInLineRenderer(int position, Vector3 vertex){ // Initialises a node in the line renderer
		lineRenderer.SetPosition(position, vertex);
	}

	void InitializeLineRenderer(){ //Initilise aline renderer with given start and end points defined along with given split points
		lineRenderer = GetComponent<LineRenderer>();
		p = new Vector3[17];
		lineRenderer.GetPositions(p);
		lineRenderer.SetVertexCount(vertexCount);

		lineRenderer.enabled = false; // make the lightning vanish
	}




	public void StrikeLightning(){ //Control function for entire lightning module
		if(!lineRenderer.enabled) //if line renderer is initialised
        {
         
            if (leftBranch) //for branching does not work
			{
				leftBranchVertex = Random.Range(0, vertexCount - 1);
			}

			if (rightBranch) //for branching does not work
            {
				rightBranchVertex = Random.Range(0, vertexCount - 1);
			}
            //initialse the split points
			vertices = new Vector3[vertexCount];
			//initialise the starting point of line
			vertices[0] = firstVertexPosition;
			vertices[vertexCount -1] = lastVertexPosition;
            //initialise the ending point of line
            lineRenderer.enabled = true;
			InsertFirstAndLastNode();			
			InsertVertexBetween(0, vertexCount-1);

			if(fadeOutAfterStrike)
				FadeOut(); //Fade after one strike
        }
    }

	public void FadeOut(){ //for making the lightning flicker and fade after strike

		StartCoroutine("FadeOutQuickly");
	}

	IEnumerator FadeOutQuickly(){ //wait for some time and fade out with vanishing flashes
		yield return new WaitForSeconds(fadeOutTime);
		FlashLightObject.SetActive(false);
		lineRenderer.enabled = false;
	}
}
