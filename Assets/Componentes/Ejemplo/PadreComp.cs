using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadreComp : BaseComp<Color> {

    public float velocidad;

    public override void Ejecutar () {
        CompHijo<HijoComp> ().ObjSalida = ObjSalida;
        GetComponent<Renderer> ().material.color = ObjSalida;

        if ( CompEntrada == null ) {
            GetComponent<Renderer> ().material.color = ObjSalida;
        } else {
            BaseComp<Color> compEn = ( BaseComp<Color> ) CompEntrada;
            ObjSalida = new Color (
                compEn.ObjSalida.r * 1.1f + 0.05f,
                compEn.ObjSalida.g * 1.08f + 0.05f,
                compEn.ObjSalida.b * .8f + 0.05f );
        }
    }

    public override void Inicializar () {
        base.Inicializar ();
    }
}
