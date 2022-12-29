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
using System.Configuration;

namespace EsdnevnikZA
{
    public partial class Osoba : Form
    {
        int br = 0;
        DataTable tabela;

        private void Load_Data()
        {
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Osoba", veza);
            tabela = new DataTable();
            adapter.Fill(tabela);
            TxtPopulate();
        }

        private void TxtPopulate()
        {
            if (tabela.Rows.Count == 0)
            {
                tb_id.Text = "";
                tb_ime.Text = "";
                tb_prezime.Text = "";
                tb_adresa.Text = "";
                tb_jmbg.Text = "";
                tb_email.Text = "";
                tb_pass.Text = "";
                tb_uloga.Text = "";
                bt_delete.Enabled = false;
            }
            else
            {
                tb_id.Text = tabela.Rows[br]["id"].ToString();
                tb_ime.Text = tabela.Rows[br]["ime"].ToString();
                tb_prezime.Text = tabela.Rows[br]["prezime"].ToString();
                tb_adresa.Text = tabela.Rows[br]["adresa"].ToString();
                tb_jmbg.Text = tabela.Rows[br]["jmbg"].ToString();
                tb_email.Text = tabela.Rows[br]["email"].ToString();
                tb_pass.Text = tabela.Rows[br]["pass"].ToString();
                tb_uloga.Text = tabela.Rows[br]["uloga"].ToString();
                bt_delete.Enabled = true;
            }

            if (br == tabela.Rows.Count - 1)
            {
                bt_next.Enabled = false;
                bt_last.Enabled = false;
            }
            else
            {
                bt_next.Enabled = true;
                bt_last.Enabled = true;
            }

            if (br == 0)
            {
                bt_prev.Enabled = false;
                bt_first.Enabled = false;
            }
            else
            {
                bt_prev.Enabled = true;
                bt_first.Enabled = true;
            }
        }

        public Osoba()
        {
            InitializeComponent();
        }

        private void Osoba_Load(object sender, EventArgs e)
        {
            Load_Data();
            TxtPopulate();
        }

        private void bt_first_Click(object sender, EventArgs e)
        {
            br = 0;
            TxtPopulate();
        }

        private void bt_prev_Click(object sender, EventArgs e)
        {
            br--;
            TxtPopulate();
        }

        private void bt_insert_Click(object sender, EventArgs e)
        {
            StringBuilder Naredba = new StringBuilder("INSERT INTO Osoba (ime, prezime, adresa, jmbg, email, pass, uloga)VALUES('");
            Naredba.Append(tb_ime.Text + "', '");
            Naredba.Append(tb_prezime.Text + "', '");
            Naredba.Append(tb_adresa.Text + "', '");
            Naredba.Append(tb_jmbg.Text + "', '");
            Naredba.Append(tb_email.Text + "', '");
            Naredba.Append(tb_pass.Text + "', '");
            Naredba.Append(tb_uloga.Text + "')");
            SqlConnection veza = Konekcija.Connect();
            SqlCommand Komanda = new SqlCommand(Naredba.ToString(), veza);
            try
            {
                veza.Open();
                Komanda.ExecuteNonQuery();
                veza.Close();
            }
            catch (Exception Greska)
            {
                MessageBox.Show(Greska.Message);
            }

            Load_Data();
            br = tabela.Rows.Count - 1;
            TxtPopulate();
            obavestenje.Text = "Podatak uspesno dodat!";
        }

        private void bt_update_Click(object sender, EventArgs e)
        {
            StringBuilder Naredba = new StringBuilder("UPDATE Osoba SET ");
            Naredba.Append("ime = '" + tb_ime.Text + "', ");
            Naredba.Append("prezime = '" + tb_prezime.Text + "', ");
            Naredba.Append("adresa = '" + tb_adresa.Text + "', ");
            Naredba.Append("jmbg = '" + tb_jmbg.Text + "', ");
            Naredba.Append("email = '" + tb_email.Text + "', ");
            Naredba.Append("pass = '" + tb_pass.Text + "', ");
            Naredba.Append("uloga = '" + tb_uloga.Text + "' ");
            Naredba.Append("WHERE id = " + tb_id.Text);
            SqlConnection veza = Konekcija.Connect();
            SqlCommand komanda = new SqlCommand(Naredba.ToString(), veza);
            try
            {
                veza.Open();
                komanda.ExecuteNonQuery();
                veza.Close();
            }
            catch (Exception Greska)
            {
                MessageBox.Show(Greska.Message);
            }
            Load_Data();
            TxtPopulate();
            obavestenje.Text = "Podatak uspesno izmenjen!";
        }

        private void bt_delete_Click(object sender, EventArgs e)
        {
            Boolean poslednje = true;
            string Naredba1 = "DELETE FROM Raspodela WHERE nastavnik_id = " + tb_id.Text;
            string Naredba2 = "DELETE FROM Ocena WHERE ucenik_id = " + tb_id.Text;
            string Naredba3 = "DELETE FROM Upisnica WHERE osoba_id = " + tb_id.Text;
            string Naredba4 = "DELETE FROM Odeljenje WHERE razredni_id = " + tb_id.Text;
            string Naredba = "DELETE FROM Osoba WHERE Osoba.id = " + tb_id.Text;
            if (br == tabela.Rows.Count - 1)
            {
                br--;
                poslednje = false;
            }
            if (br < 0) br = 0;
            SqlConnection veza = Konekcija.Connect();
            SqlCommand komanda1 = new SqlCommand(Naredba1, veza);
            SqlCommand komanda2 = new SqlCommand(Naredba2, veza);
            SqlCommand komanda3 = new SqlCommand(Naredba3, veza);
            SqlCommand komanda4 = new SqlCommand(Naredba4, veza);
            SqlCommand komanda = new SqlCommand(Naredba, veza);
            Boolean obrisano = false;
            try
            {
                veza.Open();
                komanda1.ExecuteNonQuery();
                komanda2.ExecuteNonQuery();
                komanda3.ExecuteNonQuery();
                komanda4.ExecuteNonQuery();
                komanda.ExecuteNonQuery();
                veza.Close();
                obrisano = true;
            }
            catch (Exception GRESKA)
            {
                MessageBox.Show(GRESKA.Message);
            }

            if (obrisano && poslednje)
            {
                Load_Data();
                if (br > 0) br--;
                TxtPopulate();
            }
            else
            {
                Load_Data();
                TxtPopulate();
            }
            obavestenje.Text = "Podatak uspesno obrisan!";
        }


        private void bt_next_Click(object sender, EventArgs e)
        {
            br++;
            TxtPopulate();
        }

        private void bt_last_Click(object sender, EventArgs e)
        {
            br = tabela.Rows.Count - 1;
            TxtPopulate();
        }
    }
