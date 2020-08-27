using UnityEngine;

public class MultiDigitNumber : MonoBehaviour {
    private int number = 0;

    private void Update() {
        int numberCopy = number++;
        foreach (var segmentDisplay in GetComponentsInChildren<SegmentDisplay>()) {
            segmentDisplay.SetDigit(numberCopy % 10);
            numberCopy /= 10;
        }
    }
}
