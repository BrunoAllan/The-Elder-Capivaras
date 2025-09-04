using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Criatura
{
    public string Nome { get; set; }
    public int Vida { get; set; }
    public int VidaMax { get; set; }
    public int Ataque { get; set; }
    public int Defesa { get; set; }
    public bool Jogador { get; set; }

    protected static Random rng = new Random();

    public Criatura(string nome, int vida, int ataque, int defesa, bool jogador = false)
    {
        Nome = nome;
        Vida = vida;
        VidaMax = vida;
        Ataque = ataque;
        Defesa = defesa;
        Jogador = jogador;
    }

    public virtual void Atacar(Criatura alvo, string tipo = "normal")
    {
        int dano;
        bool critico = rng.NextDouble() < 0.2;

        switch (tipo)
        {
            case "perto": dano = Ataque + 5 - alvo.Defesa; break;
            case "longe": dano = Ataque - alvo.Defesa; break;
            default: dano = Ataque - alvo.Defesa; break;
        }
        if (dano < 0) dano = 0;
        if (critico) dano *= 2;

        alvo.Vida -= dano;
        if (alvo.Vida < 0) alvo.Vida = 0;

        MostrarAtaque(this.Nome, alvo.Nome, dano, critico);
        MostrarBarraColorida(alvo);
    }

    public virtual void AtaqueEspecial(Criatura alvo)
    {
        Atacar(alvo, "perto");
    }

    public void Defender()
    {
        Defesa += 5;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{Nome} aumentou a defesa em 5 pontos neste turno!");
        Console.ResetColor();
    }

    public void RecuperarVida(int qtd)
    {
        Vida += qtd;
        if (Vida > VidaMax) Vida = VidaMax;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{Nome} recuperou {qtd} de vida! Vida atual: {Vida}/{VidaMax}");
        Console.ResetColor();
    }

    public static void MostrarBarraColorida(Criatura c)
    {
        int total = 20;
        int filled = (int)((c.Vida / (double)c.VidaMax) * total);

        Console.Write("[");
        for (int i = 0; i < filled; i++)
        {
            if (c.Vida > c.VidaMax * 0.6) Console.ForegroundColor = ConsoleColor.Green;
            else if (c.Vida > c.VidaMax * 0.3) Console.ForegroundColor = ConsoleColor.Yellow;
            else Console.ForegroundColor = ConsoleColor.Red;

            Console.Write("#");
        }
        Console.ResetColor();

        for (int i = filled; i < total; i++)
            Console.Write("-");

        Console.WriteLine($"] {c.Vida}/{c.VidaMax}");
    }

    public static void MostrarAtaque(string atacante, string alvo, int dano, bool critico = false)
    {
        if (critico) Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"{atacante} → {alvo} causou {dano} de dano{(critico ? " CRÍTICO!" : "")}");
        Console.ResetColor();
    }
}

// Criaturas do jogador
public class CapivaraGuerreira : Criatura
{
    public CapivaraGuerreira(string nome, bool jogador = false) : base(nome, 100, 20, 10, jogador) { }
    public override void AtaqueEspecial(Criatura alvo)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"{Nome} usa Golpe Poderoso!");
        Console.ResetColor();
        int dano = Ataque * 2 - alvo.Defesa;
        if (dano < 0) dano = 0;
        alvo.Vida -= dano;
        if (alvo.Vida < 0) alvo.Vida = 0;
        MostrarAtaque(Nome, alvo.Nome, dano, true);
        MostrarBarraColorida(alvo);
    }
}

public class CapivaraMaga : Criatura
{
    public int Mana { get; set; } = 30;
    public CapivaraMaga(string nome, bool jogador = false) : base(nome, 80, 25, 5, jogador) { }

    public override void AtaqueEspecial(Criatura alvo)
    {
        if (Mana >= 15)
        {
            Mana -= 15;
            int dano = Ataque + 20;
            alvo.Vida -= dano;
            if (alvo.Vida < 0) alvo.Vida = 0;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{Nome} lançou Magia Suprema em {alvo.Nome}, causando {dano} de dano! (Mana: {Mana}/30)");
            Console.ResetColor();
            MostrarBarraColorida(alvo);
        }
        else
        {
            Console.WriteLine($"{Nome} não tem mana suficiente! Ataque normal será usado.");
            Atacar(alvo, "perto");
        }
    }

    public void RecuperarMana(int qtd)
    {
        Mana += qtd;
        if (Mana > 30) Mana = 30;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{Nome} recuperou {qtd} de mana! Mana atual: {Mana}/30");
        Console.ResetColor();
    }
}

public class CapivaraArqueira : Criatura
{
    private static Random rand = new Random();
    public CapivaraArqueira(string nome, bool jogador = false) : base(nome, 90, 18, 8, jogador) { }

    public override void AtaqueEspecial(Criatura alvo)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"{Nome} usa Tiro Múltiplo!");
        Console.ResetColor();
        for (int i = 0; i < 3; i++)
        {
            if (rand.NextDouble() < 0.3) { Console.WriteLine($"{Nome} errou o tiro {i + 1}!"); continue; }
            int dano = Ataque - alvo.Defesa;
            if (dano < 0) dano = 0;
            alvo.Vida -= dano;
            if (alvo.Vida < 0) alvo.Vida = 0;
            MostrarAtaque(Nome, alvo.Nome, dano);
        }
        MostrarBarraColorida(alvo);
    }
}

public class CapivaraNinja : Criatura
{
    public CapivaraNinja(string nome, bool jogador = false) : base(nome, 85, 22, 7, jogador) { }

    public override void AtaqueEspecial(Criatura alvo)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"{Nome} usa Ataque Furtivo!");
        Console.ResetColor();
        int dano = Ataque * 3 - alvo.Defesa;
        if (dano < 0) dano = 0;
        alvo.Vida -= dano;
        if (alvo.Vida < 0) alvo.Vida = 0;
        MostrarAtaque(Nome, alvo.Nome, dano, true);
        MostrarBarraColorida(alvo);
    }
}

// Dragão Boss
public class DragaoBoss : Criatura
{
    public DragaoBoss(double difMult = 1.0)
        : base("Dragão Boss", (int)(200 * difMult), (int)(30 * difMult), (int)(15 * difMult)) { }

    public override void AtaqueEspecial(Criatura alvo)
    {
        if (rng.NextDouble() < 0.3)
        {
            int dano = Ataque + 25;
            alvo.Vida -= dano;
            if (alvo.Vida < 0) alvo.Vida = 0;
            MostrarAtaque(Nome, alvo.Nome, dano, true);
        }
        else Atacar(alvo, "perto");
    }
}

// Inimigos intermediários
public class CapivaraSelvagem : Criatura { public CapivaraSelvagem() : base("Capivara Selvagem", 60, 12, 5) { } }
public class CapivaraAssassina : Criatura
{
    public CapivaraAssassina() : base("Capivara Assassina", 50, 20, 3) { }
    public override void AtaqueEspecial(Criatura alvo)
    {
        Console.WriteLine($"{Nome} usa Ataque Surpresa!");
        int dano = Ataque * 2 - alvo.Defesa;
        if (dano < 0) dano = 0;
        alvo.Vida -= dano;
        if (alvo.Vida < 0) alvo.Vida = 0;
        MostrarAtaque
(Nome, alvo.Nome, dano, true);
        MostrarBarraColorida(alvo);
    }
}

public class CapivaraFeiticeira : Criatura
{
    private static Random rand = new Random();
    public CapivaraFeiticeira() : base("Capivara Feiticeira", 55, 15, 4) { }
    public override void AtaqueEspecial(Criatura alvo)
    {
        if (rand.NextDouble() < 0.5)
        {
            int dano = Ataque + 10;
            alvo.Vida -= dano;
            if (alvo.Vida < 0) alvo.Vida = 0;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{Nome} lançou Magia Sombria em {alvo.Nome}, causando {dano} de dano!");
            Console.ResetColor();
            MostrarBarraColorida(alvo);
        }
        else
        {
            Console.WriteLine($"{Nome} tentou lançar magia, mas errou!");
        }
    }
}

// Torneio
public class Torneio
{
    private List<Criatura> Criaturas = new List<Criatura>();
    private static Random rng = new Random();
    private double multiplicadorInimigos = 1.0;

    public void ConfigurarDificuldade()
    {
        Console.WriteLine("Escolha dificuldade: 1-Fácil, 2-Normal, 3-Difícil");
        string escolha;
        do { Console.Write("Digite a opção: "); escolha = Console.ReadLine(); }
        while (escolha != "1" && escolha != "2" && escolha != "3");

        switch (escolha)
        {
            case "1": multiplicadorInimigos = 0.8; break;
            case "2": multiplicadorInimigos = 1.0; break;
            case "3": multiplicadorInimigos = 1.3; break;
        }
    }

    public void AdicionarCriatura(Criatura c)
    {
        if (!c.Jogador && !(c is DragaoBoss))
        {
            c.Vida = (int)(c.Vida * multiplicadorInimigos);
            c.Ataque = (int)(c.Ataque * multiplicadorInimigos);
            c.Defesa = (int)(c.Defesa * multiplicadorInimigos);
        }
        Criaturas.Add(c);
    }

    public void Iniciar()
    {
        Console.WriteLine("=== TORNEIO DAS CAPIVARAS ===\n");
        int rodada = 1;
        while (Criaturas.Count > 1)
        {
            Console.WriteLine($"\n--- Rodada {rodada} ---");
            Criaturas = Criaturas.OrderBy(x => rng.Next()).ToList();
            List<Criatura> vencedores = new List<Criatura>();

            for (int i = 0; i < Criaturas.Count; i += 2)
            {
                if (i + 1 >= Criaturas.Count)
                {
                    Console.WriteLine($"{Criaturas[i].Nome} avançou sem lutar!");
                    vencedores.Add(Criaturas[i]);
                    continue;
                }

                var a = Criaturas[i];
                var b = Criaturas[i + 1];
                Console.WriteLine($"\nBatalha: {a.Nome} (Vida {a.Vida}) vs {b.Nome} (Vida {b.Vida})");
                bool venceuA = Lutar(a, b);

                vencedores.Add(venceuA ? a : b);

                if ((a.Jogador && !venceuA) || (b.Jogador && venceuA))
                {
                    Console.WriteLine("Você perdeu sua batalha... fim de jogo!");
                    return;
                }
            }

            // Recuperação entre rodadas
            foreach (var c in vencedores)
            {
                c.RecuperarVida(c.VidaMax / 5);
                if (c is CapivaraMaga mago) mago.RecuperarMana(10);
            }

            Criaturas = vencedores;
            rodada++;
        }

        var campeao = Criaturas[0];
        if (campeao.Jogador)
            Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"=== PARABÉNS! VOCÊ É O CAMPEÃO COM {campeao.Nome}! ===");
        Console.ResetColor();
    }

    private bool Lutar(Criatura a, Criatura b)
    {
        while (a.Vida > 0 && b.Vida > 0)
        {
            if (a.Jogador) TurnoJogador(a, b); else a.Atacar(b, "perto");
            if (b.Vida > 0)
            {
                if (b.Jogador) TurnoJogador(b, a); else b.Atacar(a, "perto");
            }
        }
        return a.Vida > 0;
    }

    private void TurnoJogador(Criatura jogador, Criatura inimigo)
    {
        Console.WriteLine($"\nSeu turno! Vida: {jogador.Vida}/{jogador.VidaMax} | Inimigo: {inimigo.Nome} ({inimigo.Vida}/{inimigo.VidaMax})");
        if (jogador is CapivaraMaga mago) Console.WriteLine($"Mana: {mago.Mana}/30");

        Console.WriteLine("1 - Ataque de Perto");
        Console.WriteLine("2 - Ataque de Longe");
        Console.WriteLine("3 - Ataque Especial");
        Console.WriteLine("4 - Defender (+5 defesa)");
        Console.WriteLine("5 - Recuperar Vida (+20%)");

        string escolha;
        do { Console.Write("Escolha sua ação: "); escolha = Console.ReadLine(); }
        while (escolha != "1" && escolha != "2" && escolha != "3" && escolha != "4" && escolha != "5");

        switch (escolha)
        {
            case "1": jogador.Atacar(inimigo, "perto"); break;
            case "2": jogador.Atacar(inimigo, "longe"); break;
            case "3": jogador.AtaqueEspecial(inimigo); break;
            case "4": jogador.Defender(); break;
            case "5": jogador.RecuperarVida(jogador.VidaMax / 5); break;
        }
    }
}

// Modo Batalha
public class ModoBatalha
{
    private List<Criatura> Inimigos;
    private Criatura Jogador;

    public ModoBatalha(Criatura jogador)
    {
        Jogador = jogador;

        Inimigos = new List<Criatura>
        {
            new CapivaraSelvagem(),
            new CapivaraAssassina(),
            new CapivaraFeiticeira(),
            new DragaoBoss(0.8)
        };
    }

    public void Iniciar()
    {
        Console.WriteLine("=== MODO BATALHA ===\n");

        bool continuar = true;
        while (continuar && Jogador.Vida > 0)
        {
            Console.WriteLine("\nEscolha um inimigo para enfrentar:");
            for (int i = 0; i < Inimigos.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {Inimigos[i].Nome} (Vida: {Inimigos[i].Vida}/{Inimigos[i].VidaMax})");
            }
            Console.WriteLine("0 - Sair do modo batalha");

            int escolha;
            do { Console.Write("Digite a opção: "); } 
            while (!int.TryParse(Console.ReadLine(), out escolha) || escolha < 0 || escolha > Inimigos.Count);

            if (escolha == 0) break;

            var inimigo = Inimigos[escolha - 1];

            Console.WriteLine($"\nBatalha: {Jogador.Nome} vs {inimigo.Nome}");
            bool jogadorVenceu = Lutar(Jogador, inimigo);

            if (!jogadorVenceu)
            {
                Console.WriteLine("Você perdeu a batalha!");
                continuar = false;
            }
            else
            {
                Console.WriteLine($"Você venceu {inimigo.Nome}!");
                Jogador.RecuperarVida(Jogador.VidaMax / 4);
                if (Jogador is CapivaraMaga mago) mago.RecuperarMana(10);
            }
        }

        Console.WriteLine("Modo Batalha encerrado.");
    }

    private bool Lutar(Criatura jogador, Criatura inimigo)
    {
        while (jogador.Vida > 0 && inimigo.Vida > 0)
        {
            if (jogador.Jogador) TurnoJogador(jogador, inimigo);
            else jogador.Atacar(inimigo, "perto");

            if (inimigo.Vida > 0)
            {
                int acao = new Random().Next(1, 4);
                switch (acao)
                {
                    case 1: inimigo.Atacar(jogador, "perto"); break;
                    case 2: inimigo.Atacar(jogador, "longe"); break;
                    case 3: inimigo.AtaqueEspecial(jogador); break;
                }
            }
        }
        return jogador.Vida > 0;
    }


private void TurnoJogador(Criatura jogador, Criatura inimigo)
{
    Console.WriteLine($"\nSeu turno! Vida: {jogador.Vida}/{jogador.VidaMax} | Inimigo: {inimigo.Nome} ({inimigo.Vida}/{inimigo.VidaMax})");
    if (jogador is CapivaraMaga mago) Console.WriteLine($"Mana: {mago.Mana}/30");

    Console.WriteLine("1 - Ataque de Perto");
    Console.WriteLine("2 - Ataque de Longe");
    Console.WriteLine("3 - Ataque Especial");
    Console.WriteLine("4 - Defender (+5 defesa)");
    Console.WriteLine("5 - Recuperar Vida (+20%)");

    string escolha;
    do { Console.Write("Escolha sua ação: "); escolha = Console.ReadLine(); }
    while (escolha != "1" && escolha != "2" && escolha != "3" && escolha != "4" && escolha != "5");

    switch (escolha)
    {
        case "1": jogador.Atacar(inimigo, "perto"); break;
        case "2": jogador.Atacar(inimigo, "longe"); break;
        case "3": jogador.AtaqueEspecial(inimigo); break;
        case "4": jogador.Defender(); break;
        case "5": jogador.RecuperarVida(jogador.VidaMax / 5); break;
    }
}
}

// Programa Principal
class Program
{
    static void Main()
    {
        Console.WriteLine("=== TORNEIO DAS CAPIVARAS ===");
        Console.WriteLine("Escolha sua criatura:");
        List<Criatura> opcoes = new List<Criatura>
        {
            new CapivaraGuerreira("Capivara Guerreira", true),
            new CapivaraMaga("Capivara Maga", true),
            new CapivaraArqueira("Capivara Arqueira", true),
            new CapivaraNinja("Capivara Ninja", true)
        };

        for (int i = 0; i < opcoes.Count; i++)
            Console.WriteLine($"{i + 1} - {opcoes[i].Nome}");

        int escolha;
        do
        {
            Console.Write("Digite sua escolha: ");
        } while (!int.TryParse(Console.ReadLine(), out escolha) || escolha < 1 || escolha > opcoes.Count);

        Criatura jogador = opcoes[escolha - 1];

        Console.WriteLine("\nEscolha o modo de jogo:");
        Console.WriteLine("1 - Torneio");
        Console.WriteLine("2 - Batalha");

        string modo;
        do { Console.Write("Digite a opção: "); modo = Console.ReadLine(); }
        while (modo != "1" && modo != "2");

        if (modo == "1")
        {
            var torneio = new Torneio();
            torneio.ConfigurarDificuldade();

            // Adiciona inimigos intermediários
            torneio.AdicionarCriatura(new CapivaraSelvagem());
            torneio.AdicionarCriatura(new CapivaraAssassina());
            torneio.AdicionarCriatura(new CapivaraFeiticeira());

            // Adiciona boss
            torneio.AdicionarCriatura(new DragaoBoss());

            // Adiciona jogador
            torneio.AdicionarCriatura(jogador);

            torneio.Iniciar();
        }
        else
        {
            var batalha = new ModoBatalha(jogador);
            batalha.Iniciar();
        }

        Console.WriteLine("\nFim do jogo. Obrigado por jogar!");
    }
}
