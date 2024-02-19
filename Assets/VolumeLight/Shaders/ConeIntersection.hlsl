#ifndef INTERSECT_CONE
#define INTERSECT_CONE

float3 GetRayPoint(float3 rayPos, float3 rayDir, float offset)
{
    return rayPos + rayDir * offset;
}

float2 GetSquareRoot(float a, float b, float c)
{
    float delta = (b * b) - (4 * a * c);
    delta = max(delta, 0);
    
    float deltaSqrt = sqrt(delta);    
    
    float t1 = (-b + deltaSqrt) / (2 * a);
    float t2 = (-b - deltaSqrt) / (2 * a);
    
    return float2(t1, t2);
}

//return Intersection Point of Ray and Plane, normalized in distance from Ray Position
float GetPlaneIntersection(float3 planePos, float3 planeDir, float3 rayPos, float3 rayDir)
{
    return dot((planePos - rayPos), planeDir) / dot(rayDir, planeDir);
}

//return Intersection Points of Ray and Infinite Cone, normalized in distance from Ray Position
float2 GetConeIntersections(float3 conePos, float3 coneDir, float coneAngleSqrCos, float3 rayPos, float3 rayDir)
{    
    //Vector C
    float3 vecC = rayPos - conePos;
    
    float temp1 = dot(rayDir, coneDir);
    float temp2 = dot(vecC, coneDir);
    
    //dot(rayDir,coneDir)^2 - cos(angle)^2
    float a = temp1 * temp1 - coneAngleSqrCos;
    //2*(dot(rayDir,coneDir)*dot(C,coneDir)-dot(rayDir,C)*cos(angle)^2)
    float b = 2 * (temp1 * temp2 - dot(rayDir, vecC) * coneAngleSqrCos);
    //dot(C,coneDir)^2-dot(C,C)*cos(angle)^2
    float c = temp2 * temp2 - dot(vecC, vecC) * coneAngleSqrCos;
    
    return GetSquareRoot(a, b, c);   
}

//return Intersection Points of Ray and Finite Cone, normalized in distance from Ray Position
float2 GetFiniteConeIntersections(float3 conePos, float3 coneDir, float coneAngleSqrCos, float coneHeight, float3 rayPos, float3 rayDir)
{
    float3 conePlanePos = GetRayPoint(conePos,coneDir,coneHeight);

    float2 coneIntersects = GetConeIntersections(conePos, coneDir, coneAngleSqrCos, rayPos, rayDir);
    float conePlaneIntersect = GetPlaneIntersection(conePlanePos, coneDir, rayPos, rayDir);
        
    //flip order, so first Cone Intersect point should be always on first correct edge
    if (rayDir.z < 0)
        coneIntersects.xy = coneIntersects.yx;
    
    float3 secondConePoint = GetRayPoint(rayPos, rayDir, coneIntersects[1]);
       
    float t1 = coneIntersects[0];
    //if Ray direction is flipped, order of Plane and second Cone Intersect flip
    float t2 = lerp(min(conePlaneIntersect, coneIntersects[1]), max(conePlaneIntersect, coneIntersects[1]), rayDir.z < 0);
    //if second Cone Intersect point if over Cone Position, it is in Virtual Cone (not valid)
    t2 = lerp(t2, conePlaneIntersect, secondConePoint.z < conePos.z);
    
    return float2(t1, t2);
}

void GetConeIntersections_float(float3 conePos, float3 coneDir, float angleSqrCos, float coneHeight, float3 rayPos, float3 rayDir, out float p1, out float p2)
{
    float2 coneIntersects = GetFiniteConeIntersections(conePos, coneDir, angleSqrCos, coneHeight, rayPos, rayDir);
        
    p1 = coneIntersects[0];
    p2 = coneIntersects[1];
}

void GetRayPoint_float(float3 rayPos, float3 rayDir, float offset, out float3 p1)
{
    p1 = GetRayPoint(rayPos, rayDir, offset);
}

#endif
