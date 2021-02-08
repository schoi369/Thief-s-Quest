using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject shopScreen;
    public bool isShopOpen;

    public Text shopText;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenCloseShopScreen() {
        if (!isShopOpen) {
            isShopOpen = true;
            shopScreen.SetActive(true);
            Cursor.visible = true;
            Time.timeScale = 0f;
        } else {
            isShopOpen = false;
            shopScreen.SetActive(false);
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }
}
