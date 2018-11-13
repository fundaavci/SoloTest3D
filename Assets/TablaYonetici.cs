using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablaYonetici : MonoBehaviour {

    [SerializeField] GameObject piyonPrefab;
    Piyon[,] piyonlar = new Piyon[7, 7];

	void Start () {
        TablaOlustur();
	}

    private void TablaOlustur()
    {
        for (int x = 0; x < 7;x++)
        {
            for (int y = 0; y < 7; y++)
            {
                if(x!=3 || y!=3)
                    if(x> 1 && x <5 || y>1 && y<5)
                        PiyonOlustur(x, y);
            }
        }
    }

    private void PiyonOlustur(int x, int y)
    {
        GameObject go = Instantiate(piyonPrefab) as GameObject;
        Piyon p = go.GetComponent<Piyon>();
        piyonlar[x, y] = p;
        PiyonTasi(p, x, y);
    }

    private void PiyonTasi(Piyon p, int x, int y)
    {
        p.transform.position = (Vector3.forward * x) + (Vector3.right * y);
    }
}
