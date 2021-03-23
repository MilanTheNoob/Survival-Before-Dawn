using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine;

public class GPlayManager : MonoBehaviour
{
    public static GPlayManager instance;
    public static bool access = false;

    public static string PlayerName;
    public static string PlayerID;
    public static Sprite PlayerImage;

    void Awake() 
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        transform.parent = null;
        DontDestroyOnLoad(this);

        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (success) =>
        {
            if (success == SignInStatus.Success)
            {
                PlayerName = Social.localUser.userName;
                PlayerID = Social.localUser.id;

                ProfileManager.instance.ProfileName.text = PlayerName;
                ProfileManager.instance.PSetupName.text = "Name - " + PlayerName;

                StartCoroutine(GetProfileImage());

                if (!SavingManager.SaveData.GPlayData.FirstTime)
                {
                    Social.ReportProgress(GPGSIds.achievement_you_new_here, 100f, null);
                    SavingManager.SaveData.GPlayData.FirstTime = true;
                }
                if (int.Parse(System.DateTime.Now.ToString("H")) <= 6 && !SavingManager.SaveData.GPlayData.Morning)
                {
                    Social.ReportProgress(GPGSIds.achievement_mornin_to_you_too, 100f, null);
                    SavingManager.SaveData.GPlayData.Morning = true;
                }

                access = true;
                StartCoroutine(IncrementTime());
            }
            else
            {
                PreviewInputManager.ShowError("Google Play Games login failed, Multiplayer will be unavailable");
                access = false;

                ProfileManager.instance.ProfileName.text = "Name Unavailable";
                ProfileManager.instance.PSetupName.text = "Name - Unavailable";
            }
        });
    }

    IEnumerator GetProfileImage()
    {
        while (Social.localUser.image == null)
        {
            yield return new WaitForSeconds(0.2f);
        }
        PlayerImage = Sprite.Create(Social.localUser.image, new Rect(0, 0, Social.localUser.image.width, Social.localUser.image.height), new Vector2(0.5f, 0.5f));
        ProfileManager.instance.ProfileImage.sprite = PlayerImage;
    }

    #region Actions

    public void EarlyBird()
    {
        if (!SavingManager.SaveData.GPlayData.EarlyBird)
        {
            Social.ReportProgress(GPGSIds.achievement_early_bird, 100f, null);
            SavingManager.SaveData.GPlayData.EarlyBird = true;
        }
    }

    public void RIP()
    {
        if (!SavingManager.SaveData.GPlayData.RIP)
        {
            Social.ReportProgress(GPGSIds.achievement_rip, 100f, null);
            SavingManager.SaveData.GPlayData.RIP = true;
        }
    }

    public void GetRuby()
    {
        if (!SavingManager.SaveData.GPlayData.RubyGem)
        {
            Social.ReportProgress(GPGSIds.achievement_its_beautiful, 100f, null);
            SavingManager.SaveData.GPlayData.RubyGem = true;
        }
    }

    public void BuySkin()
    {
        if (!SavingManager.SaveData.GPlayData.BuySkin)
        {
            Social.ReportProgress(GPGSIds.achievement_daymn_that_skin_looks_good_on_you, 100f, null);
            SavingManager.SaveData.GPlayData.BuySkin = true;
        }
    }

    IEnumerator IncrementTime()
    {
        while (SavingManager.SaveData.GPlayData.GameTime <= 301)
        {
            yield return new WaitForSeconds(60f);

            SavingManager.SaveData.GPlayData.GameTime++;
            if (SavingManager.SaveData.GPlayData.GameTime < 60)
            {
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_daymn_u_still_here_bro, 1, null);
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_some_dedication_right_here, 1, null);
            }
            else if (SavingManager.SaveData.GPlayData.GameTime == 60)
            {
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_daymn_u_still_here_bro, 1, null);
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_some_dedication_right_here, 1, null);
                SavingManager.SaveData.GPlayData.OneHour = true;
            }
            else if (SavingManager.SaveData.GPlayData.GameTime < 300)
            {
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_some_dedication_right_here, 1, null);
            }
            else if (SavingManager.SaveData.GPlayData.GameTime == 300)
            {
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_some_dedication_right_here, 1, null);
                SavingManager.SaveData.GPlayData.FiveHour = true;
            }

            if (SavingManager.SaveData.GPlayData.LocalExploredChunks > SavingManager.SaveData.GPlayData.ExploredChunks)
            {
                Social.ReportScore(SavingManager.SaveData.GPlayData.LocalExploredChunks, GPGSIds.leaderboard_chunks_explored, (success) =>
                {
                    if (success)
                    {
                        SavingManager.SaveData.GPlayData.ExploredChunks = SavingManager.SaveData.GPlayData.LocalExploredChunks;
                    }
                });
            }
        }
    }

    public void AddExploredChunks()
    {
        SavingManager.SaveData.GPlayData.LocalExploredChunks++;
    }

    public void AddCraftedRubies()
    {
        SavingManager.SaveData.GPlayData.LocalCraftedRubies++;

        Social.ReportScore(SavingManager.SaveData.GPlayData.LocalCraftedRubies, GPGSIds.leaderboard_rubies_crafted, (success) =>
        {
            if (success)
            {
                SavingManager.SaveData.GPlayData.CraftedRubies = SavingManager.SaveData.GPlayData.LocalCraftedRubies;
            }
        });
    }

    public void AddDeath()
    {
        SavingManager.SaveData.GPlayData.LocalDeaths++;

        Social.ReportScore(SavingManager.SaveData.GPlayData.LocalDeaths, GPGSIds.leaderboard_deaths, (success) =>
        {
            if (success)
            {
                SavingManager.SaveData.GPlayData.Deaths = SavingManager.SaveData.GPlayData.LocalDeaths;
            }
        });
    }

    public void ShowLeaderboards() { Social.ShowLeaderboardUI(); }
    public void ShowAchievements() { Social.ShowAchievementsUI(); }

    #endregion
}
