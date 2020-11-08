﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterStats))]
public class HealthUI : MonoBehaviour
{
    public GameObject uiPrefab;
    public Transform target;

    float visibleTime = 5f;
    float lastMadeVisibleTime;

    Transform ui;
    Image healthSlider;
    Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;

        foreach (Canvas c in FindObjectsOfType<Canvas>())
        {
            if (c.renderMode == RenderMode.WorldSpace)
            {
                ui = Instantiate(uiPrefab, c.transform).transform;
                healthSlider = ui.GetChild(0).GetComponent<Image>();
                ui.gameObject.SetActive(false);
                break;
            }
        }

        GetComponent<CharacterStats>().OnHPChanged += OnHPChanged;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (ui == null)
            return;

        ui.position = target.position;
        ui.forward = -cam.forward;
        
        if (Time.time - lastMadeVisibleTime > visibleTime)
        {
            ui.gameObject.SetActive(false);
        }
    }

    void OnHPChanged(int maxHP, int currentHP)
    {
        if (ui == null)
            return;

        ui.gameObject.SetActive(true);

        lastMadeVisibleTime = Time.time;

        float healthPercent = (float)currentHP / maxHP;
        healthSlider.fillAmount = healthPercent;

        if (healthPercent <= 0)
        {
            Destroy(ui.gameObject);
        }
    }
}
