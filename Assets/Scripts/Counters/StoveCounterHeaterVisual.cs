using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterHeaterVisual : MonoBehaviour {

    [SerializeField] private StoveCounterHeater stoveCounterHeater;
    [SerializeField] private GameObject stoveOnGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start() {
        stoveCounterHeater.OnHeaterChanged += StoveCounterHeater_OnHeaterChanged;
    }

    private void StoveCounterHeater_OnHeaterChanged(object sender, StoveCounterHeater.OnHeaterChangedEventArgs e) {
        stoveOnGameObject.SetActive(e.heaterOn);
        particlesGameObject.SetActive(e.heaterOn);
    }
}
