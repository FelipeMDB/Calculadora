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
        double[] valores;
        FilaLista<char> seqPosfixa;

        public frmCalculadora()
        {
            InitializeComponent();
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            txtVisor.Text = "";
            txtResultado.Text = "";
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            txtVisor.AppendText(btn.Text);
        }

        private void btnIgual_Click(object sender, EventArgs e)
        {
            FilaLista<char> seqInfixa = new FilaLista<char>();
            string infixa = "";

            seqPosfixa = new FilaLista<char>();
            char letra = 'A';
            valores = new double[26];
            string numero = "";
            int num;
            for (int i = 0; i < txtVisor.Text.Length; i++)
            {
                char caracterAtual = (txtVisor.Text)[i];

                if (int.TryParse(caracterAtual.ToString(), out num) || caracterAtual.Equals('.'))
                {
                    numero += caracterAtual;
                }
                else if (numero != "")
                {
                    valores[letra - 'A'] = double.Parse(numero);
                    infixa += letra + "" + caracterAtual;
                    seqInfixa.Enfileirar(letra);
                    seqInfixa.Enfileirar(caracterAtual);
                    numero = "";
                    letra++;
                }
                else
                {
                    seqInfixa.Enfileirar(caracterAtual);
                    infixa += caracterAtual;
                }

                if (i + 1 == txtVisor.Text.Length && (numero != ""))
                {
                    valores[letra - 'A'] = double.Parse(numero);
                    seqInfixa.Enfileirar(letra);
                    infixa += letra + "";
                    numero = "";
                    letra++;
                }
            }
            lblInfixa.Text = infixa;
            string posfixa = "";
            ConverterParaPosfixa(seqInfixa, ref posfixa);
            lblPosfixa.Text = posfixa;
            Calcular();
        }

        private void ConverterParaPosfixa(FilaLista<char> seqInfixa, ref string posfixa)
        {
            char operadorComMaiorPrecedencia;
            PilhaLista<char> pilha = new PilhaLista<char>();
            do
            {
                char simboloLido = seqInfixa.Retirar();
                if (!(simboloLido <= 'Z' && simboloLido >= 'A'))
                {
                    if (pilha.EstaVazia())
                        pilha.Empilhar(simboloLido);
                    else
                    {
                        while (!pilha.EstaVazia() && DeveDesempilhar(pilha.OTopo(), simboloLido))
                        {
                            operadorComMaiorPrecedencia = pilha.Desempilhar();
                            if (operadorComMaiorPrecedencia != '(')
                            {
                                seqPosfixa.Enfileirar(operadorComMaiorPrecedencia);
                                posfixa += operadorComMaiorPrecedencia;
                            }
                        }
                        if (simboloLido != ')')
                            pilha.Empilhar(simboloLido);
                        else
                            operadorComMaiorPrecedencia = pilha.Desempilhar();
                    }
                }
                else
                {
                    seqPosfixa.Enfileirar(simboloLido);
                    posfixa += simboloLido;
                }
            }
            while (!seqInfixa.EstaVazia());

            while (!pilha.EstaVazia())
            {
                operadorComMaiorPrecedencia = pilha.Desempilhar();
                if (operadorComMaiorPrecedencia != '(')
                {
                    seqPosfixa.Enfileirar(operadorComMaiorPrecedencia);
                    posfixa += operadorComMaiorPrecedencia;
                }
            }
        }

        private bool DeveDesempilhar(char topo, char simboloLido)
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
            return false;
        }

        private void Calcular()
        {
            PilhaLista<double> pilhaValores = new PilhaLista<double>();
            bool erro = false;
            do
            {
                char simbolo = seqPosfixa.Retirar();
                if (simbolo >= 'A' && simbolo <= 'Z')
                {
                    double valor = valores[simbolo - 'A'];
                    pilhaValores.Empilhar(valor);
                }
                else
                {
                    double valor = 0;
                    double operando2 = pilhaValores.Desempilhar();
                    double operando1 = pilhaValores.Desempilhar();
                    switch(simbolo)
                    {
                        case '*':
                            valor = operando1 * operando2;
                            break;
                        case '/':
                            if (operando2 == 0)
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
                    pilhaValores.Empilhar(valor);
                }
            }
            while (!seqPosfixa.EstaVazia() && !erro);

            if (!erro)
            {
                double resultado = pilhaValores.Desempilhar();
                txtResultado.Text = resultado.ToString();
            }
        }
    }
}
