using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SegmentDisplay : MonoBehaviour {
    public Transform segmentPrefab;
    public Material onMaterial;
    public Material offMaterial;
    private readonly Transform[] segments = new Transform[7];

    private static int Encode(params int[] segments) => 
        segments.Aggregate(0, (sum, x) => sum + (int) Math.Pow(2, x));

    private static readonly int[] litSegments0To9 = {
        Encode(0,    2, 3, 4, 5, 6),
        Encode(            4,    6),
        Encode(0, 1, 2,    4, 5   ),    //   |    2    | 
        Encode(0, 1, 2,    4,    6),    //   |3  ---  4| 
        Encode(   1,    3, 4,    6),    //   |    1    | 
        Encode(0, 1, 2, 3,       6),    //   |5  ---  6| 
        Encode(0, 1, 2, 3,    5, 6),    //   |    0    | 
        Encode(      2,    4,    6),    //       ---     
        Encode(0, 1, 2, 3, 4, 5, 6),
        Encode(   1, 2, 3, 4, 6   )
    };

    private int digitShowing;

    public void SetDigit(int digit) {
        digitShowing = digit;
        LightSegments();
    }

    private void Awake() {
        Bounds segmentBounds = GetComponent<MeshFilter>().sharedMesh.bounds;
        segmentBounds.Expand(-segmentBounds.size.y * .3f); // Leave a margin
        float[] x =   {   .5f,   .5f,   .5f,    0,    1,    0,    1 };
        float[] y =   {     0,   .5f,     1, .75f, .75f, .25f, .25f };
        bool[] vert = { false, false, false, true, true, true, true };
        var pos = transform.position;
        Vector3 dims = segmentBounds.size;
        var botLeft = segmentBounds.min;
        for (int i = 0; i < 7; i++) {
            segments[i] = Instantiate(segmentPrefab, new Vector3(
                    pos.x + botLeft.x + x[i] * dims.x,
                    pos.y + botLeft.y + y[i] * dims.y, 
                    -.1f),
                Quaternion.Euler(0, 0, vert[i] ? 0 : 90), transform);
        }

        // StartCoroutine(nameof(Cycle));
    }


    private IEnumerator Cycle() {
        while (true) {
            LightSegments();

            yield return new WaitForSeconds(.5f);
            digitShowing = (digitShowing + 1) % 10;
        }
    }

    private void LightSegments() {
        var segmentBits = litSegments0To9[digitShowing];
        for (int i = 0; i < 7; i++) {
            var seg = segments[i].gameObject;
            var segmentBit = (int) Math.Pow(2, i);
            var segOn = (segmentBit & segmentBits) != 0;
            seg.GetComponent<Renderer>().material = segOn ? onMaterial : offMaterial;
        }
    }
}
