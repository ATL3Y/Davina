using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXShaderZTest : MonoBehaviour
{
    private Material mat;
    private enum OnLight { Default, Yes, No };
    private OnLight onLight = OnLight.Default;

    private void Start ( )
    {
        mat = GetComponent<SkinnedMeshRenderer> ( ).material;
        if ( Lens.instance != null )
        {
            if ( Lens.instance.LightSide )
            {
                onLight = OnLight.Yes;
            }
            else
            {
                onLight = OnLight.No;
            }
        }
    }

	void Update ()
    {
        if( onLight == OnLight.Default )
        {
            if ( Lens.instance != null )
            {
                if ( Lens.instance.LightSide )
                {
                    onLight = OnLight.Yes;
                }
                else
                {
                    onLight = OnLight.No;
                }
            }
            else
            {
                return;
            }
        }

        // If we don't match the lens
        if( Lens.instance.LightSide && onLight != OnLight.Yes )
        {
            onLight = OnLight.Yes;
            mat.SetInt ( "_TestMode", 8 );
        }
        else if( !Lens.instance.LightSide && onLight != OnLight.No )
        {
            onLight = OnLight.No;
            mat.SetInt ( "_TestMode", 4 );
        }
	}
}
