using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : FOVEBehavior
{
    public enum LeftOrRight
    {
        Left,
        Right
    }

    [SerializeField]
    public LeftOrRight whichEye;

    public GameObject menuButton;

    public Image loadingBar;

    public GameObject subMenu;

    public GameObject settingsIcon;

    public TextMesh text;

    private float alphaValue;

    [Range(0, 0.1f)]
    public float bounceTime;

    [Range(0, 0.5f)]
    public float bounceStrength;

    public Vector2 MinMaxSize;

    private float growStat;

    public float[] bounceProcess;

    private int bounceStep;

    public float size;

    private float submenuSize;

    public GameObject[] icons;

    // Start is called before the first frame update
    void Start()
    {
        size = MinMaxSize.x;
    }

    // Update is called once per frame
    void Update()
    {
        var rays = FoveInterface.GetGazeRays();
        var ray = whichEye == LeftOrRight.Left ? rays.left : rays.right;

        float angle = Vector3.Angle(menuButton.transform.position - ray.origin, ray.direction);

        text.text = "Angle: " + angle;

        if (angle < 15)
        {
            if (alphaValue < 1)
            {
                alphaValue += Time.deltaTime * 5;
            }

            if (size < MinMaxSize.y)
            {
                size += Time.deltaTime * 5;
            }
            else { size = MinMaxSize.y; }
        }

        if ((angle >= 15 && loadingBar.fillAmount < 1) || (angle >= 26 && loadingBar.fillAmount >= 1))
        {
            if (alphaValue > 0.3f)
            {
                alphaValue -= Time.deltaTime * 5;
            }

            if (size > MinMaxSize.x)
            {
                size -= Time.deltaTime * 5;
            }
            else { size = MinMaxSize.x; }
        }

        if (size > MinMaxSize.x)
        {
            loadingBar.fillAmount += Time.deltaTime * 1.4f;
        }
        else { loadingBar.fillAmount = 0; }

        if (loadingBar.fillAmount >= 1 && submenuSize < 1)
        {
            submenuSize += Time.deltaTime * 10;
        }

        if (loadingBar.fillAmount < 1 && submenuSize > 0)
        {
            submenuSize -= Time.deltaTime * 10;
        }

        if (submenuSize < 0.5f)
        {
            subMenu.SetActive(false);
        }
        else { subMenu.SetActive(true); }

        if (submenuSize > 1)
        {
            submenuSize = 1;
        }

        if(submenuSize == 1)
        {
            foreach (GameObject icon in icons)
            {
                if(Vector3.Angle(icon.transform.position - ray.origin, ray.direction) < 5)
                {
                    icon.transform.GetChild(0).gameObject.SetActive(true);

                    icon.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
                }
                else
                {
                    icon.transform.GetChild(0).gameObject.SetActive(false);

                    icon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
            }
        }

        subMenu.transform.localScale = new Vector3(submenuSize, submenuSize, submenuSize);

        menuButton.transform.localScale = new Vector3(size, size, size);

        menuButton.GetComponent<Image>().color = new Color(1, 1, 1, alphaValue);

        settingsIcon.GetComponent<Image>().color = new Color(1, 1, 1, alphaValue);
    }
}
