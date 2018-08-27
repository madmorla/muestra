using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HijoComp : BaseComp<Color> {
    

    public override void Ejecutar () {
        GetComponent<Renderer> ().material.color = ObjSalida;

        if ( CompEntrada == null ) {
            GetComponent<Renderer> ().material.color = ObjSalida;
        } else {
            BaseComp<Color> compEn = ( HijoComp ) CompEntrada;
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