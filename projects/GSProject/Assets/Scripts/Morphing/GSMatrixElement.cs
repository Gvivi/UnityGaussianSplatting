using System.Collections.Generic;
using UnityEngine;
using GaussianSplatting.Runtime;


[System.Serializable]
public class GSMatrixElement
{
    public GaussianSplatAsset[] SubElements = new GaussianSplatAsset[0];

    public GSMatrixElement()
    {
        SubElements = new GaussianSplatAsset[0];
    }

    public GSMatrixElement(int size)
    {
        SubElements = new GaussianSplatAsset[size];
    }
}