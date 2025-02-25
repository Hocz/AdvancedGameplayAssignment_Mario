using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VSCodeEditor;

namespace NaivePhysics
{
    [ExecuteInEditMode]
    public class NaiveEngine : MonoBehaviour
    {
        [ExecuteInEditMode]
        public abstract class Shape : MonoBehaviour
        {
            private Rect            m_bounds;
            private HashSet<Shape>  m_touchingShapes = new HashSet<Shape>();

            #region Properties

            public Vector2 Position => transform.position;

            public Rect Bounds => m_bounds;

            public float MinX => m_bounds.x;

            public float MaxX => m_bounds.xMax;

            public float MinY => m_bounds.y;

            public float MaxY => m_bounds.yMax;

            public abstract bool IsTrigger { get; }

            #endregion

            protected virtual void Start()
            {
                OnMoved();
            }

            protected virtual void Update()
            {
                if (transform.hasChanged)
                {
                    OnMoved();
                    transform.hasChanged = false;
                }
            }

            protected abstract Rect CalculateBounds();

            public abstract void DrawShape();

            protected virtual void OnMoved()
            {
                m_bounds = CalculateBounds();
            }      

            public virtual void OnEnterCollision(Collision collision, Shape other)
            {
            }

            public virtual void OnLeaveCollision(Shape other)
            {
            }

            public virtual void HandleTouching(List<Collision> collisions)
            {
                // did we enter into collision?
                HashSet<Shape> newTouchingSet = new HashSet<Shape>();
                foreach (Collision collision in collisions)
                {
                    if (collision.Contains(this))
                    {
                        Shape other = collision.GetOther(this);
                        if (!m_touchingShapes.Contains(other))
                        {
                            OnEnterCollision(collision, other);
                        }

                        newTouchingSet.Add(other);
                    }
                }

                // did we leave collision?
                foreach (Shape oldShape in m_touchingShapes)
                {
                    if (!newTouchingSet.Contains(oldShape))
                    {
                        OnLeaveCollision(oldShape);
                    }
                }

                m_touchingShapes = newTouchingSet;
            }
        }

        private class ShapeEdge
        {
            public Shape    m_shape;
            public bool     m_bLeft;

            public float X => m_bLeft ? m_shape.MinX : m_shape.MaxX;
        }

        public const float          GRAVITY = 9.82f;
        public const float          MAX_VELOCITY = 100.0f;

        #region Properties

        public Shape[] Shapes => GetComponentsInChildren<Shape>();

        public NaiveBody[] Bodies => GetComponentsInChildren<NaiveBody>();

        #endregion

        private void FixedUpdate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            // tick bodies
            foreach (NaiveBody body in Bodies)
            {
                if (body.isActiveAndEnabled)
                {
                    body.TickBody();
                }
            }

            // find collisions
            List<Collision> collisions = FindCollisions();

            foreach (Collision collision in collisions)            
            {
                // resolve collisions
                NaiveBody A = collision.A.GetComponent<NaiveBody>();
                NaiveBody B = collision.B.GetComponent<NaiveBody>();
                if (A != null) A.ResolveCollision(collision, B != null ? B.m_fMass : 0.0f, B != null ? B.Velocity : Vector2.zero);
                if (B != null) B.ResolveCollision(collision, A != null ? A.m_fMass : 0.0f, A != null ? A.Velocity : Vector2.zero);
            }

            // let shapes handle touching shapes
            foreach (Shape shape in Shapes)
            {
                if (shape.IsTrigger)
                {
                    shape.HandleTouching(collisions);
                }
            }
        }

        public List<Collision> FindCollisions()
        {
            // create edges
            Shape[] shapes = Shapes;
            List<ShapeEdge> edges = new List<ShapeEdge>();
            foreach (Shape shape in shapes)
            {
                edges.Add(new ShapeEdge { m_shape = shape, m_bLeft = true });
                edges.Add(new ShapeEdge { m_shape = shape, m_bLeft = false });
            }

            // sort edges by X
            edges.Sort(delegate (ShapeEdge e1, ShapeEdge e2) { return e1.X.CompareTo(e2.X); });

            // find touching set
            HashSet<Shape> touching = new HashSet<Shape>();
            List<Collision> result = new List<Collision>();
            foreach (ShapeEdge edge in edges)
            {
                //float fY = (edge.m_shape.MinY + edge.m_shape.MaxY) * 0.5f;
                //Debug.DrawLine(new Vector3(edge.X, fY, 0.0f), new Vector3(edge.X, fY - 1000.0f, 0.0f), edge.m_bLeft ? Color.red : Color.blue);

                if (edge.m_bLeft)
                {
                    // check overlaps with touching set
                    foreach (Shape shape in touching)
                    {
                        Collision collision = Overlaps(edge.m_shape, shape);
                        if(collision != null)
                        {
                            result.Add(collision);
                        }
                    }

                    touching.Add(edge.m_shape);
                }
                else
                {
                    touching.Remove(edge.m_shape);
                }
            }

            return result;
        }

        private void OnDrawGizmos()
        {
            // draw shapes & collisions
            List<Collision> collisions = FindCollisions();
            Shape[] shapes = Shapes;
            foreach (Shape shape in shapes) 
            {
                // draw collisions
                bool bIsColliding = false;
                foreach(Collision collision in collisions) 
                {
                    bIsColliding = bIsColliding || collision.A == shape || collision.B == shape;
                    if (collision.A == shape)
                    {
                        // draw collision center
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(collision.m_vPosition, 0.05f);

                        // draw collision normals
                        Gizmos.DrawLine(collision.m_vPosition, collision.m_vPosition + collision.m_vNormal * 0.25f);
                        Gizmos.DrawLine(collision.m_vPosition, collision.m_vPosition - collision.m_vNormal * 0.25f);

                        // draw intersection
                        Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
                        Vector2 vRight = new Vector2(-collision.m_vNormal.y, collision.m_vNormal.x);
                        Gizmos.DrawLine(collision.m_vPosition + vRight * 0.05f - collision.m_vNormal * collision.m_fPenetration, collision.m_vPosition + vRight * 0.05f + collision.m_vNormal * collision.m_fPenetration);
                        Gizmos.DrawLine(collision.m_vPosition - vRight * 0.05f - collision.m_vNormal * collision.m_fPenetration, collision.m_vPosition - vRight * 0.05f + collision.m_vNormal * collision.m_fPenetration);
                    }
                }

                // draw bounds
                #if false
                Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
                Rect bounds = shape.Bounds;
                Gizmos.DrawWireCube(bounds.center, new Vector3(bounds.width, bounds.height, 0.0f));
                #endif

                // draw shape
                Gizmos.color = bIsColliding ? Color.red : Color.green;
                shape.DrawShape();
            }
        }

        public static Collision Overlaps(Shape A, Shape B)
        {
            // first do cheap AABB test
            if (A.MaxX < B.MinX || A.MinX > B.MaxX ||
                A.MaxY < B.MinY || A.MinY > B.MaxY)
            {
                return null;
            }

            // get concrete types
            Sphere sphereA = A as Sphere;
            Sphere sphereB = B as Sphere;
            AlignedBox alignedBoxA = A as AlignedBox;
            AlignedBox alignedBoxB = B as AlignedBox;
            Triangle triangleA = A as Triangle;
            Triangle triangleB = B as Triangle;

            // perform concrete overlap test based on type
            if (sphereA != null && sphereB != null)                 return Overlaps_Sphere_Sphere(sphereA, sphereB);

            else if (alignedBoxA != null && alignedBoxB != null)    return Overlaps_AlignedBox_AlignedBox(alignedBoxA, alignedBoxB);

            else if (triangleA != null && triangleB != null)        return Overlaps_Triangle_Triangle(triangleA, triangleB);

            else if (sphereA != null && alignedBoxB != null)        return Overlaps_Sphere_AlignedBox(sphereA, alignedBoxB);
            else if (sphereB != null && alignedBoxA != null)        return Overlaps_Sphere_AlignedBox(sphereB, alignedBoxA);

            else if (sphereA != null && triangleB != null)          return Overlaps_Sphere_Triangle(sphereA, triangleB);
            else if (sphereB != null && triangleA != null)          return Overlaps_Sphere_Triangle(sphereB, triangleA);

            else if (alignedBoxA != null && triangleB != null)      return Overlaps_AlignedBox_Triangle(alignedBoxA, triangleB);
            else if (alignedBoxB != null && triangleA != null)      return Overlaps_AlignedBox_Triangle(alignedBoxB, triangleA);

            // no overlap
            return null;
        }

        public static Collision Overlaps_Sphere_Sphere(Sphere A, Sphere B)
        {
            float fDistance = Vector2.Distance(A.Position, B.Position);
            if (fDistance < A.m_fRadius + B.m_fRadius)
            {
                Vector2 vAtoB = (B.Position - A.Position).normalized;
                return new Collision
                {
                    A = A,
                    B = B,
                    m_vPosition = (A.Position + vAtoB * A.m_fRadius + B.Position - vAtoB * B.m_fRadius) * 0.5f,
                    m_vNormal = vAtoB,
                    m_fPenetration = ((A.m_fRadius + B.m_fRadius) - fDistance) * 0.5f
                };
            }

            return null;
        }

        public static Collision Overlaps_AlignedBox_AlignedBox(AlignedBox A, AlignedBox B)
        {
            if (A.MaxX > B.MinX && A.MinX < B.MaxX &&
                A.MaxY > B.MinY && A.MinY < B.MaxY)
            {
                Vector2 vAtoB = B.Position - A.Position;

                Vector2 vExtentsA = new Vector2(A.MaxX - A.MinX, A.MaxY - A.MinY) * 0.5f;
                Vector2 vExtentsB = new Vector2(B.MaxX - B.MinX, B.MaxY - B.MinY) * 0.5f;

                float fOverlapX = vExtentsA.x + vExtentsB.x - Mathf.Abs(vAtoB.x);
                float fOverlapY = vExtentsA.y + vExtentsB.y - Mathf.Abs(vAtoB.y);

                Vector2 vCollisionNormal = fOverlapX < fOverlapY ? new Vector2(vAtoB.x < 0.0f ? -1.0f : 1.0f, 0.0f) :
                                                                   new Vector2(0.0f, vAtoB.y < 0.0f ? -1.0f : 1.0f);

                return new Collision
                {
                    A = A,
                    B = B,
                    m_vPosition = (A.Position + B.Position) * 0.5f,
                    m_vNormal = vCollisionNormal,
                    m_fPenetration = fOverlapX < fOverlapY ? fOverlapX : fOverlapY
                };
            }

            return null;
        }

        public static Collision Overlaps_Triangle_Triangle(Triangle A, Triangle B)
        {
            return null;

        /*
            for (int i = 0; i < 2; ++i)
            {
                Triangle tri = i == 0 ? A : B;
                Triangle other = i == 0 ? B : A;
                Vector2[] otherPoints = other.WorldPositions;

                foreach (Plane plane in tri.Planes)
                {
                    if (System.Array.FindIndex(otherPoints, v => !plane.GetSide(v)) < 0)
                    {
                        return false;
                    }
                }
            }

            return true;
            */
        }

        public static Collision Overlaps_Sphere_AlignedBox(Sphere A, AlignedBox B)
        {
            /* TODO: implement this case
            // does box contain the sphere position?
            if (A.Position.x >= B.MinX && A.Position.x <= B.MaxX &&
                A.Position.y >= B.MinY && A.Position.y <= B.MaxY)
            {
                return true;
            }*/

            // check distance to box segments
            foreach (Vector2[] line in B.Lines)
            {
                Vector2 vCP = NaiveMath.ClosestPointOnSegment(A.Position, line[0], line[1]);
                float fDistanceToLine = Vector2.Distance(vCP, A.Position);

                if (fDistanceToLine <= A.m_fRadius)
                {
                    Vector2 vToBox = (vCP - A.Position).normalized;
                    float fPenetration = (A.m_fRadius - fDistanceToLine) * 0.5f;

                    return new Collision
                    {
                        A = A,
                        B = B,
                        m_vPosition = A.Position + vToBox * (A.m_fRadius - fPenetration),
                        m_vNormal = vToBox,
                        m_fPenetration = fPenetration
                    };
                }
            }

            // no collision
            return null;
        }

        public static Collision Overlaps_Sphere_Triangle(Sphere A, Triangle B)
        {
            return null;

        /*
            // sphere inside triangle?
            Vector2[] triangle = B.WorldPositions;
            if (NaiveMath.PointInTriangle(A.Position, triangle[0], triangle[1], triangle[2]))
            {
                return true;
            }

            // sphere line overlap?
            foreach (Vector2[] line in B.Lines)
            {
                if (NaiveMath.DistanceToSegment(A.Position, line[0], line[1]) <= A.m_fRadius)
                {
                    return true;
                }
            }

            // no collision
            return false;*/
        }

        public static Collision Overlaps_AlignedBox_Triangle(AlignedBox A, Triangle B)
        {
            return null;

            /*
            Vector2[] triangle = B.WorldPositions;

            // outside box?
            if (System.Array.FindIndex(triangle, v => v.x < A.MaxX) < 0) return false;
            if (System.Array.FindIndex(triangle, v => v.x > A.MinX) < 0) return false;
            if (System.Array.FindIndex(triangle, v => v.y < A.MaxY) < 0) return false;
            if (System.Array.FindIndex(triangle, v => v.y > A.MinY) < 0) return false;

            // outside triangle?
            foreach (Plane plane in B.Planes)
            {
                if (System.Array.FindIndex(A.Corners, v => !plane.GetSide(v)) < 0)
                {
                    return false;
                }
            }

            // collision!
            return true;*/
        }
    }
}