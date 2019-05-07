using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculadora
{
    public partial class frmCalculadora : Form
    {
        double[] valores; //vetor de valores que serão representados nas expressões por letras de 'A' a 'Z'
        FilaLista<char> seqPosfixa; //fila que armazena sequência posfixa que será usada para calcular a expressão
        bool erro = false;

        public frmCalculadora()
        {
            InitializeComponent();
        }

        private void btnLimpar_Click(object sender, EventArgs e) //botão C 
        {
            txtVisor.Text = "";
            txtResultado.Text = "";
            lblInfixa.Text = "";
            lblPosfixa.Text = "";
        }

        private void btn9_Click(object sender, EventArgs e) //evento que está sendo usado por todos os botões, exceto '=' e 'C'
        {
            Button btn = (Button)sender;  //pega o botão que acionou o evento
            txtVisor.AppendText(btn.Text); //acrescenta o texto do botão ao txtVisor
        }

        private void btnIgual_Click(object sender, EventArgs e) //quando a pessoa aperta '=', o evento é disparado para se calcular a expressão
        {
            if (txtVisor.Text != "")
            {
                erro = false;

                string infixa = ""; //string da sequência infixa que será mostrada no lblInfixa
                FilaLista<char> seqInfixa = CriarInfixa(ref infixa); //fila que armazena sequência infixa (com as letras nos lugares dos números), que será convertida para posfixa

                seqPosfixa = new FilaLista<char>(); // instanciação da fila de sequência posfixa

                string posfixa = "";
                if (!erro)
                {
                    ConverterParaPosfixa(seqInfixa, ref posfixa); //chama método que converte a infixa para posfixa

                    if (!erro)
                    {
                        lblPosfixa.Text = posfixa; //exibe sequência posfixa formada no lblPosfixa
                        Calcular(); //chama método que calcula a posfixa
                    }
                    else
                        txtResultado.Text = "Verifique sua expressão";
                }
            }
        }

        private FilaLista<char> CriarInfixa(ref string infixa)
        {
            FilaLista<char> seqInfixa = new FilaLista<char>();

            char letra = 'A'; //char que irá de A a Z para "substituir" os números
            valores = new double[26]; //instanciação do vetor de valores
            string numero = ""; //string que armazenará um valor lido, caracter por caracter

            for (int i = 0; i < txtVisor.Text.Length && !erro; i++) //percorre string do txtVisor, indexando-o como um vetor, até acabar as posições
            {
                char caracterAtual = (txtVisor.Text)[i]; //pega o caracter atual pelo index do for

                int num; //variável apenas com fim de ser usada no tryParse
                if (int.TryParse(caracterAtual.ToString(), out num) || caracterAtual.Equals('.') || caracterAtual.Equals(','))//se o valor for um número inteiro(como se está analisando apenas um caracter isolado, não se precisa usar double.TryParse)
                {                                                                                  // ou se for um '.', ou seja, uma vírgula do double, ele faz parte de um número
                    if (caracterAtual.Equals('.'))
                        caracterAtual = ',';
                    numero += caracterAtual; //concatena caracter do número em formação
                }
                else if(!EhOperador(caracterAtual)) //caso não seja número nem operador, o usuário digitou algo inválido
                {
                    erro = true;
                    txtResultado.Text = "Digite operadores válidos";
                }
                else if (numero != "") //se caracter lido não fizer mais parte de um número e a variável numero não for vazia
                {
                    valores[letra - 'A'] = double.Parse(numero); //armazena-se valor do número formado, convertendo-o para double
                    infixa += letra + "" + caracterAtual; //concatena-se o número e o caracter lido na string infixa
                    seqInfixa.Enfileirar(letra); //enfileira letra referente a posição do número no vetor de valores
                    seqInfixa.Enfileirar(caracterAtual); //enfileira caracter de operação
                    numero = ""; //esvazia string de número para que comece a formação de um novo caso haja
                    letra++; //vai para a próxima letra
                }
                else //se o caracter lido for referente a uma operação mas não houver um número na variável numero (está vazia)
                {
                    seqInfixa.Enfileirar(caracterAtual); //enfileira-se caracter atual
                    infixa += caracterAtual; //concatena-se caracter atual em infixa
                }

                if (i+1 == txtVisor.Text.Length && (numero != "") && !erro) // se o próximo index do vetor for o fim da operação mas o número não estiver vazio
                {
                    valores[letra - 'A'] = double.Parse(numero); //armazena valor no vetor de valores
                    seqInfixa.Enfileirar(letra); //enfileira letra correspondente a valor
                    infixa += letra + ""; //concatena letra à sequencia infixa
                    numero = ""; //esvazia número
                    letra++; //vai à próxima letra
                }
            }

            lblInfixa.Text = infixa; //exibe sequência infixa formada no lblInfixa

            return seqInfixa;
        }

        private bool EhOperador(char op)
        {
            switch (op)
            {
                case '*':
                    return true;
                case '/':
                    return true;
                case '+':
                    return true;
                case '-':
                    return true;
                case '^':
                    return true;
                case ')':
                    return true;
                case '(':
                    return true;
            }
            return false; //se não for nada acima, não é operando 
        }

        private void ConverterParaPosfixa(FilaLista<char> seqInfixa, ref string posfixa)
        {
            char operadorComMaiorPrecedencia;
            PilhaLista<char> pilha = new PilhaLista<char>(); //pilha que será usada para empilhar operadores
            do
            {
                char simboloLido = seqInfixa.Retirar(); //simbolo lido é retirado da fila de sequencia infixa
                if (!(simboloLido <= 'Z' && simboloLido >= 'A')) //se o simbolo lido não estiver entre A e Z, ou seja, for um operador
                {
                    if (pilha.EstaVazia()) //se a pilha estiver vazia, empilha-se simbolo lido
                        pilha.Empilhar(simboloLido);
                    else //se não estiver vazia
                    {
                        while (!pilha.EstaVazia() && DeveDesempilhar(pilha.OTopo(), simboloLido)) //enquanto a pilha não estiver vazia e deve desempilhar, ou seja, o topo da pilha tem precedencia sobre o simbolo lido
                        {
                            operadorComMaiorPrecedencia = pilha.Desempilhar(); //armazena-se operador com maior precedencia, tirando-o do topo
                            if (operadorComMaiorPrecedencia != '(') //caso não seja '(', pois não se coloca parêntesis em sequências posfixas
                            {
                                seqPosfixa.Enfileirar(operadorComMaiorPrecedencia); //enfileira-se na seqPosfixa
                                posfixa += operadorComMaiorPrecedencia; //concatena-se na string de posfixa
                            }
                        }
                        if (simboloLido != ')') // empilha-se o simbolo lido, a menos que ele seja um ')'
                            pilha.Empilhar(simboloLido);
                        else
                        {
                            if (!pilha.EstaVazia() && pilha.OTopo() == '(') //se o simbolo lido for ')' e já foi feito o método acima sobre as precedencias, restará um parentesis aberto no topo da pilha, a menos que a sequência não esteja balanceada
                                operadorComMaiorPrecedencia = pilha.Desempilhar();
                            else
                                erro = true;
                        }
                    }
                }
                else // se for letra
                {
                    seqPosfixa.Enfileirar(simboloLido); //enfileira-se letra na sequência 
                    posfixa += simboloLido; //concatena-se a letra na string posfixa
                }
            }
            while (!seqInfixa.EstaVazia() && !erro); //enquando não estiver vazia e não houver nenhum erro, continua-se a expressão

            while (!pilha.EstaVazia() && !erro) // se a fila de sequência ja estiver acabado mas houver operandos na pilha e não tiver dado erro
            {
                operadorComMaiorPrecedencia = pilha.Desempilhar(); //desempilha-se elemento da pilha
                if (operadorComMaiorPrecedencia != '(') //se não for '('
                {
                    seqPosfixa.Enfileirar(operadorComMaiorPrecedencia); //enfileira elemento
                    posfixa += operadorComMaiorPrecedencia; //concatena à string posfixa
                }
            }
        }

        private bool DeveDesempilhar(char topo, char simboloLido) //verifica precedencias através de um switch
        {
            switch (topo)
            {
                case '(':
                    return false;

                case '^':
                    if(simboloLido == '^' || simboloLido == '(')
                        return false;
                    return true;

                case '*':
                    if(simboloLido == '^' || simboloLido == '(')
                        return false;
                    return true;

                case '/':
                    if (simboloLido == '^' || simboloLido == '(')
                        return false;
                    return true;

                case '+':
                    if (simboloLido == '+' || simboloLido == ')' || simboloLido == '-')
                        return true;
                    return false;

                case '-':
                    if (simboloLido == '+' || simboloLido == ')' || simboloLido == '-')
                        return true;
                    return false;

                case ')':
                    return false;
            }

            return false; //nunca chegará aqui, mas é exigido que sempre se retorne um valor
        }

        private void Calcular()
        {
            PilhaLista<double> pilhaValores = new PilhaLista<double>(); //pilha que armazena valores da expressão

            do
            {
                char simbolo = seqPosfixa.Retirar(); //retira-se um simbolo da sequência posfixa
                if (simbolo >= 'A' && simbolo <= 'Z') //se o símbolo for uma letra
                {
                    double valor = valores[simbolo - 'A']; //recupera valor referente à letra indexando o vetor valores
                    pilhaValores.Empilhar(valor); //empilha valor recebido
                }
                else // se for operador
                {
                    double valor = 0;
                    double operando2 = pilhaValores.Desempilhar(); //o segundo operando é referente ao topo da pilha, e é desempilhado
                    double operando1 = pilhaValores.Desempilhar(); //desempilha-se primeiro operando

                    switch(simbolo) //switch para realizar diferentes operações dependendo do operador
                    {
                        case '*':
                            valor = operando1 * operando2;
                            break;
                        case '/':
                            if (operando2 == 0) // na divisão, não se pode ter divisor 0
                            {
                                txtResultado.Text = "Erro: Divisão por 0";
                                erro = true;
                            }
                            else
                                valor = operando1 / operando2;
                            break;
                        case '^':
                            valor = Math.Pow(operando1, operando2);
                            break;
                        case '+':
                            valor = operando1 + operando2;
                            break;
                        case '-':
                            valor = operando1 - operando2;
                            break;
                    }

                    pilhaValores.Empilhar(valor); //empilha-se valor da sub-expressão realizada
                }
            }
            while (!seqPosfixa.EstaVazia() && !erro); // continua caso não tenha acabado a sequencia e não tenha havido erro

            if (!erro) //se não tiver dado erro
            {
                double resultado = pilhaValores.Desempilhar(); //como os resultados das sub-expressões vão sendo armazenados na pilha, ao final restará o resultado
                txtResultado.Text = resultado.ToString(); //exibe resultado
            }
        }

        private void txtVisor_TextChanged(object sender, EventArgs e)
        {
            if(txtVisor.Text != "" && txtVisor.Text[txtVisor.Text.Length-1] == '=')
            {
                txtVisor.Text = txtVisor.Text.Substring(0, txtVisor.Text.Length-1);
                btnIgual_Click(new Button(), new EventArgs());
            }
        }
    }
}
