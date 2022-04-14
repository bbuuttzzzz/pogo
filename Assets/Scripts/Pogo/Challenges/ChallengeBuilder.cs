using System;
using System.Collections.Generic;
using UnityEngine;
using WizardUtils.Math;
using WizardUtils.Tools;

namespace Pogo.Challenges
{
    public class ChallengeBuilder : MonoBehaviour
    {
        public LevelManifest ValidLevels;

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

        public void EncodeAndPrint()
        {
            var challenge = CreateChallenge();
            string result = EncodeChallenge(challenge);

            Debug.Log(result);
        }

        public string EncodeChallenge(Challenge challenge)
        {
            // todo WRAP THIS cuz maybe I use it later... i guess. this just feels so ugly being in here
            byte[] completeChallenge = new byte[sizeof(float) * 3 * 2 + 3];
            int offset = 0;

            addVector3(ref completeChallenge, offset, challenge.StartPoint);
            offset += sizeof(float) * 3;
            
            addVector3(ref completeChallenge, offset, challenge.EndPoint);
            offset += sizeof(float) * 3;
            
            byte yaw = Convert.ToByte(challenge.StartYaw / 2);
            addByte(ref completeChallenge, offset, yaw);
            offset++;

            int rawIndex = GetLevelIndex(challenge.Level);
            byte levelIndex = Convert.ToByte(rawIndex);
            addByte(ref completeChallenge, offset, yaw);
            offset++;

            string atlas = "";
            foreach(char c in Pretty256Helper.AtlasFleschutz)
            {
                atlas += c;
            }

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
