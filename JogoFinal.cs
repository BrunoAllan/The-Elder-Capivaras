using System;
using System.Collections.Generic;
using System.Linq;

namespace TheElderCapivaras
{
    // ===================== CRIATURAS =====================
    public abstract class Criatura
    {
        public string Nome { get; set; }
        public int Vida { get; set; }
        public int VidaMax { get; set; }
        public int Ataque { get; set; }
        public int Defesa { get; set; }
        public bool Jogador { get; set; }
        public int UsosEspeciaisRestantes { get; set; } = 2; // Novo campo

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

    // ===================== CLASSES JOGADOR =====================
    public class CapivaraGuerreira : Criatura
    {
        public CapivaraGuerreira(string nome, bool jogador = false) : base(nome, 100, 25, 10, jogador) { }
        public override void AtaqueEspecial(Criatura alvo)
        {
            if (UsosEspeciaisRestantes <= 0)
            {
                Console.WriteLine($"{Nome} não pode mais usar o ataque especial!");
                Atacar(alvo, "perto");
                return;
            }

            UsosEspeciaisRestantes--;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{Nome} usa Golpe Poderoso!");
            Console.ResetColor();
            int dano = (int)(Ataque * 1.5) - alvo.Defesa;
            if (dano < 0) dano = 0;
            alvo.Vida -= dano;
            if (alvo.Vida < 0) alvo.Vida = 0;
            MostrarAtaque(Nome, alvo.Nome, dano, true);
            MostrarBarraColorida(alvo);
        }

    }

    public class CapivaraMaga : Criatura
    {
        public int Mana { get; set; } = 35;
        public int ManaMax { get; set; } = 35; // novo campo opcional

        public CapivaraMaga(string nome, bool jogador = false) : base(nome, 80, 30, 5, jogador)
        {
            ManaMax = 35;
            Mana = 35;
        }

        
        public override void AtaqueEspecial(Criatura alvo)
        {
            if (UsosEspeciaisRestantes <= 0)
            {
                Console.WriteLine($"{Nome} não pode mais usar o ataque especial!");
                Atacar(alvo, "perto");
                return;
            }

            if (Mana >= 15)
            {
                Mana -= 15;
                UsosEspeciaisRestantes--;
                int dano = Ataque + 10;
                alvo.Vida -= dano;
                if (alvo.Vida < 0) alvo.Vida = 0;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"{Nome} lançou Magia Suprema em {alvo.Nome}, causando {dano} de dano! (Mana: {Mana}/{ManaMax})");
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
            if (Mana > ManaMax) Mana = ManaMax;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{Nome} recuperou {qtd} de mana! Mana atual: {Mana}/{ManaMax}");
            Console.ResetColor();
        }

    }

    public class CapivaraArqueira : Criatura
    {
        private static Random rand = new Random();
        public CapivaraArqueira(string nome, bool jogador = false) : base(nome, 90, 23, 8, jogador) { }

        public override void AtaqueEspecial(Criatura alvo)
        {
            if (UsosEspeciaisRestantes <= 0)
            {
                Console.WriteLine($"{Nome} não pode mais usar o ataque especial!");
                Atacar(alvo, "longe");
                return;
            }

            UsosEspeciaisRestantes--;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{Nome} usa Tiro Múltiplo!");
            Console.ResetColor();
            for (int i = 0; i < 3; i++)
            {
                if (rand.NextDouble() < 0.5)
                {
                    Console.WriteLine($"{Nome} errou o tiro {i + 1}!");
                    continue;
                }
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
            if (UsosEspeciaisRestantes <= 0)
            {
                Console.WriteLine($"{Nome} não pode mais usar o ataque especial!");
                Atacar(alvo, "perto");
                return;
            }

            UsosEspeciaisRestantes--;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{Nome} usa Ataque Furtivo!");
            Console.ResetColor();
            int dano = Ataque * 2 - alvo.Defesa;
            if (dano < 0) dano = 0;
            alvo.Vida -= dano;
            if (alvo.Vida < 0) alvo.Vida = 0;
            MostrarAtaque(Nome, alvo.Nome, dano, true);
            MostrarBarraColorida(alvo);
        }

    }

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

    // ===================== INIMIGOS =====================
    public class CapivaraSelvagem : Criatura { public CapivaraSelvagem() : base("Capivara Selvagem", 60, 12, 5) { } }
    public class CapivaraAssassina : Criatura { public CapivaraAssassina() : base("Capivara Assassina", 50, 20, 3) { } }
    public class CapivaraFeiticeira : Criatura { public CapivaraFeiticeira() : base("Capivara Feiticeira", 55, 15, 4) { } }

    // ===================== TORNEIO =====================
    public class Torneio
    {
        private List<Criatura> Criaturas = new List<Criatura>();
        private static Random rng = new Random();
        private double multiplicadorInimigos = 1.0;

        public void ConfigurarDificuldade()
        {
            Console.Clear();
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
            Console.Clear();
            Console.WriteLine("=== TORNEIO DAS CAPIVARAS ===\n");
            int rodada = 1;

            while (Criaturas.Count > 1)
            {
                Console.Clear();
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

                foreach (var c in vencedores)
                {
                    c.RecuperarVida(c.VidaMax / 5);
                    if (c is CapivaraMaga mago) mago.RecuperarMana(10);
                }

                Criaturas = vencedores;
                rodada++;
            }

            var campeao = Criaturas[0];
            if (campeao.Jogador) Console.ForegroundColor = ConsoleColor.Green;
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
            Console.Clear();
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

            Console.WriteLine("\nPressione Enter para próximo turno...");
            Console.ReadLine();
        }
    }

    // ===================== MODO BATALHA =====================
    public class ModoBatalha
    {
        private Criatura Jogador;
        private List<Criatura> Inimigos;
        private static Random rng = new Random();

        public ModoBatalha(Criatura jogador)
        {
            Jogador = jogador;

            // Criando inimigos
            Inimigos = new List<Criatura>
            {
                new CapivaraSelvagem(),
                new CapivaraAssassina(),
                new CapivaraFeiticeira(),
                new DragaoBoss() // boss opcional
            };
        }

        public void Iniciar()
        {
            while (Inimigos.Count > 0)
            {
                Console.Clear();
                Console.WriteLine("=== MODO BATALHA ===");
                Console.WriteLine("Escolha um inimigo para enfrentar:");

                for (int i = 0; i < Inimigos.Count; i++)
                    Console.WriteLine($"{i + 1} - {Inimigos[i].Nome} (Vida: {Inimigos[i].Vida}/{Inimigos[i].VidaMax})");

                int escolha;
                do
                {
                    Console.Write("Digite sua escolha: ");
                } while (!int.TryParse(Console.ReadLine(), out escolha) || escolha < 1 || escolha > Inimigos.Count);

                Criatura inimigo = Inimigos[escolha - 1];
                Console.Clear();
                Console.WriteLine($"Você enfrentará: {inimigo.Nome}!\n");

                bool jogadorVenceu = Lutar(Jogador, inimigo);

                if (!jogadorVenceu)
                {
                    Console.WriteLine("Você foi derrotado! Fim do jogo!");
                    return;
                }
                else
                {
                    Console.WriteLine($"Você derrotou {inimigo.Nome}!");
                    Jogador.RecuperarVida(Jogador.VidaMax / 5);
                    if (Jogador is CapivaraMaga mago) mago.RecuperarMana(10);
                    Inimigos.Remove(inimigo);
                    Console.WriteLine("Pressione Enter para continuar...");
                    Console.ReadLine();
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("PARABÉNS! Todos os inimigos foram derrotados!");
            Console.ResetColor();
        }

        private bool Lutar(Criatura jogador, Criatura inimigo)
        {
            while (jogador.Vida > 0 && inimigo.Vida > 0)
            {
                TurnoJogador(jogador, inimigo);

                if (inimigo.Vida > 0)
                {
                    inimigo.Atacar(jogador, "perto");
                    Console.WriteLine("\nPressione Enter para próximo turno...");
                    Console.ReadLine();
                }
            }
            return jogador.Vida > 0;
        }

        private void TurnoJogador(Criatura jogador, Criatura inimigo)
        {
            Console.Clear();
            Console.WriteLine($"Seu turno! Vida: {jogador.Vida}/{jogador.VidaMax} | Inimigo: {inimigo.Nome} ({inimigo.Vida}/{inimigo.VidaMax})");
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

            Console.WriteLine("\nPressione Enter para próximo turno...");
            Console.ReadLine();
        }
    }

    public class ModoDoisJogadores
{
    private Criatura Jogador1;
    private Criatura Jogador2;

    public ModoDoisJogadores()
    {
        Jogador1 = EscolherJogador(1);
        Jogador2 = EscolherJogador(2);
    }

    private Criatura EscolherJogador(int numero)
    {
        Console.Clear();
        Console.WriteLine($"=== Jogador {numero}, escolha sua criatura ===");

        List<Criatura> opcoes = new List<Criatura>
        {
            new CapivaraGuerreira($"Capivara Guerreira {numero}", true),
            new CapivaraMaga($"Capivara Maga {numero}", true),
            new CapivaraArqueira($"Capivara Arqueira {numero}", true),
            new CapivaraNinja($"Capivara Ninja {numero}", true)
        };

        for (int i = 0; i < opcoes.Count; i++)
            Console.WriteLine($"{i + 1} - {opcoes[i].Nome}");

        int escolha;
        do
        {
            Console.Write("Digite sua escolha: ");
        } while (!int.TryParse(Console.ReadLine(), out escolha) || escolha < 1 || escolha > opcoes.Count);

        return opcoes[escolha - 1];
    }

    public void Iniciar()
    {
        bool turnoJogador1 = true;

        while (Jogador1.Vida > 0 && Jogador2.Vida > 0)
        {
            Console.Clear();
            var atacante = turnoJogador1 ? Jogador1 : Jogador2;
            var defensor = turnoJogador1 ? Jogador2 : Jogador1;

            Console.WriteLine($"Turno de {atacante.Nome}");
            Console.WriteLine($"{Jogador1.Nome}: {Jogador1.Vida}/{Jogador1.VidaMax}");
            Console.WriteLine($"{Jogador2.Nome}: {Jogador2.Vida}/{Jogador2.VidaMax}");

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
                case "1": atacante.Atacar(defensor, "perto"); break;
                case "2": atacante.Atacar(defensor, "longe"); break;
                case "3": atacante.AtaqueEspecial(defensor); break;
                case "4": atacante.Defender(); break;
                case "5": atacante.RecuperarVida(atacante.VidaMax / 5); break;
            }

            Console.WriteLine("\nPressione Enter para próximo turno...");
            Console.ReadLine();
            turnoJogador1 = !turnoJogador1;
        }

        Console.Clear();
        if (Jogador1.Vida > 0)
            Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Jogador 1 ({Jogador1.Nome}) venceu!");
        Console.ResetColor();
        if (Jogador2.Vida > 0)
            Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Jogador 2 ({Jogador2.Nome}) venceu!");
        Console.ResetColor();
    }
}


    // ===================== PROGRAMA PRINCIPAL =====================
    class Program
    {
        static void Main()
        {
            Console.Clear();
            MostrarHistoria(); // << Adiciona essa chamada
            Console.Clear();
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

            Console.Clear();
            Console.WriteLine("Escolha o modo de jogo:");
            Console.WriteLine("1 - Torneio");
            Console.WriteLine("2 - Batalha");
            Console.WriteLine("3 - Dois Jogadores");

            string modo;
            do { Console.Write("Digite a opção: "); modo = Console.ReadLine(); }
           while (modo != "1" && modo != "2" && modo != "3");

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
           else if (modo == "2")
            {
                var batalha = new ModoBatalha(jogador);
                batalha.Iniciar();
            }
            else if (modo == "3")
            {
                var doisJogadores = new ModoDoisJogadores();
                doisJogadores.Iniciar();
            }

            Console.WriteLine("\nFim do jogo. Obrigado por jogar!");
        }

        static void MostrarHistoria()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("🐾 Bem-vindo ao mundo mágico de Capivária 🐾\n");
            Console.ResetColor();
            Console.WriteLine("Há muito tempo, as capivaras viviam em paz, deitadas nas margens dos rios encantados,");
            Console.WriteLine("ruminando folhas mágicas e praticando feitiçaria básica de chá e camomila.");
            Console.WriteLine("\nMas um dia, os céus ruíram com rugidos flamejantes... 🐉🔥");
            Console.WriteLine("Os Dragões de Eldrath, criaturas escamosas e nada veganas, retornaram após séculos.");
            Console.WriteLine("Seu líder, Drakharn — o Incinerador do Equilíbrio — declarou:");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  “Vocês usaram magia para fazer... panquecas? Isso é um ultraje arcano!”");
            Console.ResetColor();
            Console.WriteLine("\nAgora, os Cristais do Equilíbrio estão ameaçados, e cabe a uma capivara lendária proteger o mundo.");
            Console.WriteLine("Segundo a profecia do Capim-Luminoso, somente o Escolhido dos Quatro Caminhos poderá salvar Capivária.");
            Console.WriteLine("\nE spoiler... esse Escolhido pode ser você. (Ou talvez uma capivara aleatória com um nome engraçado.)\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Prepare-se para batalhas épicas, poderes ancestrais, e trocadilhos de gosto duvidoso.");
            Console.WriteLine("Capivária depende de você... e do seu bom senso em turnos de combate.");
            Console.ResetColor();

            Console.WriteLine("\nPressione Enter para escolher seu destino capivarístico...");
            Console.ReadLine();
        }

        
    }
}


