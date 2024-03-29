using System;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Fp.Utility
{
	/*  
		Catmull-Rom splines are Hermite curves with special tangent values.
		Hermite curve formula:
		(2t^3 - 3t^2 + 1) * p0 + (t^3 - 2t^2 + t) * m0 + (-2t^3 + 3t^2) * p1 + (t^3 - t^2) * m1
		For points p0 and p1 passing through points m0 and m1 interpolated over t = [0, 1]
		Tangent M[k] = (P[k+1] - P[k-1]) / 2
	*/
	public class CatmullRom
	{
		private int _resolution; //Amount of points between control points. [Tesselation factor]
		private bool _closedLoop;

		private CatmullRomPoint[] _splinePoints; //Generated spline points

		private Vector3[] _controlPoints;

		public CatmullRom(IReadOnlyList<Transform> controlPoints, int resolution, bool closedLoop)
		{
			if(controlPoints == null || controlPoints.Count <= 2 || resolution < 2)
			{
				throw new ArgumentException("Catmull Rom Error: Too few control points or resolution too small");
			}

			_controlPoints = new Vector3[controlPoints.Count];
			for(var i = 0; i < controlPoints.Count; i++)
			{
				_controlPoints[i] = controlPoints[i].position;
			}

			_resolution = resolution;
			_closedLoop = closedLoop;

			GenerateSplinePoints();
		}

		//Returns spline points. Count is contorolPoints * resolution + [resolution] points if closed loop.
		public CatmullRomPoint[] GetPoints()
		{
			if(_splinePoints == null)
			{
				throw new NullReferenceException("Spline not Initialized!");
			}

			return _splinePoints;
		}

		//Updates control points
		public void Update(Transform[] controlPoints)
		{
			if(controlPoints.Length <= 0 || controlPoints == null)
			{
				throw new ArgumentException("Invalid control points");
			}

			_controlPoints = new Vector3[controlPoints.Length];
			for(var i = 0; i < controlPoints.Length; i++)
			{
				_controlPoints[i] = controlPoints[i].position;
			}

			GenerateSplinePoints();
		}

		//Updates resolution and closed loop values
		public void Update(int resolution, bool closedLoop)
		{
			if(resolution < 2)
			{
				throw new ArgumentException("Invalid Resolution. Make sure it's >= 1");
			}

			_resolution = resolution;
			_closedLoop = closedLoop;

			GenerateSplinePoints();
		}

		//Draws a line between every point and the next.
		[Conditional("DEBUG")]
		public void DrawSpline(Color color)
		{
			if(ValidatePoints())
			{
				for(var i = 0; i < _splinePoints.Length; i++)
				{
					if(i == _splinePoints.Length - 1 && _closedLoop)
					{
						Debug.DrawLine(_splinePoints[i].position, _splinePoints[0].position, color);
					}
					else if(i < _splinePoints.Length - 1)
					{
						Debug.DrawLine(_splinePoints[i].position, _splinePoints[i + 1].position, color);
					}
				}
			}
		}

		[Conditional("DEBUG")]
		public void DrawNormals(float extrusion, Color color)
		{
			if(ValidatePoints())
			{
				for(var i = 0; i < _splinePoints.Length; i++)
				{
					Debug.DrawLine(_splinePoints[i].position, _splinePoints[i].position + _splinePoints[i].normal * extrusion, color);
				}
			}
		}

		[Conditional("DEBUG")]
		public void DrawTangents(float extrusion, Color color)
		{
			if(ValidatePoints())
			{
				for(var i = 0; i < _splinePoints.Length; i++)
				{
					Debug.DrawLine(_splinePoints[i].position, _splinePoints[i].position + _splinePoints[i].tangent * extrusion, color);
				}
			}
		}

		//Evaluates curve at t[0, 1]. Returns point/normal/tan struct. [0, 1] means clamped between 0 and 1.
		public static CatmullRomPoint Evaluate(
			Vector3 start,
			Vector3 end,
			Vector3 tanPoint1,
			Vector3 tanPoint2,
			float t)
		{
			Vector3 position = CalculatePosition(start, end, tanPoint1, tanPoint2, t);
			Vector3 tangent = CalculateTangent(start, end, tanPoint1, tanPoint2, t);
			Vector3 normal = NormalFromTangent(tangent);

			return new CatmullRomPoint(position, tangent, normal);
		}

		//Calculates curve position at t[0, 1]
		public static Vector3 CalculatePosition(
			Vector3 start,
			Vector3 end,
			Vector3 tanPoint1,
			Vector3 tanPoint2,
			float t)
		{
			// Hermite curve formula:
			// (2t^3 - 3t^2 + 1) * p0 + (t^3 - 2t^2 + t) * m0 + (-2t^3 + 3t^2) * p1 + (t^3 - t^2) * m1

			float tSqr = t * t;
			float tCube = tSqr * t;

			Vector3 position =
				(2.0f * tCube - 3.0f * tSqr + 1.0f) * start +
				(tCube - 2.0f * tSqr + t) * tanPoint1 +
				(-2.0f * tCube + 3.0f * tSqr) * end +
				(tCube - tSqr) * tanPoint2;

			return position;
		}

		public static Vector3 CalculatePosition(Vector3 start, Vector3 middle, Vector3 end, float t)
		{
			float dt = t * 2;

			Vector3 tanMiddle = (end - start).normalized;

			if(dt <= 1f)
			{
				Vector3 tanStart = (middle - start).normalized;
				return CalculatePosition(start, middle, tanStart, tanMiddle, dt);
			}

			dt -= 1;
			Vector3 tanEnd = (end - middle).normalized;
			return CalculatePosition(middle, end, tanMiddle, tanEnd, dt);
		}

		//Calculates tangent at t[0, 1]
		public static Vector3 CalculateTangent(
			Vector3 start,
			Vector3 end,
			Vector3 tanPoint1,
			Vector3 tanPoint2,
			float t)
		{
			// Calculate tangents
			// p'(t) = (6t² - 6t)p0 + (3t² - 4t + 1)m0 + (-6t² + 6t)p1 + (3t² - 2t)m1
			Vector3 tangent = (6 * t * t - 6 * t) * start
							  + (3 * t * t - 4 * t + 1) * tanPoint1
							  + (-6 * t * t + 6 * t) * end
							  + (3 * t * t - 2 * t) * tanPoint2;

			return tangent.normalized;
		}

		//Calculates normal vector from tangent
		public static Vector3 NormalFromTangent(Vector3 tangent)
		{
			return Vector3.Cross(tangent, Vector3.up).normalized / 2;
		}

		//Validates if splinePoints have been set already. Throws nullref exception.
		private bool ValidatePoints()
		{
			if(_splinePoints == null)
			{
				throw new NullReferenceException("Spline not initialized!");
			}

			return _splinePoints != null;
		}

		//Sets the length of the point array based on resolution/closed loop.
		private void InitializeProperties()
		{
			int pointsToCreate;
			if(_closedLoop)
			{
				pointsToCreate = _resolution * _controlPoints.Length; //Loops back to the beggining, so no need to adjust for arrays starting at 0
			}
			else
			{
				pointsToCreate = _resolution * (_controlPoints.Length - 1);
			}

			_splinePoints = new CatmullRomPoint[pointsToCreate];
		}

		//Math stuff to generate the spline points
		private void GenerateSplinePoints()
		{
			InitializeProperties();

			Vector3 p0, p1; //Start point, end point
			Vector3 m0, m1; //Tangents

			// First for loop goes through each individual control point and connects it to the next, so 0-1, 1-2, 2-3 and so on
			int closedAdjustment = _closedLoop ? 0 : 1;
			for(var currentPoint = 0; currentPoint < _controlPoints.Length - closedAdjustment; currentPoint++)
			{
				bool closedLoopFinalPoint = _closedLoop && currentPoint == _controlPoints.Length - 1;

				p0 = _controlPoints[currentPoint];

				if(closedLoopFinalPoint)
				{
					p1 = _controlPoints[0];
				}
				else
				{
					p1 = _controlPoints[currentPoint + 1];
				}

				// m0
				if(currentPoint == 0) // Tangent M[k] = (P[k+1] - P[k-1]) / 2
				{
					if(_closedLoop)
					{
						m0 = p1 - _controlPoints[_controlPoints.Length - 1];
					}
					else
					{
						m0 = p1 - p0;
					}
				}
				else
				{
					m0 = p1 - _controlPoints[currentPoint - 1];
				}

				// m1
				if(_closedLoop)
				{
					if(currentPoint == _controlPoints.Length - 1) //Last point case
					{
						m1 = _controlPoints[(currentPoint + 2) % _controlPoints.Length] - p0;
					}
					else if(currentPoint == 0) //First point case
					{
						m1 = _controlPoints[currentPoint + 2] - p0;
					}
					else
					{
						m1 = _controlPoints[(currentPoint + 2) % _controlPoints.Length] - p0;
					}
				}
				else
				{
					if(currentPoint < _controlPoints.Length - 2)
					{
						m1 = _controlPoints[(currentPoint + 2) % _controlPoints.Length] - p0;
					}
					else
					{
						m1 = p1 - p0;
					}
				}

				m0 *= 0.5f; //Doing this here instead of  in every single above statement
				m1 *= 0.5f;

				float pointStep = 1.0f / _resolution;

				if((currentPoint == _controlPoints.Length - 2 && !_closedLoop) || closedLoopFinalPoint) //Final point
				{
					pointStep = 1.0f / (_resolution - 1); // last point of last segment should reach p1
				}

				// Creates [resolution] points between this control point and the next
				for(var tesselatedPoint = 0; tesselatedPoint < _resolution; tesselatedPoint++)
				{
					float t = tesselatedPoint * pointStep;

					CatmullRomPoint point = Evaluate(p0, p1, m0, m1, t);

					_splinePoints[currentPoint * _resolution + tesselatedPoint] = point;
				}
			}
		}

		//Struct to keep position, normal and tangent of a spline point
		[Serializable]
		public struct CatmullRomPoint
		{
			public Vector3 position;
			public Vector3 tangent;
			public Vector3 normal;

			public CatmullRomPoint(Vector3 position, Vector3 tangent, Vector3 normal)
			{
				this.position = position;
				this.tangent = tangent;
				this.normal = normal;
			}
		}
	}
}