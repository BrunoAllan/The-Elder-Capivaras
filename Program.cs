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
        public string Nome {get; set;}
        public int Vida {get; set;}
        public int Ataque {get; set;}
        public int Defesa {get; set;}

        public virtual void Atacar (int Ataque)
        {

        }

        public void ReceberDano(int Dano)
        {
            int Dano = Ataque - Defesa 
        }
    }

    class CapivaraGuerreiro : Criatura
    {
        public override void Atacar()
        {

        }
    }

    class CapivaraMago : Criatura
    {

        public int Mana { get; set; }

        public override void Atacar()
        {

        }
    }

    class CapivaraArqueira : Criatura
    {
        public override void Atacar()
        {

        }
    }

    class Dragao : Criatura
    {

        public int Furia { get; set; }
        
    }

}
