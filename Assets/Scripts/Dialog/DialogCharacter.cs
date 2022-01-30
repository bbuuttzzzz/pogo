using UnityEngine;

public class DialogCharacter : ScriptableObject
{
    public string DisplayName;
    public Sprite TalkSprite;
    public AudioClip[] TalkSounds;

    public AudioClip RandomTalkSound => TalkSounds.Length == 0 ? null
        : TalkSounds[Random.Range(0, TalkSounds.Length)];
}
