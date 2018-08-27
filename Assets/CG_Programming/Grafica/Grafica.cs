using System;
using UnityEngine;
using UnityEngine.UI;

public class Grafica : MonoBehaviour {

	public ComputeShader graficaShader;
	Material matRT;

	[Header("Propiedades de la Textura")]
	private RawImage imagen;
    public int resolucionX = 1920;
    public int resolucionY = 1080;
	public Color colorDeFondo = new Color(0.0f, 0.0f, 0.0f, 1.0f);

	[Header("Propiedades de la Grafica")]
	public Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	public Vector2 escala = new Vector2(1,1);
	public Vector2 desplazamiento;
	public Vector2[] puntos; //Por pares: [puntoInicio0, puntoFin0, puntoInicio1, puntoFin1, ...]
	
	[Header("Eje")]
	public Color colorDeEje = new Color(200f/255f, 200f/255f, 200f/255f, 1.0f);

	[Header("Cuadricula")]
	public Color colorDeCuadricula = new Color(100f/255f, 100f/255f, 100f/255f, 1.0f);


	private ComputeBuffer puntosBuffer;
	private RenderTexture texturaGrafica;
	private int lengthPuntosBuffer;
	
	//Kernels
	private int pintarGraficaKernel;
	private int limpiaGraficaKernel;
	private int pintarEjesKernel;
	private int pintarCuadriculaKernel;


    void Start() {

		if(null == graficaShader) {
			Debug.Log("Shader missing.");
			enabled = false;
			return;
		}
		
		pintarGraficaKernel = graficaShader.FindKernel("PintarGrafica");
		limpiaGraficaKernel = graficaShader.FindKernel("LimpiaImagen");
		pintarEjesKernel = graficaShader.FindKernel("PintarEjes");
		pintarCuadriculaKernel = graficaShader.FindKernel("PintarCuadricula");
		

		if(pintarGraficaKernel < 0 || limpiaGraficaKernel < 0 || pintarEjesKernel < 0 || pintarCuadriculaKernel < 0) {
			Debug.Log("Initialization failed.");
			enabled = false;
			return;
		}

		//Generacion de puntos y creacion del buffer a partir de los puntos
		puntos = GeneradorArrayPuntos(2000);
		puntosBuffer = new ComputeBuffer(puntos.Length, sizeof(float)*2);
		graficaShader.SetBuffer(pintarGraficaKernel, "Puntos", puntosBuffer);

		RectTransform rectT = GetComponent<RectTransform>();
		if(rectT){
			Rect imagenRect = rectT.rect;
			resolucionX = (int)imagenRect.width;
			resolucionY = (int)imagenRect.height;
		}

		//Creacion de la RenderTexture donde vamos a generar la grafica
		texturaGrafica = new RenderTexture(resolucionX, resolucionY, 32, RenderTextureFormat.ARGB32);
		texturaGrafica.enableRandomWrite = true;
		texturaGrafica.name = "texGrafica" + GetInstanceID();
		texturaGrafica.wrapMode = TextureWrapMode.Clamp;
		texturaGrafica.filterMode = FilterMode.Trilinear;
		texturaGrafica.useMipMap = false;
		//texturaGrafica.antiAliasing = 2;
		texturaGrafica.anisoLevel = 0;
		texturaGrafica.Create();

		//Para mostrar la grafica en UI
		imagen = this.GetComponent<RawImage>();
		if(imagen){
			imagen.texture = texturaGrafica;
		}

		//Para mostrar la grafica en un objeto con un nuevo material
		MeshRenderer rend = GetComponent<MeshRenderer>();
		if(rend){
			matRT = new Material(Shader.Find("Standard"));
			matRT.mainTexture = texturaGrafica;
			rend.material = matRT;
		}

		

		

	}


	void Update()
    {

        CambiarTamanoBuffer();

		EjecutarShader();

    }

    private void CambiarTamanoBuffer()
    {
        //Si cambia la longitud del array de puntos, entonces cambiamos el tamaño del buffer
        if (lengthPuntosBuffer != puntos.Length)
        {
            if (null != puntosBuffer)
            {
                puntosBuffer.Release();
                puntosBuffer = null;
            }
            puntosBuffer = new ComputeBuffer(puntos.Length, sizeof(float) * 2);
            graficaShader.SetBuffer(pintarGraficaKernel, "Puntos", puntosBuffer);
            //Asignamos el nuevo length
            lengthPuntosBuffer = puntos.Length;
        }
    }


	void EjecutarShader() {
		//Se pone en update porque necesita asignarselo otra vez al compute shader porque si hay mas de un script lo machaca y se queda con el ultimo que se ha puesto.
		graficaShader.SetVector("resolucion", (Vector2) new Vector2(resolucionX, resolucionY));
		graficaShader.SetTexture(limpiaGraficaKernel, "Result", texturaGrafica);
		graficaShader.SetTexture(pintarEjesKernel, "Result", texturaGrafica);
		graficaShader.SetTexture(pintarCuadriculaKernel, "Result", texturaGrafica);
		graficaShader.SetTexture(pintarGraficaKernel, "Result", texturaGrafica);


		//Limpiar Grafica
		graficaShader.SetVector("colorDeFondo", (Vector4)colorDeFondo);
		
		//Escala y desplazamiento afecta a pintarejes y pintarGrafica
		graficaShader.SetVector("escala", (Vector2)escala);
        graficaShader.SetVector("desplazamiento", (Vector2)desplazamiento);
		
		//PintarEjes
		graficaShader.SetVector("colorDeEje", (Vector4)colorDeEje);

		//PintarCuadricula
		graficaShader.SetVector("colorDeCuadricula", (Vector4)colorDeCuadricula);

        //PintarGrafica
        graficaShader.SetVector("color", (Vector4)color);
        puntosBuffer.SetData(puntos);

		graficaShader.Dispatch(limpiaGraficaKernel, (texturaGrafica.width + 31) / 32, (texturaGrafica.height + 31) / 32, 1);
		graficaShader.Dispatch(pintarEjesKernel, 1, 1, 1);
		graficaShader.Dispatch(pintarCuadriculaKernel, 2, 2, 1);
        graficaShader.Dispatch(pintarGraficaKernel, 1, puntos.Length - 1, 1);
    }




    void OnDestroy() {
		if(null != puntosBuffer) {
			puntosBuffer.Release();
			puntosBuffer = null;
		}
	}

	Vector2[] GeneradorArrayPuntos(int numPuntos){
		Vector2[] pTemp = new Vector2[numPuntos];
		for(int i = 0; i < numPuntos; i++){
			//float valor = UnityEngine.Random.Range(-100,100);
			float valor = 100 * Mathf.Sin((float) i / 10);
			pTemp[i] = new Vector2(i, valor);
		}

		return pTemp;
	}


}