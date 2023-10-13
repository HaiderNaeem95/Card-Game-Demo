using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Card", menuName ="Cards")]
public class Card : ScriptableObject
{
    public int id;
    public string Name;
    public Sprite Tex;
}
