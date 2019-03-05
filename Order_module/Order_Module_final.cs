using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Order_module
{
    public partial class Order_Module_final : Form
    {
        int id = 0;
        int total, paid, remaining;
        String strFilePath = "";
        Image DefaultImage;
        Byte[] ImageByteArray;
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-52MI41T;Initial Catalog=DSP;Integrated Security=True");
        public Order_Module_final()
        {
            InitializeComponent();
        }
        public void getsrno()
        {
            label3.Text = "";
            SqlDataAdapter adp = new SqlDataAdapter("select isnull(max(cast([id]as bigint)),0)+1 from [Order_details]", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            label3.Text = dt.Rows[0][0].ToString();
        }
        private void button_save_Click(object sender, EventArgs e)
        {
            if (text_image_path.Text.Trim() != "" && text_name.Text != "" && text_address.Text != "" && text_position.Text != "" &&textBox1.Text!="" && textBox2.Text!="" && textBox3.Text!="" && comboBox1.Text!="" && textBox4.Text!="" && textBox5.Text!="")
            {

                if (strFilePath == "")
                {
                    if (ImageByteArray.Length != 0)
                        ImageByteArray = new byte[] { };
                }
                else
                {
                    Image temp = new Bitmap(strFilePath);
                    MemoryStream strm = new MemoryStream();
                    temp.Save(strm, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ImageByteArray = strm.ToArray();
                }
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlCommand sqlCmd = new SqlCommand("add_or_update_data", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                sqlCmd.Parameters.Add("@id", id);
                sqlCmd.Parameters.Add("@name", text_name.Text);
                sqlCmd.Parameters.Add("@contact", text_address.Text);
                sqlCmd.Parameters.Add("@product", textBox1.Text);
                sqlCmd.Parameters.Add("@code",textBox2.Text);
                sqlCmd.Parameters.Add("@weight",textBox3.Text);
                sqlCmd.Parameters.Add("@weight_type",comboBox1.Text);
                sqlCmd.Parameters.Add("@image_path", text_image_path.Text.Trim());
                sqlCmd.Parameters.Add("@image", ImageByteArray);
                sqlCmd.Parameters.Add("@total", text_position.Text);
                sqlCmd.Parameters.Add("@paid",textBox4.Text);
                sqlCmd.Parameters.Add("@remaining",textBox5.Text);
                sqlCmd.Parameters.Add("@save_date",dateTimePicker1.Text);
                int n= sqlCmd.ExecuteNonQuery();
                if(n>0)
                {
                    con.Close();
                    MessageBox.Show("Saved successfully");
                    Clear();
                    RefreshImageGrid();
                    getsrno();
                }
                else
                {
                    MessageBox.Show("Failed To Save");
                }      
            }
            else
            {
                MessageBox.Show("Please enter all details");
            }
            if (button_save.Text == "update")
            {
                if (text_image_path.Text.Trim() != "" && text_name.Text != "" && text_address.Text != "" && text_position.Text != "")
                {
                    if (strFilePath == "")
                    {
                        if (ImageByteArray.Length != 0)
                            ImageByteArray = new byte[] { };
                    }
                    else
                    {
                        Image temp = new Bitmap(strFilePath);
                        MemoryStream strm = new MemoryStream();
                        temp.Save(strm, System.Drawing.Imaging.ImageFormat.Jpeg);
                        ImageByteArray = strm.ToArray();
                    }
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    SqlCommand sqlCmd = new SqlCommand("add_or_update_data", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    sqlCmd.Parameters.Add("@id", id);
                    sqlCmd.Parameters.Add("@name", text_name.Text);
                    sqlCmd.Parameters.Add("@contact", text_address.Text);
                    sqlCmd.Parameters.Add("@product", textBox1.Text);
                    sqlCmd.Parameters.Add("@code", textBox2.Text);
                    sqlCmd.Parameters.Add("@weight", textBox3.Text);
                    sqlCmd.Parameters.Add("@weight_type", comboBox1.Text);
                    sqlCmd.Parameters.Add("@image_path", text_image_path.Text.Trim());
                    sqlCmd.Parameters.Add("@image", ImageByteArray);
                    sqlCmd.Parameters.Add("@total", text_position.Text);
                    sqlCmd.Parameters.Add("@paid", textBox4.Text);
                    sqlCmd.Parameters.Add("@remaining", textBox5.Text);
                    sqlCmd.Parameters.Add("@save_date", dateTimePicker1.Text);
                    int n1=sqlCmd.ExecuteNonQuery();
                    if (n1 > 0)
                    {
                        con.Close();
                        MessageBox.Show("updated successfully");
                        Clear();
                        RefreshImageGrid();
                        getsrno();
                    }
                    else
                    {
                        MessageBox.Show("Failed To Update");
                    }  
                }
                else
                {
                    MessageBox.Show("Please enter all details");
                }
            }
        }
        private void Order_Module_final_Load(object sender, EventArgs e)
        {
            getsrno();
            RefreshImageGrid();
            try
            {
                SqlConnection con1 = new SqlConnection("Data Source=DESKTOP-52MI41T;Initial Catalog=DSP;Integrated Security=True");
                SqlCommand cmd1 = new SqlCommand("select name,product from [Order_details]",con1);
                con1.Open();
                SqlDataReader reader_new = cmd1.ExecuteReader();
                AutoCompleteStringCollection MyCollection = new AutoCompleteStringCollection();
                while (reader_new.Read())
                {
                    MyCollection.Add(reader_new[0].ToString());
                }
                textBox6.AutoCompleteCustomSource = MyCollection;
            } catch(Exception ae)
            {
                MessageBox.Show(ae.Message);
            }
        }
        private void button_browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images(.jpg,.png)|*.png;*.jpg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                text_image_path.Text = "";
                strFilePath = null;
                strFilePath = ofd.FileName;
                Image_Picturebox.Image = new Bitmap(strFilePath);
                if (text_image_path.Text.Trim().Length == 0)
                    text_image_path.Text = System.IO.Path.GetFileName(strFilePath);
            }
        }
        void RefreshImageGrid()
        {
            text_image_path.Text = "";
            con.Open();
            SqlDataAdapter sqlda = new SqlDataAdapter("Display_Information", con);
            sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dtblImages = new DataTable();
            sqlda.Fill(dtblImages);
            Grid_information.DataSource = dtblImages;
            Grid_information.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Grid_information.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Grid_information.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Grid_information.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Grid_information.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Grid_information.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Grid_information.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Grid_information.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Grid_information.Columns[8].Visible = false;
            Grid_information.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Grid_information.Columns[10].Visible = false;
            Grid_information.Columns[11].Visible = false;
            Grid_information.Columns[12].Visible = false;
        }
        void Clear()
        {
            id = 0;
            text_image_path.Clear();
            Image_Picturebox.Image = null;
            strFilePath = "";
            button_save.Text = "Save";
            text_name.Text = "";
            text_address.Text = "";
            text_position.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            comboBox1.Text = "";
            label3.Text = "";
        }
        private void Grid_information_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            label3.Text = Grid_information.CurrentRow.Cells[0].Value.ToString();
            text_name.Text = Grid_information.CurrentRow.Cells[1].Value.ToString(); 
            text_address.Text = Grid_information.CurrentRow.Cells[2].Value.ToString(); 
            textBox1.Text = Grid_information.CurrentRow.Cells[3].Value.ToString();
            textBox2.Text = Grid_information.CurrentRow.Cells[4].Value.ToString();
            textBox3.Text = Grid_information.CurrentRow.Cells[5].Value.ToString();
            comboBox1.Text = Grid_information.CurrentRow.Cells[6].Value.ToString();
            text_image_path.Text = Grid_information.CurrentRow.Cells[7].Value.ToString();
            byte[] ImageArray = (byte[])Grid_information.CurrentRow.Cells[8].Value; 
            text_position.Text = Grid_information.CurrentRow.Cells[9].Value.ToString();
            textBox4.Text = Grid_information.CurrentRow.Cells[10].Value.ToString();
            textBox5.Text = Grid_information.CurrentRow.Cells[11].Value.ToString();
            dateTimePicker1.Text = Grid_information.CurrentRow.Cells[12].Value.ToString();
            if (ImageArray.Length == 0)
                Image_Picturebox.Image = null;
            else
            {
                ImageByteArray = ImageArray;
                Image_Picturebox.Image = Image.FromStream(new MemoryStream(ImageArray));
            }
            id = Convert.ToInt32(Grid_information.CurrentRow.Cells[0].Value);
            button_save.Text = "Update";
            button_save.Enabled = true;
            button_delete.Enabled = true;
        }
        private void text_name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
        private void text_address_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
        private void text_position_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
        private void textBox6_Leave(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-52MI41T;Initial Catalog=DSP;Integrated Security=True");
                SqlCommand cmd = new SqlCommand("select [id],[name],[contact],[product],[code],[weight],[weight_type],[total] from [Order_details] where [name]='"+textBox6.Text+ "' OR [product]='"+textBox6.Text+"'", con);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(sdr);
                Grid_information.DataSource = dt;
            }
            catch(Exception ae)
            {
                MessageBox.Show(ae.Message);
            }
        }
        private void textBox6_KeyUp(object sender, KeyEventArgs e)
        {

        }
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
        private void textBox4_Leave(object sender, EventArgs e)
        {
            paid = int.Parse(textBox4.Text);
            remaining = total - paid;
            textBox5.Text = remaining.ToString();
        }
        private void text_position_Leave(object sender, EventArgs e)
        {
            total = int.Parse(text_position.Text);
        }
    }
}
