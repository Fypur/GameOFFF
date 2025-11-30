using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private float switchTime = 1f;
    [SerializeField] private Vector2 outPosition = new Vector2(-750, 0);
    [SerializeField] private RectTransform playMenu;
    [SerializeField] private RectTransform optionsMenu;
    [SerializeField] private RectTransform creditsMenu;
    [SerializeField] private RectTransform levelsMenu;
    [SerializeField] private Ease.EaseType easeIn;
    [SerializeField] private Ease.EaseType easeOut;

    private RectTransform currentMenu;

    private void Start()
    {
        currentMenu = playMenu;
        RuntimeManager.GetBus("SFX:/").setVolume(0.7f);
        RuntimeManager.GetBus("music:/").setVolume(0.7f);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }

    public void OpenOptionsMenu()
    {
        StopAllCoroutines();
        StartCoroutine(SlideInNOut(currentMenu, optionsMenu));

        currentMenu = optionsMenu;
        Utils.AudioPlay("event:/Menu UI/button_click");
    }

    public void OpenPlayMenu()
    {
        StopAllCoroutines();
        StartCoroutine(SlideInNOut(currentMenu, playMenu));

        currentMenu = playMenu;
        Utils.AudioPlay("event:/Menu UI/button_back");
    }

    public void OpenLevelMenu()
    {
        StopAllCoroutines();
        StartCoroutine(SlideInNOut(currentMenu, levelsMenu));

        currentMenu = levelsMenu;
        Utils.AudioPlay("event:/Menu UI/button_click");
    }

    public void OpenCreditsMenu()
    {
        StopAllCoroutines();
        StartCoroutine(SlideInNOut(currentMenu, creditsMenu));

        currentMenu = creditsMenu;
        Utils.AudioPlay("event:/Menu UI/button_click");
    }

    public void PlayLevel(int level)
    {
        Utils.AudioPlay("event:/Menu UI/button_click");
        SlidingDoors.instance.ClosedLoadSceneOpen("Level" + level);
    }

    public void SetSFXVolume(Slider volume)
    {
        RuntimeManager.GetBus("SFX:/").setVolume(volume.value);
    }

    public void SetMusicVolume(Slider volume)
    {
        RuntimeManager.GetBus("music:/").setVolume(volume.value);
    }

    private IEnumerator SlideInNOut(RectTransform outMenu, RectTransform inMenu)
    {
        inMenu.gameObject.SetActive(true);
        inMenu.anchoredPosition = outPosition;

        foreach (Button button in outMenu.GetComponentsInChildren<Button>())
            button.interactable = false;

        yield return Utils.SlideUIObject(outMenu, outPosition, switchTime / 2, easeOut);
        yield return Utils.SlideUIObject(inMenu, Vector2.zero, switchTime / 2, easeIn);

        foreach (Button button in outMenu.GetComponentsInChildren<Button>())
            button.interactable = true;

        outMenu.gameObject.SetActive(false);
    }
}
