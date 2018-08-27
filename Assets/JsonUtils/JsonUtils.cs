using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class FlexSpringsJSON {
	public int springsCount;
	public int[] springIndices;
	public float[] springRestLengths;
	public float[] springCoefficients;
}

public static class JsonUtils {

	public const string JSONEXT = ".json";

	/***** METODOS GENERICOS *****/

	// IMPORTAR

	public static T ImportarJson<T>(string path, string nombreFichero, bool log = true) {
		string filePath = path + nombreFichero + JSONEXT;

		T datos = default(T);
		
		if(File.Exists(filePath)) {
			string dataAsJson = File.ReadAllText(filePath);

			datos = JsonConvert.DeserializeObject<T>(dataAsJson
				//Por si da este error...
				, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
				);
			if(log) {
				Debug.Log("Se han importado los datos del archivo '" + nombreFichero + JSONEXT + "'.");
			}
		} else {
			Debug.LogError("No se ha encontrado el archivo con el nombre '" + nombreFichero + JSONEXT + "' en la ruta '" + path + "'.");
		}
		return datos;
	}

	public static T ImportarJson<T>(string path, string nombreFichero, ReferenceLoopHandling refLoopHdl, bool log = true) {
		string filePath = path + nombreFichero + JSONEXT;

		T datos = default(T);

		if(File.Exists(filePath)) {
			string dataAsJson = File.ReadAllText(filePath);

			datos = JsonConvert.DeserializeObject<T>(dataAsJson
				//Por si da este error...
				, new JsonSerializerSettings { ReferenceLoopHandling = refLoopHdl }
				);
			if(log) {
				Debug.Log("Se han importado los datos del archivo '" + nombreFichero + JSONEXT + "'.");
			}
		} else {
			Debug.LogError("No se ha encontrado el archivo con el nombre '" + nombreFichero + JSONEXT + "'.");
		}
		return datos;
	}

	// EXPORTAR

	public static void ExportarJson<T>(T datos, string path, string nombreFichero, bool log = true) {
		string filePath = path + nombreFichero + JSONEXT;

		if(!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
			Debug.Log("Se ha creado el directorio en el path: " + path);
		}

		string dataAsJson = "";
		dataAsJson += JsonConvert.SerializeObject(datos, Formatting.Indented
			//Por si da este error...
			, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
			);

		File.WriteAllText(filePath, dataAsJson);
		if(log) {
			Debug.Log("Se han exportado los datos del archivo '" + nombreFichero + JSONEXT + "' en " + path);
		}
	}

    public static void ExportarJsonComprimido<T> ( T datos, string path, string nombreFichero, bool log = true ) {
        string filePath = path + nombreFichero + JSONEXT;

        if ( !Directory.Exists ( path ) ) {
            Directory.CreateDirectory ( path );
            Debug.Log ( "Se ha creado el directorio en el path: " + path );
        }

        string dataAsJson = "";
        dataAsJson += JsonConvert.SerializeObject ( datos,
            new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                FloatFormatHandling = FloatFormatHandling.Symbol,
                FloatParseHandling = FloatParseHandling.Double,
                Formatting = Formatting.None,
                MaxDepth = 2
            }
            );

        File.WriteAllText ( filePath, dataAsJson );
        if ( log ) {
            Debug.Log ( "Se han exportado los datos del archivo '" + nombreFichero + JSONEXT + "' comprimido en " + path );
        }
    }
    public static void ExportarJson<T>(T datos, string path, string nombreFichero, ReferenceLoopHandling refLoopHdl, bool log = true) {
		string filePath = path + nombreFichero + JSONEXT;

		if(!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
			Debug.Log("Se ha creado el directorio en el path: " + path);
		}

		string dataAsJson = "";
		dataAsJson += JsonConvert.SerializeObject(datos, Formatting.Indented
			//Por si da este error...
			, new JsonSerializerSettings { ReferenceLoopHandling = refLoopHdl }
			);

		File.WriteAllText(filePath, dataAsJson);
		if(log) {
			Debug.Log("Se han exportado los datos del archivo '" + nombreFichero + JSONEXT + "'.");
		}
	}

}
