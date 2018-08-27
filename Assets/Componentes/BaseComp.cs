using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public delegate void TipoAvisoComponente ( object sender, EventArgs args );

public abstract class BaseComp<TSalida> : MonoBehaviour, IComp {

    public bool autoEjecucion;

    public IComp CompEntrada {
        get { return compEntrada; }
        set { compEntrada = value; }
    }
    public List<IComp> Componentes { get { return componentes; } }

    public TSalida ObjSalida {
        get {
			// Si objSalida existe, entonces devuelve su propio objeto de salida
            if ( objSalida != null ) {
                return objSalida;
            }
			// Si tiene componentes hijos, devuelve como salida la del ultimo componente hijo
            if ( Componentes != null && Componentes.Count > 0 ) {
                IComp ultimoComp = componentes [ componentes.Count - 1 ];
                if ( ultimoComp.GetType ().IsSubclassOf ( typeof ( BaseComp<TSalida> ) )
                    || ultimoComp.GetType ().Equals ( typeof ( BaseComp<TSalida> ) ) )
                {
                    return ( ( BaseComp<TSalida> ) ultimoComp ).ObjSalida;
                }
            }
			// Si no tiene nada, pasa el objeto de salida de su entrada sin modificar
            if (CompEntrada != null) {
                if ( CompEntrada.GetType ().IsSubclassOf ( typeof ( BaseComp<TSalida> ) )
                    || CompEntrada.GetType ().Equals ( typeof ( BaseComp<TSalida> ) ) ) {
                    return ( ( BaseComp<TSalida> ) CompEntrada ).ObjSalida;
                }
            }
            return default ( TSalida );
        }
        set {
            objSalida = value;
        }
    }
    public virtual object ObjetoJSON {
        get { throw new NotImplementedException (); }
        set { throw new NotImplementedException (); }
    }
	public virtual object Parametros {
		get { throw new NotImplementedException(); }
		set { throw new NotImplementedException(); }
	}
	public event TipoAvisoComponente AvisarInicializado;
    public event TipoAvisoComponente AvisarFinalizado;
    
    protected List<IComp> componentes;
    private IComp compEntrada;
    protected TSalida objSalida;
    bool inicializado;

    protected virtual void Start () {
        if ( autoEjecucion && !inicializado)
            Inicializar ();
    }

	protected virtual void Update () {
        if ( autoEjecucion ) {
			Actualizar();
        }
    }
    public virtual void Inicializar () {
        if (isActiveAndEnabled) {
			if(inicializado)
				Debug.LogWarning(this.GetType().ToString() + ": " + this.name + ". ¡Objeto ya inicializado!");
			AsignarCompHijos();
			foreach(IComp comp in componentes) {
				MonoBehaviour monoComp = (MonoBehaviour) comp;
				if (monoComp.isActiveAndEnabled)
					comp.Inicializar();
			}
			if(AvisarInicializado != null)
				AvisarInicializado.Invoke(this, new EventArgs());
			inicializado = true;
		}
	}
	public virtual void Actualizar() {
		if(isActiveAndEnabled) {
			if(!inicializado) {
				Inicializar();
			}
			if(componentes != null) {
				foreach(IComp comp in componentes) {
					comp.Actualizar();
				}
			}
			Ejecutar();
			if(SiFinalizar()) {
				Finalizar();
			}
		}
	}

	public abstract void Ejecutar();

	public virtual bool SiFinalizar() {
		return false;
	}

    public virtual void Finalizar () {
		if(componentes != null) {
			foreach ( IComp cText in componentes ) {
				cText.Finalizar ();
			}
		}
        if ( AvisarFinalizado != null )
            AvisarFinalizado.Invoke ( this, new EventArgs () );
        // gameObject.SetActive ( false );
        autoEjecucion = false;
    }
    public CompT CompHijo<CompT> ( int i ) where CompT : MonoBehaviour, IComp {
        return ( CompT ) componentes [ i ];
    }
    public CompT CompHijo<CompT> ( bool siNecesarioActivo = false ) where CompT : MonoBehaviour, IComp {
        foreach ( IComp comp in componentes ) {
            if ( comp.GetType () == typeof ( CompT ) ) {
                if (siNecesarioActivo ? ( ( CompT ) comp).enabled : true)
                    return ( CompT ) comp;
            }
        }
        return default(CompT);
    }

    public CompT CompPadre<CompT> () where CompT : MonoBehaviour, IComp {

        if ( transform.parent != null ) {
            IComp [] tCompsPadre = transform.parent.GetComponents<IComp> ();
            if ( tCompsPadre != null ) {
                foreach ( IComp tCompPadre in tCompsPadre) {
                    if ( tCompPadre.GetType () == typeof ( CompT ) ) {
                        return ( CompT ) tCompPadre;
                    }
                    return tCompPadre.CompPadre<CompT> ();
                }
            }
        }
        return default ( CompT );
    }
    public CompT CompHermano<CompT> (bool siNecesarioActivo = false) where CompT : MonoBehaviour, IComp {
        IComp tcomp = null;
        if ( transform.parent != null ) {
            IComp [] tCompsPadre = transform.parent.GetComponents<IComp> ();
            if ( tCompsPadre != null ) {
                foreach ( IComp tCompPadre in tCompsPadre ) {
                    /*if ( tCompPadre.GetType () == typeof ( CompT ) ) {
                        return ( CompT ) tCompPadre;
                    } */
                    tcomp = tCompPadre.CompHijo<CompT> (siNecesarioActivo);
                    if ( tcomp == null ) {
                        tcomp = tCompPadre.CompHermano<CompT> (siNecesarioActivo);
                    }
					if (tcomp != null)
						return (CompT) tcomp;
                }
            }
        }
        return ( CompT ) tcomp;
    }

    void AsignarCompHijos () {
        componentes = new List<IComp> ();
        for ( int cmp = 0; cmp < transform.childCount; cmp++ ) {
            IComp comp = transform.GetChild ( cmp ).GetComponent<IComp> ();
            if (comp != null) {
                componentes.Add ( comp );
                if ( cmp > 0 )
                    comp.CompEntrada = componentes [ cmp - 1 ];
            }
        }
    }

	//Compara si el tipo de la salida del componente de entrada coincide con lo que se necesita que entre
	public bool ComparaElTipoSalidaDeCompEntradaCon(Type tipoEntrada) {
		//Debug.Log(CompEntrada.GetType().BaseType.GetGenericArguments()[0]);
		if(CompEntrada.GetType().BaseType.GetGenericArguments()[0].Equals(tipoEntrada)) {
			return true;
		}
		Debug.LogError("La entrada no es de tipo " + tipoEntrada.ToString() + ", revisalo.");
		return false;
	}
}
