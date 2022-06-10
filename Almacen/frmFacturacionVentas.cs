using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace Almacen
{
    public partial class frmFacturacionVentas : Form
    {

        public int Fila, j;
        public object numero, cliente, fechaFactura, fechaPago, formaPago, producto, cantidadProducto, valor;
        MySqlConnection conexion = new MySqlConnection("server=localhost;database=ventasalmacen;uid=root;password=");

        public frmFacturacionVentas()
        {
            InitializeComponent();
        }

        private void btnRegistrarFactura_Click(object sender, EventArgs e)
        {
            DataTable tablaEncabezadoFactura = new DataTable();
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_InsertarMaestroFactura", conexion);
            conexion.Open();
            numero = txtNumeroFactura.Text;
            cliente = cmbCliente.SelectedValue;
            fechaFactura = dtpFacturacion.Value.Year.ToString() + '-' + dtpFacturacion.Value.Month.ToString() + '-' + dtpFacturacion.Value.Date.ToString() + ' ' + dtpFacturacion.Value.Hour.ToString() + ':' + dtpFacturacion.Value.Minute.ToString() + ':' + dtpFacturacion.Value.Second.ToString();
            fechaPago = dtpPago.Value.Year.ToString() + '-' + dtpPago.Value.Month.ToString() + '-' + dtpPago.Value.Date.ToString();
            formaPago = cmbFormaPago.SelectedValue.ToString();
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;
            adaptador.SelectCommand.Parameters.Add("p_numero", MySqlDbType.Int32).Value = numero;
            adaptador.SelectCommand.Parameters.Add("p_cliente", MySqlDbType.Int32).Value = cliente;
            adaptador.SelectCommand.Parameters.Add("p_fechafact", MySqlDbType.Date).Value = fechaFactura;
            adaptador.SelectCommand.Parameters.Add("p_fechapago", MySqlDbType.Date).Value = fechaPago;
            adaptador.SelectCommand.Parameters.Add("p_formapago", MySqlDbType.String).Value = formaPago;
            adaptador.Fill(tablaEncabezadoFactura);
            conexion.Close();

            Fila = dgvVentas.Rows.Count;
            for (j = 0; j < Fila; j++)
            {
                DataTable tablaDetalleFactura = new DataTable();
                MySqlDataAdapter ADF = new MySqlDataAdapter("sp_InsertarDetalleFacturaProducto", conexion);
                conexion.Open();
                producto = dgvVentas.Rows[j].Cells[0].Value;
                cantidadProducto = dgvVentas.Rows[j].Cells[2].Value;
                valor = dgvVentas.Rows[j].Cells[3].Value;
                ADF.SelectCommand.CommandType = CommandType.StoredProcedure;
                ADF.SelectCommand.Parameters.Add("p_numfactura", MySqlDbType.Int32).Value = numero;
                ADF.SelectCommand.Parameters.Add("p_codproducto", MySqlDbType.String).Value = producto;
                ADF.SelectCommand.Parameters.Add("p_cantidad", MySqlDbType.Int32).Value = cantidadProducto;
                ADF.SelectCommand.Parameters.Add("p_valor", MySqlDbType.Int32).Value = valor;
                ADF.Fill(tablaDetalleFactura);
                conexion.Close();
            }

            MessageBox.Show("La factura con #" + numero + " se registró correctamente.");
        }

        public void cargarClientes()
        {
            DataTable TablaClientes = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter("sp_ListarClientes", conexion);
            conexion.Open();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(TablaClientes);
            if (TablaClientes.Rows.Count > 0)
            {
                cmbCliente.ValueMember = "Documento Identidad";
                cmbCliente.DisplayMember = "Cliente";
                cmbCliente.DataSource = TablaClientes;
            }
            else
            {
                MessageBox.Show("No hay registro de clientes", "Error de carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            conexion.Close();
        }

        public void cargarMetodosPago()
        {
            DataTable TablaFormaPago = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter("sp_ListarFormasPago", conexion);
            conexion.Open();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(TablaFormaPago);
            if (TablaFormaPago.Rows.Count > 0)
            {
                cmbFormaPago.ValueMember = "codigo";
                cmbFormaPago.DisplayMember = "descripcion";
                cmbFormaPago.DataSource = TablaFormaPago;
            }
            else
            {
                MessageBox.Show("¡No hay formas de pago disponibles! Debes de añadirlas antes de usar este formulario", "Error de carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            conexion.Close();
        }

        public void cargarProductos()
        {
            DataTable TablaProductos = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter("sp_ListarProductos", conexion);
            conexion.Open();
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(TablaProductos);
            if (TablaProductos.Rows.Count > 0)
            {
                cmbProducto.DataSource = TablaProductos;
                cmbProducto.DisplayMember = "nombre";
                cmbProducto.ValueMember = "codigo";
            }
            else
            {
                MessageBox.Show("¡No hay unidades de medida disponibles! Debes de añadirlas antes de usar este formulario", "Error de carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            conexion.Close();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void frmFacturacionVentas_Load(object sender, EventArgs e)
        {
            cargarClientes();
            cargarMetodosPago();
            cargarProductos();

            DataTable TablaConsecutivo = new DataTable();
            MySqlDataAdapter Adaptador = new MySqlDataAdapter("sp_UltimaFactura", conexion);
            conexion.Open();
            Adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;
            Adaptador.Fill(TablaConsecutivo);
            DataRow UltimaFactura = TablaConsecutivo.Rows[0];
            int Consecutivo = Convert.ToInt32(UltimaFactura["numero"].ToString());
            txtNumeroFactura.Text = Convert.ToString(Consecutivo + 1);
            conexion.Close();
        }

        private void cmbProducto_DropDownClosed(object sender, EventArgs e)
        {
            DataTable TablaPrecioProducto = new DataTable();
            MySqlDataAdapter Adaptador = new MySqlDataAdapter("sp_PrecioProducto", conexion);
            conexion.Open();
            String CodigoProducto = cmbProducto.SelectedValue.ToString();
            Adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;
            Adaptador.SelectCommand.Parameters.Add("p_codigo", MySqlDbType.String).Value = CodigoProducto;
            Adaptador.Fill(TablaPrecioProducto);

            DataRow Precioproducto = TablaPrecioProducto.Rows[0];
            txtValorUnidad.Text = Precioproducto["costo"].ToString();
            txtCantidad.Text = Convert.ToString(1);
            txtCantidad.Focus();
            conexion.Close();
        }

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            dgvVentas.Rows.Add();
            dgvVentas.Rows[Fila].Cells[0].Value = cmbProducto.SelectedValue.ToString();
            dgvVentas.Rows[Fila].Cells[1].Value = cmbProducto.Text;
            dgvVentas.Rows[Fila].Cells[2].Value = txtCantidad.Text;
            dgvVentas.Rows[Fila].Cells[3].Value = txtValorUnidad.Text;
            dgvVentas.Rows[Fila].Cells[4].Value = Convert.ToInt32(txtCantidad.Text) * Convert.ToInt32(txtValorUnidad.Text);
            Fila = Fila + 1;
            txtTotalVenta.Text = string.Format("{0:c}", calcularTotal());
        }

        public int calcularTotal()
        {
            int total = 0;
            Fila = dgvVentas.Rows.Count;
            for (j=0; j < Fila; j++)
            {
                total = total + Convert.ToInt32(dgvVentas.Rows[j].Cells[4].Value);
            }
            return total;
        }
    }
}
