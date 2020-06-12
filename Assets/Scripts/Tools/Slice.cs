using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{
    public GameObject prefab;
    public Mesh mesh;
    public Material material;
    public GameObject quad;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public bool reset;
    public bool cut;

    private Vector2 mouseDown;
    private Vector2 mouseUp;

    private Vector2 intersectLeft;
    private Vector2 intersectRight;
    public bool right, down, left, top;
    public bool topLeftTriangle, downRightTriangle;
    public GameObject firstPiece;
    public GameObject secondPiece;
    // Start is called before the first frame update
    void Awake()
    {
        ResetDummy();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
            mouseUp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mouseDown != Vector2.zero && mouseUp != Vector2.zero)
            Debug.DrawLine(mouseDown, mouseUp, Color.red);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Cut(Vector2.left, Vector2.right);
        }

        if (reset)
            ResetDummy();
    }

    public void ResetDummy()
    {
        Debug.Log("Reset");
        for (int i = 0; i < transform.childCount; i++)
        {
            //DestroyImmediate(transform.GetChild(i).gameObject);
            Destroy(quad);
            Destroy(firstPiece);
            Destroy(secondPiece);
        }
        quad = Instantiate(prefab, transform);
        meshRenderer = quad.GetComponent<MeshRenderer>();
        meshFilter = quad.GetComponent<MeshFilter>();
        quad.transform.parent = transform;
        firstPiece = Instantiate(prefab, transform);
        secondPiece = Instantiate(prefab, transform);
        firstPiece.name = "FirstPiece";
        secondPiece.name = "SecondPiece";
        //  Instantiate(quad, Vector3.zero, Quaternion.identity);
        reset = false;
    }

    private void OnDrawGizmos()
    {
        if (!meshFilter)
            ResetDummy();
        //LEFT & RIGHT
        Vector2 right, left = Vector2.zero;
        // Vector2 left = Vector2.zero;
        if (mouseUp.x > mouseDown.x)
        {
            right = mouseUp;
            left = mouseDown;
        }
        else
        {
            right = mouseDown;
            left = mouseUp;
        }
        /*
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(right, 0.05f);
        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(left, 0.05f);
        */
        //Intersection
        //bool rightFound, downFound, foundLeft, topFound;
        bool[] founds = new bool[4];
        //Vector2 intersectionRight, intersectionDown, intersectionLeft, intersectionTop;
        Vector3[] intersections = new Vector3[4];
        //INTERSECTIONs 
        Vector3[] vertices = meshFilter.mesh.vertices;
        vertices = TransformVerticesLocalScale(quad.transform, vertices);
        vertices = TransformVerticesRotate(quad.transform, vertices);
        vertices = TransformVerticesPosition(quad.transform, vertices);
        intersections[0] = GetIntersectionPointCoordinates(left, right, vertices[3], vertices[1], out founds[0]);// right
        intersections[1] = GetIntersectionPointCoordinates(left, right, vertices[0], vertices[1], out founds[1]);// down
        intersections[2] = GetIntersectionPointCoordinates(left, right, vertices[0], vertices[2], out founds[2]);// left
        intersections[3] = GetIntersectionPointCoordinates(left, right, vertices[2], vertices[3], out founds[3]);// top
        //for (int i = 0; i < intersections.Length; i++) intersections[i] += new Vector2(quad.transform.position.x, quad.transform.position.y);
        /*
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(vertices[0], 0.05f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(vertices[1], 0.05f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(vertices[2], 0.05f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(vertices[3], 0.05f);
        */
        List<Vector3> intersectionsLine = new List<Vector3>();
        for (int indexLine = 0; indexLine < 2; indexLine++)
        {
            int i = 0;
            while (i < 4)
            {
                if (founds[i])
                    break;
                i++;
            }
            if (i < 4)
            {
                intersectionsLine.Add(intersections[i]);
                founds[i] = false;
            }
        }
        if (intersectionsLine.Count == 2)
        {
            //intersectionsLine = new List<Vector3>(TransformVerticesPosition(quad.transform, intersectionsLine.ToArray()));
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(intersectionsLine[0], intersectionsLine[1]);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(intersectionsLine[0], 0.025f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(intersectionsLine[1], 0.025f);
            quad.SetActive(false);
            firstPiece.SetActive(true);
            secondPiece.SetActive(true);
            CheckCut(intersectionsLine[0], intersectionsLine[1], vertices);
            Cut(intersectionsLine[0], intersectionsLine[1]);
        }
        else
        {
            if(cut)
                ResetDummy();
            quad.SetActive(true);
            firstPiece.SetActive(false);
            secondPiece.SetActive(false);
            cut = false;
        }
    }
    public void CheckCut(Vector3 start, Vector3 end, Vector3[] vertices)
    {
        right = InLine(vertices[3], vertices[1], start) || InLine(vertices[3], vertices[1], end);
        down = InLine(vertices[0], vertices[1], start) || InLine(vertices[0], vertices[1], end);
        left = InLine(vertices[0], vertices[2], start) || InLine(vertices[0], vertices[2], end);
        top = InLine(vertices[3], vertices[2], start) || InLine(vertices[3], vertices[2], end);
        downRightTriangle = InTriangle(vertices[0], vertices[3], vertices[1], start) ||
            InTriangle(vertices[0], vertices[3], vertices[1], end);
        topLeftTriangle = InTriangle(vertices[0], vertices[2], vertices[3], start) ||
            InTriangle(vertices[0], vertices[2], vertices[3], end);
    }

    public void Cut(Vector3 start, Vector3 end)
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //vertices[i].Scale(objectTransform.localScale);
        //vertices[i] = new Vector3(vertices[i].x /quad.transform.localScale.x, vertices[i].y / quad.transform.localScale.y);
        //}
        start -= quad.transform.position;
        end -= quad.transform.position;
        start = Quaternion.Inverse(quad.transform.rotation) * start;
        end = Quaternion.Inverse(quad.transform.rotation) * end;
        start = new Vector3(start.x / quad.transform.localScale.x, start.y / quad.transform.localScale.y);
        end = new Vector3(end.x / quad.transform.localScale.x, end.y / quad.transform.localScale.y);
        if (topLeftTriangle && downRightTriangle)
        {
            //Debug.Log("Good cut");
            if (left && right)
            {
                if (start.x > end.x)
                {
                    Vector3 temp = start;
                    start = end;
                    end = temp;
                }
                firstPiece.GetComponent<MeshFilter>().mesh = GenerateQuadMesh(new Vector3[4] { vertices[0], vertices[1], start, end });
                secondPiece.GetComponent<MeshFilter>().mesh = GenerateQuadMesh(new Vector3[4] { start, end, vertices[2], vertices[3] });
                firstPiece.transform.position = new Vector3(0, -0.04f, 0);
                secondPiece.transform.position =  new Vector3(0, 0.04f, 0);
                //firstPiece.transform.Translate(Vector3.down * 0.2f);
                //secondPiece.transform.Translate(Vector3.up * 0.2f);
            }
            else if (top && down)
            {
                if (start.y > end.y)
                {
                    Vector3 temp = start;
                    start = end;
                    end = temp;
                }
                firstPiece.GetComponent<MeshFilter>().mesh = GenerateQuadMesh(new Vector3[4] { vertices[0], start, vertices[2], end });
                secondPiece.GetComponent<MeshFilter>().mesh = GenerateQuadMesh(new Vector3[4] { start, vertices[1], end, vertices[3] });
                firstPiece.transform.position = new Vector3(-0.04f, 0, 0);
                secondPiece.transform.position = new Vector3(0.04f, 0, 0);
                //firstPiece.transform.Translate(Vector3.left * 0.2f);
                //secondPiece.transform.Translate(Vector3.right * 0.2f);
            }
        }
        else if (topLeftTriangle || downRightTriangle)
        {
            Debug.Log("Bad Cut");
            if (topLeftTriangle)
            {
                //topleft
            }
            else
            {
                //downright
            }
        }
        else
        {
            Debug.Log("No Cut");
        }
        //Mesh meshFirst = new Mesh;
        //meshFilter

        cut = true;
    }

    public static Mesh GenerateQuadMesh(Vector3[] vertices)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = new int[6] { 0, 3, 1, 3, 0, 2 };
        mesh.uv = new Vector2[4] {
            new Vector2(vertices[0].x + 0.5f, vertices[0].y + 0.5f),
            new Vector2(vertices[1].x + 0.5f, vertices[1].y + 0.5f),
            new Vector2(vertices[2].x + 0.5f, vertices[2].y + 0.5f),
            new Vector2(vertices[3].x + 0.5f, vertices[3].y + 0.5f)
        };
        //mesh.uv = GenerateUVs(vertices);
        mesh.RecalculateNormals();
        return mesh;
    }

    public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
    }
    public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f)
        {
            lhs = (Vector3)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector3)(lhs * num2)));
    }

    /// <summary>
    /// Gets the coordinates of the intersection point of two lines.
    /// </summary>
    /// <param name="A1">A point on the first line.</param>
    /// <param name="A2">Another point on the first line.</param>
    /// <param name="B1">A point on the second line.</param>
    /// <param name="B2">Another point on the second line.</param>
    /// <param name="found">Is set to false of there are no solution. true otherwise.</param>
    /// <returns>The intersection point coordinates. Returns Vector2.zero if there is no solution.</returns>
    public Vector3 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
    {
        float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

        if (tmp == 0)
        {
            // No solution!
            found = false;
            return Vector3.zero;
        }

        float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

        found = true;

        Vector3 intersection = new Vector3(
            B1.x + (B2.x - B1.x) * mu,
            B1.y + (B2.y - B1.y) * mu,
            0
        );
        if (!InLine(B1, B2, intersection)) {
            found = false;
            return Vector3.zero;
        }
        return intersection;
    }

    public bool InLine(Vector2 A, Vector2 B, Vector2 C)
    {
        return Math.Round(Vector2.Distance(A, C), 1) + Math.Round(Vector2.Distance(B, C), 1) == Math.Round(Vector2.Distance(A, B), 1);
    }

    public bool InTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 point)
    {
        return InLine(A, B, point) || InLine(A, C, point) || InLine(B, C, point);
    }

    public Vector3 GetVerticesByIndex(Vector3[] vertices, int i)
    {
        return vertices[meshFilter.mesh.triangles[i]];
    }

    public Vector3[] TransformVerticesPosition(Transform objectTransform, Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += objectTransform.position;
        }
        return vertices;
    }
    public Vector3[] TransformVerticesRotate(Transform objectTransform, Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = objectTransform.rotation * vertices[i];
            //vertices[i].Scale(objectTransform.localScale);
        }
        return vertices;
    }
    public Vector3[] TransformVerticesLocalScale(Transform objectTransform, Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].Scale(objectTransform.localScale);
        }
        return vertices;
    }
}
