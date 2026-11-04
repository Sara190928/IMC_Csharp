using System.Globalization;

namespace IMC_CSharp
{
    public partial class Form1 : Form
    {
        // Textos de "placeholder" (dica) dos campos de entrada.
        private const string PlaceholderPeso = "Digite o seu peso...";
        private const string PlaceholderAltura = "Digite a sua altura (ex: 1.75)...";

        public Form1()
        {
            InitializeComponent();
            ConfigurarPlaceholder(txtPeso, PlaceholderPeso);
            ConfigurarPlaceholder(txtAltura, PlaceholderAltura);
        }

        /// <summary>
        /// Simula um placeholder nos TextBox (texto cinza que desaparece ao focar).
        /// </summary>
        private static void ConfigurarPlaceholder(TextBox txt, string texto)
        {
            void Entrar(object? s, EventArgs e)
            {
                if (txt.Text == texto)
                {
                    txt.Text = string.Empty;
                    txt.ForeColor = Color.Black;
                }
            }

            void Sair(object? s, EventArgs e)
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = texto;
                    txt.ForeColor = Color.Gray;
                }
            }

            txt.Text = texto;
            txt.ForeColor = Color.Gray;
            txt.GotFocus += Entrar;
            txt.LostFocus += Sair;
            // Substitui o placeholder caso o utilizador já tenha escrito antes do foco.
            txt.Enter += Entrar;
            txt.Leave += Sair;
        }

        /// <summary>
        /// Evento de clique do botão btnCalcular: valida os dados e apresenta o IMC.
        /// </summary>
        private void btnCalcular_Click(object? sender, EventArgs e)
        {
            // Conversão segura (TryParse) dos valores introduzidos.
            // Aceita vírgula ou ponto como separador decimal.
            bool pesoValido = TentarConverter(txtPeso, PlaceholderPeso, out double peso);
            bool alturaValida = TentarConverter(txtAltura, PlaceholderAltura, out double altura);

            if (!pesoValido || !alturaValida)
            {
                MostrarErro("Valores inválidos!\r\nIntroduza apenas números (ex: 70.5 e 1.75).");
                return;
            }

            if (peso <= 0 || altura <= 0)
            {
                MostrarErro("O peso e a altura devem ser\r\nmaiores que zero.");
                return;
            }

            // Fórmula padrão do IMC: peso / (altura ^ 2)
            double imc = peso / (altura * altura);

            lblResultado.Text = string.Format(
                CultureInfo.InvariantCulture,
                "IMC: {0:F2}  |  {1}",
                imc,
                ClassificarImc(imc));

            lblResultado.ForeColor = CorDoResultado(imc);
            lblDescricao.Text = "Cálculo efetuado com sucesso!";
            lblDescricao.ForeColor = Color.ForestGreen;
        }

        /// <summary>
        /// Conversão segura: ignora o placeholder e aceita ponto ou vírgula decimal.
        /// </summary>
        private static bool TentarConverter(TextBox txt, string placeholder, out double valor)
        {
            valor = 0;
            string texto = txt.Text.Trim();

            // Campo vazio ou ainda com o texto de dica: não é um número válido.
            if (texto.Length == 0 || texto == placeholder)
                return false;

            // Normaliza a vírgula decimal para ponto (formato invariável).
            texto = texto.Replace(',', '.');

            return double.TryParse(
                texto,
                NumberStyles.Float,   // não permite separadores de milhares -> só números "puros"
                CultureInfo.InvariantCulture,
                out valor);
        }

        private void MostrarErro(string mensagem)
        {
            lblResultado.Text = "Erro";
            lblResultado.ForeColor = Color.Red;
            lblDescricao.Text = mensagem;
            lblDescricao.ForeColor = Color.Red;
        }

        /// <summary>
        /// Devolve a classificação do IMC de acordo com a OMS.
        /// </summary>
        private static string ClassificarImc(double imc)
        {
            if (imc < 18.5) return "Abaixo do peso";
            if (imc < 25.0) return "Peso normal";
            if (imc < 30.0) return "Sobrepeso";
            if (imc < 35.0) return "Obesidade grau I";
            if (imc < 40.0) return "Obesidade grau II";
            return "Obesidade grau III";
        }

        /// <summary>
        /// Cor do texto consoante a classificação.
        /// </summary>
        private static Color CorDoResultado(double imc)
        {
            if (imc < 18.5) return Color.DarkOrange;
            if (imc < 25.0) return Color.ForestGreen;
            if (imc < 30.0) return Color.DarkOrange;
            return Color.Firebrick;
        }
    }
}
