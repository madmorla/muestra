using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbueloComp : BaseComp<Color> {
    public override void Ejecutar () {
        
    }

    public override void Inicializar () {
        base.Inicializar ();
        GetComponent<Renderer> ().material.color = Color.red;
    }
}
