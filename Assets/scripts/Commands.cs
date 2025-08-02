using UnityEngine;

public class Commands
{
    public float time;
    public bool facingRight;
    public bool moving;
    public bool jumping;
    public Vector2 pos;
    public Commands nextCommand;
    public Commands lastCommand;

    public Commands(float _time, bool _moving, bool _jumping, bool face,  Vector2 _pos, Commands last)
    {
        time = _time;
        moving = _moving;
        jumping = _jumping;
        facingRight = face;
        lastCommand = last;
        pos = _pos;
    }

    public Commands(float _time, bool _moving, bool _jumping, bool face, Vector2 _pos)
    {
        time = _time;
        facingRight = face;
        moving = _moving;
        jumping = _jumping;
        lastCommand = null;
        pos = _pos;
    }


    public void setNext(Commands next)
    {
        nextCommand = next;
    }
}
