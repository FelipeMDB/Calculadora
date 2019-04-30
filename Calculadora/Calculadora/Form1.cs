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
        string seqInfixa;
        string seqPosfixa;
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
            seqInfixa = "";
            seqPosfixa = "";
            char letra = 'A';
            numeros = new double[26];
            string numero = "";
            int num;
            for (int i = 0; i < txtVisor.Text.Length; i++)
            {
                string caracterAtual = (txtVisor.Text)[i].ToString();

                if (int.TryParse(caracterAtual, out num) || caracterAtual.Equals("."))
                {
                    numero += caracterAtual;
                }
                else if (numero != "")
                {
                    numeros[letra - 'A'] = double.Parse(numero);
                    seqInfixa += letra;
                    seqInfixa += caracterAtual;
                    numero = "";
                    letra++;
                }
                else
                {
                    seqInfixa += caracterAtual;
                }

                if (i + 1 == txtVisor.Text.Length && (numero != ""))
                {
                    numeros[letra - 'A'] = double.Parse(numero);
                    seqInfixa += letra;
                    numero = "";
                    letra++;
                }
            }
            MessageBox.Show(seqInfixa);
        }

        private void ConverterParaPosfixa()
        {
            PilhaHerdaLista<string> pilha = new PilhaHerdaLista<string>()

        }

        private bool HaPrecedencia(string topo, string simboloLido)
        {
            switch(simboloLido)
            {
                case "^":
                    return true;
                case "(":
                    return true;
                case ")":
                    return false;
                case "/":
                    if (topo == "+" || topo == "-" || topo == "(")
                        return true;
                    return false;
                case "*":
                    if (topo == "+" || topo == "-" || topo == "(")
                        return true;
                    return false;
                case "-":
                    if (topo == "(")
                        return true;
                    return false;
                case "+":
                    if (topo == "(")
                        return true;
                    return false;
            }
            return false;
        }
    }
}
