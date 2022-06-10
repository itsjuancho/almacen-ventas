using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Almacen
{
    public partial class frmAcceso : Form
    {
        public frmAcceso()
        {
            InitializeComponent();
        }
        // exit login button
        private void button2_Click(object sender, EventArgs e)
        {

        }
        // Login button
        private void button1_Click(object sender, EventArgs e)
        {
            MySqlConnection conexion = new MySqlConnection("server=localhost;database=ventasalmacen;uid=root;password=");
            MySqlCommand comando = new MySqlCommand();
            MySqlDataReader registro;

            try {
                comando.Connection = conexion;
                conexion.Open();
                comando.CommandText = "SELECT * FROM tblusuario WHERE usuario = '" + txtUser.Text + "' AND clave = '" + txtPw.Text + "'";
                registro = comando.ExecuteReader();
                if (registro.Read()) {
                    MessageBox.Show("Bienvenido a la app","Application Access",MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmMenuPrincipal MenuPrincipal = new frmMenuPrincipal();
                    MenuPrincipal.Text += " (Sesión: " + txtUser.Text + ")";
                    this.Hide();
                    MenuPrincipal.Show();
                } else {
                    MessageBox.Show("Datos incorrectos, por favor verificar","Wrong Application Access", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } catch (MySqlException) {
                MessageBox.Show("[#404] Error técnico. Contacte a soporte por favor.", "Wrong Connection Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            conexion.Close();
        }
    }
}
