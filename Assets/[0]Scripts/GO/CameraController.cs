using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{ 
    private Transform mainCameraTransform;

    public CameraZoomingData zooming;
    public CameraRotationData rotation;

    private float2 distanceToAnimate;
    private float distanceToAnimateDifference;
    private float2x2 rotationToAnimate;
    private float2 rotationToAnimateDifference;
    
    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    public void RotateCameraAroundCenter()
    {
        rotation.positionOffset.x = 
            mainCameraTransform.localEulerAngles.y +
            Input.GetAxis("Mouse X") * 
            rotation.sensitivity;

        rotation.positionOffset.y += Input.GetAxis("Mouse Y") * rotation.sensitivity;

        rotation.positionOffset.y =
            Mathf.Clamp(
                rotation.positionOffset.y,
                -rotation.angleLimit,
                rotation.angleLimit
            );

        SetCameraRotationRelativeCenter(
            new float2(
                -rotation.positionOffset.y,
                rotation.positionOffset.x
            )
        );
    }

    public void SetCameraRotationRelativeCenter(float2 rotation)
    {
        this.rotation.positionOffset.x = rotation.y;
        this.rotation.positionOffset.y = -rotation.x;

        mainCameraTransform.localEulerAngles =
            new Vector3(
                rotation.x,
                rotation.y,
                0
            );    
    }

    public void SetCameraDistance(float distance)
    {
        mainCameraTransform.position = mainCameraTransform.localRotation *
            new Vector3(
                0,
                0,
                -distance * zooming.currentCoefficient
            );
    }

    public IEnumerator ChangeCameraDistanceFromTo(float2 distanceFromTo, float seconds)
    {
        zooming.currentCoefficient = 1;

        distanceToAnimate = distanceFromTo;

        distanceToAnimateDifference = distanceToAnimate.y - distanceToAnimate.x;

        yield return StartCoroutine(AnimationHelper
            .DoSomethingForSeconds(AnimateCameraDistance, seconds));

        PamsManager.Instance.isVisualized = true;
    }

    public IEnumerator ChangeCameraRotationRelativeCenterFromTo(
        float2x2 rotationFromTo,
        float seconds
        )
    {
        rotationToAnimate = rotationFromTo;

        rotationToAnimateDifference = rotationFromTo.c1 - rotationFromTo.c0;

        if (Math.Abs(rotationToAnimateDifference.y) > 180)
        {

            rotationFromTo.c1.y += (rotationFromTo.c1.y <= 180) ? 360 : -360;

            rotationToAnimateDifference = rotationFromTo.c1 - rotationFromTo.c0;
        }

        yield return StartCoroutine(AnimationHelper
            .DoSomethingForSeconds(AnimateCameraRotationRelativeCenter, seconds));

        PamsManager.Instance.isVisualized = true;
    }

    public void ZoomCameraDistance(float mouseScrollWheel)
    {
        zooming.currentCoefficient -= 
            mouseScrollWheel * 
            Time.deltaTime * 
            zooming.sensitivity;

        if (zooming.currentCoefficient > zooming.coefficientRange.y)
        {
            zooming.currentCoefficient = zooming.coefficientRange.y;
        }
        else if (zooming.currentCoefficient < zooming.coefficientRange.x)
        {
            zooming.currentCoefficient = zooming.coefficientRange.x;
        }
    }

    public void AnimateCameraDistance(float coefficient)
    {
        PamsManager.Instance.isVisualized = false;

        SetCameraDistance(distanceToAnimate.x + distanceToAnimateDifference * coefficient);
    }

    public void AnimateCameraRotationRelativeCenter(float coefficient)
    {
        PamsManager.Instance.isVisualized = false;
       
        SetCameraRotationRelativeCenter(
            rotationToAnimate.c0 + 
            rotationToAnimateDifference * 
            coefficient
        );

        PamsManager.Instance.lookLinesAtMainCamera?.Invoke(mainCameraTransform);
    }

}
