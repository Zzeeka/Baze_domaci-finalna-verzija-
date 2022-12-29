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
using System.Configuration;

namespace EsdnevnikZA
{
    public partial class Raspodela : Form
    {
        DataTable raspodela;
        int br = 0;

        public Raspodela()
        {
            InitializeComponent();
        }

        private void Load_Data()
        {
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM raspodela", veza);
            raspodela = new DataTable();
            adapter.Fill(raspodela);
        }

        private void ComboFill()
        {

            if (br == raspodela.Rows.Count - 1)
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

            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter;
            DataTable dt_godina, dt_nastavnik, dt_predmet, dt_odeljenje;
            adapter = new SqlDataAdapter("SELECT * FROM skolska_godina", veza);
            dt_godina = new DataTable();
            adapter.Fill(dt_godina);

            dt_nastavnik = new DataTable();
            adapter = new SqlDataAdapter("SELECT id, ime + prezime as naziv FROM osoba WHERE uloga = 2", veza);
            adapter.Fill(dt_nastavnik);

            dt_predmet = new DataTable();
            adapter = new SqlDataAdapter("SELECT id, naziv FROM predmet", veza);
            adapter.Fill(dt_predmet);

            dt_odeljenje = new DataTable();
            adapter = new SqlDataAdapter("SELECT id, STR(razred) + '-' + indeks as naziv FROM odeljenje", veza);
            adapter.Fill(dt_odeljenje);

            cmb_godina.DataSource = dt_godina;
            cmb_godina.ValueMember = "id";
            cmb_godina.DisplayMember = "naziv";


            cmb_nastavnik.DataSource = dt_nastavnik;
            cmb_nastavnik.ValueMember = "id";
            cmb_nastavnik.DisplayMember = "naziv";


            cmb_predmet.DataSource = dt_predmet;
            cmb_predmet.ValueMember = "id";
            cmb_predmet.DisplayMember = "naziv";


            cmb_odeljenje.DataSource = dt_odeljenje;
            cmb_odeljenje.ValueMember = "id";
            cmb_odeljenje.DisplayMember = "naziv";

            tb_id.Text = raspodela.Rows[br]["id"].ToString();

            if (raspodela.Rows.Count == 0)
            {
                cmb_godina.SelectedValue = -1;
                cmb_nastavnik.SelectedValue = -1;
                cmb_predmet.SelectedValue = -1;
                cmb_odeljenje.SelectedValue = -1;
            }
            else
            {
                cmb_godina.SelectedValue = raspodela.Rows[br]["godina_id"];
                cmb_nastavnik.SelectedValue = raspodela.Rows[br]["nastavnik_id"];
                cmb_predmet.SelectedValue = raspodela.Rows[br]["predmet_id"];
                cmb_odeljenje.SelectedValue = raspodela.Rows[br]["odeljenje_id"];
            }
        }

        private void Raspodela_Load(object sender, EventArgs e)
        {
            Load_Data();
            ComboFill();
        }

        private void bt_first_Click(object sender, EventArgs e)
        {
            br = 0;
            ComboFill();
        }

        private void bt_prev_Click(object sender, EventArgs e)
        {
            br--;
            ComboFill();
        }

        private void bt_insert_Click(object sender, EventArgs e)
        {
            StringBuilder Naredba = new StringBuilder("INSERT INTO raspodela (godina_id, nastavnik_id, predmet_id, odeljenje_id)VALUES('");
            Naredba.Append(cmb_godina.SelectedValue + "', '");
            Naredba.Append(cmb_nastavnik.SelectedValue + "', '");
            Naredba.Append(cmb_predmet.SelectedValue + "', '");
            Naredba.Append(cmb_odeljenje.SelectedValue + "')");
            SqlConnection veza = Konekcija.Connect();
            SqlCommand Komanda = new SqlCommand(Naredba.ToString(), veza);
            try
            {
                veza.Open();
                Komanda.ExecuteNonQuery();
                veza.Close();
            }
            catch (Exception greska) { MessageBox.Show(greska.GetType().ToString()); }

            Load_Data();
            br = raspodela.Rows.Count - 1;
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM raspodela", veza);
            raspodela = new DataTable();
            adapter.Fill(raspodela);
            ComboFill();
            obavestenje.Text = "Podatak uspesno dodat!";
        }

        private void bt_update_Click(object sender, EventArgs e)
        {
            StringBuilder Naredba = new StringBuilder("UPDATE raspodela SET ");
            Naredba.Append("godina_id = '" + cmb_godina.SelectedValue + "', ");
            Naredba.Append("nastavnik_id = '" + cmb_nastavnik.SelectedValue + "', ");
            Naredba.Append("predmet_id = '" + cmb_predmet.SelectedValue + "', ");
            Naredba.Append("odeljenje_id = '" + cmb_odeljenje.SelectedValue + "' ");
            Naredba.Append("WHERE id = " + tb_id.Text);
            SqlConnection veza = Konekcija.Connect();
            SqlCommand komanda = new SqlCommand(Naredba.ToString(), veza);
            try
            {
                veza.Open();
                komanda.ExecuteNonQuery();
                veza.Close();
            }
            catch (Exception greska) { MessageBox.Show(greska.GetType().ToString()); }
            Load_Data();
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM raspodela", veza);
            raspodela = new DataTable();
            adapter.Fill(raspodela);
            ComboFill();
            obavestenje.Text = "Podatak uspesno izmenjen!";
        }

        private void bt_delete_Click(object sender, EventArgs e)
        {
            string Naredba = "DELETE FROM raspodela WHERE id = " + tb_id.Text;
            SqlConnection veza = Konekcija.Connect();
            SqlCommand komanda = new SqlCommand(Naredba, veza);
            Boolean obrisano = false;
            try
            {
                veza.Open();
                komanda.ExecuteNonQuery();
                veza.Close();
                obrisano = true;
            }
            catch (Exception greska) { MessageBox.Show(greska.GetType().ToString()); }

            if (obrisano)
            {
                Load_Data();
                if (br > 0) br--;
                ComboFill();
            }
            obavestenje.Text = "Podatak uspesno obrisan!";
        }

        private void bt_next_Click(object sender, EventArgs e)
        {
            br++;
            ComboFill();
        }

        private void bt_last_Click(object sender, EventArgs e)
        {
            br = raspodela.Rows.Count - 1;
            ComboFill();
        }
    }
}