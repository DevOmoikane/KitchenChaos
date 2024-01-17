using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounterHeater : BaseCounter, IHasProgress {

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnHeaterChangedEventArgs> OnHeaterChanged;
    public class  OnHeaterChangedEventArgs : EventArgs { 
        public bool heaterOn;
    }

    [SerializeField] private HeatingRecipeSO[] heatingRecipeSOArray;

    private float heatingTimer;
    HeatingRecipeSO heatingRecipeSO;

    private void Start() {
    }

    private void Update() {
        if (HasKitchenObject()) {
            heatingTimer += Time.deltaTime;
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                progressNormalized = heatingTimer / heatingRecipeSO.heatingTimerMax
            });
            if (heatingTimer >= heatingRecipeSO.heatingTimerMax) {
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(heatingRecipeSO.output, this);
                if (HasRecipeForInput(GetKitchenObject().GetKitchenObjectSO())) {
                    heatingRecipeSO = GetHeatingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    heatingTimer = 0f;
                    OnHeaterChanged?.Invoke(this, new OnHeaterChangedEventArgs { heaterOn = true });
                }
            }
        }
    }

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject()) {
                if (HasRecipeForInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    heatingRecipeSO = GetHeatingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    heatingTimer = 0f;
                    OnHeaterChanged?.Invoke(this, new OnHeaterChangedEventArgs { heaterOn = true });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = heatingTimer / heatingRecipeSO.heatingTimerMax
                    });
                }
            }
        } else {
            if (!player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
                OnHeaterChanged?.Invoke(this, new OnHeaterChangedEventArgs { heaterOn = false });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = 0f
                });
            }
        }
    }

    public override void InteractAlternate(Player player) {
    }

    private bool HasRecipeForInput(KitchenObjectSO inputKitchenObjectSO) {
        HeatingRecipeSO heatingRecipeSO = GetHeatingRecipeSOWithInput(inputKitchenObjectSO);
        return heatingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        HeatingRecipeSO heatingRecipeSO = GetHeatingRecipeSOWithInput(inputKitchenObjectSO);
        if (heatingRecipeSO != null) {
            return heatingRecipeSO.output;
        }
        return null;
    }

    private HeatingRecipeSO GetHeatingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (HeatingRecipeSO heatingRecipeSO in heatingRecipeSOArray) {
            if (heatingRecipeSO.input == inputKitchenObjectSO) {
                return heatingRecipeSO;
            }
        }
        return null;
    }
}
