using SimpleGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PamsManager : Singleton<PamsManager>, ILoadable
{
    public Transform treeContainer;
    [HideInInspector] public PamsTree tree;
    private PamsElement currentSubtree;

    [SerializeField] private GameObject linePrefab;

    private List<GameObject> currentLines = new List<GameObject>();
    private Queue<GameObject> currentElements = new Queue<GameObject>();
    public Stack<string> previousSteps = new Stack<string>();

    public bool isVisualized = true;
    private bool isPointedToNothing = false;
    private bool isLinesVisible = true;
    private bool isSectionVisible = false;

    public TextMeshProUGUI currentElementNameContainer;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private float secondsToAnimate;

    public float currentDistance;

    private Transform mainCamera;

    public delegate void LookLinesAtCamera(Transform cameraTransform);
    public LookLinesAtCamera lookLinesAtMainCamera;

    public delegate void ShowSection(bool flag);
    public ShowSection showSections;

    public delegate void ShowLines(bool isLinesVisible, bool isSectionVisible);
    public ShowLines showLines;

    public GameObject mainWindowContainer;
    public Vector2 mainCameraViewScaling;

    public GameObject infoWindowContainer;
    public Vector2 infoCameraViewScaling;
    public TextMeshProUGUI infoContainer;

    public Toggle showSectionToggle;
    public Toggle showLinesToggle;

    private float3x2 positionToAnimate;
    private float3 positionToAnimateDifference;

    public void Load()
    {
        tree = treeContainer.GetComponent<PamsTree>();
        currentSubtree = tree.root;
        PoolManager.Instance.AddPool(PoolType.PamsElements);
        PoolManager.Instance.AddPool(PoolType.Lines);
        PoolManager.Instance.AddPool(PoolType.UI);
        isVisualized = true;

        mainCamera = Camera.main.transform;

        VisualizeSubtreeByName(currentSubtree.data.name);
    }

    private void Update()
    {
        if (isVisualized)
        {
            TryToZoomCamera();
            TryToRotatePamsSubtree();
            TryToFocusCamera();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            VisualizeSubtree(currentSubtree);
        }
    }

    public void VisualizeSubtreeByName(string subtreeName)
    {
        if (isVisualized)
        {
            isVisualized = false;

            PamsElement subtree = PamsTree.FindSubtreeByName(tree.root, subtreeName);

            VisualizeSubtree(subtree);
        }
    }

    public void VisualizeChildByName(string childName)
    {
        if (isVisualized)
        {
            isVisualized = false;

            PamsElement child = null;

            for (int i = 0; i < currentSubtree.children.Count; i++)
            {
                if (currentSubtree.children[i].data.name == childName)
                {
                    child = currentSubtree.children[i];
                    break;
                }
            }

            if (child != null)
            {
                VisualizeSubtree(child);
            }
            else
            {
                isVisualized = true;
            }
        }
    }

    private void VisualizeSubtree(PamsElement subtree)
    {
        Vector3 deviationOfSubtreeFromTree;
        deviationOfSubtreeFromTree = PamsTree
                    .GetDeviationOfSubtreeFromTreeByName(subtree, currentSubtree.data.name);

        if (deviationOfSubtreeFromTree == Vector3.zero)
        {
            deviationOfSubtreeFromTree = - PamsTree
                    .GetDeviationOfSubtreeFromTreeByName(currentSubtree, subtree.data.name) +
                    (float3)currentSubtree.data.position + (float3)subtree.data.position;
        }
        
        StartCoroutine(ChangeCameraTransform(
            new float3x2(
                -deviationOfSubtreeFromTree +
                treeContainer.position +
                currentSubtree.data.position,
                -subtree.data.position),
            new float2x2(
                new float2(
                    -cameraController.rotation.positionOffset.y,
                    cameraController.rotation.positionOffset.x),
                subtree.data.previewRotation),
            new float2(
                currentSubtree.data.cameraDistance *
                cameraController.zooming.currentCoefficient,
                subtree.data.cameraDistance)
            ));

        treeContainer.position = -deviationOfSubtreeFromTree +
                treeContainer.position +
                currentSubtree.data.position;

        if (previousSteps.Count == 0 || previousSteps.Peek() != subtree.data.name)
        {
            previousSteps.Push(currentSubtree.data.name);
        }
        else
        {
            previousSteps.Pop();
        }

        DespawnCurrentSubtree();

        currentSubtree = subtree;

        currentDistance = currentSubtree.data.cameraDistance;

        currentElementNameContainer.text = currentSubtree.data.name;



        VisualizeSubtree(subtree, treeContainer);

        OnChangeShowSection(showSectionToggle);
        OnChangeShowLines(showLinesToggle);
    }

    private IEnumerator ChangeElementPosition(float3x2 positionFromTo)
    {
        positionToAnimate = positionFromTo;
        positionToAnimateDifference = positionFromTo.c1 - positionFromTo.c0;

        yield return AnimationHelper
            .DoSomethingForSeconds(AnimateElementPosition, secondsToAnimate);

    }

    private void AnimateElementPosition(float coefficient)
    {

        treeContainer.localPosition = positionToAnimate.c0 + 
            positionToAnimateDifference * coefficient;
    }

    private void VisualizeSubtree(PamsElement subtree, Transform folder)
    {
        GameObject element = PoolManager.Instance
            .Spawn(
                PoolType.PamsElements, 
                subtree.data.GO,
                subtree.data.position, 
                Quaternion.Euler(subtree.data.rotation), 
                folder
            );
      
        currentElements.Enqueue(element);

        if (subtree == currentSubtree)
        {
            VisualizeLines(subtree.data.lines, element.transform.Find("Lines"));
        }

        for (int i = 0; i < subtree.children.Count; i++)
        {
            VisualizeSubtree(subtree.children[i], element.transform);
        }
    }
    
    private void VisualizeLines(List<LineData> lines, Transform folder)
    {
        GameObject line;

        for (int i = 0; i < lines.Count; i++)
        {
            line = PoolManager.Instance.Spawn(PoolType.Lines, linePrefab);
            line.transform.SetParent(folder);
            line.GetComponent<Line>().SetOptions(lines[i]);           
            currentLines.Add(line);
        }

        line = null;
    }
    
    private IEnumerator ChangeCameraTransform(
        float3x2 positionFromTo,
        float2x2 rotationFromTo,
        float2 distanceFromTo
        )
    {
        yield return null;

        StartCoroutine(ChangeElementPosition(positionFromTo));
        StartCoroutine(cameraController.ChangeCameraRotationRelativeCenterFromTo(
            rotationFromTo,
            secondsToAnimate
        ));
        StartCoroutine(cameraController.ChangeCameraDistanceFromTo(
            distanceFromTo,
            secondsToAnimate
        ));
    }

    public void DespawnCurrentSubtree()
    {
        for (int i = 0; i < currentLines.Count; i++)
        {
            PoolManager.Instance.Despawn(PoolType.Lines, currentLines[i]);
        }

        while (currentElements.Count >= 1)
        {
            PoolManager.Instance.Despawn(PoolType.PamsElements, currentElements.Dequeue());
        }

        currentLines.Clear();

        currentSubtree = null;
    }

    private void TryToRotatePamsSubtree()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var data = PointerRaycast(Input.mousePosition);
            isPointedToNothing = data == null;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPointedToNothing = false;
        }
        else if (Input.GetMouseButton(0))
        {
            if (isPointedToNothing)
            {
                cameraController.RotateCameraAroundCenter();

                cameraController.SetCameraDistance(currentSubtree.data.cameraDistance);

                lookLinesAtMainCamera?.Invoke(mainCamera);
            }
        }
    }

    private void TryToZoomCamera()
    {
        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseScrollWheel != 0)
        {
            var data = PointerRaycast(Input.mousePosition);

            if (data == null || data.layer == 8)
            {
                cameraController.ZoomCameraDistance(mouseScrollWheel);

                cameraController.SetCameraDistance(currentSubtree.data.cameraDistance);
            }
        }
    }

    private void TryToFocusCamera()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var data = PointerRaycast(Input.mousePosition);

            if (data == null)
            {
                StartCoroutine(ChangeElementPosition(
                    new float3x2(
                        treeContainer.position,
                        -currentSubtree.data.position)
                ));
            }
            else
            {
                TextMeshProUGUI lineText = data.GetComponent<TextMeshProUGUI>();
                if (lineText != null)
                {
                    Vector3 linePosition = GetLinePositionByName(lineText.text);
                    if (linePosition != Vector3.zero)
                    {
                        StartCoroutine(ChangeElementPosition(
                            new float3x2(
                                treeContainer.position,
                                -linePosition - currentSubtree.data.position)
                        ));
                    }
                }
            }   
        }
    }

    private Vector3 GetLinePositionByName(string name)
    {
        for (int i = 0; i < currentLines.Count; i++)
        {
            Line line = currentLines[i].GetComponent<Line>();
            if (line.data.name == name)
            {
                return line.data.localPosition;
            }
        }

        return Vector3.zero;
    }

    private GameObject PointerRaycast(Vector2 position)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        List<RaycastResult> resultsData = new List<RaycastResult>();
        pointerData.position = position;
        EventSystem.current.RaycastAll(pointerData, resultsData);

        if (resultsData.Count > 0)
        {
            return resultsData[0].gameObject;
        }

        return null;
    }

    public void OnChangeShowSection(Toggle toggle)
    {
        isSectionVisible = toggle.isOn;
        showSections?.Invoke(isSectionVisible);
        showLines?.Invoke(isLinesVisible, isSectionVisible);
    }
    public void OnChangeShowLines(Toggle toggle)
    {
        isLinesVisible = toggle.isOn;
        showLines?.Invoke(isLinesVisible, isSectionVisible);
    }
    
    public void OnChangeSetInfoTextSize(Slider slider)
    {
        SetInfoTextSize(slider.value);
    }

    private void SetInfoTextSize(float value)
    {
        infoContainer.fontSize = value;
    }

    public void OnInfoClick()
    {
        mainWindowContainer.SetActive(false);
        infoWindowContainer.SetActive(true);
        Camera.main.rect = new Rect(infoCameraViewScaling, Vector2.one);

        for (int i = 0; i < currentLines.Count; i++)
        {
            currentLines[i].SetActive(false);
        }

        infoContainer.text = currentSubtree.data.description;
    }

    public void OnCloseInfoClick()
    {
        mainWindowContainer.SetActive(true);
        infoWindowContainer.SetActive(false);
        Camera.main.rect = new Rect(mainCameraViewScaling, Vector2.one);

        showLines(isLinesVisible, isSectionVisible);
    }

    public void OnExitClick()
    {
        Application.Quit();
    }

    public void OnReturnClick()
    {
        if (previousSteps.Count > 0)
            VisualizeSubtreeByName(previousSteps.Peek());
    }
}
