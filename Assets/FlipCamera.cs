using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Camera))]
public class HSymmetry : MonoBehaviour {

    void OnPreCull () {
        Matrix4x4 scale;
        if(GetComponent<Camera>().aspect >2){
            scale = Matrix4x4.Scale (new Vector3 (-1, 1, 1));
        }else{
                scale = Matrix4x4.Scale (new Vector3 (1, -1, 1));
        }
        GetComponent<Camera>().ResetWorldToCameraMatrix ();
        GetComponent<Camera>().ResetProjectionMatrix ();
        GetComponent<Camera>().projectionMatrix = GetComponent<Camera>().projectionMatrix * scale;
    }
    void OnPreRender () {
        GL.invertCulling = true;
    }
    void OnPostRender () {
        GL.invertCulling = false;
    }
}