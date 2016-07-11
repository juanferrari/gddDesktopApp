﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication1.Generar_Publicación
{
    public partial class CrearPublicacion : Form
    {
        DbQueryHandlerCreate dbQueryHandler = new DbQueryHandlerCreate();
        Dictionary<String,String> rubros;
        Dictionary<String, String> visibilidades;

        public CrearPublicacion()
        {
            InitializeComponent();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            String desc = richTextBox1.Text;
            String stock = textBox3.Text;
            String precio = textBox1.Text;
            String tipo = comboBox2.Text;
            String visib = visibilidades[comboBox3.Text].ToString();
            String rubro = rubros[comboBox4.Text].ToString();
            String estado = dbQueryHandler.cargarEstado("Borrador");
            String envios = "V";
            String preguntas = "V";

            if (radioButton2.Checked == true)
                envios = "F";

            if (radioButton3.Checked == true)
                preguntas = "F";
            
            Int32 pubId = dbQueryHandler.createPub(desc, stock,precio,tipo,visib,rubro,estado,envios,preguntas);

            if (pubId > 0)
            {
                MessageBox.Show("Publicacion creada correctamente");
                Form2 f2 = new Form2(pubId,"creacion");

                String costo = dbQueryHandler.getCostoPub(comboBox3.Text);
                String idFactura = dbQueryHandler.crearFactura(costo.ToString(), pubId.ToString(), "NULL");
                Decimal nroItem = dbQueryHandler.getNumeroItems(idFactura);

                dbQueryHandler.crearItem(idFactura, (nroItem + 1).ToString(), costo,"1","Publicacion");
                f2.Show();
                this.Close();
            }
        }

        private void CrearPublicacion_Load(object sender, System.EventArgs e)
        {

            rubros = dbQueryHandler.cargarRubros();
            visibilidades = dbQueryHandler.cargarVisibilidades();
            foreach (var map in rubros)
            {
                comboBox4.Items.Add(map.Key);
            }
            foreach (var map in visibilidades)
            {
                comboBox3.Items.Add(map.Key);
            }

            comboBox2.Items.Add("Subasta");
            comboBox2.Items.Add("Compra Inmediata");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
   
    class DbQueryHandlerCreate {

        public Int32 createPub(String desc, String stock, String precio, String tipo, String visib, String Id_Rubro,String estado,String envios,String preguntas)
        {

            DateTime myDateTime = DateTime.Now;
            string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd");
            DateTime myDateTime2 = DateTime.Now.AddDays(30);

            string sqlFormattedDate2 = myDateTime2.ToString("yyyy-MM-dd");
            SqlCommand cmd = new SqlCommand("insert into GROUP_APROVED.publicaciones values ('" + desc + "'," + stock + ",'" + sqlFormattedDate + "','" 
                + sqlFormattedDate2 + "'," + precio + ",'" + tipo + "'," + visib + "," + estado + "," + Id_Rubro + "," + CurrentUser.user.getUserId() +
                ",'"+envios+"'"+",'"+preguntas+"');SELECT Publicacion_Cod FROM GROUP_APROVED.Publicaciones WHERE Publicacion_Cod = @@Identity", DbConnection.connection.getdbconnection());
            Int32 Publicacion_Cod = (Int32)cmd.ExecuteScalar();

            return Publicacion_Cod;
        }

        public Dictionary<string, string> cargarRubros()
        {
             SqlCommand cmd = new SqlCommand("select Id_Rubro,Rubro_Desc_Corta,Rubro_Desc_Completa from GROUP_APROVED.Rubros",DbConnection.connection.getdbconnection());
             SqlDataReader dataReader = cmd.ExecuteReader();

             var rubros = new Dictionary<string, string>();

             while (dataReader.Read())
             {
                 rubros[dataReader.GetString(1)] = dataReader.GetDecimal(0).ToString();
             }

             dataReader.Close();

             return rubros;
        }

        public Dictionary<string, string> cargarVisibilidades()
        {
            SqlCommand cmd = new SqlCommand("select Visibilidad_Cod,Visibilidad_Desc from GROUP_APROVED.Visibilidades", DbConnection.connection.getdbconnection());
            SqlDataReader dataReader = cmd.ExecuteReader();

            var visibilidades = new Dictionary<string, string>();

            while (dataReader.Read())
            {
                visibilidades[dataReader.GetString(1)] = dataReader.GetDecimal(0).ToString();
            }

            dataReader.Close();

            return visibilidades;
        }

        public String cargarEstado(String est)
        {
            SqlCommand cmd = new SqlCommand("select Id_Est from GROUP_APROVED.Estado_Publ where Descripcion = '"+est+"'", DbConnection.connection.getdbconnection());
            SqlDataReader dataReader = cmd.ExecuteReader();
            String estado;

            var estados = new Dictionary<string, string>();

            dataReader.Read();
            estado = dataReader.GetInt32(0).ToString();
            dataReader.Close();

            return estado;

        }

        public String crearFactura(String precio, String pubId, String idCompra)
        {
            SqlCommand cmd = new SqlCommand("insert into GROUP_APROVED.Facturas values(getdate()," + precio.Replace(',', '.') + "," + "'Efectivo', " + pubId + "," + idCompra + ");SELECT Nro_Fact FROM GROUP_APROVED.Facturas WHERE Nro_Fact = @@Identity", DbConnection.connection.getdbconnection());

            Decimal result = (Decimal)cmd.ExecuteScalar();



            return result.ToString();


        }
         public String crearItem(String idFactura,String nroItem,String costo,String cantItems,String tipo)
        {
            SqlCommand cmd = new SqlCommand("insert into GROUP_APROVED.Items values("+idFactura+"," + nroItem + "," + costo+"," + cantItems + ",'" + tipo + "')", DbConnection.connection.getdbconnection());

            Int32 result = cmd.ExecuteNonQuery();

            return result.ToString();

        }
 

        public String getCostoPub(String visib)
        {
            SqlCommand cmd = new SqlCommand("select Visibilidad_Precio from GROUP_APROVED.Visibilidades where Visibilidad_Desc = '" + visib + "'", DbConnection.connection.getdbconnection());

            SqlDataReader dataReader = cmd.ExecuteReader();
            dataReader.Read();

            String result = dataReader.GetDecimal(0).ToString().Replace(',','.');
            dataReader.Close();
            return result;
        }


        public Decimal getNumeroItems(String factId)
        {
            SqlCommand cmd = new SqlCommand("select max(Nro_Item) items from GROUP_APROVED.Items where Nro_Fact = " + factId, DbConnection.connection.getdbconnection());

            SqlDataReader dataReader = cmd.ExecuteReader();

            Decimal max = 0;
            if (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0))
                {
                    max = dataReader.GetDecimal(0);
                }

            }
            else { max = 0; }
            dataReader.Close();
            return max;
        }
    }
}
