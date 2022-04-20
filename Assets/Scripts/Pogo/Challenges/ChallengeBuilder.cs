using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils;
using WizardUtils.Equipment;
using WizardUtils.Math;
using WizardUtils.Tools;

namespace Pogo.Challenges
{
    public class ChallengeBuilder : MonoBehaviour
    {
        public LevelManifest ValidLevels;

        public Challenge CurrentChallenge;
        public string CurrentCode;

        public UnityEvent<Challenge> OnChallengeChanged;
        public UnityEvent<string> OnCodeChanged;

        public PauseMenuController PauseMenu;
        public ToggleableUIElement OverrideMenu;

        public Trigger ChallengePickup;

        public EquipmentDescriptor ChallengeStick;

        public UnityEvent OnChallengeComplete;
        public UnityEvent OnChallengeReset;

        public void CalculateNewChallenge()
        {
            CurrentChallenge = CreateChallenge();
            CurrentCode = null;
            OnChallengeChanged?.Invoke(CurrentChallenge);
            OnCodeChanged?.Invoke(CurrentCode);
        }

        public void CompleteChallenge()
        {
            float oldTime = CurrentChallenge.BestTime;
            CurrentChallenge.FinishAttempt();
            float newTime = CurrentChallenge.LastAttemptTime;
            if (newTime != oldTime)
            {
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
            PauseMenu.OverrideMenu = null;
            CurrentChallenge = null;
            OnChallengeChanged?.Invoke(CurrentChallenge);
            PogoGameManager.PogoInstance.OnPlayerDeath.RemoveListener(resetChallenge);
        }

        public void LoadChallenge()
        {
            PauseMenu.OverrideMenu = OverrideMenu;
            PogoGameManager pogoInstance = PogoGameManager.PogoInstance;
            pogoInstance.Equip(ChallengeStick);
            UnityAction finishLoading = null;
            finishLoading = () =>
            {
                Debug.Log("Finished loading challenge");
                pogoInstance.CustomCheckpoint.transform.position = CurrentChallenge.StartPoint;
                pogoInstance.CustomCheckpoint.transform.rotation = CurrentChallenge.StartRotation;
                pogoInstance.CustomCheckpoint.OnPlaced?.Invoke();
                pogoInstance.RegisterRespawnPoint(pogoInstance.CustomCheckpoint.transform);
                ChallengePickup.transform.position = CurrentChallenge.EndPoint;
                PogoGameManager.PogoInstance.OnPlayerDeath.AddListener(resetChallenge);
                PogoGameManager.PogoInstance.KillPlayer();
                OnChallengeChanged?.Invoke(CurrentChallenge);
                pogoInstance.OnLevelLoaded.RemoveListener(finishLoading);
            };
            pogoInstance.OnLevelLoaded.AddListener(finishLoading);
            pogoInstance.LoadLevel(CurrentChallenge.Level, new LevelLoadingSettings() { ForceReload = true });
        }

        private void resetChallenge()
        {
            CurrentChallenge?.StartAttempt();
            OnChallengeReset?.Invoke();
        }


        public void EncodeAndPrint()
        {
            var challenge = CreateChallenge();
            string result = EncodeChallenge(challenge);

            Debug.Log(result);
        }

        public string EncodeChallenge(Challenge challenge)
        {
            // todo WRAP THIS cuz maybe I use it later... i guess. this just feels so ugly being in here
            byte[] completeChallenge = new byte[sizeof(short) * 3 * 2 + 3];
            int offset = 0;

            AddVector3Short(ref completeChallenge, offset, challenge.StartPointCm);
            offset += sizeof(short) * 3;

            AddVector3Short(ref completeChallenge, offset, challenge.EndPointCm);
            offset += sizeof(short) * 3;
            
            byte yaw = Convert.ToByte(challenge.StartYaw / 2);
            addByte(ref completeChallenge, offset, yaw);
            offset++;

            int rawIndex = GetLevelIndex(challenge.Level);
            byte levelIndex = Convert.ToByte(rawIndex);
            addByte(ref completeChallenge, offset, yaw);
            offset++;

            string result = Pretty256Helper.Encode(completeChallenge);
            return result;
        }

        private int GetLevelIndex(LevelDescriptor level)
        {
            foreach(LevelDescriptor validLevel in ValidLevels.Levels)
            {
                if (validLevel == level)
                {
                    return level.ShareIndex;
                }
            }

            Debug.LogError(($"level \'{level}\' was not valid for Manifest \'{ValidLevels}\'"));
            return 0;
        }

        private LevelDescriptor GetLevelFromIndex(int shareIndex)
        {
            foreach (LevelDescriptor validLevel in ValidLevels.Levels)
            {
                if (validLevel.ShareIndex == shareIndex)
                {
                    return validLevel;
                }
            }

            Debug.LogError(($"shareIndex \'{shareIndex}\' was not valid for Manifest \'{ValidLevels}\'"));
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

        static void addByte(ref byte[] array, int offset, byte value )
        {
            array[offset] = value;
        }

        public Challenge DecodeChallenge(string payload)
        {
            throw new NotImplementedException();
        }
    }
}
