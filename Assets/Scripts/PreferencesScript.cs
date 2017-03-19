using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;

public class PreferencesScript : Singleton<PreferencesScript> {

    public delegate void UnlockDelegate(Unlockable unlocked);
    public static event UnlockDelegate SomethingUnlockedEvent;
    public delegate void LevelUpDelegate(int level);
    public static event LevelUpDelegate LevelUpEvent;

    private const string FIRST_USE = "FirstUse";
    private const string TUTORIAL_COMPLETED = "TutCompleted";
    private const string GOOGLE_PLAY_AUTO_LOGIN = "GPAutoLogin";

    private void Awake() {
        EmojiSprites.LoadEmojiSprites();
        rewardPanel = FindObjectOfType<RewardPanelScript>();

        // If first use
        if (PlayerPrefs.GetString(FIRST_USE) == "") {
            ResetPreferences();
            PlayerPrefs.SetString(FIRST_USE, "IMDEADINSIDEPLSHELPME");
        }

        expBarScript = FindObjectOfType<ExpBarScript>();

        // Color mode
        currentMode = (ColorMode) Enum.Parse(typeof(ColorMode), PlayerPrefs.GetString(COLOR_MODE));
        currentTheme = ColorThemes.GetTheme(PlayerPrefs.GetString(THEME_MODE));
        UpdateSignResourceStrgColors();

        // Player level
        playerLevel = PlayerPrefs.GetInt(PLAYER_LEVEL);
        playerExp = PlayerPrefs.GetInt(PLAYER_EXP);

        for (int i = 2; i <= maxPlayerLevel; i++)
            expNeededForLevel[i] = ExpNeededForLevel(i);
        
        // Unlocks
        UpdateEmojiUnlocks();

        GPAchievements.Init();
    }
    
    new void OnDestroy() {
        base.OnDestroy();
        PlayerPrefs.Save();
    }

    public void ResetPreferences() {
        PlayerPrefs.SetString(COLOR_MODE, ColorMode.LIGHT.ToString());
        PlayerPrefs.SetString(THEME_MODE, "DefaultTheme");

        for (int i = 0; i < EmojiSprites.emojiPaths.Length; i++)
            PlayerPrefs.SetInt(IS_UNLOCKED + EmojiSprites.emojiPaths[i], 0);

        PlayerPrefs.SetString(EMOJI_NAME + "0", "smilingEmoji");
        PlayerPrefs.SetString(EMOJI_NAME + "1", "angryEmoji");
        PlayerPrefs.SetString(EMOJI_NAME + "2", "fistBumpEmoji");
        PlayerPrefs.SetString(EMOJI_NAME + "3", "smilingEmoji");
        PlayerPrefs.SetString(EMOJI_NAME + "4", "angryEmoji");
        PlayerPrefs.SetString(EMOJI_NAME + "5", "fistBumpEmoji");
        PlayerPrefs.SetInt(IS_UNLOCKED + "smilingEmoji", 1);
        PlayerPrefs.SetInt(IS_UNLOCKED + "angryEmoji", 1);
        PlayerPrefs.SetInt(IS_UNLOCKED + "fistBumpEmoji", 1);

        PlayerPrefs.SetInt(EMOJI_SLOT_COUNT, 3);

        PlayerPrefs.SetInt(TUTORIAL_COMPLETED, 0);
        PlayerPrefs.SetInt(GOOGLE_PLAY_AUTO_LOGIN, 0);

        PlayerPrefs.SetInt(PLAYER_LEVEL, 1);
        PlayerPrefs.SetInt(PLAYER_EXP, 0);

        PlayerPrefs.SetInt(SOUND_LEVEL, 100);
        PlayerPrefs.SetInt(MUSIC_LEVEL, 100);
        PlayerPrefs.Save();
    }

    public void ResetForDebugPrefs() {
        ResetPreferences();

        PlayerPrefs.SetInt(TUTORIAL_COMPLETED, 1);
        PlayerPrefs.SetInt(PLAYER_LEVEL, 20);
    }

    public bool IsTutorialCompleted() {
        return PlayerPrefs.GetInt(TUTORIAL_COMPLETED) == 1;
    }
    public void SetTutorialToCompleted() { PlayerPrefs.SetInt(TUTORIAL_COMPLETED, 1); }

    private IEnumerator ExecuteAfterSeconds(float seconds, Action action) {
        yield return new WaitForSeconds(seconds);

        action.Invoke();
    }

    /// <summary>
    /// Can player log in automatically?
    /// </summary>
    /// <returns></returns>
    public bool GPCanAutoLogin() {
        return PlayerPrefs.GetInt(GOOGLE_PLAY_AUTO_LOGIN) == 1;
    }
    /// <summary>
    /// Call when the player accepts to login automatically
    /// </summary>
    public void GPFromNowCanAutoLogin() {
        PlayerPrefs.SetInt(GOOGLE_PLAY_AUTO_LOGIN, 1);
    }

    // ________________________________________SOUND/MUSIC______________________________

    private const string MUSIC_LEVEL = "MusicLevel";
    private const string SOUND_LEVEL = "SoundLevel";

    /// <summary>
    /// 0 - 100
    /// </summary>
    /// <returns></returns>
    public int GetMusicVolume() { return PlayerPrefs.GetInt(MUSIC_LEVEL); }
    /// <summary>
    /// 0 - 100
    /// </summary>
    /// <returns></returns>
    public int GetSoundVolume() { return PlayerPrefs.GetInt(SOUND_LEVEL); }

    public float GetNomalizedMusicVolume() { return GetMusicVolume() / 100f; }
    public float GetNomalizedSoundVolume() { return GetSoundVolume() / 100f; }

    public void SetMusicVolume(int volume) {
        PlayerPrefs.SetInt(MUSIC_LEVEL, volume);
        MusicManager.Instance.SetVolume(volume / 100f);
    }
    public void SetSoundVolume(int volume) { PlayerPrefs.SetInt(SOUND_LEVEL, volume); }

    // ____________________________________UNLOCKS_____________________________________

    /// <summary>
    /// Concat a string at the end about which item you want the information
    /// </summary>
    private const string IS_UNLOCKED = "unlocked";
    private const string EMOJI_SLOT_COUNT = "EmojiSlotCount";

    public bool IsBluetoothUnlocked() { return PlayerLevel >= bluetoothUnlockAtLevel; }
    public bool IsLocalMultiUnlocked() { return PlayerLevel >= localMultiUnlockAtLevel; }
    public bool IsGPMultiUnlocked() { return PlayerLevel >= gpMultiUnlockAtLevel; }

    /// <summary>
    /// At which level bluetooth is unlocked
    /// </summary>
    public const int bluetoothUnlockAtLevel = 5;
    /// <summary>
    /// At which level local multi is unlocked
    /// </summary>
    public const int localMultiUnlockAtLevel = 2;
    /// <summary>
    /// At which level google play multi is unlocked
    /// </summary>
    public const int gpMultiUnlockAtLevel = 6;

    /// <summary>
    /// These correspond to the one in EmojiSprites
    /// </summary>
    private bool[] emojisUnlocked = new bool[EmojiSprites.emojiPaths.Length];

    /// <summary>
    /// Returns whether the emoji is unlocked based on it's name. It goes through the list to search so if you can go for the other function.
    /// </summary>
    public bool IsEmojiUnlocked(string name) {
        for (int i = 0; i < EmojiSprites.emojiPaths.Length; i++)
            if (EmojiSprites.emojiPaths[i] == name)
                return emojisUnlocked[i];

        return false;
    }

    private void UpdateEmojiUnlocks() {
        for (int i = 0; i < emojisUnlocked.Length; i++) {
            emojisUnlocked[i] = PlayerPrefs.GetInt(IS_UNLOCKED + EmojiSprites.emojiPaths[i]) == 1;
        }
    }

    /// <summary>
    /// Returns whether the emoji is unlocked based on it's id in EmojiSprites path array
    /// </summary>
    public bool IsEmojiUnlocked(int idInEmojiSprites) {
        return emojisUnlocked[idInEmojiSprites];
    }

    /// <summary>
    /// How many unlocks the player gets at level
    /// </summary>
    public int GetUnlockCountAtLevel(int level) {
        if (level <= 3) return 3;
        if (level <= 7) {
            if (level % 2 == 1) return 3;
            else return 2;
        }
        if (level <= 22) return 2;
        if (level <= 30) return 1;

        return 0;
    }

    /// <summary>
    /// Returns the three unlocks that the given level has
    /// </summary>
    public Unlockable[] GetUnlocks(int level) {
        // What kind of unlockables are left that should be given at random
        List<Unlockable> left = new List<Unlockable>();

        int unlockCountAtLevel = GetUnlockCountAtLevel(level);

        for (int i = 0; i < emojisUnlocked.Length; i++)
            if (!emojisUnlocked[i])
                left.Add(new Unlockable(UnlockableType.Emoji, EmojiSprites.emojiPaths[i]));

        // There are going to be 3 of them
        Unlockable[] unlock = new Unlockable[unlockCountAtLevel];
        int unlockAt = 0;

        // assign the ones that we have to
        switch (level) {
            case localMultiUnlockAtLevel:
                unlock[0] = new Unlockable(UnlockableType.LocalMulti, "");
                unlockAt++;
                break;
            case bluetoothUnlockAtLevel:
                unlock[0] = new Unlockable(UnlockableType.Bluetooth, "");
                unlockAt++;
                break;
            case 15:
            case 10:
            case 7:
                unlock[0] = new Unlockable(UnlockableType.EmojiSlot, "");
                unlockAt++;
                break;
            case gpMultiUnlockAtLevel:
                unlock[0] = new Unlockable(UnlockableType.GooglePlay, "");
                unlockAt++;
                break;
        }

        int fromLeftUnlockedNowCount = 0;
        while (unlockAt < unlockCountAtLevel && left.Count - fromLeftUnlockedNowCount != 0) {
            Unlockable ul;
            do {
                ul = left[UnityEngine.Random.Range(0, left.Count)];
            } while (ul.extra == "ITHASALREADYBEEN__!!/%/");

            fromLeftUnlockedNowCount++;
            unlock[unlockAt] = new Unlockable(ul);
            ul.extra = "ITHASALREADYBEEN__!!/%/";
            unlockAt++;
        }

        return unlock;
    }

    /// <summary>
    /// Unlocks all unlockabled given to it
    /// </summary>
    public void Unlock(Unlockable[] unlocks) {
        for (int i = 0; i < unlocks.Length; i++) {
            if (unlocks[i] != null) {
                if (SomethingUnlockedEvent != null) {
                    SomethingUnlockedEvent(unlocks[i]);
                }

                switch (unlocks[i].type) {
                    case UnlockableType.Emoji: PlayerPrefs.SetInt(IS_UNLOCKED + unlocks[i].extra, 1); break;
                    case UnlockableType.EmojiSlot:
                        int curr = PlayerPrefs.GetInt(EMOJI_SLOT_COUNT);
                        if (curr <= 6) {
                            PlayerPrefs.SetInt(EMOJI_SLOT_COUNT, curr + 1);
                        }
                        break;
                }
            }
        }

        UpdateEmojiUnlocks();
    }

    /// <summary>
    /// Does the job of GetUnlocks and Unlock as well
    /// </summary>
    public Unlockable[] GetUnlocksForLevelAndUnlock(int level) {
        Unlockable[] unlocks = GetUnlocks(level);
        Unlock(unlocks);

        return unlocks;
    }


    // ____________________________________LEVELS____________________________________

    private const string PLAYER_LEVEL = "PlayerLevel";
    private const string PLAYER_EXP = "PlayerExp";

    private RewardPanelScript rewardPanel;

    private int playerLevel;
    public int PlayerLevel { get { return playerLevel; } }

    private int playerExp;
    public int PlayerEXP { get { return playerExp; } }

    private ExpBarScript expBarScript;

    /// <summary>
    /// Return whether player has levelled up. If they did it automatically levels the player up.
    /// </summary>
    public bool AddEXP(int exp) {
        // If reached max player level
        if (playerLevel >= maxPlayerLevel) return false;

        // Animation
        expBarScript.AddExpAnimation(exp);

        playerExp += exp;

        if (playerExp > expNeededForLevel[playerLevel + 1]) {
            LevelUp();
            return true;
        }

        // At this point we know that we haven't levelled up
        expBarScript.UpdateCurrExp(playerExp, ExpForNextLevel(), false);

        PlayerPrefs.SetInt(PLAYER_EXP, playerExp);
        return false;
    }

    /// <summary>
    /// First pulls the exp bar down the adds exp. If it wasn't pulled down it will push it up
    /// </summary>
    /// <param name="exp"></param>
    public void PullExpBarThenAdd(int exp) {
        if (expBarScript.IsPulledDown) {
            AddEXP(exp);
        } else {
            expBarScript.PullDownExpBar(new DG.Tweening.TweenCallback(() => {
                StartCoroutine(ExecuteAfterSeconds(0.2f, new Action(() => {
                    AddEXP(exp);

                    StartCoroutine(ExecuteAfterSeconds(2f, new Action(() => {
                        expBarScript.PushUpExpBar();
                    })));
                })));
            }));
        }
    }

    /// <summary>
    /// Levels up the player. Updates the levels and the exp as well. Also writes to playerprefs.
    /// </summary>
    private void LevelUp() {
        if (playerLevel >= maxPlayerLevel) return;

        playerExp = playerExp - expNeededForLevel[playerLevel + 1];
        playerLevel++;

        expBarScript.UpdateLevelUpTexts(playerLevel, ExpForNextLevel(), playerExp);

        rewardPanel.LevelUpAnimation();

        PlayerPrefs.SetInt(PLAYER_LEVEL, playerLevel);
        PlayerPrefs.SetInt(PLAYER_EXP, playerExp);

        if (LevelUpEvent != null) LevelUpEvent(playerLevel);
    }

    private const int maxPlayerLevel = 30;
    public int MaxPlayerLevel { get { return maxPlayerLevel; } }

    /// <summary>
    /// Stores the calculated expneeded
    /// </summary>
    private int[] expNeededForLevel = new int[maxPlayerLevel + 1];
    /// <summary>
    /// Returns -1 if the level you given is not between 2 and the maxLevel (both included)
    /// </summary>
    public int ExpForLevel(int level) {
        if (level < 2 || level > maxPlayerLevel) return -1;

        return expNeededForLevel[level];
    }
    /// <summary>
    /// Returns how much exp is needed alles zusammen for the player to level up
    /// </summary>
    public int ExpForNextLevel() {
        return expNeededForLevel[playerLevel + 1];
    }
    /// <summary>
    /// Returns how much exp is left for the player to collect to level up
    /// </summary>
    public int ExpLeftForNextLevel() {
        return expNeededForLevel[playerLevel + 1] - playerExp;
    }

    /// <summary>
    /// This is only used for the first calculation at the start
    /// </summary>
    private int ExpNeededForLevel(int level) {
        if (level <= 1) return 0;

        if (level <= 7) {
            return 1000 + (level - 2) * 200;
        } else if (level <= maxPlayerLevel) {
            return 1600 + (int) (Mathf.Pow(level, 3f) * 1.5f);
        }

        return 0;
    }


    // _________________________Emojis_______________________________________________

    /// <summary>
    /// There are 4 emojis which can be chosen so after this you need to put 0...3
    /// </summary>
    private const string EMOJI_NAME = "EmojiName";

    public int EMOJI_COUNT {
        get {
            return PlayerPrefs.GetInt(EMOJI_SLOT_COUNT);
        }
    }
    

    public string[] GetEmojiNames() {
        string[] s = new string[EMOJI_COUNT];
        for (int i = 0; i < s.Length; i++)
            s[i] = PlayerPrefs.GetString(EMOJI_NAME + i);

        return s;
    }

    public Sprite[] GetEmojiSprites() {
        Sprite[] s = new Sprite[EMOJI_COUNT];
        for (int i = 0; i < s.Length; i++)
            s[i] = EmojiSprites.GetEmoji(PlayerPrefs.GetString(EMOJI_NAME + i));

        return s;
    }

    public string GetEmojiNameInSlot(int slot) {
        return PlayerPrefs.GetString(EMOJI_NAME + slot);
    }

    public Sprite GetEmojiSpriteInSlot(int slot) {
        return EmojiSprites.GetEmoji(PlayerPrefs.GetString(EMOJI_NAME + slot));
    }

    public void SetEmojiInSlotTo(int slot, string name) {
        PlayerPrefs.SetString(EMOJI_NAME + slot, name);
    }


    // ______________________Color mode variables_________________________________

    private const string COLOR_MODE = "ColorMode";
    private const string THEME_MODE = "ThemeMode";

    public ColorMode currentMode;

    /// <summary>
    /// How long tha changing animation should take
    /// </summary>
    private const float changeDuration = 0.5f;

    /// <summary>
    /// Delegate used for color changes
    /// </summary>
    public delegate void OnColorChange(ColorMode mode, float time);
    /// <summary>
    /// When we subscribe to this we can be sure that the color in SignResourceScript has already been changed
    /// </summary>
    public static OnColorChange ColorChangeEvent;

    // _______________________Which colors are chosen in colormode____________________________
    public ColorTheme currentTheme;

    /// <summary>
    /// Delegate used for theme changes
    /// </summary>
    public delegate void OnThemeChange(ColorTheme newTheme, float time);
    /// <summary>
    /// Subscribe to get notification when theme changes
    /// When we subscribe to this we can be sure that the color in SignResourceScript has already been changed
    /// </summary>
    public static OnThemeChange ThemeChangeEvent;



    public void ChangeToColorMode(ColorMode mode) {
        currentMode = mode;
        PlayerPrefs.SetString(COLOR_MODE, currentMode.ToString());

        UpdateSignResourceStrgColors(); // First update colors because some delegate listeners use it for simplicity
        ColorChangeEvent(mode, changeDuration); // Call delaegateategateggatagegatge
    }

    public void ChangeToColorTheme(ColorTheme newTheme, string nameOfTheme) {
        currentTheme = newTheme;
        PlayerPrefs.SetString(THEME_MODE, nameOfTheme + "Theme");

        UpdateSignResourceStrgColors(); // First update colors because some delegate listeners use it for simplicity
        ThemeChangeEvent(newTheme, changeDuration);// Call delaegateategateggasdasdsfeewedscxycasaatagegatge
    }
	
    private void UpdateSignResourceStrgColors() {
        SignResourceStorage.Instance.ChangeToColorMode(currentTheme.GetXColorOfMode(currentMode), currentTheme.GetOColorOfMode(currentMode));
    }


    public enum ColorMode {
        DARK, LIGHT
    }

    /// <summary>
    /// Which color theme is chosen in ColorMode
    /// </summary>
    public struct ColorTheme {
        public Color xColorLight;
        public Color oColorLight;

        public Color xColorDark;
        public Color oColorDark;

        public string themeName;

        public ColorTheme(Color xColorLight, Color oColorLight, Color xColorDark, Color oColorDark, string themeName) {
            this.xColorDark = xColorDark;
            this.oColorDark = oColorDark;
            this.xColorLight = xColorLight;
            this.oColorLight = oColorLight;
            this.themeName = themeName;
        }

        public Color GetXColorOfMode(ColorMode mode) {
            switch (mode) {
                case ColorMode.DARK: return xColorDark;
                case ColorMode.LIGHT: return xColorLight;
            }
            return Color.red;
        }

        public Color GetOColorOfMode(ColorMode mode) {
            switch (mode) {
                case ColorMode.DARK: return oColorDark;
                case ColorMode.LIGHT: return oColorLight;
            }
            return Color.blue;
        }
    }
}

public class GPAchievements {
    public const string achievementFirstWin = "CgkIu5mLkuoXEAIQAg";

    public const string achievementLocalMultiplayerMode = "CgkIu5mLkuoXEAIQAw"; // Implemented
    public const string achievementBluetoothMode = "CgkIu5mLkuoXEAIQBA"; // Implemented
    public const string achievementGooglePlayMultiplayerMode = "CgkIu5mLkuoXEAIQBQ"; // Implemented

    public const string achievementLevel5 = "CgkIu5mLkuoXEAIQDw"; // Implemented
    public const string achievementLevel15 = "CgkIu5mLkuoXEAIQEA"; // Implemented
    public const string achievementLevel30 = "CgkIu5mLkuoXEAIQEQ"; // Implemented
    public const string achievementLevel50 = "CgkIu5mLkuoXEAIQEg"; // Implemented

    public const string achievementTenGamesAgainstAi = "CgkIu5mLkuoXEAIQBg"; // Implemented
    public const string achievementFiftyGamesAgainstAi = "CgkIu5mLkuoXEAIQBw"; // Implemented
    public const string achievement100GamesAgainstAi = "CgkIu5mLkuoXEAIQCA"; // Implemented
    public const string achievement1000GamesAgainstTheAi = "CgkIu5mLkuoXEAIQCQ"; // Implemented

    public const string achievement10WinsAgainstTheAi = "CgkIu5mLkuoXEAIQCg"; // Implemented
    public const string achievement50WinsAgainstTheAi = "CgkIu5mLkuoXEAIQCw"; // Implemented
    public const string achievement100WinsAgainstTheAi = "CgkIu5mLkuoXEAIQDQ"; // Implemented
    public const string achievement250WinsAgainstTheAi = "CgkIu5mLkuoXEAIQDA"; // Implemented
    public const string achievement1000WinsAgainstTheAi = "CgkIu5mLkuoXEAIQDg"; // Implemented

    public const string achievementWin50TimesAgainstHardAi = "CgkIu5mLkuoXEAIQFg"; // Implemented
    public const string achievementWin250TimesAgainstTheHardAi = "CgkIu5mLkuoXEAIQFw"; // Implemented
    public const string achievementWin1000TimesAgainstTheHardAi = "CgkIu5mLkuoXEAIQGA"; // Implemented

    public const string achievementPlay50Games = "CgkIu5mLkuoXEAIQHQ"; // Implemented
    public const string achievementPlay100Games = "CgkIu5mLkuoXEAIQHg"; // Implemented
    public const string achievementPlay500Games = "CgkIu5mLkuoXEAIQHw"; // Implemented
    public const string achievementPlay1000Games = "CgkIu5mLkuoXEAIQIA"; // Implemented
    public const string achievementPlay5000Games = "CgkIu5mLkuoXEAIQIQ"; // Implemented
    public const string achievementPlay10000Games = "CgkIu5mLkuoXEAIQIg"; // Implemented

    public const string achievementTheSuperiorColorMode = "CgkIu5mLkuoXEAIQGQ";
    public const string achievementApprenticeSpammer = "CgkIu5mLkuoXEAIQEw";
    public const string achievementMasterSpammer = "CgkIu5mLkuoXEAIQFA";

    public const string achievementFabulous = "CgkIu5mLkuoXEAIQFQ"; // Implemented
    public const string achievementIsItOnlyAnEggplantwinkwink = "CgkIu5mLkuoXEAIQGg"; // Implemented
    public const string achievementrekt = "CgkIu5mLkuoXEAIQGw"; // Implemented
    public const string achievementNoobnoobnoobnoobnoobnoobnoob = "CgkIu5mLkuoXEAIQHA"; // Implemented

    private const string allAchievementsUnlocked = "AllAchievementsUnlocked";

    /// <summary>
    /// Called from playerprefs awake. Inits this class, so everything connected to achievements stays here and they are not littered all over the other classes
    /// </summary>
    public static void Init() {
        PreferencesScript.SomethingUnlockedEvent += NewUnlock;
        PreferencesScript.LevelUpEvent += LevelUp;
        ScaneManager.OnScreenChange += ScreenChanged;
    }

    /// <summary>
    /// Called when scene is changed
    /// </summary>
    private static void ScreenChanged(string from, string to) {
        TTTGameLogic gameLogic = GameObject.FindObjectOfType<TTTGameLogic>();
        ClientCellStorage clientCellStrg = GameObject.FindObjectOfType<ClientCellStorage>();

        // if we found one of those abve we are on a game screen
        if (gameLogic != null) {
            if (to == "GameAI") {
                gameLogic.SomeoneWonGameEvent += GameWonAI;
            } else {
                gameLogic.SomeoneWonGameEvent += GameWon;
            }
        } else if (clientCellStrg != null) {
            ClientCellStorage.SomeoneWonEvent += GameWon;
        }
    }

    /// <summary>
    /// Called when someone won the game
    /// </summary>
    private static void GameWon(Cell.CellOcc type) {
        // Player hasn't won yet
        if (PlayerPrefs.GetInt(achievementFirstWin) == 0) {
            UnlockAchievement(achievementFirstWin);
        }

        IncrementAchievement(achievementPlay50Games);
        IncrementAchievement(achievementPlay100Games);
        IncrementAchievement(achievementPlay500Games);
        IncrementAchievement(achievementPlay1000Games);
        IncrementAchievement(achievementPlay5000Games);
        IncrementAchievement(achievementPlay10000Games);
    }

    /// <summary>
    /// Called when someone won the game on AI screen
    /// </summary>
    private static void GameWonAI(Cell.CellOcc type) {
        AIScript ai = GameObject.FindObjectOfType<AIScript>();

        IncrementAchievement(achievementTenGamesAgainstAi);
        IncrementAchievement(achievementFiftyGamesAgainstAi);
        IncrementAchievement(achievement100GamesAgainstAi);
        IncrementAchievement(achievement1000GamesAgainstTheAi);

        // Player won
        if (type != AIScript.AIType) {
            IncrementAchievement(achievement10WinsAgainstTheAi);
            IncrementAchievement(achievement50WinsAgainstTheAi);
            IncrementAchievement(achievement100WinsAgainstTheAi);
            IncrementAchievement(achievement250WinsAgainstTheAi);
            IncrementAchievement(achievement1000WinsAgainstTheAi);

            // won against hard difficulty
            if (ai.Difficulty == 3) {
                IncrementAchievement(achievementWin50TimesAgainstHardAi);
                IncrementAchievement(achievementWin250TimesAgainstTheHardAi);
                IncrementAchievement(achievementWin1000TimesAgainstTheHardAi);
            }
        }

        GameWon(type);
    }

    /// <summary>
    /// Called when player levels up
    /// </summary>
    private static void LevelUp(int level) {
        switch (level) {
            case 5: UnlockAchievement(achievementLevel5); break;
            case 15: UnlockAchievement(achievementLevel15); break;
            case 30: UnlockAchievement(achievementLevel30); break;
            case 50: UnlockAchievement(achievementLevel50); break;
        }
    }

    /// <summary>
    /// Called when something is unlocked
    /// </summary>
    private static void NewUnlock(Unlockable unlock) {
        switch (unlock.type) {
            case UnlockableType.Bluetooth: UnlockAchievement(achievementBluetoothMode); break;
            case UnlockableType.LocalMulti: UnlockAchievement(achievementLocalMultiplayerMode); break;
            case UnlockableType.GooglePlay: UnlockAchievement(achievementGooglePlayMultiplayerMode); break;
            case UnlockableType.Emoji:
                switch (unlock.extra) {
                    case EmojiSprites.UnicornEmoji: UnlockAchievement(achievementFabulous); break;
                    case EmojiSprites.EggplantEmoji: UnlockAchievement(achievementIsItOnlyAnEggplantwinkwink); break;
                    case EmojiSprites.RektEmoji: UnlockAchievement(achievementrekt); break;
                    case EmojiSprites.NoobEmoji: UnlockAchievement(achievementNoobnoobnoobnoobnoobnoobnoob); break;
                }
                break;
        }
    }

    public static void IncrementAchievement(string name, int amount = 1) {
        PlayGamesPlatform.Instance.IncrementAchievement(name, amount, null);
        PlayerPrefs.SetInt(name, PlayerPrefs.GetInt(name) + amount);
    }

    public static void UnlockAchievement(string name) {
        // we don't already have it there
        if (!DoesAllAchievementsContain(name)) {
            PlayerPrefs.SetString(allAchievementsUnlocked, PlayerPrefs.GetString(allAchievementsUnlocked) + "|" + name);
            PlayerPrefs.SetInt(name, 1);

            Social.ReportProgress(name, 100, (bool success) => { });
        }
    }

    public static string[] GetAllUnlockedAchievemnts() {
        return PlayerPrefs.GetString(allAchievementsUnlocked).Split('|');
    }

    private static bool DoesAllAchievementsContain(string name) {
        string[] splitAll = GetAllUnlockedAchievemnts();

        for (int i = 0; i < splitAll.Length; i++)
            if (splitAll[i] == name)
                return true;

        return false;
    }
}

public class Unlockable {
    public UnlockableType type;
    public string extra;

    public Unlockable(UnlockableType type, string extra) {
        this.type = type;
        this.extra = extra;
    }

    public Unlockable(Unlockable other) {
        this.type = other.type;
        this.extra = other.extra;
    }
}

public enum UnlockableType {
    Emoji, Bluetooth, LocalMulti, EmojiSlot, GooglePlay
}