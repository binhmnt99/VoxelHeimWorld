using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class UnitActionSystem : Singleton<UnitActionSystem>
{

    [SerializeField] private AudioClip click, pop;
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;


    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask tileLayerMask;

    private Tile currentTile;

    private BaseAction selectedAction;
    private bool isBusy;

    private void Start()
    {

    }

    private void Update()
    {
        if (isBusy)
        {
            return;
        }
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Clear();
        if (TryGetTile())
        {
            return;
        }

    }

    private bool TryGetTile()
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, tileLayerMask))
        {
            return false;
        }
        currentTile = hit.transform.GetComponent<Tile>();
        InspectTile();
        return true;
    }

    private void Clear()
    {
        if (currentTile == null || currentTile.Occupied == false)
            return;

        //currentTile.ModifyCost(currentTile.terrainCost-1);//Reverses to previous cost and color after being highlighted
        currentTile.ClearHighlight();
        currentTile = null;
    }

    private void InspectTile()
    {
        //Alter cost by right clicking
        if (InputManager.Instance.GetRightMouseButtonDown())
        {
            //currentTile.ModifyCost();
            return;
        }

        if (currentTile.Occupied)
        {
            if (currentTile.occupyingCharacter.IsEnemy())
            {
                return;
            }
            HandleSelectUnit();
        }
        else
            HandleSelectAction();
    }

    private void HandleSelectAction()
    {
        if (selectedUnit == null)
        {
            return;
        }
        if (!selectedAction.IsValidActionGridPosition(currentTile))
        {
            return;
        }
        Path path = Pathfinder.Instance.FindPath(selectedUnit.unitTile, currentTile);
        if (InputManager.Instance.GetLeftMouseButtonDown())
        {

            if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
            {
                return;
            }

            SetBusy();
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
        if (currentTile.occupyingCharacter.Moving)
            return;

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
        GetComponent<AudioSource>().PlayOneShot(pop);
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
