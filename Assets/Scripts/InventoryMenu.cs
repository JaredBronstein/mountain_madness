﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class InventoryMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject inventoryMenuItemTogglePrefab;

    [Tooltip("Content of the scrollview for the list of inventory items.")]
    [SerializeField]
    private Transform inventoryListContentArea;

    [Tooltip("Place in the UI for displaying the name of the selected inventory item")]
    [SerializeField]
    private Text itemLabelText;

    [Tooltip("Place in the UI for displaying info about the selected inventory item")]
    [SerializeField]
    private Text descriptionAreaText;

    private static InventoryMenu instance;
    private CanvasGroup canvasGroup;
    private FirstPersonController firstPersonController;
    private AudioSource audioSource;

    public static InventoryMenu Instance
    {
        get
        {
            if (instance == null)
                throw new System.Exception("There is currently no InventoryMenu instance, but you are trying to access it! Makes sure the InventoryMenu script is attached to something in the scene!");
            return instance;
        }
        private set { instance = value; }
    }

    private bool IsVisible => canvasGroup.alpha > 0;

    public void ExitMenuButtonClicked()
    {
        HideMenu();
    }

    /// <summary>
    /// Instantiates a new InventoryMenuItemToggle prefab and adds it to the menu
    /// </summary>
    /// <param name="inventoryObjectToAdd"></param>
    public void AddItemToMenu(InventoryObject inventoryObjectToAdd)
    {
        GameObject clone = Instantiate(inventoryMenuItemTogglePrefab, inventoryListContentArea);
        InventoryMenuItemToggle toggle = clone.GetComponent<InventoryMenuItemToggle>();
        toggle.AssociatedInventoryObject = inventoryObjectToAdd;
    }

    private void ShowMenu()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        firstPersonController.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        audioSource.Play();
    }

    private void HideMenu()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        firstPersonController.enabled = true;
        audioSource.Play();
    }

    /// <summary>
    /// This is the event handler for InventoryMenuItemSelected.
    /// </summary>
    private void OnInventoryMenuItemSelected(InventoryObject inventoryObjectThatWasSelected)
    {
        itemLabelText.text = inventoryObjectThatWasSelected.ObjectName;
        descriptionAreaText.text = inventoryObjectThatWasSelected.Description;
    }

    private void OnEnable()
    {
        InventoryMenuItemToggle.InventoryMenuItemSelected += OnInventoryMenuItemSelected;
    }

    private void OnDisable()
    {
        InventoryMenuItemToggle.InventoryMenuItemSelected -= OnInventoryMenuItemSelected;
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            throw new System.Exception("There is already an instance of InventoryMenu and there can only be one!");

        canvasGroup = GetComponent<CanvasGroup>();
        firstPersonController = FindObjectOfType<FirstPersonController>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        HideMenu();
        StartCoroutine(WaitForAudioClip());
    }

    private IEnumerator WaitForAudioClip()
    {
        float ogVolume = audioSource.volume;
        audioSource.volume = 0;
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.volume = ogVolume;
    }

    private void Update()
    {
        HandleInput();
    }
    private void HandleInput()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (IsVisible == false)
            {
                ShowMenu();
            }
            else
            {
                HideMenu();
            }
        }
    }
}
