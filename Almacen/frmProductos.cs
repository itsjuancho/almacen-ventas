using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic;

namespace Almacen
{
    public partial class frmProductos : Form
    {

        MySqlConnection conexion = new MySqlConnection("server=localhost;database=ventasalmacen;uid=root;password=");
        object codigo, nombre, costo, existencia, unidadMedida, codigoUnidadMedida;
        Boolean switchEncontrarProducto, switchActualizarProducto;

        public frmProductos()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void frmProductos_Load(object sender, EventArgs e)
        {
            cargarProductos();
            cargarUnidadesMedida();
        }

        public void cargarProductos()
        {
            DataTable tablaProductos = new DataTable();
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_ListarProductos", conexion);
            conexion.Open();
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;
            adaptador.Fill(tablaProductos);
            if(tablaProductos.Rows.Count > 0)
            {
                dgvProductos.DataSource = tablaProductos;
                dgvProductos.Columns[4].Visible = false;
                dgvProductos.Columns[0].Width = 20;
            }else
            {
                MessageBox.Show("La tabla de datos está vacía", "Listar Productos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            conexion.Close();
        }

        public void buscarProducto()
        {
            DataTable tablaUnProducto = new DataTable();
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_ListarUnProducto", conexion);
            conexion.Open();
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;
            codigo = Interaction.InputBox("Ingrese el código del producto a buscar");
            adaptador.SelectCommand.Parameters.Add("p_codigo", MySqlDbType.String).Value = codigo;
            adaptador.Fill(tablaUnProducto);
            if (tablaUnProducto.Rows.Count > 0)
            {
                dgvProductos.DataSource = tablaUnProducto;
                dgvProductos.Columns[4].Visible = false;
                dgvProductos.Columns[0].Width = 20;
                conexion.Close();
                switchEncontrarProducto = true;
            }
            else
            {
                MessageBox.Show("El código no corresponde a un producto de la base de datos", "Listar Productos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                conexion.Close();
                cargarProductos();
            }
        }

        public void cargarUnidadesMedida()
        {
            DataTable unidadesMedida = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter("sp_ListarUnidadMedida", conexion);
            conexion.Open();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(unidadesMedida);
            if (unidadesMedida.Rows.Count > 0)
            {
                cmbUnidadMedida.ValueMember = "codigo";
                cmbUnidadMedida.DisplayMember = "descripcion";
                cmbUnidadMedida.DataSource = unidadesMedida;
            }else
            {
                MessageBox.Show("¡No hay unidades de medida disponibles! Debes de añadirlas antes de usar este formulario", "Error de carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            conexion.Close();
        }

        public void restablecerForm()
        {
            txtCodigo.Text = "";
            txtCodigo.Enabled = true;
            txtNombre.Text = "";
            txtCosto.Text = "";
            txtExistencia.Text = "";
            cmbUnidadMedida.SelectedValue = 0;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            buscarProducto();
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            restablecerForm();
            cargarProductos();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            buscarProducto();
            if (switchEncontrarProducto)
            {
                DialogResult confirmar = MessageBox.Show("¿Desea eliminar este producto? Esta acción no podrá revertirse", "Eliminar producto", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmar == DialogResult.Yes)
                {
                    DataTable tablaEliminarProducto = new DataTable();
                    MySqlDataAdapter adapter = new MySqlDataAdapter("sp_EliminarProducto", conexion);
                    try
                    {
                        conexion.Open();
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                        adapter.SelectCommand.Parameters.Add("p_codigo", MySqlDbType.String).Value = codigo;
                        adapter.Fill(tablaEliminarProducto);
                        conexion.Close();
                    }
                    catch (MySqlException err)
                    {
                        if (err.Number == 1451)
                        {
                            MessageBox.Show("Producto enlazado a una venta, no es posible eliminarlo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        conexion.Close();
                    }
                }
                cargarProductos();
                switchEncontrarProducto = false;
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            DataTable TablaActualizarProducto = new DataTable();
            MySqlDataAdapter adapterUpdate = new MySqlDataAdapter("sp_ActualizarProducto", conexion);
            conexion.Open();
            codigo = txtCodigo.Text;
            nombre = txtNombre.Text;
            costo = txtCosto.Text;
            existencia = txtExistencia.Text;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                txtCodigo.Text = dgvProductos.CurrentRow.Cells[0].Value.ToString();
                txtCodigo.Enabled = false;
                txtNombre.Text = dgvProductos.CurrentRow.Cells[1].Value.ToString();
                txtCosto.Text = dgvProductos.CurrentRow.Cells[2].Value.ToString();
                txtExistencia.Text = dgvProductos.CurrentRow.Cells[3].Value.ToString();
                codigoUnidadMedida = dgvProductos.CurrentRow.Cells[4].Value.ToString();
                //cmbUnidadMedida.SelectedValue = dgvProductos.CurrentRow.Cells[4].Value.ToString();

            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        public Boolean validarCampos()
        {
            if (txtCodigo.Text == "" || txtNombre.Text == "" || txtCosto.Text == "" || txtExistencia.Text == "")
            {
                return false;
            }else
            {
                return true;
            }
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            Boolean statusChildsForm = validarCampos();
            if (!statusChildsForm)
            {
                MessageBox.Show("Los campos están vacíos, verificalos", "Campos incompletos!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    DataTable tablaNuevoProducto = new DataTable();
                    MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_InsertarProducto", conexion);
                    conexion.Open();
                    codigo = txtCodigo.Text;
                    nombre = txtNombre.Text;
                    costo = txtCosto.Text;
                    existencia = txtExistencia.Text;
                    unidadMedida = cmbUnidadMedida.SelectedValue.ToString();
                    adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adaptador.SelectCommand.Parameters.Add("p_codigo", MySqlDbType.String).Value = codigo;
                    adaptador.SelectCommand.Parameters.Add("p_nombre", MySqlDbType.String).Value = nombre;
                    adaptador.SelectCommand.Parameters.Add("p_costo", MySqlDbType.Int32).Value = costo;
                    adaptador.SelectCommand.Parameters.Add("p_existencia", MySqlDbType.Int32).Value = existencia;
                    adaptador.SelectCommand.Parameters.Add("p_unidadmedida", MySqlDbType.String).Value = unidadMedida;
                    adaptador.Fill(tablaNuevoProducto);
                    conexion.Close();
                    restablecerForm();
                    cargarProductos();
                }
                catch (MySqlException error)
                {
                    if (error.Number == 1062)
                    {
                        MessageBox.Show("El código de producto ingresado ya existe");
                    }
                    conexion.Close();
                }
            }
        }
    }
}
