using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TwoDRollDice : MonoBehaviour
{
    [SerializeField] public List<GameObject> dice;
    public GameObject menu;
    public int currentDie = 0;
    private bool menuBool = false;
    private static int scene = 0;
    // Start is called before the first frame update

    public void Roll()
    {
        Camera rollCam = GameObject.Find("Roll Camera").GetComponent<Camera>();

        Vector3 newDice = new Vector3(rollCam.transform.position.x, rollCam.transform.position.y + 5, rollCam.transform.position.z);

        Vector3 torque = new Vector3();
        torque.x = Random.Range(-200, 200);
        torque.y = Random.Range(-200, 200);
        torque.z = Random.Range(-200, 200);

        GameObject res = Instantiate(dice[currentDie], newDice , this.transform.rotation);
        res.transform.rotation = Quaternion.Euler(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
        if (currentDie == 1)
        {
            res.transform.localScale = new Vector3(20, 20, 20);
        }
        else if (currentDie == 5) {
            res.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        }
        else
        {
            res.transform.localScale = new Vector3(150, 150, 150);
        }
        Rigidbody rb = res.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddTorque(torque);
    }

    public void getDieRight() {
        if (currentDie < dice.Count - 1)
        {
            currentDie += 1;
            
        }
        else {
            currentDie = 0;
        }
    }

    public void getDieLeft() {
        if (currentDie > 0)
        {
            currentDie -=1;
        }
        else {
            currentDie = dice.Count - 1;
        }
    }

    public void setHamburgerActive() {
        menu.SetActive(true);
    }

    public void destroyDice() {
        GameObject[] dieArr = GameObject.FindGameObjectsWithTag("dice");
        foreach (GameObject die in dieArr) {
            Destroy(die);
        }
        
    }

    public void switchScenes()
    {
        if (scene == 0){
            scene = 1;
            SceneManager.LoadScene(1);
        }
        else if (scene == 1) {
            scene = 0;
            SceneManager.LoadScene(0);
        }
    }

    public void slideMenu() {
        if (menuBool)
        {
            menu.SetActive(false);
            menuBool = false;
        }
        else
        {
            menu.SetActive(true);
            menuBool = true;
        }

    }


    void Start()
    {
        //+ Random.Range(0f, 150f)
        //d20.transform.localRotation = Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)))
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
