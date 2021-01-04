using System;
using System.Collections.Generic;
using System.Data.SqlClient;//Cliente para conectarse a SqL server
using System.Data;
using System.IO;
using Ionic.Zip;
using System.Globalization;
using System.Threading;
using AccesoDatos.Properties;

namespace AccesoDatos
{
    public class ConexionSQLServer : ConexionDB //Hereda metodos de conexion BD
    {
        public string status;
        public string Mensaje;
        // public RTConsecutivo;
        SqlConnection conexion;//Instancia de conexion sql server       
        Config Configuracion = new Config();

        /// <summary>
        /// Estabelcer conexion con el servidor usando la cadena sconexion
        /// </summary>
        public override void Conectar()
        {
            string sConexion = "Server="+Configuracion.Servidor+";User Id="+Configuracion.Usuario+";Password="+Configuracion.Password+";Database="+Configuracion.bd;    
           // string sConexion = "Server=192.168.1.9;User Id=Administrator;Password=s14f2014!!;Database=financiera";    
            conexion = new SqlConnection(sConexion);
        }
        public override void Conectar(string Server, string User, string Password, string DB)
        {
            string sConexion = "Server=" +Server + ";User Id=" + User + ";Password=" + Password + ";Database=" +DB;
            conexion = new SqlConnection(sConexion);
        }
        public override void ConectarMaestra()
        {
            string sMasterConnection = "Server=" + Configuracion.Servidor + ";User Id=" + Configuracion.Usuario + ";Password=" + Configuracion.Password + ";Database=master";    
            conexion = new SqlConnection(sMasterConnection);
        }
        public override void AbrirConexion()
        {
            try
            {
                conexion.Open();
                if (conexion.State == ConnectionState.Open)
                {
                    status = "Ok";
                }
                else
                {
                    status = "error";
                }
            }
            catch(SqlException ex)
            {
                status = "error";
            }
        }
        /// <summary>
        /// Cerrar conexion con servvidor
        /// </summary>
        public override void Desconectar()
        {
            conexion.Close();
        }
            
        //}
        /// <summary>
        /// Metodo para actualizar registros
        /// </summary>
        /// <param name="sSQL">consulta sql con paramentros de actualizacion</param>
        /// <returns></returns>
        public override bool Actualizar(string sSQL)
        {
            SqlCommand comando = new SqlCommand(sSQL, conexion); //Nuevo comnado sql
            AbrirConexion();//conecatr con bd
            int iFilas = comando.ExecuteNonQuery();//ejecutar consulta
            Desconectar();//cerrar conexion
            //retornar verdadero si se inserto el registo
            if (iFilas > 0)
                return true;
            else
                return false;
        }
        public override bool Actualizar(string sSQL, int TOut)
        {
            SqlCommand comando = new SqlCommand(sSQL, conexion); //Nuevo comnado sql
            comando.CommandTimeout = TOut;
            AbrirConexion();//conecatr con bd
            int iFilas = comando.ExecuteNonQuery();//ejecutar consulta
            Desconectar();//cerrar conexion
            //retornar verdadero si se inserto el registo
            if (iFilas > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Metodo para eliminar registros
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public override bool Eliminar(string sSQL)
        {
            SqlCommand comando = new SqlCommand(sSQL, conexion);//Nuevo comnado sql
            AbrirConexion();//conecatr con bd
            int iFilas = comando.ExecuteNonQuery();//ejecutar consulta
            Desconectar();//cerrar conexion
            //retornar verdadero si se elimino el registo
            if (iFilas == 1)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Extraer informacion de la base de datos y retornarla en un dataset
        /// </summary>
        /// <param name="sSQL">Consulta en sql</param>
        /// <returns></returns>
        public override DataSet Consultar(string sSQL)
        {
            SqlCommand comando = new SqlCommand(sSQL, conexion);//Nuevo comnado sql
            comando.CommandTimeout = 10000;
            SqlDataAdapter adaptador = new SqlDataAdapter(comando);//crear adaptador para la informacion que viene de la base de datos
            DataSet dsDatos = new DataSet();//Dataset para guardar informacion en memoria
            adaptador.Fill(dsDatos);//llenar dataset
            return dsDatos;//retornar dataset
        }
        public override DataSet Consultar(string sSQL, int timeout)
        {
            SqlCommand comando = new SqlCommand(sSQL, conexion);//Nuevo comnado sql
            comando.CommandTimeout = timeout;
            SqlDataAdapter adaptador = new SqlDataAdapter(comando);//crear adaptador para la informacion que viene de la base de datos
            DataSet dsDatos = new DataSet();//Dataset para guardar informacion en memoria
            adaptador.Fill(dsDatos);//llenar dataset
            return dsDatos;//retornar dataset
        }
        public override void EjecutarComando(string sSQL)
        {
            SqlCommand comando = new SqlCommand(sSQL, conexion);//Nuevo comnado sql
            AbrirConexion();//conecatr con bd
            comando.CommandTimeout = 0;
            comando.Prepare();
            comando.ExecuteNonQuery();
            Desconectar();
        }
        /// <summary>
        /// Eliminar informacion de la base de datos y retornarla en un dataset
        /// </summary>
        /// <param name="sSQL">Consulta sql</param>
        /// <returns></returns>
        public override int Insertar(string sSQL)
        {
            SqlCommand comando = new SqlCommand(sSQL, conexion);//Nuevo comnado sql
            AbrirConexion();//conecatr con bd
            int iFilas = comando.ExecuteNonQuery();//Ejecutar consulta
            Desconectar();//desconectar bd
            return iFilas;//retornar total de registros eliminados
        }

        public override DataSet LlenarDataset(string[] sSQL, string[] dDatatables)
        {
            DataSet dsDatos = new DataSet();//Dataset para guardar informacion en memoria
            for (int i = 0; i < sSQL.Length; i++)
            {
                SqlCommand comando = new SqlCommand(sSQL[i], conexion);//Nuevo comnado sql
                SqlDataAdapter adaptador = new SqlDataAdapter(comando);//crear adaptador para la informacion que viene de la base de datos
                adaptador.Fill(dsDatos, dDatatables[i]);//llenar dataset
            }
            return dsDatos;//retornar dataset
        }
        public override int InsertarParametros(string sSQL,System.IO.MemoryStream Logo1,System.IO.MemoryStream Logo2,System.IO.MemoryStream FirmaRepresentante,System.IO.MemoryStream FirmaTesorero)
        {
            SqlCommand comando = new SqlCommand(sSQL, conexion);
            AbrirConexion();
            comando.Parameters.Add("@logo1", Logo1.GetBuffer());
            comando.Parameters.Add("@logo2", Logo2.GetBuffer());
            comando.Parameters.Add("@FirmaRepresentante", FirmaRepresentante.GetBuffer());
            comando.Parameters.Add("@FirmaTesorero", FirmaTesorero.GetBuffer());
            int i = comando.ExecuteNonQuery();
            Desconectar();
            return i;
        }
        public override bool VolcarDataTable(DataTable TablaOrigen, string sTablaDestino)
        {
            bool Volcado = false;
            AbrirConexion();
            using (SqlBulkCopy s = new SqlBulkCopy(conexion))
            {
                s.DestinationTableName = sTablaDestino;

                foreach (var column in TablaOrigen.Columns)
                {
                    s.ColumnMappings.Add(column.ToString(), column.ToString());
                }
                try
                {
                    s.WriteToServer(TablaOrigen);
                    Volcado = true;
                }
                catch (Exception ex)
                {
                    this.Mensaje = ex.Message;
                    Volcado = false;
                }
                finally
                {
                    Desconectar();
                }
                return Volcado;
            }
        }
        public override void ActualizarTabla(string TablaDestino, DataTable dt, string sSQLUpdate, string sSQLCreate)
        {
            Mensaje = string.Empty;
            using (SqlCommand comando = new SqlCommand("", conexion))
            {
                try
                {
                    AbrirConexion();
                    comando.CommandText = sSQLCreate;
                    comando.ExecuteNonQuery();

                    using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conexion))
                    {
                        bulkcopy.BulkCopyTimeout = 660;
                        bulkcopy.DestinationTableName = "#temporal";
                        bulkcopy.WriteToServer(dt);
                        bulkcopy.Close();
                    }
                    comando.CommandTimeout = 300;
                    comando.CommandText = sSQLUpdate;
                    comando.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Mensaje = ex.Message;
                }
                finally
                {
                    Desconectar();
                }
            }
        }
        public override bool InsertarEgresoNP(DataTable dtMC, DataTable dtEgreso)
        {
            bool bMCInsertado = false;
            bool bNPInsertado = false;
            AbrirConexion();
            SqlBulkCopy sbc = new SqlBulkCopy(conexion, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
            try
            {
                sbc.DestinationTableName = "tbl_mc";
                sbc.ColumnMappings.Add("codigo", "codigo");
                sbc.ColumnMappings.Add("debito", "debito");
                sbc.ColumnMappings.Add("credito", "credito");
                sbc.ColumnMappings.Add("cheque", "cheque");
                sbc.ColumnMappings.Add("descri", "descri");
                sbc.ColumnMappings.Add("documento_id", "documento_id");
                sbc.ColumnMappings.Add("valbase", "valbase");
                sbc.ColumnMappings.Add("fuente", "fuente");
                //Insertar Registros
                sbc.WriteToServer(dtMC);
                bMCInsertado = true;
            }
            catch (Exception ex)
            {
                bMCInsertado = false;
                Mensaje = ex.Message;
            }
            try
            {
                sbc = new SqlBulkCopy(conexion, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
                sbc.DestinationTableName = "tbl_egresos_np";
                sbc.ColumnMappings.Add("movimiento_docu_id", "movimiento_docu_id");
                sbc.ColumnMappings.Add("codigo_contable", "codigo_contable");
                sbc.ColumnMappings.Add("debito", "debito");
                sbc.ColumnMappings.Add("credito", "credito");
                sbc.ColumnMappings.Add("tipo_retencion", "tipo_retencion");
                sbc.ColumnMappings.Add("valor_iva", "valor_iva");
                sbc.ColumnMappings.Add("valor_base", "valor_base");
                sbc.ColumnMappings.Add("fecha_pago", "fecha_pago");
                sbc.ColumnMappings.Add("factura", "factura");
                sbc.ColumnMappings.Add("est_mov", "est_mov");
                sbc.WriteToServer(dtMC);
                bNPInsertado = true;
            }
            catch (Exception ex)
            {
                bNPInsertado = false;
                Mensaje = ex.Message;
            }
            if (!bMCInsertado || !bNPInsertado)
            {
                Eliminar("delete from movimiento_docu where id='" + Convert.ToInt32(dtMC.Rows[0]["documento_id"]) + "'");
            }
            Desconectar();
            return bMCInsertado && bNPInsertado;
        }
        public override bool InsertarSolicitudCDP(DataTable dtSolicitudRubros, DataTable dtItems, int idSolicitud)
        {
            bool RubrosInsertados = false;
            bool DetallesInsertados = false;
            AbrirConexion();
            SqlBulkCopy solicitud_rubros = new SqlBulkCopy(conexion, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
            try
            {
                solicitud_rubros.DestinationTableName = "solicitud_rubros";
                solicitud_rubros.ColumnMappings.Add("nrodoc", "nrodoc");
                solicitud_rubros.ColumnMappings.Add("rubro", "rubro");
                solicitud_rubros.ColumnMappings.Add("fuente", "fuente");
                solicitud_rubros.ColumnMappings.Add("valor", "valor");
                solicitud_rubros.ColumnMappings.Add("tipo", "tipo");
                //Insertar Registros
                solicitud_rubros.WriteToServer(dtSolicitudRubros);
                RubrosInsertados = true;
            }
            catch (Exception ex)
            {
                RubrosInsertados = false;
                Mensaje = ex.Message;
            }
            SqlBulkCopy solicitud_Productos = new SqlBulkCopy(conexion, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
            try
            {
                solicitud_Productos.DestinationTableName = "solicitud_productos";
                solicitud_Productos.ColumnMappings.Add("nrodoc", "nrodoc");
                solicitud_Productos.ColumnMappings.Add("item", "item");
                solicitud_Productos.ColumnMappings.Add("nombre_producto", "nombre_producto");
                solicitud_Productos.ColumnMappings.Add("cantidad", "cantidad");
                solicitud_Productos.WriteToServer(dtItems);
                DetallesInsertados = true;
            }
            catch (Exception ex)
            {
                DetallesInsertados = false;
                Mensaje = ex.Message;
            }

            if (!RubrosInsertados || !DetallesInsertados)
            {
                Desconectar();
                return false;
            }
            else
            {
                Desconectar();
                return true;
            }

        }

        public override bool InsertarSolicitudRP(DataTable dtSolicitudRubros,int idSolicitud)
        {
            bool RubrosInsertados = false;
            bool DetallesInsertados = false;
            AbrirConexion();
            SqlBulkCopy solicitud_rubros_rp = new SqlBulkCopy(conexion, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
            try
            {
                solicitud_rubros_rp.DestinationTableName = "solicitud_rubros_rp";
                solicitud_rubros_rp.ColumnMappings.Add("nrodoc", "nrodoc");
                solicitud_rubros_rp.ColumnMappings.Add("rubro", "rubro");
                solicitud_rubros_rp.ColumnMappings.Add("fuente", "fuente");
                solicitud_rubros_rp.ColumnMappings.Add("valor", "valor");
                solicitud_rubros_rp.ColumnMappings.Add("tipo", "tipo");
                solicitud_rubros_rp.ColumnMappings.Add("nrodis", "nrodis");
                //Insertar Registros
                solicitud_rubros_rp.WriteToServer(dtSolicitudRubros);
                RubrosInsertados = true;
            }
            catch (Exception ex)
            {
                RubrosInsertados = false;
                Mensaje = ex.Message;
            }
            if (!RubrosInsertados || !DetallesInsertados)
            {
                Desconectar();
                return false;
            }
            else
            {
                Desconectar();
                return true;
            }
        }


        public override bool InsertarCPD(DataTable dtRubros, int idSolicitud)
        {
            bool RubrosInsertados = false;
            AbrirConexion();
            SqlBulkCopy solicitud_rubros = new SqlBulkCopy(conexion, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
            try
            {
                solicitud_rubros.DestinationTableName = "pre_movi";
                solicitud_rubros.ColumnMappings.Add("documento_id", "documento_id");
                solicitud_rubros.ColumnMappings.Add("rubro", "rubro");
                solicitud_rubros.ColumnMappings.Add("fuente", "fuente");
                solicitud_rubros.ColumnMappings.Add("tipo", "tipo");
                solicitud_rubros.ColumnMappings.Add("fecha_documento", "fecha_documento");
                solicitud_rubros.ColumnMappings.Add("nrodis", "nrodis");
                solicitud_rubros.ColumnMappings.Add("nropet", "nropet");
                solicitud_rubros.ColumnMappings.Add("valor", "valor");
                solicitud_rubros.ColumnMappings.Add("saldo_disponible", "saldo_disponible");
                solicitud_rubros.ColumnMappings.Add("saldo_documento", "saldo_documento");
                //Insertar Registros
                solicitud_rubros.WriteToServer(dtRubros);
                RubrosInsertados = true;
            }
            catch (Exception ex)
            {
                RubrosInsertados = false;
                Mensaje = ex.Message;
            }
            Desconectar();
            return RubrosInsertados;
        }
        public override bool InsertarReduccion(DataTable dtDatos, int idReduccion)
        {
            bool bInsertado = false;
            AbrirConexion();
            SqlBulkCopy sbc = new SqlBulkCopy(conexion, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
            try
            {
                sbc.DestinationTableName = "pre_movi";
                sbc.ColumnMappings.Add("documento_id", "documento_id");
                sbc.ColumnMappings.Add("rubro", "rubro");
                sbc.ColumnMappings.Add("fuente", "fuente");
                sbc.ColumnMappings.Add("tipo", "tipo");
                sbc.ColumnMappings.Add("fecha_documento", "fecha_documento");
                sbc.ColumnMappings.Add("valor", "valor");
                sbc.WriteToServer(dtDatos);
                bInsertado = true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                bInsertado = false;
            }
            Desconectar();
            return bInsertado;
        }
        public override bool VolcarPeriodos(DataTable dtPeriodos)
        {
            Eliminar("delete from tbl_periodos");
            bool bInsertado = false;
            int iDocumentoId;
            AbrirConexion();
            SqlBulkCopy sbc = new SqlBulkCopy(conexion, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
            try
            {
                sbc.DestinationTableName = "tbl_periodos";
                sbc.ColumnMappings.Add("fecha_inicial", "fecha_inicial");
                sbc.ColumnMappings.Add("fecha_final", "fecha_final");
                sbc.ColumnMappings.Add("cerrado", "cerrado");
                sbc.WriteToServer(dtPeriodos);
                bInsertado = true;
            }
            catch (Exception ex)
            {
                bInsertado = false;
                Mensaje = ex.Message;
            }
            return bInsertado;
        }
        public override bool GenerarBackup(string sPath)
        {
            try
            {
                if (System.IO.File.Exists(sPath))
                {
                    System.IO.File.Delete(sPath);
                }
                AbrirConexion();
                string sSQL = "backup database "+Configuracion.bd+" to disk='" + sPath + "'";
                SqlCommand comando = new SqlCommand(sSQL, conexion);
                comando.ExecuteNonQuery();
                Desconectar();
                string sZIPName = sPath.Replace(".bak", ".zip");
                if (System.IO.File.Exists(sZIPName))
                {
                    System.IO.File.Delete(sZIPName);
                }
                ZipFile zf = new ZipFile(@sZIPName);
                zf.AddFile(@sPath, "").Password = "s14f";
                zf.Save();
                System.IO.File.Delete(sPath);
                Mensaje = "";
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
            finally
            {
                Desconectar();
            }

        }
        public override bool RestaurarBackup(string sPath)
        {
            try
            {
                ZipFile zf = new ZipFile(sPath);
                zf.Password = "s14f";
                zf.ExtractAll(Path.GetDirectoryName(sPath), ExtractExistingFileAction.OverwriteSilently);
                string sBAKPath = sPath.Replace(".zip", ".bak");
                AbrirConexion();
                string sSQL = "alter database "+Configuracion.bd+" set SINGLE_USER WITH ROLLBACK IMMEDIATE; ";
                sSQL += "restore database financiera from disk = '" + sBAKPath + "' with replace";
                SqlCommand comando = new SqlCommand(sSQL, conexion);
                comando.CommandTimeout = 0;
                comando.ExecuteNonQuery();
                sSQL = "ALTER DATABASE financiera SET MULTI_USER;";
                comando = new SqlCommand(sSQL, conexion);
                comando.ExecuteNonQuery();
                System.IO.File.Delete(sBAKPath);
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
            finally
            {

                Desconectar();
            }
        }
        public override void InsertarRT(DataTable RT)
        {
            NumberStyles style;
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = culture;
            style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            
            //SqlTransaction Transaccion = new SqlTransaction();
           // Transaccion = conexion.BeginTransaction();
            int iID;
            string sSQL = "";
            SqlCommand comando;
            AbrirConexion();
            foreach(DataRow row in RT.Rows)
            {
             sSQL="   INSERT INTO [dbo].[movimiento_docu]\n"+
                   "([consecutivo]\n"+
                   ",[tipdoc]\n"+
                   ",[fecha]\n"+
                   ",[valor]\n"+
                   ",[descri])\n"+
        "VALUES\n"+
          " ( '" + row["consecutivort"] + "'\n" +
          // " ((select isnull(max(consecutivo),0)+1 from movimiento_docu where tipdoc='RT')\n" +
          " ,'RT'\n" +
          " ,'"+row["fecpag"]+"'\n"+
          " ,'" + row["valor"] + "'\n" +
          " ,'"+row["descri"]+"') select @@identity";

             comando = new SqlCommand(sSQL, conexion);
             //comando.Transaction = Transaccion;
             SqlDataAdapter adaptador = new SqlDataAdapter(comando);
             DataSet dsDatos = new DataSet();
             adaptador.Fill(dsDatos);
             iID = int.Parse(dsDatos.Tables[0].Rows[0][0].ToString());

                 sSQL = "INSERT INTO [dbo].[pre_movi] \n" +
               "([documento_id] \n" +
               ",[rubro] \n" +
               ",[fuente] \n" +
               ",[tipo] \n" +
               ",[fecha_documento] \n" +
               ",[valor] \n" +
                       ",[descri]) \n" +
                 "VALUES \n" +
                 "      ('"+iID+"' \n" +
                 "      ,'"+row["rubro_retencion"]+"' \n" +
                 "      ,'"+row["fuente_retencion"]+"' \n" +
                 "      ,2 \n" +
                 "      ,'"+Convert.ToDateTime(row["fecpag"]).ToString("yyyy-MM-dd")+"' \n" +
                 "      ,'"+row["valor"]+"' \n" +
                 "      ,'"+row["descri"]+"' \n" +
                 "     )";
                 comando = new SqlCommand(sSQL, conexion);
                 //comando.Transaction = Transaccion;
                 comando.ExecuteNonQuery();
            }
            //Transaccion.Commit();
            Desconectar();//cerrar conexion
        }
        public override void EjecutarComando(string sSQL, List<SqlParameter> Parametros)
        {
            SqlCommand comando = new SqlCommand(sSQL, conexion);
            foreach(SqlParameter parametro in Parametros)
            {
                comando.Parameters.Add(parametro);
            }
            try
            {
                AbrirConexion();
                comando.CommandTimeout = 0;
                //comando.Prepare();
                comando.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }
            finally
            {
                Desconectar();
            }
        }
        public override SqlParameter NuevoParametro(SqlParameter parametro, DateTime Value)
        {
            parametro.Value = Value;
            return parametro;
        }
        public override SqlParameter NuevoParametro(SqlParameter parametro, int Value)
        {
            parametro.Value = Value;
            return parametro;
        }
        public override SqlParameter NuevoParametro(SqlParameter parametro, string Value)
        {
            parametro.Value = Value;
            return parametro;
        }
        public override SqlParameter NuevoParametro(SqlParameter parametro, double Value)
        {
            parametro.Value = Value;
            return parametro;
        }
        public override void EjecutarComando(SqlCommand comando)
        {

            AbrirConexion();
            comando.Connection=conexion;
            comando.CommandTimeout = 0;
            comando.CommandType = CommandType.Text;
            try
            {
                comando.ExecuteNonQuery();   
            }
            catch(Exception ex)
            {
                var x = ex.Message;
            }
            Desconectar();
        }
    }
}

