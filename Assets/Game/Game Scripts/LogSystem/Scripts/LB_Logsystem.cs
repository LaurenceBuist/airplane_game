using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LaurenceBuist
{
    public class LB_Logsystem : Bolt.GlobalEventListener
    {
        public TMP_FontAsset font;
        public Transform verticalLayout;

        public override void OnEvent(PlayerJoinedEvent evnt)
        {
            Debug.Log("Count: " + verticalLayout.childCount);
            if (verticalLayout.childCount < 5)
            {
                createMessage(evnt.Message);
            }
            else
            {
                Destroy(verticalLayout.GetChild(0).GetComponent<GameObject>());
                createMessage(evnt.Message);
            }
        }
        
        public void createMessage(string text)
        {
            GameObject newText = new GameObject(text.Replace(" ", "-"), typeof(RectTransform));
            
            var newTextComp = newText.AddComponent<TextMeshProUGUI>();
            //newText.AddComponent<CanvasRenderer>();

            //Text newText = transform.gameObject.AddComponent<Text>();
            newTextComp.text = text;
            newTextComp.font = font;
            newTextComp.color = Color.white;
            newTextComp.fontSize = 36;
            newTextComp.alignment = TextAlignmentOptions.Right;
            
            newText.AddComponent<LB_DestroyThis>();
            
            newText.transform.SetParent(verticalLayout);
            
            newText.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        }
    }
}