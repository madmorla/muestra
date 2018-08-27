using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EjemploJSON : MonoBehaviour {
	public GameObject mesh;
	string path;
	public string localPath;

	// Use this for initialization
	void Start () {
		path = Application.dataPath + "/JsonUtils/";
		Vector3[] vertices = mesh.GetComponent<MeshFilter>().mesh.vertices;
		JsonUtils.ExportarJson<Vector3[]>(vertices, path + localPath, "EjemploJSON");

		vertices = JsonUtils.ImportarJson<Vector3[]>(path + localPath, "EjemploJSON");

	}

}
