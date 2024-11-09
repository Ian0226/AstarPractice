using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    private Transform mainCanvas = null;
    private Button showTutorialBtn = null;
    private RectTransform tutorialPanel = null;

    private Vector3 tutorialPanelOriginPos;

    private bool tutorialIsDisplay = false;

    private Action panelControlAction = null;

    private void Awake()
    {
        mainCanvas = this.transform;
        showTutorialBtn = mainCanvas.Find("TutorialButton").GetComponent<Button>();
        tutorialPanel = mainCanvas.Find("TutorialPanel").GetComponent<RectTransform>();

        showTutorialBtn.onClick.AddListener(() => { 
            if(panelControlAction != null)
            {
                panelControlAction.Invoke();
            }
        });

        tutorialPanelOriginPos = tutorialPanel.position;

        panelControlAction = ControlTutorialPanel;
    }

    private void ControlTutorialPanel()
    {
        panelControlAction = null;
        if (!tutorialIsDisplay)
        {
            tutorialIsDisplay = true;
            showTutorialBtn.transform.GetChild(0).GetComponent<Text>().text = "收起操作教學 <<";
            StartCoroutine("PanelPosControl", showTutorialBtn.transform.position);
        }
        else
        {
            //tutorialPanel.position = new Vector3(tutorialPanelOriginX, tutorialPanel.position.y, tutorialPanel.position.z);
            tutorialIsDisplay = false;
            showTutorialBtn.transform.GetChild(0).GetComponent<Text>().text = "展開操作教學 >>";
            StartCoroutine("PanelPosControl", tutorialPanelOriginPos);
        }
    }

    IEnumerator PanelPosControl(Vector3 tartgetPos)
    {
        float currentX = 0.0f;
        while(MathF.Abs(tutorialPanel.position.x - tartgetPos.x) > 0.1f)
        {
            float xPos = Mathf.SmoothDamp(tutorialPanel.position.x, tartgetPos.x, ref currentX, 0.2f);
            tutorialPanel.position = new Vector3(xPos, tutorialPanel.position.y, tutorialPanel.position.z);
            yield return null;
        }
        panelControlAction = ControlTutorialPanel;
    }
}
