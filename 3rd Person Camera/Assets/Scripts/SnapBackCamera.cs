using UnityEngine;

public class SnapBackCamera
{
    public bool Active;
    public float Destination;
    public void MoveTo(float destination)
    {
        Destination = (destination > 180f) ? - (destination - 180f) : destination;
        Active = true;
    }
    public float GetNextPosition(float position)
    {
        if (Destination > position)
            position += 0.1f;
        else
            position -= 0.1f;

        if (Mathf.Abs(position - Destination) < 1f)
        {
            Active = false;
            return Destination;
        }

        return position;
    }
}
