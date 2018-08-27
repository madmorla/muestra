using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public interface IComp {

    IComp CompEntrada  {
        get;
        set;
    }
    List<IComp> Componentes {
        get;
    }
    object ObjetoJSON {
        get;
        set;
    }
	object Parametros {
		get;
		set;
	}
	void Inicializar ();
	void Actualizar ();
    void Ejecutar ();
    void Finalizar ();

    TComp CompHijo<TComp> ( int i ) where TComp : MonoBehaviour, IComp;
    TComp CompHijo<TComp> (bool siNecesarioActivo) where TComp : MonoBehaviour, IComp;
    TComp CompPadre<TComp> () where TComp : MonoBehaviour, IComp;
    TComp CompHermano<TComp> (bool siNecesarioActivo) where TComp : MonoBehaviour, IComp;
}
