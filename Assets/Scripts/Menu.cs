using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
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
    private EventInstance menuMusic;

    private void Start()
    {
        currentMenu = playMenu;
        RuntimeManager.GetBus("bus:/SFX").setVolume(0.7f);
        RuntimeManager.GetBus("bus:/Music").setVolume(0.7f);

        menuMusic = RuntimeManager.CreateInstance("event:/Music/menu_music_loop");
        menuMusic.start();
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
        menuMusic.stop(STOP_MODE.ALLOWFADEOUT);
        menuMusic.release();
        SlidingDoors.instance.ClosedLoadScene("Level" + level);
    }

    public void SetSFXVolume(Slider volume)
    {
        RuntimeManager.GetBus("bus:/SFX").setVolume(volume.value);
    }

    public void SetMusicVolume(Slider volume)
    {
        RuntimeManager.GetBus("bus:/Music").setVolume(volume.value);
    }

    private IEnumerator SlideInNOut(RectTransform outMenu, RectTransform inMenu)
    {
        inMenu.gameObject.SetActive(true);
        inMenu.anchoredPosition = outPosition;

        foreach (Button button in outMenu.GetComponentsInChildren<Button>())
            button.interactable = false;

        foreach (Button button in inMenu.GetComponentsInChildren<Button>())
            button.interactable = true;

        yield return Utils.SlideUIObject(outMenu, outPosition, switchTime / 2, easeOut);
        yield return Utils.SlideUIObject(inMenu, Vector2.zero, switchTime / 2, easeIn);

        outMenu.gameObject.SetActive(false);
    }
}
