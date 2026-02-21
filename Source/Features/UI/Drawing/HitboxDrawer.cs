using System;
using System.Collections.Generic;
using UnityEngine;

namespace NKVDebugMod.Features.UI.Drawing {
    internal static class HitboxDrawer {
        private static Vector2 LocalToScreenPoint(Camera camera, Collider2D collider2D, Vector2 point) =>
            //TransformPoint - local space position to world space position
            LocalToScreenPoint(camera, collider2D.transform.TransformPoint(point + collider2D.offset));

        private static Vector2 LocalToScreenPoint(Camera camera, Vector2 point) {
            //World position to screen position(lower left 0,0 to upper right)
            Vector2 result = camera.WorldToScreenPoint(point);

            return new Vector2((int)Math.Round(result.x), (int)Math.Round(Screen.height - result.y));
        }

        internal static void DrawCollider2D(Collider2D collider2D, Color color) {
            if(GameCore.Instance.gameLevel is not { } level) {
                return;
            }
            if(level.cameraCore?.theRealSceneCamera is not { } camera) {
                return;
            }

            if (!collider2D || !collider2D.isActiveAndEnabled) return;

            if (collider2D is BoxCollider2D or EdgeCollider2D or PolygonCollider2D) {
                switch (collider2D) {
                    case BoxCollider2D boxCollider2D:
                        var halfSize = boxCollider2D.size / 2f;
                        Vector2 topLeft = new(-halfSize.x, halfSize.y);
                        var topRight = halfSize;
                        Vector2 bottomRight = new(halfSize.x, -halfSize.y);
                        var bottomLeft = -halfSize;
                        var boxPoints = new List<Vector2> {
                        topLeft, topRight, bottomRight, bottomLeft, topLeft,
                    };
                        DrawPoints(camera, collider2D, boxPoints, color);
                        break;
                    case EdgeCollider2D edgeCollider2D:
                        DrawPoints(
                            camera,
                            collider2D,
                            [.. edgeCollider2D.points],
                            color);
                        break;
                    case PolygonCollider2D polygonCollider2D:
                        for (var i = 0; i < polygonCollider2D.pathCount; i++) {
                            List<Vector2> polygonPoints = [.. polygonCollider2D.GetPath(i)];
                            if (polygonPoints.Count > 0) polygonPoints.Add(polygonPoints[0]);
                            DrawPoints(camera, collider2D, polygonPoints, color);
                        }

                        break;
                }
            }
        }

        private static void DrawPoints(Camera camera, Collider2D collider, List<Vector2> points, Color color) {
            for (var i = 0; i < points.Count - 1; i++) {
                var pointA = LocalToScreenPoint(camera, collider, points[i]);
                var pointB = LocalToScreenPoint(camera, collider, points[i + 1]);
                Drawing.DrawLine(pointA, pointB, color, 3, true);
            }
        }
    }
}
