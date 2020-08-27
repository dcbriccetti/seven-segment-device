using UnityEngine;
using static System.Linq.Enumerable;
using static SegmentDisplay.Orientation;

public class SegmentDisplay : MonoBehaviour {
    public Transform segmentPrefab;
    public Material onMaterial;
    public Material offMaterial;
    private Transform[] segments;
    private const int NumSegments = 7;

    private class Shape {
        private readonly byte shapeBits;

        public Shape(params byte[] segmentIndexes) {
            shapeBits = (byte) segmentIndexes.Aggregate(0, (segmentBits, segmentIndex) => 
                segmentBits | (1 << segmentIndex));
        }

        public bool HasSegment(int segmentIndex) {
            int bit = 1 << segmentIndex;
            return (shapeBits & bit) == bit;
        }
    }

    private static readonly Shape[] digitShapes = {
        new Shape(0,    2, 3, 4, 5, 6),
        new Shape(            4,    6),
        new Shape(0, 1, 2,    4, 5   ),    //   |    2    | 
        new Shape(0, 1, 2,    4,    6),    //   |3  ---  4| 
        new Shape(   1,    3, 4,    6),    //   |    1    | 
        new Shape(0, 1, 2, 3,       6),    //   |5  ---  6| 
        new Shape(0, 1, 2, 3,    5, 6),    //   |    0    | 
        new Shape(      2,    4,    6),    //       ---     
        new Shape(0, 1, 2, 3, 4, 5, 6),
        new Shape(   1, 2, 3, 4, 6   )
    };

    private int digitShowing;

    public void SetDigit(int digit) {
        digitShowing = digit;
        LightSegments();
    }

    public enum Orientation { Horizontal, Vertical };
    
    private class Geom {
        public readonly float xFraction;
        public readonly float yFraction;
        public readonly Orientation orientation;
        public Geom(int xPercent, int yPercent, Orientation orientation) {
            this.orientation = orientation;
            xFraction = xPercent / 100f;
            yFraction = yPercent / 100f;
            this.orientation = orientation;
        }
    }

    private static readonly Geom[] segmentGeometries = {
        new Geom( 50,   0, Horizontal),
        new Geom( 50,  50, Horizontal),
        new Geom( 50, 100, Horizontal),
        new Geom(  0,  75, Vertical  ),
        new Geom(100,  75, Vertical  ),
        new Geom(  0,  25, Vertical  ),
        new Geom(100,  25, Vertical  )
    };

    private void Start() {
        var segmentBounds = GetComponent<MeshFilter>().sharedMesh.bounds;
        segmentBounds.Expand(-segmentBounds.size.y * .27f); // Leave a margin
        Bounds bounds = segmentBounds;
        var pos = transform.position;
        segments = segmentGeometries.Select(segGeom => Instantiate(segmentPrefab, new Vector3(
            pos.x + bounds.min.x + segGeom.xFraction * bounds.size.x,
            pos.y + bounds.min.y + segGeom.yFraction * bounds.size.y,
            -.1f), Quaternion.Euler(0, 0, segGeom.orientation == Vertical ? 0 : 90), transform)).ToArray();
    }

    private void LightSegments() {
        for (int i = 0; i < NumSegments; i++) {
            var segmentRenderer = segments[i].gameObject.GetComponent<Renderer>();
            segmentRenderer.material = digitShapes[digitShowing].HasSegment(i) ? onMaterial : offMaterial;
        }
    }
}
