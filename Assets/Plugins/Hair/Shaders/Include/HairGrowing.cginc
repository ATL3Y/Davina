float3 GetBezierPoint(float3 p0, float3 p1, float3 p2, float t)
{
	float invT = 1 - t;
	return invT*invT*p0 + 2 * invT*t*p1 + t*t*p2;
}

uint ToIndex1D(uint x, uint y, uint sizeY)
{
	return x*sizeY + y;
}

struct Body
{
	float3 guidePosition;
	float3 position;
	float3 lastPosition;
	float radius;
};

float3 GetSplinePoint(int x, float t, StructuredBuffer<Body> bodies, uint sizeY)
{
	int y = (int)(t*sizeY);
	float tStep = 1.0f / sizeY;
	float localT = (t % tStep) * sizeY;

	int startI = x*sizeY;

	int y0 = max(0, y - 1);
	int y1 = min(y, sizeY - 1);
	int y2 = min(y + 1, sizeY - 1);

	float3 p0 = bodies[startI + y0].position;
	float3 p1 = bodies[startI + y1].position;
	float3 p2 = bodies[startI + y2].position;

	float3 cPoint1 = (p0 + p1)*0.5f;
	float3 cPoint2 = (p1 + p2)*0.5f;

	return GetBezierPoint(cPoint1, p1, cPoint2, localT);
}