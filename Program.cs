using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
     static void Main()
    {

    }
     
public abstract class Criatura
{
    public string nome {get; set;}
    public int vida {get; set;}
    public int ataque {get; set;}
    public int defesa {get; set;}

    public virtual void atacar ()
    {

    }

    public void receberDano(int dano)
    {
        int dano = ataque - defesa 
    }
}

}
