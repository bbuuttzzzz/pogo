using Inputter;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WizardUI;
using WizardUtils;
using WizardUtils.Equipment;
using WizardUtils.Math;
using WizardUtils.Tools;

namespace Pogo.Challenges
{
    public class ChallengeBuilder : MonoBehaviour
    {
        public LevelManifest ValidLevels;

        [NonSerialized]
        public Challenge CurrentChallenge;
        private string currentCode;
        private bool codeIsValid;
        public string CurrentCode { get => currentCode; set => currentCode = value; }

        public UnityEvent<Challenge> OnChallengeChanged;
        public UnityEvent<string> OnCodeChanged;

        public PauseMenuController PauseMenu;
        public ToggleableUIElement ChallengeMenu;

        public Trigger ChallengePickup;

        public EquipmentDescriptor ChallengeStick;

        public UnityEvent OnChallengeComplete;
        public UnityEvent OnChallengeReset;

        public ToggleableUIElement StartChallengeMenu;

        public UIElementSpawner PopupSpawner;

        public GameObject ClearCheckPrefab;
        public GameObject SilverMedalPrefab;
        public GameObject GoldMedalPrefab;

        public void Start()
        {
            GameManager.GameInstance.OnSoftQuit += OnReturnToMainMenu;

            OnDecodeFailed?.AddListener((reason) => Debug.LogWarning($"Failed to decode challenge: {reason}"));
        }

        public void Update()
        {
            bool balloonPressed = InputManager.CheckKeyDown(KeyName.Balloon);

            if (CurrentChallenge == null
                && PogoGameManager.PogoInstance.CustomRespawnActive
                && PogoGameManager.PogoInstance.CurrentDifficulty == PogoGameManager.Difficulty.Freeplay
                && InputManager.CheckKeyDown(KeyName.Balloon))
            {
                PromptForNewChallenge();
            }
            if (PogoGameManager.PogoInstance.CurrentDifficulty == PogoGameManager.Difficulty.Challenge
                && InputManager.CheckKeyDown(KeyName.Balloon))
            {
                OpenChallengeMenu();
            }
        }

        public void SetCode(string code)
        {
            SetCode(code, false);
        }

        public void SetCode(string code, bool codeIsValid)
        {
            if (CurrentCode == code) return;

            CurrentCode = code;
            this.codeIsValid = codeIsValid;
            OnCodeChanged?.Invoke(CurrentCode);
        }

        private void OnReturnToMainMenu(object sender, EventArgs e)
        {
            ExitChallenge();
        }

        private void OpenChallengeMenu()
        {
            PauseMenu.OverrideMenu = ChallengeMenu;
            PauseMenu.Pause();
            PauseMenu.OverrideMenu = null;
        }

        private void PromptForNewChallenge()
        {
            PauseMenu.OverrideMenu = StartChallengeMenu;
            PauseMenu.Pause();
            PauseMenu.OverrideMenu = null;
        }

        public void CalculateNewChallenge()
        {
            CurrentChallenge = CreateChallenge();
            CurrentCode = null;
            codeIsValid = false;
            OnChallengeChanged?.Invoke(CurrentChallenge);
            OnCodeChanged?.Invoke(CurrentCode);
        }

        public void CompleteChallenge()
        {
            var data = CurrentChallenge.FinishAttempt();
            if (data.NewTimeBetter)
            {
                if (CurrentChallenge.BestTimeMS < 60_000)
                {

                    if (data.GoldMedalEarned)
                    {
                        PopupSpawner.SpawnPrefab(GoldMedalPrefab);
                    }
                    else if (data.FirstClear)
                    {
                        if (CurrentChallenge.ChallengeType == Challenge.ChallengeTypes.Create)
                        {
                            PopupSpawner.SpawnPrefab(ClearCheckPrefab);
                        }
                        else if (CurrentChallenge.ChallengeType == Challenge.ChallengeTypes.PlayDeveloper)
                        {
                            PopupSpawner.SpawnPrefab(SilverMedalPrefab);
                        }
                    }
                    CurrentCode = EncodeChallenge(CurrentChallenge);
                    codeIsValid = true;
                    OnCodeChanged?.Invoke(CurrentCode);
                }
                OnChallengeChanged?.Invoke(CurrentChallenge);
            }
            OnChallengeComplete?.Invoke();
        }

        public Challenge CreateChallenge()
        {
            if (!PogoGameManager.GameInstanceIsValid())
            {
                throw new MissingComponentException("No valid game manager");
            }

            PogoGameManager pogoInstance = PogoGameManager.PogoInstance;
            var startTransform = pogoInstance.GetRespawnTransform();
            var level = pogoInstance.RealTargetRespawnLevel ?? pogoInstance.LevelManager.CurrentLevel;
            var endPoint = pogoInstance.Player.transform.position;
            return new Challenge(level, startTransform, endPoint);
        }

        public void ExitChallenge()
        {
            ChallengePickup.transform.position = new Vector3(0, -20, 0);
            PauseMenu.OverrideMenu = null;
            CurrentChallenge = null;
            OnChallengeChanged?.Invoke(CurrentChallenge);
            PogoGameManager.PogoInstance.OnPlayerDeath.RemoveListener(resetChallenge);
        }

        public void LoadChallenge()
        {
            PogoGameManager pogoInstance = PogoGameManager.PogoInstance;
            pogoInstance.Equip(ChallengeStick);
            UnityAction finishLoading = null;
            finishLoading = () =>
            {
                pogoInstance.CustomCheckpoint.Place(CurrentChallenge.StartPoint, CurrentChallenge.StartRotation);
                pogoInstance.RegisterRespawnPoint(pogoInstance.CustomCheckpoint.transform);
                ChallengePickup.transform.position = CurrentChallenge.EndPoint;
                PogoGameManager.PogoInstance.OnPlayerDeath.AddListener(resetChallenge);
                PogoGameManager.PogoInstance.ResetPlayer();
                resetChallenge();
                OnChallengeChanged?.Invoke(CurrentChallenge);
                pogoInstance.OnLevelLoaded.RemoveListener(finishLoading);
                Debug.Log("Finished loading challenge");
            };
            pogoInstance.OnLevelLoaded.AddListener(finishLoading);
            pogoInstance.LoadLevel(CurrentChallenge.Level, new LevelLoadingSettings() { ForceReload = true, InstantChangeAtmosphere = true });
        }

        private void resetChallenge()
        {
            CurrentChallenge?.StartAttempt();
            OnChallengeReset?.Invoke();
        }

        const string composeTweetLinkHeader = "https://twitter.com/compose/tweet?text=";
        public static readonly string[] TweetFormats =
        {
@"Find my balloon in #pogo3dballoons  
Code: {0}
My Best Time: {1:N3} seconds",

@"Beat my time in #pogo3dballoons  
Code: {0}
My Best Time: {1:N3} seconds",

@"Try my challenge in #pogo3dballoons  
Code: {0}
My Best Time: {1:N3} seconds"
        };
        public void TweetChallenge()
        {
            if (CurrentChallenge.PersonalBestTimeMS >= 60_000 && CurrentChallenge.BestTimeMS >= 60_000)
            {
                OnDecodeFailed?.Invoke(DecodeFailReason.CantShareUncleared);
                return;
            }
            if (CurrentCode == null || CurrentCode == "" || CurrentChallenge == null || !codeIsValid)
            {
                OnDecodeFailed?.Invoke(DecodeFailReason.CantShareInvalid);
                return;
            }

            string format = TweetFormats[UnityEngine.Random.Range(0, TweetFormats.Length)];

            float bestTime = CurrentChallenge.PersonalBestTime >= 60_000 ? CurrentChallenge.BestTimeMS : CurrentChallenge.PersonalBestTime;
            string formattedTweet = string.Format(format, CurrentCode, bestTime);
            string link = composeTweetLinkHeader + UnityEngine.Networking.UnityWebRequest.EscapeURL(formattedTweet);
            Application.OpenURL(link);
        }

        #region Encoding
        public const int PayloadLength =
            // StartPointCM
            sizeof(short) * 3
            // EndPointCM
            + sizeof(short) * 3
            // Yaw
            + 1
            // Level Index
            + 1
            // Best Time
            + sizeof(short)
            // Checksum
            + 1;

        public void EncodeAndPrint()
        {
            var challenge = CreateChallenge();
            string result = EncodeChallenge(challenge);

            Debug.Log(result);
        }

        public string EncodeChallenge(Challenge challenge)
        {
            return EncodeChallenge(challenge, ValidLevels);
        }

        public static string EncodeChallenge(Challenge challenge, LevelManifest manifest)
        {
            // todo WRAP THIS cuz maybe I use it later... i guess. this just feels so ugly being in here
            byte[] completeChallenge = new byte[PayloadLength];
            int offset = 0;

            AddVector3Short(ref completeChallenge, offset, challenge.StartPointCm);
            offset += sizeof(short) * 3;

            AddVector3Short(ref completeChallenge, offset, challenge.EndPointCm);
            offset += sizeof(short) * 3;

            byte yaw = Convert.ToByte(challenge.StartYaw / 2);
            addByte(ref completeChallenge, offset, yaw);
            offset++;

            int rawIndex = GetLevelIndex(challenge.Level, manifest);
            byte levelIndex = Convert.ToByte(rawIndex);
            addByte(ref completeChallenge, offset, levelIndex);
            offset++;

            addShort(ref completeChallenge, offset, (short)challenge.BestTimeMS);
            offset += sizeof(short);

            // really lazy error checking 
            byte hash = getHash(completeChallenge);
            addByte(ref completeChallenge, offset, (byte)hash);

            string result = Pretty256Helper.Encode(completeChallenge);
            return result;
        }

        private static int GetLevelIndex(LevelDescriptor level, LevelManifest manifest)
        {
            foreach (LevelDescriptor validLevel in manifest.Levels)
            {
                if (validLevel == level)
                {
                    return level.ShareIndex;
                }
            }

            Debug.LogError(($"level \'{level}\' was not valid for Manifest \'{manifest}\'"));
            return 0;
        }

        public static LevelDescriptor GetLevelFromIndex(int shareIndex, LevelManifest manifest)
        {
            foreach (LevelDescriptor validLevel in manifest.Levels)
            {
                if (validLevel.ShareIndex == shareIndex)
                {
                    return validLevel;
                }
            }

            Debug.LogError(($"shareIndex \'{shareIndex}\' was not valid for Manifest \'{manifest}\'"));
            return null;
        }

        static void AddVector3Short(ref byte[] array, int offset, Vector3Short value)
        {
            addShort(ref array, offset, value.x);
            addShort(ref array, offset + sizeof(short), value.y);
            addShort(ref array, offset + 2 * sizeof(short), value.z);
        }

        static void addShort(ref byte[] array, int offset, short value)
        {
            byte[] valueArray = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueArray);
            }

            addByteArray(ref array, offset, valueArray);
        }

        static void addVector3(ref byte[] array, int offset, Vector3 value)
        {
            addFloat(ref array, offset, value.x);
            addFloat(ref array, offset + sizeof(float), value.y);
            addFloat(ref array, offset + 2 * sizeof(float), value.z);
        }

        static void addFloat(ref byte[] array, int offset, float value)
        {
            byte[] valueArray = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueArray);
            }

            addByteArray(ref array, offset, valueArray);
        }

        static void addByteArray(ref byte[] array, int offset, byte[] valueArray)
        {
            Buffer.BlockCopy(valueArray, 0, array, offset, valueArray.Length);
        }

        static void addByte(ref byte[] array, int offset, byte value)
        {
            array[offset] = value;
        }

        static byte getHash(byte[] array)
        {
            byte hash = 69;
            foreach(byte b in array)
            {
                hash ^= b;
            }

            return hash;
        }
        #endregion

        #region Decoding
        public enum DecodeFailReason
        {
            _none,
            WrongLength,
            Invalid,
            CantShareInvalid,
            CantShareUncleared
        }

        public UnityEvent<DecodeFailReason> OnDecodeFailed;
        public void DecodeAndLoadCurrentCode()
        {
            var challenge = DecodeChallenge(CurrentCode,ValidLevels, out DecodeFailReason failReason);

            if (challenge == null)
            {
                OnDecodeFailed?.Invoke(failReason);
                return;
            }

            CurrentChallenge = challenge;
            LoadChallenge();
        }

        public static Challenge DecodeChallenge(string encodedPayload, LevelManifest manifest, out DecodeFailReason failReason)
        {
            if (encodedPayload.Length != PayloadLength)
            {
                failReason = DecodeFailReason.WrongLength;
                return null;
            }

            byte[] rawPayload = Pretty256Helper.Decode(encodedPayload);

            // really lazy error checking 
            byte suppliedHash = rawPayload[PayloadLength - 1];
            rawPayload[PayloadLength - 1] = 0;
            byte calculatedHash = getHash(rawPayload);
            if (calculatedHash != suppliedHash)
            {
                failReason = DecodeFailReason.Invalid;
                return null;
            }

            Challenge challenge = new Challenge();

            int offset = 0;

            challenge.StartPointCm = GetVector3Short(rawPayload, offset);
            offset += sizeof(short) * 3;

            challenge.EndPointCm = GetVector3Short(rawPayload, offset);
            offset += sizeof(short) * 3;

            byte yaw = getByte(rawPayload, offset);
            challenge.StartYaw = Convert.ToInt32(yaw * 2);
            offset++;


            byte levelIndex = getByte(rawPayload, offset);
            challenge.Level = GetLevelFromIndex(levelIndex, manifest);
            offset++;


            challenge.BestTimeMS = (ushort)getShort(rawPayload, offset);
            //offset += sizeof(short); don't need this because it's the last entry we're reading

            failReason = DecodeFailReason._none;
            challenge.ChallengeType = Challenge.ChallengeTypes.PlayCustom;
            return challenge;
        }

        static Vector3Short GetVector3Short(byte[] array, int offset)
        {
            Vector3Short value = new Vector3Short()
            {
                x = getShort(array, offset),
                y = getShort(array, offset + sizeof(short)),
                z = getShort(array, offset + 2 * sizeof(short))
            };

            return value;
        }

        static short getShort(byte[] array, int offset)
        {

            byte[] shortArray = getByteArray(array, offset, sizeof(short));

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(shortArray);
            }

            return BitConverter.ToInt16(shortArray);
        }

        static byte[] getByteArray(byte[] array, int offset, int length)
        {
            byte[] valueArray = new byte[length];
            Buffer.BlockCopy(array, offset, valueArray, 0, length);
            return valueArray;
        }

        static byte getByte(byte[] array, int offset)
        {
            return array[offset];
        }

        #endregion
    }
}
