using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tesste : MonoBehaviour
{

    int numVidas;
    int numMoedas;
    string Nome;
    int energia;
    int engine = 11;

    int val1, val2, result;

    // Start is called before the first frame update
    void Start()
    {

        List<int> lista = new List<int>();

        string[] teste = new string[5];

        teste[0] = "fogo";
        teste[1] = "caraças";
        teste[2] = "some daqui meu";
        teste[3] = "eita porra";
        teste[4] = "oshe";

        val1 = 10;
        val2 = 5;

        result = val1 + val2;

        print(result);

        if (engine > 10)
        {
            print("Unity");
        }

        for (int i = 0; i <= 10; i++)
        {
            print(i);
        }

        for (int i = 0; i <= teste.Length - 1; i++)
        {
            print(teste[i]);
        }

        for (int i = 0; i <= 20; i++)
        {
            lista.Add(i);
            print(lista[i]);
        }

        zumbi testezumbi = new zumbi();
        zumbi_filho testezumbifilho = new zumbi_filho();

        testezumbi.andar(true);
        testezumbifilho.andar("fooooodasse");

    }

    // Update is called once per frame
    void Update()
    {

    }

    class zumbi
    {

        public int hp;
        public string tipo;


        public void andar(bool consegue)
        {

            if (consegue == true)
            {
                print("consegue andar");
            }

        }

    }

    class zumbi_filho : zumbi
    {

        public void andar(string som)
        {
            print(som);
        }

    }
}


