using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class UnitActionSystem : Singleton<UnitActionSystem>
{

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private AudioClip click, pop;
    [SerializeField] private LayerMask tileLayerMask;

    private AudioSource audioSource;
    private Unit selectedUnit;
    private Tile currentTile;
    private Tile lastVisualTile;
    private Tile.TileVisualType lastMaterial;
    private BaseAction selectedAction;
    private bool isBusy;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isBusy || !TurnSystem.Instance.IsPlayerTurn() || EventSystem.current.IsPointerOverGameObject())
            return;

        Clear();
        if (TryGetTile())
            return;
    }

    private bool TryGetTile()
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, tileLayerMask))
            return false;

        currentTile = hit.transform.GetComponent<Tile>();
        InspectTile();
        return true;
    }

    private void Clear()
    {
        if (currentTile == null || !currentTile.Occupied)
            return;

        currentTile.ClearHighlight();
        currentTile = null;
    }

    private void InspectTile()
    {
        if (InputManager.Instance.GetRightMouseButtonDown())
            return;

        if (currentTile.Occupied)
        {
            if (currentTile.occupyingCharacter.IsEnemy())
                return;

            HandleSelectUnit();
        }
        else
        {
            HandleSelectAction();
        }
    }

    private void HandleSelectAction()
    {
        if (selectedUnit == null || !selectedAction.IsValidActionGridPosition(currentTile))
            return;

        Path path = Pathfinder.Instance.FindPath(selectedUnit.unitTile, currentTile);
        if (lastVisualTile != null)
        {
            lastVisualTile.SetMaterial(lastMaterial);
        }
        lastVisualTile = currentTile;
        lastMaterial = currentTile.selectTileVisualType;
        if (lastVisualTile != null)
        {
            lastVisualTile.SetMaterial(Tile.TileVisualType.White);
        }

        if (InputManager.Instance.GetLeftMouseButtonDown())
        {
            if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
                return;

            SetBusy();
            Pathfinder.Instance.ResetPathfinder();
            selectedAction.TakeAction(path, null, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void HandleSelectUnit()
    {
        currentTile.Highlight();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectedAction != null)
            {
                selectedAction.HideValidTile();
                Pathfinder.Instance.ResetPathfinder();
            }
            SelectUnit();
        }
    }

    private void SelectUnit()
    {
        selectedUnit = currentTile.occupyingCharacter;
        audioSource.PlayOneShot(pop);
        SetSelectedAction(selectedUnit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        selectedAction.ShowValidTile();
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }
}
