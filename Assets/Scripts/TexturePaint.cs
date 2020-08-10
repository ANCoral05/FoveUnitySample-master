using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
using UnityEngine.UI;

public class TexturePaint : FOVEBehavior
{

    // ======================================================================================================================
    // PARAMETERS -----------------------------------------------------------------------------------------------
    public  Texture          baseTexture;                  // used to deterimne the dimensions of the runtime texture
    public  Material         meshMaterial;                 // used to bind the runtime texture as the albedo of the mesh
    public  GameObject       meshGameobject;
    public  Shader           UVShader;                     // the shader usedto draw in the texture of the mesh
    public  Mesh             meshToDraw;
    public  Shader           ilsandMarkerShader;
    public  Shader           fixIlsandEdgesShader;
    public  Shader           combineMetalicSmoothnes;                         
    public  static Vector3   mouseWorldPosition;

    // --------------------------------
  
    public Camera           mainC;
    private int              clearTexture;
    private RenderTexture    markedIlsandes;
    private CommandBuffer    cb_markingIlsdands;
    private int              numberOfFrames;
    private Material         fixEdgesMaterial;
    private Material         createMetalicGlossMap;
    private RenderTexture    metalicGlossMapCombined;

    private int colorPercentage; //by Alex
    public DistanceFromSaccadePathSound saccadePath;
    public RotationMeasurement rotationMeasurement;
    public PlayerCollisionCounter collisionCounter;
    public float totalPercentage;
    private int lastOneSecondPixels;
    private int lastThreeSecondPixels;
    private int lastFiveSecondPixels;
    private int lastTenSecondPixels;
    public float lastOneSecondPercentage;
    public float lastThreeSecondPercentage;
    public float lastFiveSecondPercentage;
    public float lastTenSecondPercentage;
    private float captureTimer;
    //public MeshRenderer heatmapVisualization;
    public GameObject canvasScore;
    public TextMesh textScore;
    public GameObject goal;
    public bool finished;
    private ManagerScript manager;
    public bool startMeasurement;
    public bool searchFinished;

    // ---------------------------------
    private PaintableTexture albedo;
    private PaintableTexture metalic;
    public  PaintableTexture smoothness;
    public bool toggleMouse;
    public RawImage hue;
    [Range(1,100), Tooltip("Decides the percentage of pixels read out when calculating covered area.")]
    public int readSteps;
    public List<float> averageGazeAreaOne;
    public List<float> averageGazeAreaThree;
    public List<float> averageGazeAreaFive;
    public List<float> averageGazeAreaTen;
    public float averageGazeAreaNumberOne;
    public float averageGazeAreaNumberThree;
    public float averageGazeAreaNumberFive;
    public float averageGazeAreaNumberTen;
    // ======================================================================================================================
    // INITIALIZE -------------------------------------------------------------------

    void Start () {

        // Main cam initialization ---------------------------------------------------
        //                   mainC = Camera.main;
        //if (mainC == null) mainC = this.GetComponent<Camera>();
        //if (mainC == null) mainC = GameObject.FindObjectOfType<Camera>();


        // Texture and Mat initalization ---------------------------------------------
        markedIlsandes = new RenderTexture(baseTexture.width, baseTexture.height, 0, RenderTextureFormat.R8);
        albedo         = new PaintableTexture(Color.black, baseTexture.width, baseTexture.height, "_MainTex"
            ,UVShader, meshToDraw, fixIlsandEdgesShader,markedIlsandes);
        metalic        = new PaintableTexture(Color.white, baseTexture.width, baseTexture.height, "_MetallicGlossMap"
              , UVShader, meshToDraw, fixIlsandEdgesShader, markedIlsandes);

        smoothness     = new PaintableTexture(Color.black, baseTexture.width, baseTexture.height, "_GlossMap"
              , UVShader, meshToDraw, fixIlsandEdgesShader, markedIlsandes);

        metalicGlossMapCombined = new RenderTexture(metalic.runTimeTexture.descriptor)
        {
            format = RenderTextureFormat.ARGB32,
        };

        //Alex
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManagerScript>();

        meshMaterial.SetTexture(albedo.id, albedo.runTimeTexture);
        meshMaterial.SetTexture(metalic.id, metalicGlossMapCombined);
        
        meshMaterial.EnableKeyword("_METALLICGLOSSMAP");


        createMetalicGlossMap = new Material(combineMetalicSmoothnes);


        // Command buffer inialzation ------------------------------------------------

        cb_markingIlsdands      = new CommandBuffer();
        cb_markingIlsdands.name = "markingIlsnads";

      
        cb_markingIlsdands.SetRenderTarget(markedIlsandes);
        Material mIlsandMarker  = new Material(ilsandMarkerShader);
        cb_markingIlsdands.DrawMesh(meshToDraw, Matrix4x4.identity, mIlsandMarker);
        mainC.AddCommandBuffer(CameraEvent.AfterDepthTexture, cb_markingIlsdands);



        albedo.SetActiveTexture(mainC);
    }
    // ======================================================================================================================
    // LOOP ---------------------------------------------------------------------------

    private void Update()
    {
        if (numberOfFrames > 2) mainC.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, cb_markingIlsdands);

        createMetalicGlossMap.SetTexture("_Smoothness", smoothness.runTimeTexture);
        createMetalicGlossMap.SetTexture("_MainTex", metalic.runTimeTexture);
        Graphics.Blit(metalic.runTimeTexture, metalicGlossMapCombined, createMetalicGlossMap);

        hue.color = new Color(1 - 0.033f * Time.time, 0.033f * Time.time, 1f/256f*Time.time);

        #region MyAddon (added by Alex)

        if(goal == null)
        {
            goal = GameObject.FindGameObjectWithTag("goal");
        }

        float distanceGoal = Vector3.Magnitude(mainC.transform.position - goal.transform.position);

        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.T) || ((distanceGoal < 3 || searchFinished) && !finished))
        {
            Texture mainTexture = meshGameobject.GetComponent<Renderer>().material.mainTexture;

            Texture2D texture2D = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);

            //Texture2D pngTex = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);

            RenderTexture currentRT = RenderTexture.active;

            RenderTexture renderTexture = new RenderTexture(mainTexture.width, mainTexture.height, 32);

            Graphics.Blit(mainTexture, renderTexture);

            RenderTexture.active = renderTexture;

            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            if (Input.GetKeyDown(KeyCode.T) || ((distanceGoal < 3 || searchFinished) && !finished))
            {
                //pngTex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                //pngTex.Apply();
                byte[] bytes = texture2D.EncodeToPNG();
                File.WriteAllBytes(rotationMeasurement.directory + "SavedScreen" + System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".png", bytes);
                //heatmapVisualization.material.mainTexture = pngTex;

                if (!searchFinished)
                    canvasScore.SetActive(true);

                for (int j = 0; j < averageGazeAreaOne.Count; j++)
                {
                    averageGazeAreaNumberOne += averageGazeAreaOne[j];
                }
                for (int j = 0; j < averageGazeAreaThree.Count; j++)
                {
                    averageGazeAreaNumberThree += averageGazeAreaThree[j];
                }
                for (int j = 0; j < averageGazeAreaFive.Count; j++)
                {
                    averageGazeAreaNumberFive += averageGazeAreaFive[j];
                }
                for (int j = 0; j < averageGazeAreaTen.Count; j++)
                {
                    averageGazeAreaNumberTen += averageGazeAreaTen[j];
                }

                averageGazeAreaNumberOne = averageGazeAreaNumberOne / averageGazeAreaOne.Count;
                averageGazeAreaNumberThree = averageGazeAreaNumberThree / averageGazeAreaThree.Count;
                averageGazeAreaNumberFive = averageGazeAreaNumberFive / averageGazeAreaFive.Count;
                averageGazeAreaNumberTen = averageGazeAreaNumberTen / averageGazeAreaTen.Count;


                textScore.text = "Kollisionen: " + collisionCounter.obstacleCollision + "\n Zeit: " + Time.time.ToString("F2") + "s \n Durchschnittliche Sehfläche: " + averageGazeAreaNumberThree.ToString("F2") + "% \n Gesamte Sehfläche: " + totalPercentage.ToString("F2") + "%";

                finished = true;
            }

            RenderTexture.active = currentRT;
        }

        if (Time.time > captureTimer && startMeasurement)
        {
            Texture mainTexture = meshGameobject.GetComponent<Renderer>().material.mainTexture;

            Texture2D texture2D = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);

            RenderTexture currentRT = RenderTexture.active;

            RenderTexture renderTexture = new RenderTexture(mainTexture.width, mainTexture.height, 32);

            Graphics.Blit(mainTexture, renderTexture);

            RenderTexture.active = renderTexture;

            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            Color[] pixels = texture2D.GetPixels();

            lastOneSecondPixels = 0;
            lastThreeSecondPixels = 0;
            lastFiveSecondPixels = 0;
            lastTenSecondPixels = 0;

            colorPercentage = 0;

            for (int i = 0; i < pixels.Length; i += (100 / readSteps))
            {
                if (pixels[i].r + pixels[i].g > 0.1f) // && pixels[i].b < 0.1f)
                {
                    colorPercentage += 1;
                }
                if (pixels[i].b > (Time.time - 1) / 256f && Time.time > 1)
                {
                    lastOneSecondPixels += 1;
                }
                if (pixels[i].b > (Time.time - 3) / 256f && Time.time > 3)
                {
                    lastThreeSecondPixels += 1;
                }
                if (pixels[i].b > (Time.time - 5) / 256f && Time.time > 5)
                {
                    lastFiveSecondPixels += 1;
                }
                if (pixels[i].b > (Time.time - 10) / 256f && Time.time > 10)
                {
                    lastTenSecondPixels += 1;
                }
                //print("Pixel color " + pixels[i]);

            }
            totalPercentage = colorPercentage * 100.00f / (pixels.Length / (100.00f / readSteps));
            lastOneSecondPercentage = lastOneSecondPixels * 100.00f / (pixels.Length / (100.00f / readSteps));
            lastThreeSecondPercentage = lastThreeSecondPixels * 100.00f / (pixels.Length / (100.00f / readSteps));
            lastFiveSecondPercentage = lastFiveSecondPixels * 100.00f / (pixels.Length / (100.00f / readSteps));
            lastTenSecondPercentage = lastTenSecondPixels * 100.00f / (pixels.Length / (100.00f / readSteps));
            if(Time.time > 1)
                averageGazeAreaOne.Add(lastOneSecondPercentage);
            if (Time.time > 3)
                averageGazeAreaThree.Add(lastThreeSecondPercentage);
            if (Time.time > 5)
                averageGazeAreaFive.Add(lastFiveSecondPercentage);
            if (Time.time > 10)
                averageGazeAreaTen.Add(lastTenSecondPercentage);
            if (!finished)
                print(totalPercentage.ToString("F2") + "% of the visual field were covered in total, " + lastThreeSecondPercentage.ToString("F2") + "% within the last 3 seconds.");
            RenderTexture.active = currentRT;

            captureTimer = Time.time + 1;
        }

        

        #endregion

        numberOfFrames++;

        // ----------------------------------------------------------------------------
        // This MUST be called to set up the painting with the mouse. 
        albedo    .UpdateShaderParameters(meshGameobject.transform.localToWorldMatrix);
        metalic   .UpdateShaderParameters(meshGameobject.transform.localToWorldMatrix);
        smoothness.UpdateShaderParameters(meshGameobject.transform.localToWorldMatrix);


        // ---------------------------------------------------------------------------
        // Setting up Mouse Parameters

        RaycastHit hit;
        Ray ray;
        if (toggleMouse)
        {
            ray = mainC.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            ray = FoveInterface.GetGazeRays().right;
        }

        Vector4    mwp = Vector3.positiveInfinity;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "PaintObject") 
            mwp = hit.point;
        }

        //mwp.w = Input.GetMouseButton(0)? 1 : 0;
        mwp.w = 1;

        mouseWorldPosition = mwp;
        Shader.SetGlobalVector("_Mouse", mwp);


        
    }

    public void GetPercentageCovered()
    {

    }

    // ======================================================================================================================
    // HELPER FUNCTIONS ---------------------------------------------------------------------------
    public void SetAlbedoActive()
    {
        metalic   .SetInactiveTexture(mainC);
        smoothness.SetInactiveTexture(mainC);
        albedo    .SetActiveTexture(mainC);
    }

    public void SetMetalicActive()
    {
        albedo    .SetInactiveTexture(mainC);
        smoothness.SetInactiveTexture(mainC);
        metalic   .SetActiveTexture(mainC);
    }

    public void SetGlossActive()
    {
        metalic   .SetInactiveTexture(mainC);
        albedo    .SetInactiveTexture(mainC);
        smoothness.SetActiveTexture(mainC);
    }
    
}


[System.Serializable]
public class PaintableTexture
{
    public  string        id;
    public  RenderTexture runTimeTexture;
    public  RenderTexture paintedTexture;

    public  CommandBuffer cb;

    private Material      mPaintInUV;
    private Material      mFixedEdges;
    private RenderTexture fixedIlsands;

    public PaintableTexture(Color clearColor, int width, int height, string id, 
        Shader sPaintInUV, Mesh mToDraw, Shader fixIlsandEdgesShader, RenderTexture markedIlsandes)
    {
        this.id        = id;

        runTimeTexture = new RenderTexture(width, height, 0)
        {
            anisoLevel = 0,
            useMipMap  = false,
            filterMode = FilterMode.Bilinear
        };

        paintedTexture = new RenderTexture(width, height, 0)
        {
            anisoLevel = 0,
            useMipMap  = false,
            filterMode = FilterMode.Bilinear
        };


        fixedIlsands   = new RenderTexture(paintedTexture.descriptor);

        Graphics.SetRenderTarget(runTimeTexture);
        GL.Clear(false, true, clearColor);
        Graphics.SetRenderTarget(paintedTexture);
        GL.Clear(false, true, clearColor);


        mPaintInUV  = new Material(sPaintInUV);
        if (!mPaintInUV.SetPass(0)) Debug.LogError("Invalid Shader Pass: " );
        mPaintInUV.SetTexture("_MainTex", paintedTexture);

        mFixedEdges = new Material(fixIlsandEdgesShader);
        mFixedEdges.SetTexture("_IlsandMap", markedIlsandes);
        mFixedEdges.SetTexture("_MainTex", paintedTexture);

        // ----------------------------------------------

        cb      = new CommandBuffer();
        cb.name = "TexturePainting"+ id;


        cb.SetRenderTarget(runTimeTexture);
        cb.DrawMesh(mToDraw, Matrix4x4.identity, mPaintInUV);

        cb.Blit(runTimeTexture, fixedIlsands, mFixedEdges);
        cb.Blit(fixedIlsands, runTimeTexture);
        cb.Blit(runTimeTexture, paintedTexture);
    
    }

    public void SetActiveTexture(Camera mainC)
    {
        mainC.AddCommandBuffer(CameraEvent.AfterDepthTexture, cb);
    }
    
    public void SetInactiveTexture(Camera mainC)
    {
        mainC.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, cb);
    }

    public void UpdateShaderParameters(Matrix4x4 localToWorld)
    {
        mPaintInUV.SetMatrix("mesh_Object2World", localToWorld); // Mus be updated every time the mesh moves, and also at start
    }
}

