using UnityEngine;
using Unity.Mathematics;
using System;
public class CameraVision : MonoBehaviour
{
    // public bool activateVision = true;
    [Range(0, 1)] public float DirectionalLightsIntensity = 1f;
    public Light[] lights;
    public LayerMask worldMask;
    public float darkHeightPointHigh = 30f;
    public float darkHeightPointLow = 20f;
    public float fadeTargetAlpha = 0;
    public float fadeTime = 0.2f;
    public float fadeDistance = 2f;
    public bool fadeLastValue;
    Player player;

    void Start()
    {
        player = TagResolver.i.player;
    }

    void FadeUp(GameObject f)
    {
        //iTween.Stop(f);
        iTween.FadeTo(f, iTween.Hash("alpha", 1, "time", fadeTime, "oncomplete", "SetDifuseShading", "oncompletetarget", this.gameObject, "oncompleteparams", f));
    }

    void FadeDown(GameObject f)
    {
        //iTween.Stop(f);
        iTween.FadeTo(f, iTween.Hash("alpha", fadeTargetAlpha, "time", fadeTime));
    }

    private void OnPreCull()
    {
        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.intensity = DirectionalLightsIntensity;
                light.enabled = DirectionalLightsIntensity > 0;
            }
        }
    }
    private void OnPreRender()
    {
        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.intensity = DirectionalLightsIntensity;
                light.enabled = DirectionalLightsIntensity > 0;
            }
        }
    }
    private void OnPostRender()
    {
        foreach (Light light in lights)
        {
            if (light != null)
            {
                {
                    light.intensity = 1.0f;
                    light.enabled = true;
                }
            }
        }
    }
    public bool useHeightControl = false;
    public bool useLaser = false;
    public bool useSpikes = false;
    public bool useChunkLight = true;
    void Update()
    {
        if (useHeightControl)
        {
            DoHeightControl();
        }
        if (useLaser)
        {
            DoLaser();
        }
        if (useSpikes)
        {
            DoSpikes();
        }
        if (useChunkLight)
        {
            var _cp = Utils.ChunkPosbyPosition(Settings.PlayerPosition);
            int4 cp = new int4(_cp.x, 0, _cp.z, 0);
            var id = Utils.CoordByPosition(Settings.PlayerPosition);
            if (LightData.Data.ContainsKey(cp))
            {
                float? light = LightData.Data[cp][id.x, id.y, id.z];
                DirectionalLightsIntensity = light != null ? Unity.Mathematics.math.min(light.Value, 1f) : 0f;
            }

        }
        if (Vector3.Distance(transform.position, player.transform.position) <= fadeDistance)
        {
            player.transform.Find("mesh_root").gameObject.SetActive(false);
            player.transform.Find("Character1_Reference").gameObject.SetActive(false);
        }
        else
        {
            player.transform.Find("mesh_root").gameObject.SetActive(true);
            player.transform.Find("Character1_Reference").gameObject.SetActive(true);

        }

    }

    private void DoHeightControl()
    {
        float intencity = 1.0f;
        float playerPosY = transform.parent.position.y;
        if (playerPosY > darkHeightPointHigh)
        {
            intencity = 1;
        }
        else if (playerPosY < darkHeightPointLow)
        {
            intencity = 0;
        }
        else
        {
            float range = darkHeightPointHigh - darkHeightPointLow;
            intencity = Mathf.Abs(playerPosY - darkHeightPointLow) / range;
        }
        DirectionalLightsIntensity = intencity;
    }
    [Header("general")]
    [Range(1, 100)] public int rayCount = 4;
    [Range(0, 50)] public int maxStepDistance = 10;
    [Range(0, 50)] public int maxReflectionCount = 10;

    [Header("length")]
    public float lengthTimer = 0f;
    public float lengthDeadline = 1f;
    public int lengthTTL = 1;
    public float maxLength = 20;
    [Range(0, 1)] public float maxLengthTTL = 0;

    [Header("distance")]
    public float distanceTimer = 0f;
    public float distanceDeadline = 1f;
    public int distanceTTL = 1;
    public float maxDistance = 20;
    public float decayTime = 3;
    [Range(0, 1)] public float maxDistanceTTL = 0;
    public void DoLaser()
    {
        lengthTimer += Time.deltaTime;
        maxLengthTTL = lengthTimer;
        if (lengthTimer > lengthDeadline)
        {

        }
        distanceTimer += Time.deltaTime;
        maxDistanceTTL = distanceTimer;
        if (distanceTimer > distanceDeadline)
        {
            float timeElapsed = distanceTimer - distanceDeadline;
            float timeLeft = decayTime - timeElapsed;
            if (timeLeft < 0) timeLeft = 0;
            if (timeLeft > decayTime) timeLeft = decayTime;
            float val = timeLeft / decayTime;
            DirectionalLightsIntensity = val;
        }
        var direction = UnityEngine.Random.insideUnitSphere.normalized;
        for (var i = 1; i <= rayCount; i++)
        {
            Laser(transform.position, direction);
        }
    }


    void Laser(Vector3 from, Vector3 direction)
    {
        Vector3 startPoint = transform.position;
        (float length, Vector3 endPoint) result = DrawReflectionPattern(from, direction, maxReflectionCount);
        float distance = Vector3.Distance(startPoint, result.endPoint);
        if (result.length >= maxLength)
        {
            lengthTimer = 0f;
        }
        if (distance >= maxDistance)
        {
            distanceTimer = 0f;
        }
    }

    private (float length, Vector3 endPoint) DrawReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining, float totalDistance = 0)
    {
        if (reflectionsRemaining == 0)
        {
            return (totalDistance, position);
        }

        Vector3 startingPosition = position;

        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
            totalDistance += hit.distance;
        }
        else
        {
            position += direction * maxStepDistance;
            totalDistance += maxStepDistance;
        }

        Debug.DrawLine(startingPosition, position, Color.blue);
        return DrawReflectionPattern(position, direction, reflectionsRemaining - 1, totalDistance);
    }

    public float inverseResolution = 10f;

    private void DoSpikes()
    {
        lengthTimer += Time.deltaTime;
        maxLengthTTL = lengthTimer;
        if (lengthTimer > lengthDeadline)
        {

        }
        distanceTimer += Time.deltaTime;
        maxDistanceTTL = distanceTimer;
        if (distanceTimer > distanceDeadline)
        {
            float timeElapsed = distanceTimer - distanceDeadline;
            float timeLeft = decayTime - timeElapsed;
            if (timeLeft < 0) timeLeft = 0;
            if (timeLeft > decayTime) timeLeft = decayTime;
            float val = timeLeft / decayTime;
            DirectionalLightsIntensity = val;
        }

        Vector3 direction = Vector3.right;
        int steps = Mathf.FloorToInt(360f / inverseResolution);
        Quaternion xRotation = Quaternion.Euler(Vector3.right * inverseResolution);
        Quaternion yRotation = Quaternion.Euler(Vector3.up * inverseResolution);
        Quaternion zRotation = Quaternion.Euler(Vector3.forward * inverseResolution);
        for (int x = 0; x < steps / 2; x++)
        {
            direction = zRotation * direction;
            for (int y = 0; y < steps; y++)
            {
                direction = xRotation * direction;
                Laser(transform.position, direction);
            }
        }
    }
}