using System;
using UnityEngine;

public static class Geometry
{

	/// Distance of point C from the line passing through points A and B.
	public static float DistanceFromPointToLine(Vector2 a, Vector2 b, Vector2 c)
	{
		float s1 = -b.y + a.y;
		float s2 = b.x - a.x;
		return Mathf.Abs((c.x - a.x) * s1 + (c.y - a.y) * s2) / Mathf.Sqrt(s1*s1 + s2*s2);
	}

	public static Vector2 ClosestPointOnLineSeg(Vector2 a, Vector2 b, Vector2 p) {
		Vector2 aToP = p - a;
		Vector2 aToB = b - a;
		float s = aToB.sqrMagnitude;

		float dS = aToP.x * aToB.x + aToP.y * aToB.y;

		float d = dS / s;
		Vector2 res = a + aToB * d;

		int sideA = SideOfLine (p, res, a);
		int sideB = SideOfLine (p, res, b);

		if (sideA == sideB) {
			float d1 = (b - p).sqrMagnitude;
			float d2 = (a - p).sqrMagnitude;
			return (d1 < d2) ? b : a;
		}
		return res;
	}
		
	public static int SideOfLine(Vector2 a, Vector2 b, Vector2 c)
	{
		return (int)Mathf.Sign((c.x - a.x) * (-b.y + a.y) + (c.y - a.y) * (b.x - a.x));
	}

}
