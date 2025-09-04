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

        public virtual void Atacar (Criatura Alvo)
        {

        }

        public void ReceberDano(int dano)
        {
          Vida -= dano;
          if (Vida < 0) Vida = 0;
        }

    }

    class CapivaraGuerreiro : Criatura
     {
    private static Random rand = new Random();

    public override void Atacar(Criatura alvo)
    {
        int dano = Ataque - alvo.Defesa;
        if (dano < 0) dano = 0;

        if (rand.NextDouble() < 0.2) // 20% de crítico
        {
            dano *= 2;
            Console.WriteLine($"{Nome} acertou um CRÍTICO em {alvo.Nome}!");
        }

        alvo.ReceberDano(dano);
        Console.WriteLine($"{Nome} causou {dano} de dano em {alvo.Nome} (Vida restante: {alvo.Vida})");
    }
}


    class CapivaraMago : Criatura
{
    public int Mana { get; set; }
    private static Random rand = new Random();

    public override void Atacar(Criatura alvo)
    {
        if (Mana >= 10)
        {
            int dano = Ataque; // ignora defesa
            Mana -= 10;
            alvo.ReceberDano(dano);
            Console.WriteLine($"{Nome} lançou magia em {alvo.Nome}, causando {dano} de dano (Mana restante: {Mana})");
        }
        else
        {
            Console.WriteLine($"{Nome} tentou atacar, mas está sem mana!");
        }
    }
}


   class CapivaraArqueira : Criatura
{
    private static Random rand = new Random();

    public override void Atacar(Criatura alvo)
    {
        if (rand.NextDouble() < 0.3)
        {
            Console.WriteLine($"{Nome} errou o ataque contra {alvo.Nome}!");
            return;
        }

        int dano = Ataque - alvo.Defesa;
        if (dano < 0) dano = 0;

        alvo.ReceberDano(dano);
        Console.WriteLine($"{Nome} acertou {alvo.Nome}, causando {dano} de dano (Vida restante: {alvo.Vida})");
    }
}


    class Dragao : Criatura
    {

        public int Furia { get; set; }
        
    }

}
