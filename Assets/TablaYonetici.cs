using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TablaYonetici : MonoBehaviour {

    [SerializeField] GameObject piyonPrefab;
    Piyon[,] piyonlar = new Piyon[7, 7];

    Piyon secilenPiyon;

    Vector3 piyonOffset = new Vector3(-0.5f,0f,-0.5f);

    Vector2 mouseOver;
    Vector2 startDrag;
    Vector2 endDrag;

    int[] sonucDerece = new int[9];

    [SerializeField] GameObject sonuc;
    [SerializeField] GameObject sonucReferans;

    void UpdateMouseOver()
    {
        if (!Camera.main)
            Debug.Log("Main camera bulunamadı!");

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,25.0f,LayerMask.GetMask("Tabla")))
        {
            mouseOver.y =(int)(hit.point.x-piyonOffset.x);
            mouseOver.x = (int)(hit.point.z-piyonOffset.z);
        }
        else
        {
            mouseOver.x = -100;
            mouseOver.y = -100;
        }
    }

    private void Update()
    {
        UpdateMouseOver();

       // Debug.Log(mouseOver.x + " " + mouseOver.y);

        int x = (int)mouseOver.x;
        int y = (int)mouseOver.y;

        if(secilenPiyon != null)
        {
            if (Input.GetMouseButtonDown(0))
                TasimaDene((int)startDrag.x, (int)startDrag.y, x, y);
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                PiyonSec(x, y);

                if(secilenPiyon != null)
                {
                    UpdatePiyonDrag(secilenPiyon);
                }
            }
        }

        if (OyunBittimi())
        {
            //Debug.Log("Kalan piyon" + PiyonSayac());
            sonuc.transform.position = Vector3.Lerp(sonuc.transform.position, sonucReferans.transform.position, Time.deltaTime);
            sonuc.transform.rotation = Quaternion.Slerp(sonuc.transform.rotation,sonucReferans.transform.rotation, Time.deltaTime);
        }

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hit))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.name == "sonuc")
                    {
                        SceneManager.LoadScene("game");

                    }

                }

            }
        }
    }

    private void PiyonSec(int x, int y)
    {
        //Out of bounds
        if (x < 0 || x >= 7 || y < 0 || y >= 7)
            return;

        Piyon p = piyonlar[x, y];

        if(p!=null)
        {
            secilenPiyon = p;
            startDrag = mouseOver;
        }
        
    }

    private void UpdatePiyonDrag(Piyon p)
    {
        if (!Camera.main)
            Debug.Log("Main camera bulunamadı!");

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Tabla")))
            p.transform.position = hit.point + (Vector3.up * 2);

    }

    bool OyunBittimi()
    {
        for(int x= 0; x < 7;x++)
        {
            for(int y =0;y<7;y++)
            {
                if(x < 5)
                {
                    if(TasimaDogrula(x,y,x+2,y))
                    {
                        return false;
                    }
                }

                if (y < 5)
                {
                    if (TasimaDogrula(x, y, x, y + 2))
                    {
                        return false;
                    }
                }

                if(x > 1)
                {
                    if (TasimaDogrula(x, y, x - 2, y))
                    {
                        return false;
                    }
                }

                if (y > 1)
                {
                    if (TasimaDogrula(x, y, x, y -2))
                    {
                        return false;
                    }
                }
            }
        }

        int sayac = PiyonSayac();
        if (PiyonSayac() >= 9)
            sayac = 0;

        sonucReferans.transform.eulerAngles = new Vector3(270.0f, sonucDerece[sayac], 0.0f);

        return true;
    }

    int PiyonSayac()
    {
        int sayac = 0;

        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                if (piyonlar[i, j] != null)
                    sayac++;

        return sayac;
    }

    private void TasimaDene(int x1, int y1, int x2, int y2)
    {
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2,y2);
        secilenPiyon = piyonlar[x1, y1];

        //Out of bounds
        if (x2 < 0 || x2 >= 7 || y2 < 0 || y2 >= 7)
        {
            if (secilenPiyon != null)
                PiyonTasi(secilenPiyon, x1, y1);

            startDrag = Vector2.zero;
            secilenPiyon = null;
            return;
        }

        if(secilenPiyon != null)
        {
            //Eğer aynı yere taşınmak isterse taşıma iptal edilir
            if(endDrag == startDrag)
            {
                PiyonTasi(secilenPiyon, x1, y1);

                startDrag = Vector2.zero;
                secilenPiyon = null;
                return;

            }

            //Taşınabilir mi kontrol edilecek
            if (TasimaDogrula(x1, y1, x2, y2))
            {
                if(x1 == x2)
                {
                    Destroy(piyonlar[x1, (y1 + y2) / 2].gameObject);
                    piyonlar[x1, (y1 + y2) / 2] = null;
                }
                else
                {
                    Destroy(piyonlar[(x1 + x2) / 2, y1].gameObject);
                    piyonlar[(x1 + x2) / 2, y1] = null;
                }

                Debug.Log(PiyonSayac());

                piyonlar[x2, y2] = secilenPiyon;
                piyonlar[x1, y1] = null;
                PiyonTasi(secilenPiyon, x2, y2);
                secilenPiyon = null;
                startDrag = Vector2.zero;
            }
        }

    }

    private bool TasimaDogrula(int x1, int y1, int x2, int y2)
    {
        //yasaklı konumlar
        if (x2 == 0 && y2 == 0 || x2 == 0 && y2 == 1 || x2 == 0 && y2 == 5 || x2 == 0 && y2 == 6
         || x2 == 1 && y2 == 0 || x2 == 1 && y2 == 1 || x2 == 1 && y2 == 5 || x2 == 1 && y2 == 6
         || x2 == 5 && y2 == 0 || x2 == 5 && y2 == 1 || x2 == 5 && y2 == 5 || x2 == 5 && y2 == 6
         || x2 == 6 && y2 == 0 || x2 == 6 && y2 == 1 || x2 == 6 && y2 == 5 || x2 == 6 && y2 == 6)
            return false;

        if(x1 == x2)
        {
            int y3;

            if(y1 > y2)
            {
                if (y1 - y2 > 2 || y1 - y2 < 2)
                    return false;

                y3 = y1 - 1;
            }
            else
            {
                if (y2 - y1 > 2 || y2 - y1 < 2)
                    return false;

                y3 = y2 - 1;
            }

            if (piyonlar[x1, y1] != null && piyonlar[x2, y2] == null && piyonlar[x1, y3] != null)
                return true;
        }
        else if (y1 == y2)
        {
            int x3;

            if (x1 > x2)
            {
                if (x1 - x2 > 2 || x1 - x2 < 2)
                    return false;

                x3 = x1 - 1;
            }
            else
            {
                if (x2 - x1 > 2 || x2 - x1 < 2)
                    return false;

                x3 = x2 - 1;
            }

            if (piyonlar[x1, y1] != null && piyonlar[x2, y2] == null && piyonlar[x3, y1] != null)
                return true;
        }

        return false;
    }

    void Start () {

        SonucDereceHesapla();

        TablaOlustur();
	}

    private void SonucDereceHesapla()
    {
        int derece = 10;
        for(int i = 0;i<9;i++)
        {
            sonucDerece[i] = derece;
            derece += 40;
        }
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
