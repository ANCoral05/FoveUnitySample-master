using Fove.Unity;
using UnityEngine;

public class LookAtTarget : FOVEBehavior
{
    public Collider my_collider;

    public NewSetNumbers newSetNumbers;

    public float TimeCounter = 0.4f;

    public AudioSource audioSource;

    public AudioClip audioClip;

    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        //my_collider = GetComponent<Collider>();

        audioSource = GameObject.FindGameObjectWithTag("AudioSourceBling").GetComponent<AudioSource>();

        cam = GameObject.FindObjectOfType<Camera>();

        newSetNumbers = FindObjectOfType<NewSetNumbers>();
    }

    // Update is called once per frame
    void Update()
    {
        if (FoveInterface.Gazecast(my_collider))
        {
            if (TimeCounter >= 0)
            {
                TimeCounter -= Time.deltaTime;
            }
        }
        else
        {
            TimeCounter = 0.4f;
        }

        //GetComponent<TextMesh>().color = new Vector4(0.4f - 0.5f * TimeCounter, 0.4f - 0.5f * TimeCounter, 0.4f - 0.5f * TimeCounter, 1);
        GetComponent<TextMesh>().color = new Vector4(0.6f + TimeCounter, 0.6f + TimeCounter, 1, 1);

        transform.localScale = new Vector3(0.04f + 0.01f * (0.4f - TimeCounter), 0.04f + 0.01f * (0.4f - TimeCounter), 0.04f + 0.01f * (0.4f - TimeCounter));

        //if ((FoveInterface.Gazecast(my_collider) && Input.GetKeyDown(KeyCode.UpArrow)) && int.Parse(GetComponent<TextMesh>().text) == GameObject.Find("Manager").GetComponent<NewSetNumbers>().TargetNumber)
        //{
        //    audioSource.PlayOneShot(audioClip);

        //    newSetNumbers.targetPosition.Add(transform.position);
        //    newSetNumbers.targetTime.Add(Time.time - newSetNumbers.startTime);

        //    gameObject.SetActive(false);
        //}
        //GetComponent<SphereCollider>().radius = 2 * 3.141f * Vector3.Distance(cam.transform.position, transform.position) * 1.1f / 360 * 80;
    }
}
