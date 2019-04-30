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
        double[] numeros;
        FilaLista<char> seqInfixa;
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
            seqInfixa = new FilaLista<char>();
            seqPosfixa = new FilaLista<char>();
            char letra = 'A';
            numeros = new double[26];
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
                    numeros[letra - 'A'] = double.Parse(numero);
                    seqInfixa.Enfileirar(letra);
                    seqInfixa.Enfileirar(caracterAtual);
                    numero = "";
                    letra++;
                }
                else
                {
                    seqInfixa.Enfileirar(caracterAtual);
                }

                if (i + 1 == txtVisor.Text.Length && (numero != ""))
                {
                    numeros[letra - 'A'] = double.Parse(numero);
                    seqInfixa.Enfileirar(letra);
                    numero = "";
                    letra++;
                }
            }
            ConverterParaPosfixa();
            string s = "";
            while (!seqPosfixa.EstaVazia())
                s += seqPosfixa.Retirar();
            lbSequencias.Text = s;
        }

        private void ConverterParaPosfixa()
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
                        bool parar = false;
                        while (!parar && !pilha.EstaVazia() && HaPrecedencia(pilha.OTopo(), simboloLido))
                        {
                            operadorComMaiorPrecedencia = pilha.Desempilhar();
                            if (operadorComMaiorPrecedencia != '(')
                                seqPosfixa.Enfileirar(operadorComMaiorPrecedencia);
                            else
                                parar = true;
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
                }
            }
            while (!seqInfixa.EstaVazia());

            while (!pilha.EstaVazia())
            {
                operadorComMaiorPrecedencia = pilha.Desempilhar();
                if (operadorComMaiorPrecedencia != '(')
                    seqPosfixa.Enfileirar(operadorComMaiorPrecedencia);
            }
        }

        private bool HaPrecedencia(char topo, char simboloLido)
        {
            switch (topo)
            {
                case '(':
                    return true;

                case '^':
                    return true;

                case '*':
                    if(simboloLido == '^' || simboloLido == '*' || simboloLido == '/')
                        return true;
                    return false;

                case '/':
                    if (simboloLido == '^' || simboloLido == '*' || simboloLido == '/')
                        return true;
                    return false;

                case '+':
                    if (simboloLido == '(' || simboloLido == ')')
                        return true;
                    return false;

                case '-':
                    if (simboloLido == '(' || simboloLido == ')')
                        return true;
                    return false;

                case ')':
                    if (simboloLido == '(')
                        return true;
                    return false;
            }
            return false;
        }
    }
}
