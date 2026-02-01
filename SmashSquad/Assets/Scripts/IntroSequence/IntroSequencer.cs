using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroSequencer : MonoBehaviour
{
    public Image UoN_Logo;
    public Image LoadingSpinnerOuter;
    public Image LoadingSpinnerInner;
    public Image LoadingCanvasBackground;
    public TMP_Text LoadingText;

    private float UoN_IntroTimer = 0;
    private float FadeOutTimer = 0;
    private float LoadingFadeInTimer = 0;
    private int IntroStage = 0;

    private void Update()
    {
        if (IntroStage == Constants.INTRO_SEQU_START)
        {
            float Progress = UoN_IntroTimer / 1.5f;
            float Alpha = Mathf.SmoothStep(0, 1, Progress);
            float Zoom = Mathf.Lerp(0.9f, 1.1f, Progress);
            UoN_Logo.color = new Color(1, 1, 1, Alpha);
            UoN_Logo.transform.localScale = new Vector3(Zoom, Zoom, 0);

            if (UoN_IntroTimer > 1.5f)
            {
                if (Registry.MapLoaded == Constants.MAP_NOT_LOADED)
                {
                    IntroStage = Constants.INTRO_SEQU_WAIT_FOR_LOAD;
                }
                else
                {
                    Registry.CoreGameInfrastructureObject.LoadMenu(Constants.MAIN_MENU);
                    IntroStage = Constants.INTRO_SEQU_END;
                }
            }

            UoN_IntroTimer += Time.deltaTime;
        }
        else if (IntroStage == Constants.INTRO_SEQU_WAIT_FOR_LOAD)
        {
            float Progress = LoadingFadeInTimer / 0.25f;
            float Alpha = Mathf.SmoothStep(0, 1, Progress);
            Color LoadingColor = new Color(0, 0, 0, Alpha);
            LoadingSpinnerOuter.color = LoadingColor;
            LoadingSpinnerInner.color = LoadingColor;
            LoadingText.color = LoadingColor;
            if (Registry.MapLoaded != Constants.MAP_NOT_LOADED && LoadingFadeInTimer > 0.25f)
            {
                Registry.CoreGameInfrastructureObject.LoadMenu(Constants.MAIN_MENU);
                IntroStage = Constants.INTRO_SEQU_END;
            }

            LoadingFadeInTimer += Time.deltaTime;
        }
        else
        {
            float Progress = FadeOutTimer / 0.5f;
            float Alpha = Mathf.Lerp(1, 0, Progress);
            UoN_Logo.color = new Color(1, 1, 1, Alpha);
            LoadingCanvasBackground.color = new Color(0.8f, 0.8f, 0.8f, Alpha);
            if (LoadingFadeInTimer > 0)
            {
                Color LoadingColor = new Color(0, 0, 0, Alpha);
                LoadingSpinnerOuter.color = LoadingColor;
                LoadingSpinnerInner.color = LoadingColor;
                LoadingText.color = LoadingColor;
            }

            if (FadeOutTimer > 0.5f)
            {
                Destroy(gameObject);
            }

            FadeOutTimer += Time.deltaTime;
        }
    }
}