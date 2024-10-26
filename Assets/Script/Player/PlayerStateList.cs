using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    public bool Jumping = false;
    public bool dashing = false;
    public bool recoilingX, recoilingY;
    public bool lookingRight;
    public bool Invincible;
    public bool healing;
    public bool casting;
    public bool cutscenes = false;
    public bool Falling = false;
    public bool alive;
    public bool isWallSliding;
    public bool isWallJumping;
}
