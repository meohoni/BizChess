using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickObjectHelper : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10))
            {
                GameObject obj = hit.collider.gameObject;  
                if(obj.CompareTag("Card"))
                {
                    Card card = obj.GetComponent<Card>();
                    card.OnClick();
                    print("Card is clicked.");
                }
            }
        }
    }
}
