using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class MenuScr : MonoBehaviour
{
    public static MenuScr Instance { set; get; }
    public GameObject mainMenu;
    public GameObject serverMenu;
    public GameObject connectMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;
    private void Start()
    {
        Instance = this;
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectButton()
    {
        mainMenu.SetActive(false);
        connectMenu.SetActive(true);
    }
    public void HostButton()
    {
        try
        {
            Server s = Instantiate(serverPrefab).GetComponent<Server>();
            s.Init();
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = "Y";
            c.isHost = true;
            c.ConnectToServer("127.0.0.1", 6321);

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        mainMenu.SetActive(false);
        serverMenu.SetActive(true);
    }
    public void ConnectToServerButton()
    {
        string hostAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if(hostAddress == "")
        {
            hostAddress = "127.0.0.1"; 
        }

        try
        {
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = "B";
            if(c.ConnectToServer(hostAddress, 6321))
                connectMenu.SetActive(false);// : connectMenu.SetActive(true);
            else
                Destroy(c.gameObject);
            
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public void BackButton()
    {
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);
        mainMenu.SetActive(true);


        Server s = FindObjectOfType<Server>();
        if (s != null)
        {
            s.EndHost();
            Destroy(s.gameObject);
        }
        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(c.gameObject);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MAP");
    }
    public void StartMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
