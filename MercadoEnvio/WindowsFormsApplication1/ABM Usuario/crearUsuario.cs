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

namespace WindowsFormsApplication1.ABM_Usuario
{
    public partial class crearUsuario : Form
    {
        DbQueryHandler consultador = new DbQueryHandler();
        
        public crearUsuario()
        {
            InitializeComponent();
        }
        private bool verificarLlenadoDeCampos()
        {
            if (textBoxNuevoUsuario.Text == "")
            {MessageBox.Show("Ingresar Id Usuario");return false;}
            else
            {
                if (textBoxNuevoUsuarioPassw.Text == "")
                {MessageBox.Show("Ingresar Contraseña"); return false;}
                else
                {
                    if ((!(radioButEmpresa.Checked) & !(radioButCliente.Checked)))
                    {MessageBox.Show("Seleccionar Rol a asignar"); return false;}
                    else {
                        if (!(idUserExiste(textBoxNuevoUsuario.Text))) { MessageBox.Show("Id de usuario en uso"); return false; }
                        else return true;
                    };
                }
            }
        }

        private bool idUserExiste(string user)
        {
            return consultador.existeUser(user);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void crearUsuario_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void butContinuar_Click(object sender, EventArgs e)
        {
            if (verificarLlenadoDeCampos()){

            if (radioButCliente.Checked) {
                CreacionUsuarioCliente cliente = new CreacionUsuarioCliente();
                cliente.password = textBoxNuevoUsuarioPassw.Text;
                cliente.idusuario = textBoxNuevoUsuario.Text;
                agregarUsuarioCliente pantallaAgregarUsuarioCliente = new agregarUsuarioCliente(cliente);
                pantallaAgregarUsuarioCliente.datosParaCrear = cliente;
                pantallaAgregarUsuarioCliente.Show();
            };
            if (radioButEmpresa.Checked)
            {
                CreacionUsuarioEmpresa cliente = new CreacionUsuarioEmpresa();
                cliente.password = textBoxNuevoUsuarioPassw.Text;
                cliente.idusuario = textBoxNuevoUsuario.Text;
                agregarUsuarioEmpresa pantallaAgregarUsuarioEmpresa = new agregarUsuarioEmpresa(cliente);
                pantallaAgregarUsuarioEmpresa.datosParaCrear = cliente;
                pantallaAgregarUsuarioEmpresa.Show();
            };
        }
        }
    }

public class CreacionUsuarioCliente
{
    public string idusuario;
    public string password;
    public string nombre;
    public string apellido;
    public string tipoDoc;
    public int documento;
    public string email;
    public int telefono;
    public string calle;
    public int nrocalle;
    public int piso;
    public string dpto;
    public string localidad;
    public int codigopostal;
    public string fechaCreacion;
    public string fechaNac;

    //setters
    public void setNombre(string n){ nombre = n; }
    public void setApellido(string a) { apellido = a; }
    public void setTipoDoc(string t) { tipoDoc = t; }
    public void setDoc(int d) { documento = d; }
    public void setEmail(string e) { email = e; }
    public void setTel(int t) { telefono = t; }
    public void setCalle(string c) { calle = c;}
    public void setNCalle(int n) { nrocalle = n; }
    public void setPiso(int p) { piso = p; }
    public void setDpto(string d) { dpto = d; }
    public void setLoc(string l) { localidad = l; }
    public void setCP(int cp) { codigopostal = cp; }
    public void setFecCre(string date) { fechaCreacion = date; }
    public void setFecNac(string date) { fechaNac = date; }
}

public class CreacionUsuarioEmpresa
{
    public string idusuario;
    public string password;
    public string razonSocial;
    public string cuit;
    public string email;
    public int telefono;
    public string calle;
    public int nrocalle;
    public int piso;
    public string dpto;
    public string localidad;
    public string ciudad;
    public int codigopostal;
    public string nombreContacto;
    public string rubroTrabajoEmpresa;
    public string fechaCreacion;

    //setters
    public void setRazSoc(string n) { razonSocial = n; }
    public void setCuit(string t) { cuit = t; }
    public void setEmail(string e) { email = e; }
    public void setTel(int t) { telefono = t; }
    public void setCalle(string c) { calle = c; }
    public void setNCalle(int n) { nrocalle = n; }
    public void setPiso(int p) { piso = p; }
    public void setDpto(string d) { dpto = d; }
    public void setLoc(string l) { localidad = l; }
    public void setCiudad(string c) { ciudad= c; }
    public void setCP(int cp) { codigopostal = cp; }
    public void setNombreCont(string nc) { nombreContacto = nc; }
    public void setRubroTrabajo(string r) { rubroTrabajoEmpresa = r; }
    public void setFecCreacion(string f) { fechaCreacion = f; }

}

public class DbQueryHandler{

    public bool existeUser(string user)
    {
        SqlDataReader lector;
        SqlCommand comando = new SqlCommand("select count(*) from GROUP_APROVED.Usuarios where Username = '" + user + "'", DbConnection.connection.getdbconnection());
        lector = comando.ExecuteReader();
        lector.Read();
        int retorno = lector.GetInt32(0);
        lector.Close();
        return (retorno == 0);
    }
        
    }
}