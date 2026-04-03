using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Event/Vector2", fileName = "Vector2EventSO", order = 3000)]
public class Vector2EventSO : ScriptableObject
{
    public UnityAction<Vector2> OnEventRaised;

    public void RaiseEvent(Vector2 value)
    {
        OnEventRaised?.Invoke(value);
    }
}
