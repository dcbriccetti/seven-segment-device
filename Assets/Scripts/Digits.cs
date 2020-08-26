using UnityEngine;

public class Digits : MonoBehaviour
{
    private void Start() {
        int num = 01134;
        foreach (var sd in gameObject.GetComponentsInChildren<SegmentDisplay>()) {
            sd.SetDigit(num % 10);
            num /= 10;
        }
    }

}
